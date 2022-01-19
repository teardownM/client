using SledgeLib;

public class ToolManager {
    public static string? currentTool = null;
    private static string? previousTool = null;

    // Currently fired on every tick to send and keep track of the current *local* players tool
    public static void PlayerTool() {
        if (Client.m_Socket == null)
            return;

        currentTool = Registry.GetString("game.player.tool");
        
        if (previousTool != currentTool) {
            Client.m_Socket.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_TOOL_CHANGE, currentTool);
            previousTool = currentTool;
        }
    }

    public void Update() {

    }
}
