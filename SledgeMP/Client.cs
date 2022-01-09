using SledgeLib;
using System.Numerics;

using Steamworks;
using Nakama.TinyJson;

using Newtonsoft.Json.Linq;
using System.Text;

using Newtonsoft.Json;

using Nakama;

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
    public bool? Spawn;
    public bool? CanSpawn;

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

    private static Dictionary<string, IClientData> m_Clients = new();

    private static string? m_DeviceID = "";

    private static ISession? m_Session = null;
    private static IClient? m_Connection = null;
    private static ISocket? m_Socket = null;

    private static bool m_Connecting = false;
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

        try {
            m_Session = await m_Connection.AuthenticateDeviceAsync(m_DeviceID, m_DeviceID);
        } catch (Exception) {
            return await Task.FromResult<ISession>(null!);
        }

        Log.General("Your ID: {0}", m_Session.UserId);

        m_Socket = Socket.From(m_Connection);
        await m_Socket.ConnectAsync(m_Session);
        m_Socket.ReceivedMatchState += OnMatchState;
        m_Socket.ReceivedMatchPresence += OnMatchPresence;

        // Join the server
        m_Connecting = true;

        IApiRpc response = await m_Connection.RpcAsync(m_Session, "rpc_get_matches");
        JsonConvert.DeserializeObject<Server>(response.Payload.Substring(1, response.Payload.Length - 2));

        IMatch match = await m_Socket.JoinMatchAsync(Server.MatchID);

        foreach (IUserPresence client in match.Presences) {
            if (client.UserId != m_Session.UserId)
                CreatePlayer(client);
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
            m_Connecting = false;
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
        if (!m_Connected)
            return;

        for (int i = 0; i < m_ModelsToLoad.Count; i++) {
            if (m_Clients[m_ModelsToLoad[i]].Spawn == false || m_Clients[m_ModelsToLoad[i]].CanSpawn == false)
                continue;

            SpawnPlayer(m_ModelsToLoad[i]);
            m_ModelsToLoad.RemoveAt(i);// ! Possible issue
        }
    }

    public static void CreatePlayer(IUserPresence player) {
        Log.General("Creating player {0}", player.UserId);

        IClientData newPlayer = new() {
            SessionID = player.SessionId,
            Username = player.UserId,
            Status = player.Status,
            UserID = player.UserId,
            Spawn = false
        };

        m_Clients.Add(player.UserId, newPlayer);
        m_ModelsToLoad.Add(player.UserId);
    }

    public static void SpawnPlayer(string clientID) {
        Log.General("{0} has spawned", m_Clients[clientID].Username!);

        uint body = Body.Create();

        m_Clients[clientID].Model = new Model() {
            Body = body,
            sBody = Shape.Create(body),
        };

        m_Clients[clientID].Transform.Position = new Vector3(0, 0, 0);

        Body.SetPosition(m_Clients[clientID].Model.Body, new Vector3(0, 0, 0));
        Body.SetRotation(m_Clients[clientID].Model.Body, new Quaternion(1, 1, 1, 1));

        m_Clients[clientID].CanSpawn = false;

        bool success = Shape.LoadVox(m_Clients[clientID].Model.sBody, "Assets/Models/Player.vox", "", 1.0f);

        Vector3 pos = Player.GetPosition();

        m_Socket!.SendMatchStateAsync(Server.MatchID, (Int64)OPCODE.PLAYER_SPAWN, new Dictionary<string, dynamic> {
            { "user_id", m_Session!.UserId },
            { "currentX", pos.X },
            { "currentY", pos.Y },
            { "currentZ", pos.Z }
        }.ToJson());

        Log.General("Sending player spawn to {0}", clientID);
    }

    public static void OnStateChange(uint iState) {
        switch (iState) {
            case (uint)EGameState.Menu:
                if (Server.MatchID != null)
                    Disconnect();
                break;
            case (uint)EGameState.Playing:
                if (!m_Connecting)
                    return;

                Log.General("Connected to server");
                m_Connecting = false;
                m_Connected = true;

                Vector3 pos = Player.GetPosition();

                m_Socket!.SendMatchStateAsync(Server.MatchID, (Int64)OPCODE.PLAYER_SPAWN, new Dictionary<string, dynamic> {
                    { "user_id", m_Session!.UserId },
                    { "currentX", pos.X },
                    { "currentY", pos.Y },
                    { "currentZ", pos.Z }
                }.ToJson());

                Log.General("Sending PLAYER_SPAWN from OnStateChange");
                break;
            default:
                break;
        }
    }

    /*
        Client1 spawns in and sends PLAYER_SPAWN
        Rest of the clients recieve that and say, "Hey, I'll spawn you in and send you a PLAYER_SPAWN"
    */

    public static void OnMatchState(IMatchState state) {
        switch (state.OpCode) {
            case (Int64)OPCODE.PLAYER_MOVE:
                Log.General("Received player transform");
                break;
            case (Int64)OPCODE.PLAYER_SPAWN:
                string id = Encoding.Default.GetString(state.State).Split("\"", 3)[1];
                if (id == m_Session!.UserId)
                    return;

                Log.General("Received player spawn from {0}", id);
                m_Clients[id].Spawn = true;

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

                CreatePlayer(client);
            }
        } else if (presence.Leaves.Any()) {
            foreach (IUserPresence? client in presence.Leaves) {
                if (client.UserId == m_Session!.UserId)
                    continue;

                m_Clients.Remove(client.UserId);

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