using Nakama;

public static class Client {
    public static string m_DeviceID = "";

    public static ISession? m_Session = null;
    public static IClient? m_Connection = null;
    public static ISocket? m_Socket = null;
    public static bool m_Connected = false;
}