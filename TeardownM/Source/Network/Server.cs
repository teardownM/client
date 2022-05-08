using Newtonsoft.Json;

namespace TeardownM.Network;

public class Label {
    protected string? value { get; set; }
}

public class Mods {
    protected string? download_url { get; set; }
    protected string? name { get; set; }
    protected string? version { get; set; }
    protected string? is_workshop { get; set; }
}

public class Server {
    [JsonProperty("match_id")]
    public static string? MatchID { get; set; }

    [JsonProperty("authoritative")]
    public static bool? Authoritative { get; set; }

    [JsonProperty("label")]
    public static Label? Label { get; set; }

    [JsonProperty("tick_rate")]
    public static int TickRate { get; set; } = 24; // ! Hardcoded

    [JsonProperty("handler_name")]
    public static string? HandlerName { get; set; }

    [JsonProperty("gamemode")]
    public static string? Gamemode { get; set; }
    
    [JsonProperty("server_name")]
    public static string? ServerName { get; set; }

    [JsonProperty("map")]
    public static string? Map { get; set; }

    [JsonProperty("mods")]
    public static Mods? Mods { get; set; }
}