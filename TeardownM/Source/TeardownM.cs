// Documentation: https://github.com/44lr/sledge/wiki/API-Documentation

using SledgeLib;
using Nakama;

public class TeardownM : ISledgeMod {
    public string GetName() { return "TeardownM"; }
    public string GetDescription() { return ""; }
    public string GetVersion() { return "0.0.1"; }
    public string GetAuthor() { return "github.com/teardownM"; }

    public EGameState GameState;

    public async void Load() {
        if (!Discord.Initialize()) {
            Log.Error("Failed to initialize Discord client");
            return;
        }

        Discord.SetPresence(Discord.EDiscordState.MainMenu);

        /* The following is temporary code */
        Client.m_DeviceID = Guid.NewGuid().ToString();

        Log.General("Connecting to server");
        ISession session = await Network.Connect("127.0.0.1", 7350);
        if (session == null) {
            Log.Error("Failed to connect to server");
        } else {
            Log.General("Connected to server");
        }
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