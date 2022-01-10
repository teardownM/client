using DiscordRPC;

public static class Discord {
    public enum EDiscordState : uint {
        MainMenu,
        Connected,
        Connecting
    }

    private static DiscordRpcClient? Client;

    public static Dictionary<EDiscordState, RichPresence> Presences = new Dictionary<EDiscordState, RichPresence>();

    public static bool Initialize() {
        Presences.Add(EDiscordState.MainMenu, new RichPresence {
            State = "In Main-Menu",
            Timestamps = new Timestamps() {
            Start = DateTime.UtcNow
        }});

        Presences.Add(EDiscordState.Connected, new RichPresence {
            State = "Playing (server_name) with x people",
            Timestamps = new Timestamps() {
            Start = DateTime.UtcNow
        }});

        Presences.Add(EDiscordState.Connecting, new RichPresence {
            State = "Connecting to (server_name)",
            Timestamps = new Timestamps() {
            Start = DateTime.UtcNow
        }});

        Client = new DiscordRpcClient("929980695663747083", pipe: -1);

        Client.OnReady += (sender, msg) => {
            Console.WriteLine("Connected to discord with user {0}", msg.User.Username);
        };

        if (!Client.Initialize()) {
            Console.WriteLine("Discord RPC failed to initialize");
            return false;
        } else {
            Console.WriteLine("Discord RPC initialized");
        }

        return true;
    }

    public static void SetPresence(EDiscordState state) {
        Client!.SetPresence(Presences[state]);
    }

    public static void Update() {
        if (Client != null)
            Client.Invoke();
    }
    
    public static void Shutdown() {
        if (Client != null)
            Client.Dispose();
    }
}