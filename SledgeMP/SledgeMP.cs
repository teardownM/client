// Documentation: https://github.com/44lr/sledge/wiki/API-Documentation

using SledgeLib;
using Steamworks;
using Nakama;

using DiscordRPC;

/* Main things go here such as callbacks */

public class SledgeMP {
    // private static string m_IP = "159.89.18.92";
    private static string m_IP = "127.0.0.1";
    private static ushort m_Port = 7350;

    private static CCallback? cb_PrePlayerUpdate;
    private static CCallback? cb_PostPlayerUpdate;
    private static CCallback? cb_PreUpdate;
    private static CCallback? cb_PostUpdate;
    private static CCallback? cb_PlayerSpawn;
    private static CCallback? cb_StateChange;

    private static CBind? ConnectGameBind;
    private static CBind? DisconnectGameBind;

    private static bool m_bSteamInitialized = false;

    private static dBindCallback fDisconnectCallback = new dBindCallback(Client.Disconnect);
    private static dUIntCallback fStateChange = new dUIntCallback((uint iState) => {
        Client.OnStateChange(iState);
    });

    private static dCallback cb_PostPlayerUpdateFunc = new dCallback(() => {
    
    });

    private static dCallback cb_PostUpdateFunc = new dCallback(() => {
        Client.OnUpdate();
        Discord.Update();
    });

    private static dCallback cb_PrePlayerUpdateFunc = new dCallback(() => {
        
    });

    private static dCallback cb_PreUpdateFunc = new dCallback(() => {
    
    });

    private static dCallback cb_PlayerSpawnFunc = new dCallback(() => {

    });

    public static void OnInitialize() {
        if (Game.GetState() == EGameState.Unknown || Game.GetState() == EGameState.Splash)
            Game.SetState(EGameState.Menu);
        
        if (SteamAPI.Init()) {
            Log.General("SteamAPI initialized");
            m_bSteamInitialized = true;
        } else {
            Log.Error("SteamAPI failed to initialize");
            return;
        }

        if (!Discord.Initialize()) {
            Log.Error("Discord RPC failed to initialize");
            return;
        }

        Client.OnInitialize();

        cb_PostPlayerUpdate = new CCallback(ECallbackType.PostPlayerUpdate, cb_PostPlayerUpdateFunc);
        cb_PostUpdate = new CCallback(ECallbackType.PostUpdate, cb_PostUpdateFunc);
        cb_PrePlayerUpdate = new CCallback(ECallbackType.PrePlayerUpdate, cb_PrePlayerUpdateFunc);
        cb_PreUpdate = new CCallback(ECallbackType.PreUpdate, cb_PrePlayerUpdateFunc);
        cb_PlayerSpawn = new CCallback(ECallbackType.PlayerSpawn, cb_PlayerSpawnFunc);
        cb_StateChange = new CCallback(ECallbackType.StateChange, fStateChange);

        DisconnectGameBind = new CBind(EKeyCode.VK_B, fDisconnectCallback);
        ConnectGameBind = new CBind(EKeyCode.VK_K, async () => {
            ISession connection = await Client.Connect(m_IP, m_Port);
            if (connection == null) {
                Log.General("Failed connecting to server: {0}:{1}", m_IP, m_Port);
            } else {
                Log.General("Connecting to server: {0}:{1}", m_IP, m_Port);
            }
        });

        Discord.SetPresence(Discord.EDiscordState.MainMenu);
    }

    public static void OnReload() {
        Log.General("[ SledgeMP Reloaded ]");
    }

    public static void OnShutdown() {
        if (cb_PrePlayerUpdate != null) { cb_PrePlayerUpdate.Unregister(); cb_PrePlayerUpdate = null; }
        if (cb_PostPlayerUpdate != null) { cb_PostPlayerUpdate.Unregister(); cb_PostPlayerUpdate = null; }
        if (cb_PreUpdate != null) { cb_PreUpdate.Unregister(); cb_PreUpdate = null; }
        if (cb_PostUpdate != null) { cb_PostUpdate.Unregister(); cb_PostUpdate = null; }
        if (cb_PlayerSpawn != null) { cb_PlayerSpawn.Unregister(); cb_PlayerSpawn = null; }
        if (cb_StateChange != null) { cb_StateChange.Unregister(); cb_StateChange = null; }

        Log.General("SledgeMP Shutdown");

        if (m_bSteamInitialized)
            SteamAPI.Shutdown();

        Discord.Shutdown();
    }
}