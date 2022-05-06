using SledgeLib;
using System.Reflection;
using static Keyboard;

public class TeardownM : ISledgeMod {
    /******************************************/
    /**************** Manifest ****************/
    /******************************************/
    public string GetName() { return "TeardownM"; }
    public string GetDescription() { return ""; }
    public string GetVersion() { return "0.0.1"; }
    public string GetAuthor() { return "github.com/teardownM"; }

    /******************************************/
    /*************** Variables ****************/
    /******************************************/
    public EGameState GameState;
    public static Dictionary<string, string> gHashes = new Dictionary<string, string>();

    private static string sRequiredVersion = "1.0.0";

    private static bool bMenuInitialized = false;

    private static Thread? tUpdateKeys;
    private static bool bReloadStart = false;
    private static bool bReloadEnd = false;
    private static bool bDebugMenu = false;

    /******************************************/
    /*************** Functions ****************/
    /******************************************/
    private static void UpdateKeys() {
        while (true) {
            if ((GetAsyncKeyState(((int)Keycode.VK_HOME)) & 1) == 1) {
                bDebugMenu = !bDebugMenu;
                if (bDebugMenu) {
                    Teardown.EnableDebugMenu();
                } else {
                    Teardown.DisableDebugMenu();
                }
            }
        }
    }

    private static void GetHashes() {
        gHashes.Add("Teardown", Sha256.Hash(File.ReadAllBytes(Teardown.sGamePath + "\\teardown.exe").ToString()!));
        // We also need the hash of this dll, we have to wait for the launcher so we can get the env var of the launcher path
    }

    /******************************************/
    /************* Load / Unload **************/
    /******************************************/
    private static void Initialize() {
        // This function gets called once the menu has loaded
        Teardown.Initialize();

        Log.General("Game Version: " + Teardown.GetGameVersion());
        if (Teardown.GetGameVersion() != sRequiredVersion) {
            Log.Error("Game version is not " + sRequiredVersion + "!");
            return;
        }

        // Initialize discord
        if (!Discord.Initialize()) {
            Log.Error("Failed to initialize Discord client");
            return;
        }

        // Create a new thread for updating keys
        tUpdateKeys = new Thread(UpdateKeys);
        tUpdateKeys.Start();

        Discord.SetPresence(Discord.EDiscordState.MainMenu);

        GetHashes();

        MainMenu.Update();
        bReloadStart = true;
        bMenuInitialized = true;

        Client.m_DeviceID = Guid.NewGuid().ToString();
    }

    public void Unload() {
        MainMenu.Revert();

        TeardownConsole.Close();
        Discord.Shutdown();
    }

    /******************************************/
    /*************** Callbacks ****************/
    /******************************************/
    [Callback(ECallbackType.StateChange)]
    public static void OnStateChange(EGameState GameState) {
        if (GameState == EGameState.Menu) {
            // Update the main menu (modify the lua file)
            if (!bMenuInitialized) {
                Initialize();
            }

            // Disconnect from the server if connected
            if (Client.m_Connected)
                Network.Disconnect();
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
}