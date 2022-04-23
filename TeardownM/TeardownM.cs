// Documentation: https://github.com/44lr/sledge/wiki/API-Documentation

using SledgeLib;

public class TeardownM : ISledgeMod {
    public string GetName() { return "TeardownM"; }
    public string GetDescription() { return ""; }
    public string GetVersion() { return "0.0.1"; }
    public string GetAuthor() { return "github.com/teardownM"; }

    public void Load() {
        if (!Discord.Initialize()) {
            Log.Error("Failed to initialize Discord client");
            return;
        }

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