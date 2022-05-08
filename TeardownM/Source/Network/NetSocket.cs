using Nakama;

namespace TeardownM.Network;

public static class NetSocket {
    public static async Task<ISocket> CreateSocket(IClient Connection, ISession? Session) {
        ISocket? socket;
        
        socket = Socket.From(Connection);
        await socket.ConnectAsync(Session);
        
        socket.ReceivedError += ReceivedError;
        socket.Connected += Connected;
        socket.Closed += Closed;
        
        socket.ReceivedMatchState += MatchState.OnMatchState;
        socket.ReceivedMatchPresence += MatchState.OnMatchPresence;

        Log.Verbose("Socket created");
        
        return socket;
    }
    
    public static void ReceivedError(Exception e) {
        Log.Error("NetSocket ReceivedError: " + e.Message);
    }

    public static void Connected() {
        Log.Verbose("NetSocket Connected");
    }

    public static void Closed() {
        Log.Verbose("NetSocket Closed");
    }
}