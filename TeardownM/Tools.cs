using SledgeLib;


public class ToolManager
{
    public static string currentTool = null;
    private static string previousTool = null;

    // Currently fired on every tick to send and keep track of the current *local* players tool
    public static void Tick() {
        if (Client.m_Socket == null)
            return;

        currentTool = Registry.GetString("game.player.tool");
        
        if (previousTool != currentTool)
        {
            Client.m_Socket.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_TOOL_CHANGE, currentTool);
            previousTool = currentTool;
        }

        if (CPlayer.m_M1Down)
        {
            Client.m_Socket.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_SHOOTS, ToolManager.currentTool);
        }
    }
}
