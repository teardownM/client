using Newtonsoft.Json;

public class Server {
    public class ServerLabel {
        public string? value { get; set; }
    }

    [JsonProperty("match_id")]
    public static string? MatchID { get; set; }

    [JsonProperty("authoritative")]
    public static bool? Authoritative { get; set; }

    [JsonProperty("label")]
    public static ServerLabel? Label { get; set; }

    [JsonProperty("tick_rate")]
    public static int TickRate { get; set; } = 24; // ! Hardcoded

    [JsonProperty("handler_name")]
    public static string? HandlerName { get; set; }

    public static string Gamemode = "Sandbox";
    public static string Map = "Villa Gordon";
    public static uint Clients = 1;
}