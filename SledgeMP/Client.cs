using SledgeLib;
using System.Numerics;

using Newtonsoft.Json;

using Nakama;

public static class IClientData {
    public static string? SessionID;
    public static string? Username;
    public static string? Status;
    public static string? UserID;
    public static string? Node;

    public static ushort? Reason;

    public static class Model {
        public static uint? bBody;
        public static uint? bHead;
        public static uint? bLeftArm;
        public static uint? bLeftLeg;
        public static uint? bRightArm;
        public static uint? bRightLeg;

        public static uint? sBody;
        public static uint? sHead;
        public static uint? sLeftArm;
        public static uint? sLeftLeg;
        public static uint? sRightArm;
        public static uint? sRightLeg;
    }

    public static class Transform {
        public static Vector3? Position;
        public static Quaternion? Rotation;
    }
}

public static class Client {
    public class ServerLabel {
        public string? value { get; set; }
    }

    internal class Server {
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

    private static string? m_DeviceID = "";

    private static ISession? m_Session = null;
    private static IClient? m_Connection = null;
    private static ISocket? m_Socket = null;

    private static bool m_Connecting = false;

    private enum OPCODE : ushort {
        PlayerTransform = 1,
        PlayerSpawn = 2,
        PlayerSpawnAll = 3
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

        m_Socket = Socket.From(m_Connection);
        await m_Socket.ConnectAsync(m_Session);
        m_Socket.ReceivedMatchState += OnMatchState;

        // Join the server
        m_Connecting = true;

        IApiRpc response = await m_Connection.RpcAsync(m_Session, "rpc_get_matches");
        JsonConvert.DeserializeObject<Server>(response.Payload.Substring(1, response.Payload.Length - 2));

        await m_Socket.JoinMatchAsync(Server.MatchID);

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

            Log.General("Disconnected from server");

            if (Game.GetState() != EGameState.Menu)
                Game.SetState(EGameState.Menu);
        } else {
            Log.General("Not connected to server");
        }
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
                break;
            default:
                break;
        }
    }

    public static void OnMatchState(IMatchState state) {
        switch (state.OpCode) {
            case (ushort)OPCODE.PlayerTransform:
                Log.General("Received player transform");
                break;
            case (ushort)OPCODE.PlayerSpawn:
                Log.General("Spawned");
                break;
            case (ushort)OPCODE.PlayerSpawnAll:
                Log.General("Spawned all");
                break;
            default:
                break;
        }
    }
}