using SledgeLib;
using TeardownM.Miscellaneous;
using static TeardownM.Miscellaneous.Keyboard;
using TeardownM.Network;

namespace TeardownM;

public class TeardownM : ISledgeMod {
    /******************************************/
    /**************** Manifest ****************/
    /******************************************/
    public string GetName() { return "TeardownM"; }
    public string GetDescription() { return "A Multiplayer Mod for Teardown"; }
    public string GetVersion() { return "0.0.1"; }
    public string GetAuthor() { return "github.com/teardownM"; }

    /******************************************/
    /*************** Variables ****************/
    /******************************************/
    public EGameState GameState;
    public static Dictionary<string, string> gHashes = new Dictionary<string, string>();

    private static string sRequiredVersion = "1.0.0";
    private static bool bMenuInitialized;

    private static bool bReloadStart;
    private static bool bReloadEnd;
    private static bool bDebugMenu;

    /******************************************/
    /*************** Functions ****************/
    /******************************************/
    private static void GetHashes() {
        gHashes.Add("Teardown", Sha256.Hash(File.ReadAllBytes(Teardown.sGamePath + "\\teardown.exe").ToString()!));
        string sLuaFinalHash = "";

        foreach (string sFile in Directory.GetFiles(Teardown.sGamePath + "\\mods\\TeardownM")) {
            sLuaFinalHash += Sha256.Hash(File.ReadAllBytes(sFile).ToString()!);
        }

        gHashes.Add("TeardownMLua", Sha256.Hash(sLuaFinalHash));

        // We also need the hash of this dll, we have to wait for the launcher so we can get the env var of the launcher path
    }

    /******************************************/
    /************* Load / Unload **************/
    /******************************************/
    public void Load() {
        // This function gets called once the menu has loaded
        if (!Teardown.Initialize()) {
            Log.Error("Failed to initialize TeardownM");
            return;
        }

        // Make sure the required version is met
        string sVersion = Teardown.GetGameVersion();
        if (sVersion != sRequiredVersion) {
            Log.Error("Version {0} != {1}", Teardown.GetGameVersion(), sVersion);
            return;
        }

        // Initialize discord
        if (!Discord.Initialize()) {
            Log.Error("Failed to initialize Discord client");
            return;
        }

        Discord.SetPresence(Discord.EDiscordState.MainMenu);

        Client.m_DeviceID = Guid.NewGuid().ToString();
        GetHashes();
    }

    public void Unload() {
        if (!MainMenu.Revert()) {
            Log.Error("Failed to revert to main menu");
            return;
        }

        TeardownConsole.Close();
        Discord.Shutdown();
    }

    /******************************************/
    /*************** Callbacks ****************/
    /******************************************/
    [Callback(ECallbackType.StateChange)]
    public static void OnStateChange(EGameState GameState) {
        if (GameState == EGameState.Menu) {
            if (!bMenuInitialized) {
                if (!MainMenu.Update()) {
                    Log.Error("Failed to initialize main menu");
                    return;
                }

                bReloadStart = true;
                bMenuInitialized = true;
                
                Log.General("TeardownM Initialized");
            }

            // Disconnect from the server if connected
            if (Network.Network.bConnected) Network.Network.Disconnect();
        }
    }

    [Callback(ECallbackType.PostUpdate)]
    public static void OnPostUpdate() {
        Discord.Update();

        // Revert the bytes of the debug menu and uireload
        if (!bReloadStart && bReloadEnd) {
            MainMenu.RevertMemory();
            bReloadEnd = false;
        }

        // Reload the UI
        if (bReloadStart) {
            MainMenu.ReloadUI();
            bReloadStart = false;
            bReloadEnd = true;
        }
    }

    [Callback(ECallbackType.PreUpdate)]
    public static void OnPreUpdate() {
        if (!Teardown.IsFocused()) return;

        if ((GetAsyncKeyState((int) Keycode.VK_HOME) & 1) == 1) {
            bDebugMenu = !bDebugMenu;

            if (bDebugMenu) {
                Teardown.EnableDebugMenu();
            } else {
                Teardown.DisableDebugMenu();
            }
        }
    }
}