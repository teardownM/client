using SledgeLib;
using Nakama;

public class LuaFunctions {
    /**************************/
    /*    Logging Functions   */
    /**************************/
    [LuaFunction("LogVerbose")]
    public static void LogVerbose(string message) {
        Log.Verbose(message);
    }

    [LuaFunction("LogGeneral")]
    public static void LogGeneral(string message) {
        Log.General(message);
    }

    [LuaFunction("LogWarning")]
    public static void LogWarning(string message) {
        Log.Warning(message);
    }

    /**************************/
    /*      Miscellaneous     */
    /**************************/
    [LuaFunction("TDM_SetRichPresence")]
    public static void TDM_SetRichPresence(int iState) {
        Discord.SetPresence((Discord.EDiscordState)iState);
    }

    [LuaFunction("TDM_SendMessage")]
    public static void TDM_SendMessage(int iUserID, string sMessage) { // if iUserID == 0 then send globally
        
    }

    [LuaFunction("TDM_GetGameState")]
    public static int TDM_GetGameState() {
        return ((int)Game.State);
    }

    [LuaFunction("TDM_SetGameState")]
    public static void TDM_SetGameState(int iState) {
        Game.State = (EGameState)iState;
    }

    /**************************/
    /*         Player        */
    /**************************/
    [LuaFunction("TDM_GetPlayerName")]
    public static string TDM_GetPlayerName(int iUserID) {
        if (!Client.m_Connected)
            return "";

        return "";
    }

    [LuaFunction("TDM_GetPlayerSteamID")]
    public static string TDM_GetPlayerSteamID(int iUserID) {
        if (!Client.m_Connected)
            return "";

        return "";
    }

    [LuaFunction("TDM_GetPlayers")]
    public static string TDM_GetPlayers() { // Return type should be a table/array (returns a list of id's)
        if (!Client.m_Connected)
            return "";

        return "None";
    }

    [LuaFunction("TDM_GetPlayerCount")]
    public static int TDM_GetPlayerCount() {
        if (!Client.m_Connected)
            return 0;

        return 0;
    }

    /**************************/
    /*         Server        */
    /**************************/
    [LuaFunction("TDM_ConnectToServer")]
    public static async void TDM_ConnectToServer(string sAddress, int iPort) {
        Log.General("Connecting to server {0}:{1}", sAddress, iPort);
        
        ISession session = await Network.Connect(sAddress, iPort);
        if (session == null) {
            Log.Error("Unable to reach server");
            // Lua.Invoke("TDM_AddToast", "Failed to connect to server");
        } else {
            Log.General("Connected to server");
            // Lua.Invoke("TDM_AddToast", "Successfully connected to server");
            SledgeLib.Game.LoadLevel("hub0", "hub0", "hub0", "");
        }
    }

    [LuaFunction("TDM_DisconnectFromServer")]
    public static void TDM_DisconnectFromServer() {
        if (!Client.m_Connected)
            return;

        Network.Disconnect();
        Game.State = EGameState.Menu;
    }

    [LuaFunction("TDM_GetServerName")]
    public static string TDM_GetServerName() {
        return "Server";
    }

    [LuaFunction("TDM_GetServerIP")]
    public static string TDM_GetServerIP() {
        if (!Client.m_Connected)
            return "";

        return Network.m_sAddress;
    }

    [LuaFunction("TDM_GetServerPort")]
    public static int TDM_GetServerPort() {
        if (!Client.m_Connected)
            return 0;

        return Network.m_iPort;
    }

    [LuaFunction("TDM_GetServerMap")]
    public static string TDM_GetServerMap() {
        if (!Client.m_Connected)
            return "";

        return Network.m_sMap;
    }
}