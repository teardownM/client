using SledgeLib;
using System.Numerics;

using Steamworks;

using Newtonsoft.Json;

using Nakama;
using System.Globalization;

using System.Diagnostics;

public class IClientData {
    public string? SessionID;
    public string? Username;
    public string? Status;
    public string? UserID;
    public string? Node;
    public bool? Spawned;

    public ushort? Reason;

    public Model? Model = null;
    public Transform Transform = new Transform();
}

public static class Client {
    // Dictionary of all current players in the game
    private static Dictionary<string, IClientData> m_Clients = new();

    private static string? m_DeviceID = "";

    private static ISession? m_Session = null;
    private static IClient? m_Connection = null;
    private static ISocket? m_Socket = null;

    private static bool m_Connected = false;

    private static List<string> m_ModelsToLoad = new();

    private enum OPCODE : Int64 {
        PLAYER_MOVE = 1,
        PLAYER_SPAWN = 2,
        PLAYER_SHOOTS = 3,
        PLAYER_JOINS = 4,
        PLAYER_GRABS = 5
    }

    public static void OnInitialize() {
        m_DeviceID = Guid.NewGuid().ToString();
    }

    public static async Task<ISession> Connect(string ip, ushort port) {
        if (Server.MatchID != null)
            Disconnect();

        // Establish a connection to Nakama server
        m_Connection = new Nakama.Client("http", ip, port, "defaultkey");
        m_Connection.Timeout = 1;
        if (m_Connection == null) {
            Log.Error("Failed to create connection");
            return await Task.FromResult<ISession>(null!);
        }

        Discord.SetPresence(Discord.EDiscordState.Connecting);

        try {
            m_Session = await m_Connection.AuthenticateDeviceAsync(m_DeviceID, m_DeviceID);
            Log.Verbose("Successfully authenticated");
            Log.General("Your ID: {0}", m_Session.UserId);
        } catch (Exception) {
            return await Task.FromResult<ISession>(null!);
        }

        m_Socket = Socket.From(m_Connection);
        await m_Socket.ConnectAsync(m_Session);

        m_Socket.ReceivedMatchState += OnMatchState;
        m_Socket.ReceivedMatchPresence += OnMatchPresence;

        IApiRpc response = await m_Connection.RpcAsync(m_Session, "rpc_get_matches");
        JsonConvert.DeserializeObject<Server>(response.Payload.Substring(1, response.Payload.Length - 2));

        IMatch match = await m_Socket.JoinMatchAsync(Server.MatchID);
        Log.Verbose("Successfully connected to match {0}", Server.MatchID!);

        foreach (IUserPresence client in match.Presences) {
            Log.General("{0} already in the game", client.UserId);
            CreatePresence(client);
            m_ModelsToLoad.Add(client.UserId);
            Server.Clients += 1;
            if (m_Connected)
                OnClientLoad();
        }

        return m_Session;
    }

    public static async void Disconnect() {
        if (m_Socket != null) {
            if (Server.MatchID != null) {
                await m_Socket.LeaveMatchAsync(Server.MatchID);
            }

            await m_Socket.CloseAsync();

            Server.MatchID = null;
            m_Socket = null;
            m_Session = null;
            m_Connected = false;

            m_Clients = new() {};

            Log.General("Disconnected from server");

            if (Game.GetState() != EGameState.Menu)
                Game.SetState(EGameState.Menu);

            Discord.SetPresence(Discord.EDiscordState.MainMenu);
        } else {
            Log.General("Not connected to server");
        }
    }

    public static void Tick() {
        if (!m_Connected) {
            return;
        }

        if (m_ModelsToLoad.Any()) {
            foreach (var userId in m_ModelsToLoad.ToList()) {
                // If it's not the local player, load in their vox player model
                if (userId != m_Session!.UserId) {
                    m_ModelsToLoad.Remove(userId);
                    SpawnPlayer(userId);

                    Log.General("Loaded {0}'s model into the game", userId);
                }
            }
        }

        if (m_Socket == null || m_Session == null || Server.MatchID == null)
            return;

        Vector2 playerInput = Player.GetPlayerMovementInput();
        Transform playerTransform = Player.GetCameraTransform();
        Quaternion playerRotation = Player.GetPlayerCameraTransform().Rotation;

        var posData = playerTransform.Position.X.ToString() + "," + playerTransform.Position.Y.ToString() + "," + playerTransform.Position.Z.ToString()
            + "," + playerRotation.X.ToString() + "," + playerRotation.Y.ToString() + "," + playerRotation.Z.ToString() + "," + playerRotation.W.ToString();
        // Log.General("Sending position data: {0}", posData);
        // Log.General("{0} {1} {2} {3} {4} {5} {6}", playerTransform.Position.X, playerTransform.Position.Y, playerTransform.Position.Z, playerRotation.X, playerRotation.Y, playerRotation.Z, playerRotation.W);

        // Every local game tick, send client's position data to Nakama
        m_Socket.SendMatchStateAsync(Server.MatchID, (long)OPCODE.PLAYER_MOVE, posData);

        // if (!m_Connected || Game.GetState() != EGameState.Playing || Server.MatchID != null)
        //     return;

        // if (m_ModelsToLoad.Any()) {
        //     foreach (var userID in m_ModelsToLoad.ToList()) {
        //         // If it's not the local player, load in their vox player model
        //         if (userID != m_Session!.UserId) {
        //             SpawnPlayer(userID);
        //             m_ModelsToLoad.Remove(userID);

        //             Log.General("Loaded {0}'s model into the game", userID);
        //         }
        //     }
        // }

        // // if (m_Socket == null || m_Session == null || Server.MatchID == null)
        // //     return;

        // Vector2 playerInput = Player.GetPlayerMovementInput();
        // Transform playerTransform = Player.GetCameraTransform();

        // var posData = playerTransform.Position.X.ToString() + "," + playerTransform.Position.Y.ToString() + "," + playerTransform.Position.Z.ToString();

        // // Every local game tick, send client's position data to Nakama
        // m_Socket!.SendMatchStateAsync(Server.MatchID, (long)OPCODE.PLAYER_MOVE, posData);
    }

    public static void CreatePresence(IUserPresence player) {
        // Create a static body for the player, set their position
        uint body = Body.Create();
        Body.SetDynamic(body, false);
        Body.SetPosition(body, new Vector3(0, 0, 0));

        // Model playerModel = new() {
        //     Body = body,
        //     sBody = Shape.Create(body)
        // };

        IClientData newPlayer = new() {
            SessionID = player.SessionId,
            Username = player.UserId,
            Status = player.Status,
            UserID = player.UserId,
            Spawned = false,
            // Model = playerModel
        };

        m_Clients.Add(player.UserId, newPlayer);
    }

    public static void SpawnPlayer(string clientID) {
        Log.General("{0} has spawned", clientID);

        m_Clients[clientID].Model = new Model(new Vector3(0, 0, 0));
        m_Clients[clientID].Model!.Load();
        // Shape.LoadVox(m_Clients[clientID].Model.sBody, "Assets/Models/Player.vox", "", 1.0f);
        // Body.SetTransform(m_Clients[clientID].Model.Body, new Transform(new Vector3(50, 10, 10), new Quaternion(0, 0, 0, 1)));
        m_Clients[clientID].Spawned = true;
    }

    public static void OnStateChange(uint iState) {
        switch (iState) {
            case (uint)EGameState.Menu:
                if (Server.MatchID != null)
                    Disconnect();
                break;

            case (uint)EGameState.Playing:
                // 1. Player joins server from the menu
                // 2. Connects to match instantly
                // 3. TODO: Force game to load map that server is using
                // 4. Player loads into the map
                // 5. EGameState changes to playing
                // 6. Spawn players in m_ModelsToLoad
                if (Server.MatchID != null) { // <-- Checking for Server.MatchID is the same as checking if m_Connected is true
                    m_Connected = true;
                    Log.Verbose("Local client has loaded into the map.");
                    // m_Connected = true;

                    // 7. Notify every player local client has loaded in and should spawn their model
                    m_Socket!.SendMatchStateAsync(Server.MatchID, (long)OPCODE.PLAYER_SPAWN, "");
                    Discord.SetPresence(Discord.EDiscordState.Connected);
                }

                break;
            default:
                break;
        }
    }

    public static void OnMatchState(IMatchState newState) {
        switch (newState.OpCode) {
            case (Int64)OPCODE.PLAYER_MOVE:
                List<string> playerMoveData = System.Text.Encoding.Default.GetString(newState.State).Split(',').ToList();
                if (m_Clients[playerMoveData[0]].Model == null || m_Clients[playerMoveData[0]].Model!.Body == null)
                    return;

                float x = float.Parse(playerMoveData[1], CultureInfo.InvariantCulture.NumberFormat);
                float y = float.Parse(playerMoveData[2], CultureInfo.InvariantCulture.NumberFormat);
                float z = float.Parse(playerMoveData[3], CultureInfo.InvariantCulture.NumberFormat);
                float rx = float.Parse(playerMoveData[4], CultureInfo.InvariantCulture.NumberFormat);
                float ry = float.Parse(playerMoveData[5], CultureInfo.InvariantCulture.NumberFormat);
                float rz = float.Parse(playerMoveData[6], CultureInfo.InvariantCulture.NumberFormat);
                float rw = float.Parse(playerMoveData[7], CultureInfo.InvariantCulture.NumberFormat);

                Log.General("{0} {1} {2} {3} {4} {5}", x, y, z, rx, ry, rz);


                Vector3 startPos = Body.GetPosition(m_Clients[playerMoveData[0]].Model!.Body!.Value);
                Vector3 endPos = new Vector3(x, y, z);

                m_Clients[playerMoveData[0]].Model!.Update(startPos, endPos, 1.0f, new Quaternion(rx, ry, rz, rw));
                break;

            case (Int64)OPCODE.PLAYER_SPAWN:
                // This message could be received in the menu (while connecting and not in game yet)
                if (Game.GetState() != EGameState.Playing)
                    break;

                string id = System.Text.Encoding.Default.GetString(newState.State);
                if (id == m_Session!.UserId)
                    return;

                // m_Clients[id].Spawned = true;
                Log.General("{0} has spawned", id);
                
                break;
            case (Int64)OPCODE.PLAYER_SHOOTS:
                break;
            default:
                break;
        }
    }

    public static void OnClientLoad() {
        Discord.Client!.UpdateState($"Playing with {Server.Clients} others");
        Discord.Client!.UpdatePartySize((int)Server.Clients);
    }

    public static void OnMatchPresence(IMatchPresenceEvent presence) {
        if (presence.Joins.Any()) {
            foreach (IUserPresence? client in presence.Joins) {
                if (client.UserId == m_Session!.UserId)
                    continue;

                Log.General("Player {0} is joining the match!", client.UserId);
                CreatePresence(client);
                m_ModelsToLoad.Add(client.UserId);

                Server.Clients += 1;

                if (m_Connected)
                    OnClientLoad();
            }
        } else if (presence.Leaves.Any()) {
            foreach (IUserPresence? client in presence.Leaves) {
                if (client.UserId == m_Session!.UserId)
                    continue;

                if (m_Clients.ContainsKey(client.UserId) && !m_ModelsToLoad.Contains(client.UserId))
                    Body.Destroy(m_Clients[client.UserId].Model!.Body!.Value);

                if (m_ModelsToLoad.Contains(client.UserId))
                    m_ModelsToLoad.Remove(client.UserId);

                if (m_Clients[client.UserId].Model!.Body != null) {
                    Log.General("Destroying body for {0}", client.UserId);
                    Body.Destroy(m_Clients[client.UserId].Model!.Body!.Value);
                }

                m_Clients.Remove(client.UserId);

                Log.General("Player {0} has left the match!", client.UserId);

                Server.Clients -= 1;
                if (m_Connected)
                    Discord.Client!.UpdatePartySize((int)Server.Clients);
            }
        }
    }
}