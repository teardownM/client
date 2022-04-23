// Documentation: https://github.com/44lr/sledge/wiki/API-Documentation

/*
TODO: Watch teardown's lua directory for changes and compare hash with this one.
        If a file has been changed and the hash is different, overwrite them with our own.
        If we don't compare hashes then when we replace the files, the file watcher will keep triggering.
TODO: Send hash of lua files to server
        If the hash is invalid or the server doesn't receive it, the server will not register the player.
TODO: Send hash of client files to server
        If the hash is invalid or the server doesn't receive it, the server will not register the player.

TODO: Add Nakama
TODO: Figure out how we're going to call any functions that are in other people's mods (For example they might have "OnPlayerConnected(...)", we need to call that)
*/

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