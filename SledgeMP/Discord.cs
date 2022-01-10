using DiscordRPC;
using SledgeLib;

public static class Discord {
    public enum EDiscordState : uint {
        MainMenu,
        Connected,
        Connecting
    }

    public static DiscordRpcClient? Client;

    public static Dictionary<EDiscordState, RichPresence> Presences = new Dictionary<EDiscordState, RichPresence>();

    public static bool Initialize() {
        Random rand = new Random();

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
            Details = $"Playing {Server.Gamemode} on {Server.Map}",
            State = $"Playing with {Server.Clients} others",
            Party = new Party() {
                ID = Secrets.CreateFriendlySecret(rand),
                Privacy = Party.PrivacySetting.Public
            },
            Secrets = new Secrets() {
                JoinSecret = Secrets.CreateFriendlySecret(rand),
            },
            Assets = new Assets() {
                LargeImageKey = "logo",
                LargeImageText = "Teardown Multiplayer",
            },
            Timestamps = new Timestamps() {
                Start = DateTime.UtcNow,
            }
        });

        Presences.Add(EDiscordState.Connecting, new RichPresence {
            State = "Connecting to Sandbox Server",
            Timestamps = new Timestamps() {
                Start = DateTime.UtcNow
            },
            Assets = new Assets() {
                LargeImageKey = "logo",
                LargeImageText = "Teardown Multiplayer"
            },
        });

        Client = new DiscordRpcClient("929980695663747083", pipe: 0);

        Client.OnJoin += (sender, e) => {
            Log.General("Join: " + e.Secret);
        };

        Client.OnJoinRequested += (sender, e) => {
            Log.General("Join requested: " + e.User.ID);
        };

        Client.Subscribe(EventType.Join | EventType.JoinRequest);
        Client.RegisterUriScheme();

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