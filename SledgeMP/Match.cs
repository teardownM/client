using Nakama;
using Newtonsoft.Json;
using SledgeLib;
using System.Globalization;
using System.Numerics;

public static class Match {
    // Dictionary of all current players in the game
    public static Dictionary<string, IClientData> m_Clients = new();

    public enum OPCODE : Int64 {
        PLAYER_MOVE = 1,
        PLAYER_SPAWN = 2,
        PLAYER_SHOOTS = 3,
        PLAYER_JOINS = 4,
        PLAYER_GRABS = 5,
        PLAYER_TOOL_CHANGE = 6
    }

    /**
     * Creates an instance of a client connected.
     */
    public static void CreatePresence(IUserPresence player) {
        IClientData newPlayer = new() {
            SessionID = player.SessionId,
            Username = player.UserId,
            Status = player.Status,
            UserID = player.UserId,
            Spawned = false,
        };

        m_Clients.Add(player.UserId, newPlayer);
        Log.General("Created Player Presence");
    }

    public static async Task<ISession> Connect(string ip, ushort port) {
        if (Server.MatchID != null)
            Disconnect();

        // Establish a connection to Nakama server
        Client.m_Connection = new Nakama.Client("http", ip, port, "defaultkey");
        Client.m_Connection.Timeout = 1;
        if (Client.m_Connection == null) {
            Log.Error("Failed to create connection");
            return await Task.FromResult<ISession>(null!);
        }

        try {
            Client.m_Session = await Client.m_Connection.AuthenticateDeviceAsync(Client.m_DeviceID, Client.m_DeviceID);
            Log.Verbose("Successfully authenticated");
            Log.General("Your ID: {0}", Client.m_Session.UserId);
        } catch (Exception) {
            return await Task.FromResult<ISession>(null!);
        }

        Client.m_Socket = Socket.From(Client.m_Connection);
        await Client.m_Socket.ConnectAsync(Client.m_Session);

        Client.m_Socket.ReceivedMatchState += OnMatchState;
        Client.m_Socket.ReceivedMatchPresence += OnMatchPresence;

        IApiRpc response = await Client.m_Connection.RpcAsync(Client.m_Session, "rpc_get_matches");
        JsonConvert.DeserializeObject<Server>(response.Payload.Substring(1, response.Payload.Length - 2));

        IMatch match = await Client.m_Socket.JoinMatchAsync(Server.MatchID);
        Log.Verbose("Successfully connected to match {0}", Server.MatchID!);
        Client.m_Connected = true;

        foreach (IUserPresence client in match.Presences) {
            Log.General("{0} already in the game", client.UserId);
            Match.CreatePresence(client);
            Client.m_PlayerModelsToLoad.Add(client.UserId);
        }

        return Client.m_Session;
    }

    public static void OnMatchState(IMatchState newState) {
        switch (newState.OpCode) {
            case (Int64)OPCODE.PLAYER_MOVE:
                List<string> playerMoveData = System.Text.Encoding.Default.GetString(newState.State).Split(',').ToList();

                float x = float.Parse(playerMoveData[1], CultureInfo.InvariantCulture.NumberFormat);
                float y = float.Parse(playerMoveData[2], CultureInfo.InvariantCulture.NumberFormat);
                float z = float.Parse(playerMoveData[3], CultureInfo.InvariantCulture.NumberFormat);
                float rx = float.Parse(playerMoveData[4], CultureInfo.InvariantCulture.NumberFormat);
                float ry = float.Parse(playerMoveData[5], CultureInfo.InvariantCulture.NumberFormat);
                float rz = float.Parse(playerMoveData[6], CultureInfo.InvariantCulture.NumberFormat);
                float rw = float.Parse(playerMoveData[7], CultureInfo.InvariantCulture.NumberFormat);
                                
                Vector3 startPos = m_Clients[playerMoveData[0]].PlayerModel!.Body!.m_Position;
                Vector3 endPos = new Vector3(x, y, z);

                m_Clients[playerMoveData[0]].PlayerModel!.Update(startPos, endPos, new Quaternion(rx, ry, rz, rw));
                break;

            // OPCODE.PLAYER_SPAWN gets called when a player joins the game, the server send a message directly to them to spawn at a specific spawn point
            case (Int64)OPCODE.PLAYER_SPAWN: 
                if (Game.GetState() != EGameState.Playing)
                    break;

                List<string> playerSpawnData = System.Text.Encoding.Default.GetString(newState.State).Split(',').ToList();

                float spawnX = float.Parse(playerSpawnData[0], CultureInfo.InvariantCulture.NumberFormat);
                float spawnY = float.Parse(playerSpawnData[1], CultureInfo.InvariantCulture.NumberFormat);
                float spawnZ = float.Parse(playerSpawnData[2], CultureInfo.InvariantCulture.NumberFormat);

                CPlayer.m_Position = new Vector3(spawnX, spawnY, spawnZ);
                Client.OnClientLoad();
                break;
            case (Int64)OPCODE.PLAYER_SHOOTS:
                List<string> shootData = System.Text.Encoding.Default.GetString(newState.State).Split(',').ToList();

                var player = m_Clients[shootData[0]].PlayerModel!.Body!;
                var playerTool = m_Clients[shootData[0]].PlayerModel!.ToolBody!;

                Scene.Shoot(playerTool.m_Position, new Vector3(100, 0, 0), EProjectileType.Shotgun, 100, 50);
                Log.General("Shooting with the {0} from m_Velocity {1}", shootData[1], player.m_Velocity.ToString());
                break;
            case (Int64)OPCODE.PLAYER_TOOL_CHANGE:
                List<string> toolChangeData = System.Text.Encoding.Default.GetString(newState.State).Split(',').ToList();
                m_Clients[toolChangeData[0]].PlayerModel!.UpdateTool(toolChangeData[1]);
                break;
            default:
                break;
        }
    }


    public static void OnMatchPresence(IMatchPresenceEvent presence) {
        if (presence.Joins.Any()) {
            foreach (IUserPresence? client in presence.Joins) {
                if (client.UserId == Client.m_Session!.UserId)
                    continue;

                Log.General("Player {0} is joining the match!", client.UserId);
                CreatePresence(client);
                Client.m_PlayerModelsToLoad.Add(client.UserId);
            }
        }
        else if (presence.Leaves.Any()) {
            foreach (IUserPresence? client in presence.Leaves) {
                if (client.UserId == Client.m_Session!.UserId)
                    continue;

                if (m_Clients[client.UserId].PlayerModel!.Body != null) {
                    Log.General("Destroying body for {0}", client.UserId);

                    m_Clients[client.UserId].PlayerModel!.Body!.Destroy();
                }

                m_Clients.Remove(client.UserId);

                Log.General("Player {0} has left the match!", client.UserId);
            }
        }
    }

    public static async void Disconnect() {
        if (Client.m_Socket != null) {
            if (Server.MatchID != null) {
                await Client.m_Socket.LeaveMatchAsync(Server.MatchID);
            }

            m_Clients.Clear();

            await Client.m_Socket.CloseAsync();

            Server.MatchID = null;
            Client.m_Socket = null;
            Client.m_Session = null;
            Client.m_Connected = false;

            m_Clients = new() {};

            Log.General("Disconnected from server");

            if (Game.GetState() != EGameState.Menu)
                Game.SetState(EGameState.Menu);

            Discord.SetPresence(Discord.EDiscordState.MainMenu);
        } else {
            Log.General("Not connected to server");
        }
    }
}