using SledgeLib;

using Nakama;
using Steamworks;

// Error: No m_Client or m_Socket found when authenticating
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
    private static string m_DeviceID { get; set; } = "";
    private static bool m_UseSteam = false;

    private static dBindCallback fJoinGameCallback = new dBindCallback(Client.JoinGame);
    private static dBindCallback fDisconnectCallback = new dBindCallback(Client.Disconnect);

    private static CBind? ConnectGameBind;
    private static CBind? JoinGameBind;
    private static CBind? DisconnectGameBind;

    private static dCallback fPostPlayerUpdate = new dCallback(Client.OnUpdate);
    private static CCallback? cb_PostPlayerUpdate;

    private static async void Authenticate() {
        if (Client.m_Client == null) {
            Log.Error("No client found when authenticating");
            return;
        }

        Client.m_Session = await Client.m_Client.AuthenticateDeviceAsync(m_DeviceID, m_DeviceID);
        if (!Client.m_Session.Created) {
            Log.Error("Authentication Failed");
            return;
        }

        Client.m_Socket = Socket.From(Client.m_Client);
        await Client.m_Socket.ConnectAsync(Client.m_Session);

        Log.General("TeardownNakama Running");
        Client.InitializeListeners();
    }

    private static void InitializeBindsAndCallbacks() {
        cb_PostPlayerUpdate = new CCallback(ECallbackType.PostPlayerUpdate, fPostPlayerUpdate);

        JoinGameBind = new CBind(EKeyCode.VK_N, fJoinGameCallback);
        DisconnectGameBind = new CBind(EKeyCode.VK_B, fDisconnectCallback);
        ConnectGameBind = new CBind(EKeyCode.VK_K, () => { // Mainly for debugging purposes to test disconnecting and reconnecting to the server
            Client.m_Client = new Nakama.Client("http", "127.0.0.1", 7350, "defaultkey");
            Authenticate();
        });

        Client.m_Client = new Nakama.Client("http", "127.0.0.1", 7350, "defaultkey");
        Authenticate();
    }

    public static void OnInitialize() {
        Log.General("OnInitialize");

        if (SteamAPI.Init()) {
            Log.General("SteamAPI initialized");
        } else {
            Log.Error("SteamAPI failed to initialize");
            return;
        }

        if (m_UseSteam) {
            m_DeviceID = SteamUser.GetSteamID().ToString();
        } else {
            m_DeviceID = Guid.NewGuid().ToString();
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
            m_DeviceID = SteamUser.GetSteamID().ToString();
        } else {
            m_DeviceID = Guid.NewGuid().ToString();
        }

        Client.m_Client = new Nakama.Client("http", "127.0.0.1", 7350, "defaultkey");
        Authenticate();
    }

    public static void OnShutdown() {
        Log.General("OnShutdown");
        
        if (cb_PostPlayerUpdate != null) { cb_PostPlayerUpdate.Unregister(); cb_PostPlayerUpdate = null; }
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