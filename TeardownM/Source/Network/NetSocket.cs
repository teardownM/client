using System.Net.Sockets;
using Nakama;
using Socket = Nakama.Socket;

namespace TeardownM.Network;

public static class NetSocket {
    public static Task<ISocket> CreateSocket(IClient Connection) {
        ISocket socket = Socket.From(Connection);

        socket.ReceivedError += ReceivedError;
        socket.Connected += Connected;
        socket.Closed += Closed;
        
        socket.ReceivedMatchState += MatchState.OnMatchState;
        socket.ReceivedMatchPresence += MatchState.OnMatchPresence;

        Log.Verbose("Socket created");
        
        return Task.FromResult(socket);
    }
    
    private static void ReceivedError(Exception e) {
        Log.Error("NetSocket ReceivedError: " + e.Message);
    }

    private static void Connected() {
        Log.Verbose("NetSocket Connected");
    }

    private static void Closed() {
        Log.Verbose("NetSocket Closed");
    }
}