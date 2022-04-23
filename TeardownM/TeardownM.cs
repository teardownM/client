// Documentation: https://github.com/44lr/sledge/wiki/API-Documentation

using SledgeLib;

public class TeardownM : ISledgeMod {
    public string GetName() { return "TeardownM"; }
    public string GetDescription() { return ""; }
    public string GetVersion() { return "0.0.1"; }
    public string GetAuthor() { return "github.com/teardownM"; }

    private static void CopyLuaFiles() {
        string sTeardownDirectory = Directory.GetCurrentDirectory() + @"\mods\";
        string sCurrentDirectory = Environment.GetEnvironmentVariable("SLEDGE_ROOT_DIR") + @"\mods\client\TeardownM\lua";

        foreach (string sFolder in Directory.GetDirectories(sCurrentDirectory)) {
            string sFolderName = Path.GetFileName(sFolder);
            string sDestination = sTeardownDirectory + sFolderName;

            if (Directory.Exists(sDestination)) {
                Directory.Delete(sDestination, true);
            }

            Directory.CreateDirectory(sDestination);

            foreach (string sFile in Directory.GetFiles(sFolder)) {
                File.Copy(sFile, sDestination + @"\" + Path.GetFileName(sFile));
            }
        }
    }

    [LuaFunction("Test")]
    public static void Test(string message) {
        Log.General("{0}", message);
    }

    public void Load() {
        if (!Discord.Initialize()) {
            Log.Error("Failed to initialize Discord client");
            return;
        }

        CopyLuaFiles();

        Discord.SetPresence(Discord.EDiscordState.MainMenu);
    }

    public void Unload() {
        Discord.Shutdown();
    }

    [Callback(ECallbackType.StateChange)]
    public static void OnStateChange(EGameState GameState) {
        Log.Verbose("Changed state to " + GameState.ToString());
    }

    [Callback(ECallbackType.PostUpdate)]
    public static void OnPostUpdate() {
        Discord.Update();
    }
}