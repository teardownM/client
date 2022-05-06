using Nakama;
using SledgeLib;

public static class MatchState {
    private enum OPCODE : Int64 {
        PLAYER_MOVE = 1,
        PLAYER_SPAWN = 2,
        PLAYER_SHOOTS = 3,
        PLAYER_JOINS = 4,
        PLAYER_GRABS = 5,
        PLAYER_TOOL_CHANGE = 6,
        PLAYER_VEHICLE = 7,
        PLAYER_VEHICLE_MOVE = 8
    }

    public static void OnMatchState(IMatchState State) {
        switch (State.OpCode) {
            case (Int64)OPCODE.PLAYER_MOVE:
                break;
            case (Int64)OPCODE.PLAYER_SPAWN:
                break;
            case (Int64)OPCODE.PLAYER_SHOOTS:
                break;
            case (Int64)OPCODE.PLAYER_JOINS:
                break;
            case (Int64)OPCODE.PLAYER_GRABS:
                break;
            case (Int64)OPCODE.PLAYER_TOOL_CHANGE:
                break;
            case (Int64)OPCODE.PLAYER_VEHICLE:
                break;
            case (Int64)OPCODE.PLAYER_VEHICLE_MOVE:
                break;
        }
    }

    public static void OnMatchPresence(IMatchPresenceEvent Presence) {
        if (Client.m_Session == null)
            return;

        if (Presence.Joins.Any()) { // Player joined
            foreach (IUserPresence user in Presence.Joins) {
                if (user.UserId == Client.m_Session.UserId)
                    continue;

                Log.Verbose("Player {0} joined", user.UserId);
            }
        } else if (Presence.Leaves.Any()) { // Player left
            foreach (IUserPresence user in Presence.Leaves) {
                if (user.UserId == Client.m_Session.UserId)
                    continue;

                Log.Verbose("Player {0} left", user.UserId);
            }
        }
    }
}