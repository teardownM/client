using DiscordRPC;

namespace TeardownM.Miscellaneous;

public static class Discord {
    public enum EDiscordState : uint {
        MainMenu,
        Connected,
        Connecting
    }

    private static DiscordRpcClient? Client;
    private static Dictionary<EDiscordState, RichPresence> Presences = new Dictionary<EDiscordState, RichPresence>();

    public static bool Initialize() {
        Presences.Add(EDiscordState.MainMenu, new RichPresence {
            State = "In Main-Menu",
            Timestamps = new Timestamps() {
                Start = DateTime.UtcNow
            },
            Assets = new Assets() {
                LargeImageKey = "logo",
                LargeImageText = "Teardown Multiplayer"
            },
        });

        Presences.Add(EDiscordState.Connected, new RichPresence {
            State = "In Game",
            Timestamps = new Timestamps() {
                Start = DateTime.UtcNow
            },
            Assets = new Assets() {
                LargeImageKey = "logo",
                LargeImageText = "Teardown Multiplayer"
            },
        });

        Client = new DiscordRpcClient("929980695663747083", 0);
        Client.RegisterUriScheme();

        if (!Client.Initialize()) {
            Log.Error("Failed to initialize Discord client");
            return false;
        }

        Log.Verbose("Discord client initialized");

        return true;
    }

    public static void Update() {
        if (Client != null)
            Client.Invoke();
    }

    public static void Shutdown() {
        if (Client != null)
            Client.Dispose();
    }

    public static void SetPresence(EDiscordState state) {
        if (Client != null)
            Client.SetPresence(Presences[state]);
    }
}