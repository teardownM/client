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

public class ModelSpawner {
    public uint m_iHandle;
    public string? m_VoxPath;
    public string m_VoxName = "";
    public float m_fScale = 0.5f;
}

public static class Client {
    public static string? m_DeviceID = Guid.NewGuid().ToString();
    public static List<string> m_PlayerModelsToLoad = new();

    // Used to load models dynamically in the game.
    // Voxel spawning only works in the tick function.
    public static List<ModelSpawner> m_ModelsToLoad = new();

    public static ISession? m_Session = null;
    public static IClient? m_Connection = null;
    public static ISocket? m_Socket = null;
    public static bool m_Connected = false;

    public static void OnInitialize() {
        m_DeviceID = Guid.NewGuid().ToString();
    }

    public static void Tick() {
        if (!m_Connected) {
            return;
        }

        ToolManager.PlayerTool();

        if (m_PlayerModelsToLoad.Any()) {
            foreach (var userId in m_PlayerModelsToLoad.ToList()) {
                // If it's not the local player, load in their vox player model
                if (userId != m_Session!.UserId) {
                    SpawnPlayer(userId);
                    m_PlayerModelsToLoad.Remove(userId);

                    Log.General("Loaded {0}'s model into the game", userId);
                }
            }
        }

        if (m_ModelsToLoad.Any())
        {
            foreach (var model in m_ModelsToLoad.ToList())
            {
                if (model != null && model.m_VoxPath != null)
                {
                    CShape.LoadVox(model.m_iHandle, model.m_VoxPath, model.m_VoxName, model.m_fScale);
                    m_ModelsToLoad.Remove(model);
                }
            }
        }

        if (m_Socket == null || m_Session == null || Server.MatchID == null)
            return;

        Vector3 playerPos = CPlayer.m_Position;
        Quaternion camRotation = CPlayer.m_CameraTransform.Rotation;

        var posData = Math.Round(playerPos.X, 3).ToString() + "," + Math.Round(playerPos.Y, 3).ToString() + "," + Math.Round(playerPos.Z, 3).ToString()
            + "," + Math.Round(camRotation.X, 3).ToString() + "," + Math.Round(camRotation.Y, 3).ToString() + "," + Math.Round(camRotation.Z, 3).ToString() + "," + Math.Round(camRotation.W, 3).ToString();

        // Every local game tick, send client's position data to Nakama
        m_Socket.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_MOVE, posData);

        if (CPlayer.m_M1Down)
        {
            m_Socket.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_SHOOTS, ToolManager.currentTool);
        }
    }

    public static void SpawnPlayer(string clientID) {
        Match.m_Clients[clientID].PlayerModel = new PlayerModel();
        Match.m_Clients[clientID].PlayerModel!.CreatePlayerTool();
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
                // 6. Spawn players in m_PlayerModelsToLoad
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