using SledgeLib;
using System.Numerics;

public class PlayerManager
{

    // Currently fired on every tick to send and keep track of the current *local* players vehicle
    public static void Tick()
    {
        Vector3 playerPos = CPlayer.m_Position;
        Quaternion camRotation = CPlayer.m_CameraTransform.Rotation;

        var posData = Math.Round(playerPos.X, 3).ToString() + "," + Math.Round(playerPos.Y, 3).ToString() + "," + Math.Round(playerPos.Z, 3).ToString()
            + "," + Math.Round(camRotation.X, 3).ToString() + "," + Math.Round(camRotation.Y, 3).ToString() + "," + Math.Round(camRotation.Z, 3).ToString() + "," + Math.Round(camRotation.W, 3).ToString();

        // Every local game tick, send client's position data to Nakama
        Client.m_Socket?.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_MOVE, posData);
    }
}
