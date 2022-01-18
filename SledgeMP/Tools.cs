using SledgeLib;


public class ToolManager
{
    public static string currentTool = null;
    private static string previousTool = null;

    // Currently fired on every tick to send and keep track of the current *local* players tool
    public static void PlayerTool() {
        if (Client.m_Socket == null)
            return;

        currentTool = Registry.GetString("game.player.tool");
        
        if (previousTool != currentTool)
        {
            Client.m_Socket.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_TOOL_CHANGE, currentTool);
            previousTool = currentTool;
        }


        /*
        if (m_PreviousTool != m_CurrentTool)
        {
            if (m_PreviousTool == ETools.None)
            {
                m_ToolBody = new CBody();
                m_ToolShape = new CShape(m_ToolBody.m_Handle);
                Log.General("Creating first tool");
            }
            else
            {
                if (m_ToolShape != null)
                    m_ToolBody!.Destroy();
                m_ToolBody = new CBody();
                m_ToolShape = new CShape(m_ToolBody.m_Handle);

                m_ToolShape = new CShape(m_ToolBody!);

                try
                {
                    CShape.LoadVox(m_ToolShape!.m_Handle, "Assets/Models/" + toolStr + ".vox", "", 0.5f);
                    m_ToolBody.m_Dynamic = false;
                }
                catch (System.Exception)
                {
                    Log.Error("Modded tools are not supported");
                }

                Log.General("Switch to tool: " + toolStr);
            }

            m_PreviousTool = m_CurrentTool;
        }

        if (m_ToolBody != null)
        {
            Quaternion toolRot = CPlayer.m_ToolBody.m_Rotation;
            Vector3 toolPos = CPlayer.m_ToolBody.m_Position;

            toolRot = Quaternion.Multiply(toolRot, new Quaternion(0, 0.7071068f, 0.7071068f, 0));
            toolRot = Quaternion.Multiply(toolRot, new Quaternion(0, 0, -1, 0));

            var l = m_ToolBody.GetChildren();
            var l2 = CPlayer.m_ToolBody.GetChildren();
            foreach (var c in l)
            {
                foreach (var c2 in l2)
                {
                    if (c.m_Bounds.m_Min == c2.m_Bounds.m_Min)
                    {
                        c.m_LocalTransform = c2.m_LocalTransform;
                    }
                }
            }

            m_ToolBody.m_Position = toolPos;
            m_ToolBody.m_Rotation = toolRot;
       
        } */
    }
}
