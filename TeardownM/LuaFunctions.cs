using SledgeLib;

public class LuaFunctions {
    [LuaFunction("TDM_SetRichPresence")]
    public static void TDM_SetRichPresence(int iState) {
        Discord.SetPresence((Discord.EDiscordState)iState);
    }
}