using Nakama;
using Newtonsoft.Json;
using TeardownM.Miscellaneous;

namespace TeardownM.Network;

public static class Network {
    /******************************************/
    /*************** Variables ****************/
    /******************************************/
    public static string sAddress = "";
    public static int iPort = 0;

    public static bool bConnected { get; set; }
    public static ISession? Session = null;
    public static IClient? Connection = null;
    public static ISocket? Socket = null;

    /******************************************/
    /*************** Functions ****************/
    /******************************************/
    public static async Task<ISession> Connect(string sAddress, int iPort) {
        // Disconnect if we're already connected
        if (bConnected) Disconnect();

        Network.sAddress = sAddress;
        Network.iPort = iPort;

        ISession? session = null;

        // Connect to Nakama server
        Connection = new Nakama.Client("http", sAddress, iPort, "defaultkey");
        Connection.Timeout = 4;

        if (Connection == null) {
            Log.Error("Failed to create connection");
            return await Task.FromResult<ISession>(null!);
        }

        try {
            session = await Connection.AuthenticateDeviceAsync(Client.m_DeviceID, Client.m_DeviceID);
            Log.Verbose("Successfully authenticated");
            Log.General("Your ID: {0}", session.UserId);
        } catch (Exception) {
            Log.Error("Failed to authenticate");
            return await Task.FromResult<ISession>(null!);
        }

        if (session.HasExpired(DateTime.UtcNow.AddSeconds(1))) {
            Log.Error("Failed to create session");
            return await Task.FromResult<ISession>(null!);
        }

        Log.Verbose("Successfully authenticated");

        Task<ISocket> socket = NetSocket.CreateSocket(Connection, session);
        Socket = await socket;

        IApiRpc response = await Connection.RpcAsync(session, "rpc_get_matches");
        JsonConvert.DeserializeObject<Server>(response.Payload.Substring(1, response.Payload.Length - 2));

        IMatch match = await Socket.JoinMatchAsync(Server.MatchID);
        Log.Verbose("Joined match {0}", match.Id);

        bConnected = true;

        return session;
    }

    public static async void Disconnect() {
        // Don't disconnect if we're not connected
        if (Socket == null || !bConnected) return;

        // Leave the match
        if (Server.MatchID != null) {
            await Socket.LeaveMatchAsync(Server.MatchID);
            Log.Verbose("Successfully left match {0}", Server.MatchID!);
        }

        Log.General("Disconnected from server");
        bConnected = false;

        Discord.SetPresence(Discord.EDiscordState.MainMenu);

        iPort = 0;
        sAddress = "";

        await Socket.CloseAsync();
    }
}