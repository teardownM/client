using SledgeLib;
using TeardownM.Miscellaneous;
using TeardownM.Network;

namespace TeardownM;

public class LuaFunctions {
    private static bool IsExternalMod() {
        string sCaller = Lua.GetCaller();
        return !(sCaller.Contains("mods/TeardownM/") || sCaller.Contains("data/ui/"));
    }
    
    /******************************************/
    /*************** Logging ****************/
    /******************************************/
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

    /******************************************/
    /************* Miscellaneous **************/
    /******************************************/
    [LuaFunction("TDM_SetRichPresence")]
    public static void TDM_SetRichPresence(int iState) {
        if (IsExternalMod()) return;
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
        if (IsExternalMod()) return;
        Game.State = (EGameState)iState;
    }

    /******************************************/
    /***************** Player *****************/
    /******************************************/
    [LuaFunction("TDM_GetPlayerName")]
    public static string TDM_GetPlayerName(int iUserID) {
        if (!Network.Network.bConnected)
            return "";

        return "";
    }

    [LuaFunction("TDM_GetPlayerSteamID")]
    public static string TDM_GetPlayerSteamID(int iUserID) {
        if (!Network.Network.bConnected)
            return "";

        return "";
    }

    [LuaFunction("TDM_GetPlayers")]
    public static string TDM_GetPlayers() { // Return type should be a table/array (returns a list of id's)
        if (!Network.Network.bConnected)
            return "";

        return "None";
    }

    [LuaFunction("TDM_GetPlayerCount")]
    public static int TDM_GetPlayerCount() {
        if (!Network.Network.bConnected)
            return 0;

        return 0;
    }

    /******************************************/
    /***************** Server *****************/
    /******************************************/
    [LuaFunction("TDM_ConnectToServer")]
    public static async void TDM_ConnectToServer(string sAddress, int iPort) {
        if (IsExternalMod()) return;
        
        Log.General("Connecting to server {0}:{1}", sAddress, iPort);
        
        Network.Network.Session = await Network.Network.Connect(sAddress, iPort);
        if (Network.Network.Session == null) {
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
        if (!Network.Network.bConnected || IsExternalMod()) return;

        Network.Network.Disconnect();
        Game.State = EGameState.Menu;
    }

    [LuaFunction("TDM_GetServerName")]
    public static string TDM_GetServerName() {
        return "Server";
    }

    [LuaFunction("TDM_GetServerIP")]
    public static string TDM_GetServerIP() {
        if (!Network.Network.bConnected)
            return "";

        return Network.Network.sAddress;
    }

    [LuaFunction("TDM_GetServerPort")]
    public static int TDM_GetServerPort() {
        if (!Network.Network.bConnected)
            return 0;

        return Network.Network.iPort;
    }

    [LuaFunction("TDM_GetServerMap")]
    public static string TDM_GetServerMap() {
        if (!Network.Network.bConnected)
            return "";

        return Server.Map!;
    }
}