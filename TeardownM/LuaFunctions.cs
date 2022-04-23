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
    public static void TDM_GetGameState() {
        
    }

    [LuaFunction("TDM_SetGameState")]
    public static void TDM_SetGameState(int iState) {
        
    }

    /**************************/
    /*         Player        */
    /**************************/
    [LuaFunction("TDM_GetPlayerName")]
    public static void TDM_GetPlayerName(int iUserID) {
        
    }

    [LuaFunction("TDM_GetPlayerSteamID")]
    public static void TDM_GetPlayerSteamID(int iUserID) {
        
    }

    [LuaFunction("TDM_GetPlayers")]
    public static void TDM_GetPlayers() { // Return type should be a table/array (returns a list of id's)
        
    }

    [LuaFunction("TDM_GetPlayerCount")]
    public static void TDM_GetPlayerCount() {
        
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
    public static void TDM_ConnectToServer(string sAddress, int iPort) {
        // Network.Connect(sAddress, iPort);
    }

    [LuaFunction("TDM_DisconnectFromServer")]
    public static void TDM_DisconnectFromServer() {
        // Network.Disconnect();
    }

    [LuaFunction("TDM_GetServerName")]
    public static void TDM_GetServerName() {
        
    }

    [LuaFunction("TDM_GetServerIP")]
    public static void TDM_GetServerIP() {
        
    }

    [LuaFunction("TDM_GetServerPort")]
    public static void TDM_GetServerPort() {
        
    }
}