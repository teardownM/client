using SledgeLib;

// Things todo:
// 1. Fix on spawn update vehicle location
// 2. When a player gets out of a car while driving the car suddenly stops for the all other players
// 3. Getting into the same vehicle as someone else (passenger)

public class VehicleManager
{
    private static bool inVehicle = false;

    // Currently fired on every tick to send and keep track of the current *local* players vehicle
    public static void Tick()
    {
        if (!inVehicle && Registry.GetBool("playerInVehicle"))
        {
            inVehicle = true;
            Client.m_Socket?.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_VEHICLE, "true," + Registry.GetInt("vehicleHandle"));
        }
        else if (inVehicle && !Registry.GetBool("playerInVehicle"))
        {
            inVehicle = false;
            Client.m_Socket?.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_VEHICLE, "false," + Registry.GetInt("playerVehicleHandle"));
        }

        if (inVehicle)
        {
            Log.General(Registry.GetString("vehicleMovement") + "," + Registry.GetString("vehicleTransform"));
            Client.m_Socket?.SendMatchStateAsync(Server.MatchID, (long)Match.OPCODE.PLAYER_VEHICLE_MOVE, Registry.GetString("vehicleMovement") + "," + Registry.GetString("vehicleTransform"));
        }
    }

    // Move the vehicle to a position 
    public static void Drive(List<string> vehicleMoveData)
    {
        string vehicleHandle = vehicleMoveData[1];
        IDictionary<string, string> vehiclePos = new Dictionary<string, string>
        {
            { "x", vehicleMoveData[7] },
            { "y", vehicleMoveData[8] },
            { "z", vehicleMoveData[9] }
        };

        IDictionary<string, string> vehicleQuat = new Dictionary<string, string>
        {
            { "x", vehicleMoveData[10] },
            { "y", vehicleMoveData[11] },
            { "z", vehicleMoveData[12] },
            { "w", vehicleMoveData[13] }
        };

        int drive = 0;
        int steer = 0;
        bool handbreak = Int16.Parse(vehicleMoveData[6]) == 1 ? true : false;

        if (Int16.Parse(vehicleMoveData[2]) == 1)
        {
            drive += 1;
        }
        else if (Int16.Parse(vehicleMoveData[3]) == 1)
        {
            drive -= 1;
        }

        if (Int16.Parse(vehicleMoveData[4]) == 1)
        {
            steer += 1;
        }
        else if (Int16.Parse(vehicleMoveData[5]) == 1)
        {
            steer -= 1;
        }

        Log.General("vehicleQuat: {0}", vehicleQuat["x"] + "," + vehicleQuat["y"] + "," + vehicleQuat["z"] + "," + vehicleQuat["w"]);

        // I = incoming data from C# to Lua
        Registry.SetString("I_vehicleHandle", vehicleHandle);
        Registry.SetString("I_vehicleDriveInput", drive.ToString() + "," + steer.ToString() + "," + handbreak.ToString());
        Registry.SetString("I_vehiclePos", vehiclePos["x"] + "," + vehiclePos["y"] + "," + vehiclePos["z"]);
        Registry.SetString("I_vehicleQuat", vehicleQuat["x"] + "," + vehicleQuat["y"] + "," + vehicleQuat["z"] + "," + vehicleQuat["w"]);
    }
}