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
    private static bool m_UseSteam = true;

    private static dBindCallback JoinGameCallbackFunc = new dBindCallback(Client.JoinGame);
    private static dCallback PostPlayerUpdateCallbackFunc = new dCallback(Client.OnUpdate);

    private static CBind? JoinGameBind;
    private static CCallback? PostPlayerUpdateCallback;

    private static async void Authenticate() {
        if (Client.m_Client == null) {
            Log.Error("No client found when authenticating");
            return;
        }

        Log.General("Attempting to authenticate with device id of: {0}", m_DeviceID);

        Client.m_Session = await Client.m_Client.AuthenticateDeviceAsync(m_DeviceID, m_DeviceID);
        Client.m_Socket = Socket.From(Client.m_Client);
        await Client.m_Socket.ConnectAsync(Client.m_Session);

        Log.General("Nakama session and Client.m_Socket created");
        Client.InitializeListeners();
    }

    public static void Init() {
        if (m_UseSteam) {
            if (SteamAPI.Init()) {
                Log.General("SteamAPI initialized");
            } else {
                Log.Error("SteamAPI failed to initialize");
                return;
            }

            m_DeviceID = SteamUser.GetSteamID().ToString();
        } else {
            m_DeviceID = Guid.NewGuid().ToString();
        }

        JoinGameBind = new CBind(EKeyCode.VK_N, Client.JoinGame);
        PostPlayerUpdateCallback = new CCallback(ECallbackType.PostPlayerUpdate, PostPlayerUpdateCallbackFunc);

        // Creates Nakama client
        Client.m_Client = new Nakama.Client("http", "127.0.0.1", 7350, "defaultkey");
        Authenticate();
    }

    public static void Shutdown() {
        Log.General("Teardown Nakama shutting down");

        if (m_UseSteam)
            SteamAPI.Shutdown();
    }
}