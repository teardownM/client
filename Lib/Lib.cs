using SledgeLib;

using Nakama;
using Steamworks;

// Error: No m_Connection or m_Socket found when authenticating
// Logger error while formatting: Input string was not in a correct format

class IPlayer : IUserPresence {
    public uint m_Body { get; set; }
    public uint m_Shape { get; set; }
    public bool voxelLoaded { get; set; }
    public bool Persistence { get; set; }
    public string? SessionId { get; set; }
    public string? Status { get; set; }
    public string? Username  { get; set; }
    public string? UserId { get; set; }
}

public class TeardownNakama {
    private static bool m_UseSteam = false;

    private static dBindCallback fJoinGameCallback = new dBindCallback(Client.JoinGame);
    private static dBindCallback fDisconnectCallback = new dBindCallback(Client.Disconnect);

    private static dCallback fPostUpdate = new dCallback(OnUpdate);
    private static CCallback? cb_PostUpdate;

    private static dCallback fPlayerSpawn = new dCallback(Client.OnLevelLoad);
    private static CCallback? cb_PlayerSpawn;

    private static CBind? ConnectGameBind;
    private static CBind? JoinGameBind;
    private static CBind? DisconnectGameBind;

    private static void InitializeBindsAndCallbacks() {
        cb_PostUpdate = new CCallback(ECallbackType.PostUpdate, fPostUpdate); // TODO: Change Update to Tick
        cb_PlayerSpawn = new CCallback(ECallbackType.PlayerSpawn, fPlayerSpawn);

        JoinGameBind = new CBind(EKeyCode.VK_N, fJoinGameCallback);
        DisconnectGameBind = new CBind(EKeyCode.VK_B, fDisconnectCallback);
        ConnectGameBind = new CBind(EKeyCode.VK_K, () => {
            Client.Connect("127.0.0.1", 7350);
        });
    }

    public static void OnInitialize() {
        if (SteamAPI.Init()) {
            Log.General("SteamAPI initialized");
        } else {
            Log.Error("SteamAPI failed to initialize");
            return;
        }

        if (m_UseSteam) {
            Client.m_DeviceID = SteamUser.GetSteamID().ToString();
        } else {
            Client.m_DeviceID = Guid.NewGuid().ToString();
        }

        InitializeBindsAndCallbacks();
    }

    public static void OnReload() {
        if (Client.m_Socket != null) {
            if (Client.m_Socket.IsConnected) {
                Client.Disconnect();
            }

            Client.m_Socket.Dispose();
            Client.m_Socket = null;
        }

        if (m_UseSteam) {
            Client.m_DeviceID = SteamUser.GetSteamID().ToString();
        } else {
            Client.m_DeviceID = Guid.NewGuid().ToString();
        }

        InitializeBindsAndCallbacks();
    }

    public static void OnUpdate() {
        Client.OnUpdate();
    }

    public static void OnShutdown() {
        if (cb_PostUpdate != null) { cb_PostUpdate.Unregister(); cb_PostUpdate = null; }
        if (cb_PlayerSpawn != null) { cb_PlayerSpawn.Unregister(); cb_PlayerSpawn = null; }
        if (JoinGameBind != null) { JoinGameBind.Unregister(); }
        if (DisconnectGameBind != null) { DisconnectGameBind.Unregister(); }
        if (ConnectGameBind != null) { ConnectGameBind.Unregister(); }

        if (Client.m_Socket != null) {
            if (Client.m_Socket.IsConnected) {
                Client.Disconnect();
            }

            Client.m_Socket.Dispose();
            Client.m_Socket = null;
        }

        SteamAPI.Shutdown();
    }
}