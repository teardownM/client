using SledgeLib;
using Nakama;
using Newtonsoft.Json;

public static class Network {
    private static string m_sAddress = "";
    private static int m_iPort = 0;

    public static async Task<ISession> Connect(string sAddress, int iPort) {
        if (Server.MatchID != null)
            Disconnect();

        m_sAddress = sAddress;
        m_iPort = iPort;

        Client.m_Connection = new Nakama.Client("http", sAddress, iPort, "defaultKey");
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
            return await Task.FromResult<ISession>(null!);
        }

        Client.m_Socket = Socket.From(Client.m_Connection);
        await Client.m_Socket.ConnectAsync(Client.m_Session);

        // Client.m_Socket.ReceivedMatchState += OnMatchState;
        // Client.m_Socket.ReceivedMatchPresence += OnMatchPresence;

        IApiRpc response = await Client.m_Connection.RpcAsync(Client.m_Session, "rpc_get_matches");
        JsonConvert.DeserializeObject<Server>(response.Payload.Substring(1, response.Payload.Length - 2));

        IMatch match = await Client.m_Socket.JoinMatchAsync(Server.MatchID);
        Log.Verbose("Successfully connected to match {0}", Server.MatchID!);
        Client.m_Connected = true;

        return Client.m_Session;
    }

    public static async void Disconnect() {
        if (Client.m_Socket == null)
            return;

        if (Server.MatchID != null) {
            await Client.m_Socket.LeaveMatchAsync(Server.MatchID);
            Log.Verbose("Successfully left match {0}", Server.MatchID!);
        }

        Log.General("Disconnected from server");

        Discord.SetPresence(Discord.EDiscordState.MainMenu);

        await Client.m_Socket.CloseAsync();
    }
}