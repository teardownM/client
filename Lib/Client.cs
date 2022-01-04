using SledgeLib;

using Nakama;
using Nakama.TinyJson;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Text;
using System.Numerics;

public class Client {
    private static string? m_MatchID;
    public static string m_DeviceID = "";
    public static ISession? m_Session { get; set; }
    public static IClient? m_Connection { get; set; }
    public static ISocket? m_Socket { get; set; }

    private static Dictionary<string, IPlayer> currentPresences = new();

    private enum OP_CODES : ushort {
        PLAYER_POS = 1,
        PLAYER_SPAWN = 2,
        SPAWN_ALL = 3
    }

    // Listeners
    private static void Listener_UpdatePosition(IMatchState newState) {
        string stringJson = Encoding.Default.GetString(newState.State);
        UserID? data = JsonConvert.DeserializeObject<UserID>(stringJson);

        if (data != null) {
            for (int i = 0; i < 32; i++) { // 32 is the max number of players?
                if (data.ClientData[i] != null) {
                    IClientData client = data.ClientData[i];
                    if (currentPresences.ContainsKey(client.user_id)) {
                        float x = (float)Math.Round((float)client.x);
                        float y = (float)Math.Round((float)client.y);
                        float z = (float)Math.Round((float)client.z);

                        Body.SetTransform(currentPresences[client.user_id].m_Body, new Transform(new Vector3(x, y, z), new Quaternion(0, 0.7071068f, 0.7071068f, 0)));
                    }
                }
            }
        }
    }

    private static void Listener_PlayerJoin(IMatchPresenceEvent player) {
        if (player.Joins.Any()) {
            foreach (var presence in player.Joins) {
                if (m_Session != null && presence.UserId != m_Session.UserId) {
                    Log.General("{0} has joined the game", presence.UserId);
                    CreatePlayer(presence);
                    Body.SetPosition(currentPresences[presence.UserId].m_Body, new Vector3((float)0, (float)0, (float)0));
                    Body.SetRotation(currentPresences[presence.UserId].m_Body, new Quaternion(0, 0.7071068f, 0.7071068f, 0));
                }
            }
        };
    }

    private static void InitializeListeners() {
        if (m_Socket == null || m_Session == null && m_MatchID != null) {
            Log.Error("No socket or session found when initializing listeners");
            return;
        }

        // This will get run every server tick (currently a tickrate of 28)
        m_Socket.ReceivedMatchState += newState => {
            switch (newState.OpCode) {
                case (ushort)OP_CODES.PLAYER_POS:
                    Listener_UpdatePosition(newState);
                    break;
                default:
                    Log.Error("Unknown opcode: {0}", newState.OpCode);
                    return;
            }
        };

        m_Socket.ReceivedMatchPresence += player => Listener_PlayerJoin(player);
    }

    // Connections
    private static async void Authenticate() {
        if (m_Connection == null) {
            Log.Error("No client found when authenticating");
            return;
        }

        m_Session = await m_Connection.AuthenticateDeviceAsync(m_DeviceID, m_DeviceID);

        Log.General("Authentication Successful");

        m_Socket = Socket.From(m_Connection);
        await m_Socket.ConnectAsync(m_Session);

        InitializeListeners();
    }

    public static void Connect(string ip, int port) {
        m_Connection = new Nakama.Client("http", ip, port, "defaultkey");
        if (m_Connection == null) {
            Log.General("Failed to connect to server");
        }

        Authenticate();

        // TODO: Join game: need to be able to load the map and get the map from the server
    }

    // General Functions
    public static void CreatePlayer(IUserPresence presence) {
        uint m_Body = Body.Create();
        Body.SetDynamic(m_Body, false);
        Body.SetPosition(m_Body, new Vector3(0, -100, 0));

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
        Log.General("{0}: Added player into currentPlayers", presence.UserId);
    }

    public static async void JoinGame() {
        if (m_Connection == null || m_Socket == null) {
            Log.General("Unable to find connection to remote server");
            return;
        }

        if (m_MatchID != null)
            Disconnect();

        // TODO: Retrieve active mods from server (server has a list of workshop ID's, client then downloads them and activates them)

        /*
         * We currently create 1 match on startup of our Nakama server.
         * This is just for testing purposes, in the future there'll be a server browser.
         * 
         * This function sends an api request to fetch the match id and then joins that match
         */
        IApiRpc response = await m_Connection.RpcAsync(m_Session, "get_match_id");
        m_MatchID = response.Payload.Trim(new char[] { '"' });

        IMatch match = await m_Socket.JoinMatchAsync(m_MatchID);

        if (m_Session != null)
            Log.General("Current local UserID: {0}", m_Session.UserId);

        // All the players already in the game
        foreach (IUserPresence presence in match.Presences) {
            if (m_Session != null && presence.UserId != m_Session.UserId) {
                Log.General("User Already In-Game: {0}", presence.UserId);
                CreatePlayer(presence);
            }
        }
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
                Body.SetPosition(presence.Value.m_Body, new Vector3((float)0, (float)0, (float)0));
                Body.SetRotation(presence.Value.m_Body, new Quaternion(0, 0.7071068f, 0.7071068f, 0));
                presence.Value.voxelLoaded = true;

                Log.General("{0}: Voxel Loaded", presence.Key);
            }
        }

        if (m_Socket == null || m_Session == null || m_MatchID == null)
            return;

        Vector2 playerInput = Player.GetPlayerMovementInput();
        Transform playerTransform = Player.GetCameraTransform();

        // TODO: Save last position of player in order to compare to current position.

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

        // Every local game tick, send m_Connection's position data to Nakama
        m_Socket.SendMatchStateAsync(m_MatchID, (int)OP_CODES.PLAYER_POS, newState);
    }

    public static async void Disconnect() {
        if (Client.m_Socket != null && Client.m_Socket.IsConnected && m_MatchID != null) {
            foreach (var presence in currentPresences) {
                // If it's not the local player, load in their vox player model
                if (m_Session != null && presence.Key != m_Session.UserId && presence.Value.voxelLoaded) {
                    Body.Destroy(presence.Value.m_Body);
                    presence.Value.voxelLoaded = false;
                }
            }

            await m_Socket.LeaveMatchAsync(m_MatchID);
            m_MatchID = null;
            m_Session = null;
            currentPresences = new();

            Log.General("Disconnected");
        } else {
            Log.General("You are not connected to a lobby");
        }
    }
}