using SledgeLib;

using Nakama;
using Nakama.TinyJson;

using Newtonsoft.Json;

using System.Text;
using System.Numerics;

public class Client {
    private static string? m_MatchID;
    public static string m_DeviceID = "";
    public static ISession? m_Session { get; set; }
    public static IClient? m_Connection { get; set; }
    public static ISocket? m_Socket { get; set; }

    private static bool m_bCanUpdatePlayer = false;

    public static bool m_bConnecting { get; set; } = false;

    private static Vector3 m_PrevPosition = Vector3.Zero;

    private static Dictionary<string, IPlayer> currentPresences = new();

    private enum OP_CODES : ushort {
        PLAYER_POS = 1,
        PLAYER_SPAWN = 2,
        SPAWN_ALL = 3
    }

    // Listeners
    private static void Listener_UpdatePosition(IMatchState newState) {
        if (!m_bCanUpdatePlayer)
            return;

        string stringJson = Encoding.Default.GetString(newState.State);
        UserID? data = JsonConvert.DeserializeObject<UserID>(stringJson);

        if (data == null)
            return;

        for (int i = 0; i < 32; i++) { // 32 is the max number of players?
            if (data.ClientData[i] != null) {
                IClientData client = data.ClientData[i];
                if (currentPresences.ContainsKey(client.user_id)) {
                    if (m_Session != null && client.user_id != m_Session.UserId) {
                        float x = (float)Math.Round((float)client.x);
                        float y = (float)Math.Round((float)client.y);
                        float z = (float)Math.Round((float)client.z);

                        Body.SetPosition(currentPresences[client.user_id].Body, new Vector3(x, y, z));
                        Body.SetRotation(currentPresences[client.user_id].Body, new Quaternion(0, 0.7071068f, 0.7071068f, 0));
                    }
                }
            }
        }

        m_bCanUpdatePlayer = false;
    }

    public static void OnPlayerUpdate() {
        m_bCanUpdatePlayer = true;
        // Quaternion camRot = Player.GetCameraTransform().Rotation;
        // Quaternion rot = new Quaternion(0, 0.7071068f, 0.7071068f, 0);

        // camRot = Quaternion.Multiply(camRot, rot);
    
        // Body.SetPosition(tempBody, new Vector3((float)0, (float)0, (float)0));
        // Body.SetRotation(tempBody, camRot);
    }

    private static void Listener_PlayerJoin(IMatchPresenceEvent player) {
        if (player.Joins.Any()) {
            foreach (var presence in player.Joins) {
                if (m_Session != null && presence.UserId != m_Session.UserId) {
                    Log.General("{0} has joined the game", presence.UserId);
                    CreatePlayer(presence);
                }
            }
        } else if (player.Leaves.Any()) {
            foreach (var presence in player.Leaves) {
                if (m_Session != null && presence.UserId != m_Session.UserId) {
                    currentPresences.Remove(presence.UserId);
                    Log.General("{0} has left the game", presence.UserId);
                }
            }

            foreach (var presence in currentPresences) {
                if (m_Session != null && presence.Key != m_Session.UserId) {
                    Body.Destroy(presence.Value.Body);
                    Log.General("Destroying");
                }
            }
        }
    }

    public static void OnStateChange(uint iState) {
        if (iState == (uint)EGameState.Playing) {
            if (!m_bConnecting)
                return;

            JoinGame();
            m_bConnecting = false;
        } else if (iState == (uint)EGameState.Menu) {
            if (m_MatchID != null)
                Disconnect();
        }
    }

    private static void SpawnModel(IPlayer? presence) {
        if (presence == null)
            return;

        Log.General("Spawn Shape: {0}", currentPresences[presence.UserId].Shape);
        Log.General("Spawn Body: {0}", currentPresences[presence.UserId].Body);

        Shape.LoadVox(currentPresences[presence.UserId].Shape, "Assets/Vox/player.vox", "", 1.0f);
        Body.SetPosition(currentPresences[presence.UserId].Body, new Vector3((float)0, (float)0, (float)0));
        Body.SetRotation(currentPresences[presence.UserId].Body, new Quaternion(0, 0.7071068f, 0.7071068f, 0));

        Log.General("Spawned Model");
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
        m_bConnecting = true;

        Log.General("Connecting to {0}:{1}", ip, port);
        // TODO: Join game: need to be able to load the map and get the map from the server
    }

    // General Functions
    public static void CreatePlayer(IUserPresence presence) {
        uint body = Body.Create();
        uint shape = Shape.Create(body);

        // tempBody = Body.Create();
        // tempShape = Shape.Create(tempBody);

        // Log.General("tempBody: {0}", tempBody);
        // Log.General("tempShape: {0}", tempShape);

        IPlayer newPlayer = new() {
            Body = body,
            Shape = shape,
            Persistence = presence.Persistence,
            SessionId = presence.SessionId,
            Status = presence.Status,
            Username = presence.Username,
            UserId = presence.UserId
        };

        Log.General("NewPly Shape: {0}", newPlayer.Shape);
        Log.General("NewPly Body: {0}", newPlayer.Body);

        currentPresences.Add(presence.UserId, newPlayer);
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
        m_MatchID = response.Payload.Trim(new char[] { '"' }); // maybe here?

        IMatch match = await m_Socket.JoinMatchAsync(m_MatchID);

        if (m_Session != null)
            Log.General("Current local UserID: {0}", m_Session.UserId);

        // All the players already in the game
        foreach (IUserPresence presence in match.Presences) {
            if (m_Session != null && presence.UserId != m_Session.UserId) {
                Log.General("User Already In-Game: {0}", presence.UserId);
                CreatePlayer(presence);
                SpawnModel(currentPresences[presence.UserId]);
            }
        }
    }

    public static void OnUpdate() {
        if (m_Socket == null || m_Session == null || m_MatchID == null)
            return;

        foreach (var presence in currentPresences) {
            // If it's not the local player, load in their vox player model
            if (m_Session != null && presence.Key != m_Session.UserId && !presence.Value.voxelLoaded) {
                Shape.LoadVox(presence.Value.Shape, "Assets/Vox/player.vox", "", 1.0f);

                presence.Value.voxelLoaded = true;

                Log.General("{0}: Voxel Loaded", presence.Key);
            }
        }

        Vector2 playerInput = Player.GetPlayerMovementInput();
        Transform playerTransform = Player.GetCameraTransform();

        // TODO: Save last position of player in order to compare to current position.

        float cx = (float)MathF.Round((float)playerTransform.Position.X, 1);
        float cy = (float)MathF.Round((float)playerTransform.Position.Y, 1);
        float cz = (float)MathF.Round((float)playerTransform.Position.Z, 1);

        float ox = (float)MathF.Round((float)m_PrevPosition.X, 1);
        float oy = (float)MathF.Round((float)m_PrevPosition.Y, 1);
        float oz = (float)MathF.Round((float)m_PrevPosition.Z, 1);

        if (cx != ox || cy != oy || cz != oz) {
            m_PrevPosition = playerTransform.Position;

            string? newState = new Dictionary<string, dynamic> {
                { "user_id", m_Session.UserId },
                { "currentX", playerTransform.Position.X },
                { "currentY", playerTransform.Position.Y },
                { "currentZ", playerTransform.Position.Z },
                { "rotationX", playerTransform.Rotation.X },
                { "rotationY", playerTransform.Rotation.Y },
                { "rotationZ", playerTransform.Rotation.Z },
                { "rotationW", playerTransform.Rotation.W }
            }.ToJson();

            m_Socket.SendMatchStateAsync(m_MatchID, (int)OP_CODES.PLAYER_POS, newState);
        }
    }

    public static async void Disconnect() {
        try { // Temporary
            m_bConnecting = false;

            if (Client.m_Socket != null && Client.m_Session != null && m_MatchID != null) {
                await m_Socket.LeaveMatchAsync(m_MatchID);
                currentPresences = new();
                m_MatchID = null;
                m_Session = null;
                m_Socket = null;

                Log.General("Disconnected");

                if (Game.GetState() != EGameState.Menu)
                    Game.SetState(EGameState.Menu);
            } else {
                Log.General("You are not connected to a lobby");
            }
        } catch (Exception e) {
            Log.Error("Error disconnecting from lobby: {0}", e.Message);
        }
    }
}