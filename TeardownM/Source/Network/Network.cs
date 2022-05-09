using System.Text.RegularExpressions;
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

        if (iPort < 1 || iPort > 65535) {
            Log.Error("Port must be between 1 and 65535.");
            return null!;
        }
        
        // IP Regex
        var rIP = new Regex(@"^(?:\d{1,3}\.){3}\d{1,3}$");

        if (!rIP.IsMatch(sAddress)) {
            Log.Error("Invalid IP address.");
            return null!;
        }

        Network.sAddress = sAddress;
        Network.iPort = iPort;

        ISession? session = null;

        // Connect to Nakama server
        Connection = new Nakama.Client("http", sAddress, iPort, "defaultkey");
        Connection.Timeout = 1;
        
        Task<ISocket> socket = NetSocket.CreateSocket(Connection);
        Socket = await socket;

        if (Connection == null) {
            Log.Error("Failed to create connection");
            return await Task.FromResult<ISession>(null!);
        }
        
        CancellationTokenSource sCancellationTokenS = new CancellationTokenSource();
        CancellationToken cCancellationToken = sCancellationTokenS.Token;
        
        RetryConfiguration rRetryConfiguration = new RetryConfiguration(1, 2);

        try {
            session = await Connection.AuthenticateDeviceAsync(Client.m_DeviceID, Client.m_DeviceID, retryConfiguration: rRetryConfiguration, canceller: cCancellationToken);
        } catch (HttpRequestException) {
            Log.Error("Server not found");
            return await Task.FromResult<ISession>(null!);
        } catch (Exception) {
            Log.Error("Failed Connecting to {0}:{1}", sAddress, iPort);
            return await Task.FromResult<ISession>(null!);
        }

        if (session == null) {
            Log.Error("Failed Connecting to {0}:{1}", sAddress, iPort);
            return await Task.FromResult<ISession>(null!);
        }

        Log.Verbose("Successfully authenticated");

        await Socket.ConnectAsync(session);

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