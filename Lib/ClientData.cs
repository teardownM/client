public class IClientData {
    public string session_id { get; set; } = "";
    public string username { get; set; } = "";
    public string user_id { get; set; } = "";
    public string node { get; set; } = "";

    public int reason { get; set; }

    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
}