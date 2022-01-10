using SledgeLib;
using System.Numerics;

using Steamworks;

using Newtonsoft.Json;

using Nakama;
using System.Globalization;

public class Model {
    public uint Body = 0;

    public uint sBody = 0;
    public uint sHead = 0;
    public uint sLeftArm = 0;
    public uint sLeftLeg = 0;
    public uint sRightArm = 0;
    public uint sRightLeg = 0;
}

public class IClientData {
    public string? SessionID;
    public string? Username;
    public string? Status;
    public string? UserID;
    public string? Node;
    public bool? Spawned;

    public ushort? Reason;

    public Model Model = new Model();
    public Transform Transform = new Transform();
}

public static class Client {
    public class ServerLabel {
        public string? value { get; set; }
    }

    private class Server {
        [JsonProperty("match_id")]
        public static string? MatchID { get; set; }

        [JsonProperty("authoritative")]
        public static bool? Authoritative { get; set; }

        [JsonProperty("label")]
        public static ServerLabel? Label { get; set; }

        [JsonProperty("tick_rate")]
        public static int? TickRate { get; set; }

        [JsonProperty("handler_name")]
        public static string? HandlerName { get; set; }
    }

    // Dictionary of all current players in the game
    private static Dictionary<string, IClientData> m_Clients = new();

    private static string? m_DeviceID = "";

    private static ISession? m_Session = null;
    private static IClient? m_Connection = null;
    private static ISocket? m_Socket = null;

    private static bool m_Connected = false;

    private static List<string> m_ModelsToLoad = new();

    private enum OPCODE : Int64 {
        PLAYER_MOVE = 1,
        PLAYER_SPAWN = 2,
        PLAYER_SHOOTS = 3,
        PLAYER_JOINS = 4,
        PLAYER_GRABS = 5
    }

    public static void OnInitialize() {
        m_DeviceID = Guid.NewGuid().ToString();
    }

    public static async Task<ISession> Connect(string ip, ushort port) {
        // Establish a connection to Nakama server
        m_Connection = new Nakama.Client("http", ip, port, "defaultkey");
        m_Connection.Timeout = 1;
        if (m_Connection == null) {
            Log.Error("Failed to create connection");
            return await Task.FromResult<ISession>(null!);
        }

        try
        {
            m_Session = await m_Connection.AuthenticateDeviceAsync(m_DeviceID, m_DeviceID);
            Log.Verbose("Successfully authenticated");
            Log.General("Your ID: {0}", m_Session.UserId);
        } catch (Exception) {
            return await Task.FromResult<ISession>(null!);
        }

        m_Socket = Socket.From(m_Connection);
        await m_Socket.ConnectAsync(m_Session);

        m_Socket.ReceivedMatchState += OnMatchState;
        m_Socket.ReceivedMatchPresence += OnMatchPresence;

        IApiRpc response = await m_Connection.RpcAsync(m_Session, "rpc_get_matches");
        JsonConvert.DeserializeObject<Server>(response.Payload.Substring(1, response.Payload.Length - 2));

        IMatch match = await m_Socket.JoinMatchAsync(Server.MatchID);
        m_Connected = true;
        Log.Verbose("Successfully connected to match {0}", Server.MatchID!);

        foreach (IUserPresence client in match.Presences) {
            Log.General("{0} already in the game", client.UserId);
            CreatePresence(client);
            m_ModelsToLoad.Add(client.UserId);
        }

        return m_Session;
    }

    public static async void Disconnect() {
        if (m_Socket != null) {
            if (Server.MatchID != null) {
                await m_Socket.LeaveMatchAsync(Server.MatchID);
            }

            await m_Socket.CloseAsync();

            Server.MatchID = null;
            m_Socket = null;
            m_Session = null;
            m_Connected = false;

            m_Clients = new() {};

            Log.General("Disconnected from server");

            if (Game.GetState() != EGameState.Menu)
                Game.SetState(EGameState.Menu);
        } else {
            Log.General("Not connected to server");
        }
    }

    public static void OnUpdate() {
        if (!m_Connected || Game.GetState() != EGameState.Playing)
            return;

        if (m_ModelsToLoad.Any()) {
            foreach (var userId in m_ModelsToLoad.ToList()) {
                // If it's not the local player, load in their vox player model
                if (userId != m_Session!.UserId) {
                    SpawnPlayer(userId);
                    m_ModelsToLoad.Remove(userId);

                    Log.General("Loaded {0}'s model into the game", userId);
                }
            }
        }

        if (m_Socket == null || m_Session == null || Server.MatchID == null)
            return;

        Vector2 playerInput = Player.GetPlayerMovementInput();
        Transform playerTransform = Player.GetCameraTransform();

        var posData = playerTransform.Position.X.ToString() + "," + playerTransform.Position.Y.ToString() + "," + playerTransform.Position.Z.ToString();

        // Every local game tick, send client's position data to Nakama
        m_Socket.SendMatchStateAsync(Server.MatchID, (long)OPCODE.PLAYER_MOVE, posData);
    }

    // Presence means another player / client that is NOT the local player.
    public static void CreatePresence(IUserPresence player) {
        Log.General("Creating presence {0}", player.UserId);

        // Create a static body for the player, set their position
        uint body = Body.Create();
        Body.SetDynamic(body, false);
        Body.SetPosition(body, new Vector3(0, 0, 0));

        Model playerModel = new() {
            Body = body,
            sBody = Shape.Create(body)
        };

        IClientData newPlayer = new() {
            SessionID = player.SessionId,
            Username = player.UserId,
            Status = player.Status,
            UserID = player.UserId,
            Spawned = false,
            Model = playerModel
        };

        m_Clients.Add(player.UserId, newPlayer);
    }

    public static void SpawnPlayer(string clientID) {
        Shape.LoadVox(m_Clients[clientID].Model.sBody, "Assets/Models/player2.vox", "", 1.0f);
        Body.SetTransform(m_Clients[clientID].Model.Body, new Transform(new Vector3(50, 10, 10), new Quaternion(0, 0, 0, 1)));
        m_Clients[clientID].Spawned = true;

        Log.General("{0} has spawned", clientID);
    }

    public static void OnStateChange(uint iState) {
        switch (iState) {
            case (uint)EGameState.Menu:
                if (Server.MatchID != null)
                    Disconnect();
                break;
            case (uint)EGameState.Playing:
                // 1. Player joins server from the menu
                // 2. Connects to match instantly
                // 3. TODO: Force game to load map that server is using
                // 4. Player loads into the map
                // 5. EGameState changes to playing
                // 6. Spawn players in m_ModelsToLoad
                if (Server.MatchID != null) { // <-- Checking for Server.MatchID is the same as checking if m_Connected is true
                    Log.Verbose("Local client has loaded into the map.");

                    // 7. Notify every player local client has loaded in and should spawn their model
                    m_Socket!.SendMatchStateAsync(Server.MatchID, (long)OPCODE.PLAYER_SPAWN, "");
                }
                break;
            default:
                break;
        }
    }

    /*
        Client1 spawns in and sends PLAYER_SPAWN
        Rest of the clients recieve that and say, "Hey, I'll spawn you in and send you a PLAYER_SPAWN"
    */

    public static void OnMatchState(IMatchState newState) {
        switch (newState.OpCode) {
            case (Int64)OPCODE.PLAYER_MOVE:
                // Date structure: user_id,x,y,z    
                var playerMoveData = System.Text.Encoding.Default.GetString(newState.State).Split(',').ToList();

                float x = float.Parse(playerMoveData[1], CultureInfo.InvariantCulture.NumberFormat);
                float y = float.Parse(playerMoveData[2], CultureInfo.InvariantCulture.NumberFormat);
                float z = float.Parse(playerMoveData[3], CultureInfo.InvariantCulture.NumberFormat);

                Body.SetPosition(m_Clients[playerMoveData[0]].Model.Body, new Vector3(x, y, z));
                Body.SetTransform(m_Clients[playerMoveData[0]].Model.Body, new Transform(new Vector3(x, y, z), new Quaternion(0, 0.7071068f, 0.7071068f, 0)));
                break;
            case (Int64)OPCODE.PLAYER_SPAWN:
                // This message could be received in the menu (while connecting and not in game yet)
                if (!Game.IsPlaying())
                    break;

                string id = System.Text.Encoding.Default.GetString(newState.State);
                if (id == m_Session!.UserId)
                    return;

                Log.General("Received player spawn from {0}", id);
                m_ModelsToLoad.Add(id);
                
                break;
            case (Int64)OPCODE.PLAYER_SHOOTS:
                break;
            default:
                break;
        }
    }

    public static void OnMatchPresence(IMatchPresenceEvent presence) {
        if (presence.Joins.Any()) {
            foreach (IUserPresence? client in presence.Joins) {
                if (client.UserId == m_Session!.UserId)
                    continue;

                Log.General("Player {0} is joining the match!", client.UserId);
                CreatePresence(client);
                m_ModelsToLoad.Add(client.UserId);
            }
        } else if (presence.Leaves.Any()) {
            foreach (IUserPresence? client in presence.Leaves) {
                if (client.UserId == m_Session!.UserId)
                    continue;

                m_Clients.Remove(client.UserId);

                if (m_Clients.ContainsKey(client.UserId) && !m_ModelsToLoad.Contains(client.UserId))
                    Body.Destroy(m_Clients[client.UserId].Model.Body);

                if (m_ModelsToLoad.Contains(client.UserId))
                    m_ModelsToLoad.Remove(client.UserId);
            }
        }
    }
}

/*
    =----- Information -----=

    Connections:
        -> Connect to server <-
            -> Tell everyone you're connecting
            -> Everyone will now append your ID to the list of connected clients and to an array called modelsToLoad
            -> When connected <-
                -> Tell everyone you're connected
                -> Clients will now set connected to true
                -> Clients now spawn your model if they're not already spawned (check via modelsToLoad)

        -> Disconnect <-
            -> Tell everyone you're disconnecting via Nakama Presence
            -> Everyone will now remove your ID from the list of connected clients and from the array called modelsToLoad if exists

    Positions:
        1. PLAYER_POS Called -> Set Position of player
        2. OnUpdate -> Update position of player and use interpolation
*/