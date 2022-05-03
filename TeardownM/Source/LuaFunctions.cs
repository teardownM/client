using SledgeLib;

public class LuaFunctions {
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
        return 0;
    }

    [LuaFunction("TDM_SetGameState")]
    public static void TDM_SetGameState(int iState) {
        
    }

    /**************************/
    /*         Player        */
    /**************************/
    [LuaFunction("TDM_GetPlayerName")]
    public static string TDM_GetPlayerName(int iUserID) {
        return "";
    }

    [LuaFunction("TDM_GetPlayerSteamID")]
    public static string TDM_GetPlayerSteamID(int iUserID) {
        return "";
    }

    [LuaFunction("TDM_GetPlayers")]
    public static string TDM_GetPlayers() { // Return type should be a table/array (returns a list of id's)
        return "";
    }

    [LuaFunction("TDM_GetPlayerCount")]
    public static int TDM_GetPlayerCount() {
        return 0;
    }

    [LuaFunction("TDM_KickPlayer")]
    public static void TDM_KickPlayer(int iUserID, string sReason) {

    }

    [LuaFunction("TDM_BanPlayer")]
    public static void TDM_BanPlayer(int iUserID, int lTime, string sReason) {

    }

    [LuaFunction("TDM_SetPlayerHealth")]
    public static void TDM_SetPlayerHealth(int iUserID, int iHealth) {

    }

    /**************************/
    /*         Server        */
    /**************************/
    [LuaFunction("TDM_ConnectToServer")]
    public static void TDM_ConnectToServer() {
        // Log.General("Connecting to server");
        // Network.Connect(sAddress, iPort);
    }

    [LuaFunction("TDM_DisconnectFromServer")]
    public static void TDM_DisconnectFromServer() {
        // Network.Disconnect();
    }

    [LuaFunction("TDM_GetServerName")]
    public static string TDM_GetServerName() {
        return "";
    }

    [LuaFunction("TDM_GetServerIP")]
    public static string TDM_GetServerIP() {
        return "";
    }

    [LuaFunction("TDM_GetServerPort")]
    public static int TDM_GetServerPort() {
        return 0;
    }
}