using SledgeLib;
using System.Numerics;

using Nakama;

public class IClientData {
    public string? SessionID;
    public string? Username;
    public string? Status;
    public string? UserID;
    public string? Node;
    public bool? Spawned;

    public ushort? Reason;

    public PlayerModel? PlayerModel;
    public Transform Transform = new Transform();
}

public static class Client {
    public static string? m_DeviceID = Guid.NewGuid().ToString();
    public static List<string> m_ModelsToLoad = new();

    public static ISession? m_Session = null;
    public static IClient? m_Connection = null;
    public static ISocket? m_Socket = null;
    public static bool m_Connected = false;

    public static void OnInitialize() {
        m_DeviceID = Guid.NewGuid().ToString();
    }

    public static void Tick() {
        if (!m_Connected || Match.m_Clients.Count <= 0) {
            return;
        }

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

        Vector3 playerPos = CPlayer.m_Position; //Player.GetPosition();
        Quaternion camRot = CPlayer.m_CameraAngles.Rotation;

        var posData = playerPos.X.ToString() + "," + playerPos.Y.ToString() + "," + playerPos.Z.ToString()
            + "," + camRot.X.ToString() + "," + camRot.Y.ToString() + "," + camRot.Z.ToString() + "," + camRot.W.ToString();

        // Every local game tick, send client's position data to Nakama
        m_Socket.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_MOVE, posData);
    }

    public static void SpawnPlayer(string clientID) {
        Match.m_Clients[clientID].PlayerModel = new PlayerModel();
        Match.m_Clients[clientID].PlayerModel!.Load();

        Match.m_Clients[clientID].Spawned = true;

        Log.General("{0} has spawned", clientID);
    }

    public static void OnStateChange(uint iState) {
        switch (iState) {
            case (uint)EGameState.Menu:
                if (Server.MatchID != null)
                    Match.Disconnect();
                break;
            case (uint)EGameState.Playing:
                // 1. Player joins server from the menu
                // 2. Connects to match instantly
                // 3. TODO: Force game to load map that server is using
                // 4. Player loads into the map
                // 5. EGameState changes to playing
                // 6. Spawn players in m_ModelsToLoad
                if (Server.MatchID != null) { // <-- Checking for Server.MatchID is the same as checking if m_Connected is true
                    m_Connected = true;
                    Log.General("Connected to server");

                    // 7. Notify every player local client has loaded in and should spawn their model
                    m_Socket!.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_SPAWN, "");
                    Discord.SetPresence(Discord.EDiscordState.Connected);
                }
                break;
            default:
                break;
        }
    }

    public static void OnClientLoad() {
        Discord.Client!.UpdateState($"Playing with {Match.m_Clients.Count()} others");
    }
}