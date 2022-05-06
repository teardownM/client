using SledgeLib;
using Nakama;
using Newtonsoft.Json;

public static class Network {
    public static string m_sAddress = "";
    public static int m_iPort = 0;
    public static string m_sMap = "";
    public static int m_ClientCount = 0;

    public static async Task<ISession> Connect(string sAddress, int iPort) {
        // Disconnect if we're already connected
        if (Server.MatchID != null)
            Disconnect();

        m_sAddress = sAddress;
        m_iPort = iPort;

        // Connect to Nakama server
        Client.m_Connection = new Nakama.Client("http", sAddress, iPort, "defaultkey");
        Client.m_Connection.Timeout = 1;

        if (Client.m_Connection == null) {
            Log.Error("Failed to create connection");
            return await Task.FromResult<ISession>(null!);
        }

        try {
            Client.m_Session = await Client.m_Connection.AuthenticateDeviceAsync(Client.m_DeviceID, Client.m_DeviceID);
            Log.Verbose("Successfully authenticated");
            Log.General("Your ID: {0}", Client.m_Session.UserId);
        } catch (Exception) {
            Log.Error("Failed to authenticate");
            return await Task.FromResult<ISession>(null!);
        }

        // Connect to the socket
        Client.m_Socket = Socket.From(Client.m_Connection);
        await Client.m_Socket.ConnectAsync(Client.m_Session);

        Client.m_Socket.ReceivedMatchState += MatchState.OnMatchState;
        Client.m_Socket.ReceivedMatchPresence += MatchState.OnMatchPresence;

        IApiRpc response = await Client.m_Connection.RpcAsync(Client.m_Session, "rpc_get_matches");
        JsonConvert.DeserializeObject<Server>(response.Payload.Substring(1, response.Payload.Length - 2));

        IMatch match = await Client.m_Socket.JoinMatchAsync(Server.MatchID);

        return Client.m_Session;
    }

    public static async void Disconnect() {
        // Don't disconnect if we're not connected
        if (Client.m_Socket == null || !Client.m_Connected)
            return;

        // Leave the match
        if (Server.MatchID != null) {
            await Client.m_Socket.LeaveMatchAsync(Server.MatchID);
            Log.Verbose("Successfully left match {0}", Server.MatchID!);
        }

        Log.General("Disconnected from server");
        Client.m_Connected = false;

        Discord.SetPresence(Discord.EDiscordState.MainMenu);

        m_iPort = 0;
        m_sAddress = "";
        m_sMap = "";

        await Client.m_Socket.CloseAsync();
    }
}