using SledgeLib;

using Nakama;
using Nakama.TinyJson;
using Steamworks;

using Newtonsoft.Json;

using System.Text;
using System.Numerics;

public class Client {
    private static string? matchId;
    public static ISession? m_Session { get; set; }
    public static IClient? m_Client { get; set; }
    public static ISocket? m_Socket { get; set; }

    private static Dictionary<string, IPlayer> currentPresences = new();

    private enum OP_CODES : ushort {
        PLAYER_POS = 1,
        PLAYER_SPAWN = 2,
        SPAWN_ALL = 3
    }

    public static void CreatePlayer(IUserPresence presence) {
        // Create a static body for the player, set their position
        uint m_Body = Body.Create();
        Body.SetDynamic(m_Body, false);
        Body.SetPosition(m_Body, new Vector3(0, -100, 0));

        // Add user to our current dictionary of players
        IPlayer newPlayer = new() {
            m_Body = m_Body,
            m_Shape = Shape.Create(m_Body),
            voxelLoaded = false,
            Persistence = presence.Persistence,
            SessionId = presence.SessionId,
            Status = presence.Status,
            Username = presence.Username,
            UserId = presence.UserId
        };

        currentPresences.Add(presence.UserId, newPlayer);
        Log.General("{0}: Added into player into currentPlayers", presence.UserId);
    }

    public static async void JoinGame() {
        if (m_Client == null || m_Socket == null) {
            Log.Error("No client or socket found when authenticating");
            return;
        }

        /*
         * We currently create 1 match on startup of our Nakama server.
         * This is just for testing purposes, in the future there'll be a server browser.
         * 
         * This function sends an api request to fetch the match id and then joins that match
         */
        IApiRpc response = await m_Client.RpcAsync(m_Session, "get_match_id");
        matchId = response.Payload.Trim(new char[] { '"' });

        IMatch match = await m_Socket.JoinMatchAsync(matchId);

        Log.General("Joined game with id: {0}", matchId);
        if (m_Session != null)
            Log.General("Current local user id: {0}", m_Session.UserId);

        // All the players already in the game
        // TODO: Figure out spawning of each player
        //       Remember that the current local player is included in here too! Make sure you add a guard check for that (user_id != m_Session.user_id)
        foreach (IUserPresence presence in match.Presences) {
            if (m_Session != null && presence.UserId != m_Session.UserId) {
                Log.General("Found user id '{0}' already in the game", presence.UserId);
                CreatePlayer(presence);
            }
        }
    }

    public static void InitializeListeners() {
        if (m_Socket == null || m_Session == null && matchId != null)
            return;

        // This will get run every server tick (currently a tickrate of 28)
        m_Socket.ReceivedMatchState += newState => {
            switch (newState.OpCode) {
                case (ushort)OP_CODES.PLAYER_POS:
                    string stringJson = Encoding.Default.GetString(newState.State);
                    var incomingData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(stringJson);

                    if (incomingData != null) {
                        foreach (var presence in incomingData) {
                            // Check to see if the incoming data is a user in our current dictionary of players
                            if (currentPresences.ContainsKey(presence.Key)) {
                                if (presence.Value.x != null && presence.Value.y != null && presence.Value.z != null) {
                                    // Set the position of the player
                                    Log.General("Moving player {0} to {1} {2} {3}", presence.Key, presence.Value.x, presence.Value.y, presence.Value.z);
                                    Body.SetTransform(currentPresences[presence.Key].m_Body, new Transform(new Vector3((float)presence.Value.x, (float)presence.Value.y, (float)presence.Value.z), new Quaternion(0, 0, 0, 1)));
                                } else {
                                    Log.General("Received invalid data from {0}", presence.Key);
                                }
                            }
                        }
                    }
                    break;
                default:
                    return;
            }
        };

        m_Socket.ReceivedMatchPresence += player => {
            if (player.Joins.Any()) {
                foreach (var presence in player.Joins) {
                    if (m_Session != null && presence.UserId != m_Session.UserId) {
                        Log.General("A player has joined the game with id of {0}", presence.UserId);
                        Body.SetTransform(currentPresences[presence.UserId].m_Body, new Transform(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1)));
                        CreatePlayer(presence);
                    }
                }
            };
        };

        Log.General("Sockets loaded");
    }

    public static void OnUpdate() {
        // if the user is in the menu / doing something else, don't run this
        if (!Game.IsPlaying())
            return;

        foreach (var presence in currentPresences) {
            // If it's not the local player, load in their vox player model
            if (m_Session != null && presence.Key != m_Session.UserId && !presence.Value.voxelLoaded) {
                Shape.LoadVox(presence.Value.m_Shape, "Assets/Vox/player.vox", "", 1.0f);
                Shape.SetCollisionFilter(presence.Value.m_Shape, 0, 0);
                Body.SetTransform(presence.Value.m_Body, new Transform(new Vector3(50, 10, 10), new Quaternion(0, 0, 0, 1)));
                presence.Value.voxelLoaded = true;

                Log.General("{0}: Voxel Loaded", presence.Key);
            }
        }

        if (m_Socket == null || m_Session == null || matchId == null)
            return;

        Vector2 playerInput = Player.GetPlayerMovementInput();
        Transform playerTransform = Player.GetCameraTransform();

        var newState = new Dictionary<string, dynamic> {
            { "user_id", m_Session.UserId },
            { "currentX", playerTransform.Position.X },
            { "currentY", playerTransform.Position.Y },
            { "currentZ", playerTransform.Position.Z },
            { "rotationX", playerTransform.Rotation.X },
            { "rotationY", playerTransform.Rotation.Y },
            { "rotationZ", playerTransform.Rotation.Z },
            { "rotationW", playerTransform.Rotation.W }
        }.ToJson();

        // Every local game tick, send m_Client's position data to Nakama
        m_Socket.SendMatchStateAsync(matchId, 1, newState);
    }

    public static async void Disconnect() {
        if (Client.m_Socket != null && Client.m_Socket.IsConnected && matchId != null) {
            await m_Socket.LeaveMatchAsync(matchId);
            matchId = null;
            Log.General("Disconnected");
        } else {
            Log.General("You are not connected to a lobby");
        }
    }
}