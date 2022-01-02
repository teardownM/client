using SledgeLib;

using Nakama;
using Nakama.TinyJson;
using Steamworks;

using Newtonsoft.Json;

using System.Text;
using System.Numerics;

class IPlayer : IUserPresence
{
    public uint m_Body { get; set; }
    public uint m_Shape { get; set; }
    public bool voxelLoaded { get; set; }
    public bool Persistence { get; set; }
    public string? SessionId { get; set; }
    public string? Status { get; set; }
    public string? Username  { get; set; }
    public string? UserId { get; set; }
}

public class TeardownNakama
{
    static readonly string deviceId = SteamUser.GetSteamID().ToString(); // TODO - Make this steamId

    static IClient? client;
    static ISession? session;
    static ISocket? socket;
    static string? matchId;

    static Dictionary<string, IPlayer> currentPresences = new();

    enum OP_CODES : ushort
    {
        PLAYER_POS = 1,
        PLAYER_SPAWN = 2,
        SPAWN_ALL = 3
    }

    static void CreatePlayer(IUserPresence presence)
    {
        // Create a static body for the player, set their position
        uint m_Body = Body.Create();
        Body.SetDynamic(m_Body, false);
        Body.SetPosition(m_Body, new Vector3(0, 0, 0));

        // Add user to our current dictionary of players
        IPlayer newPlayer = new()
        {
            m_Body = m_Body,
            m_Shape = Shape.Create(m_Body),
            voxelLoaded = false,
            Persistence = presence.Persistence,
            SessionId = presence.SessionId,
            Status = presence.Status,
            Username = presence.Username,
            UserId = presence.UserId
        };

        currentPresences.Add(presence.UserId, newPlayer);
        Log.General("{0}: Added into player into currentPlayers", presence.UserId);
    }

    static void InitListeners()
    {
        if (socket == null || session == null && matchId != null)
            return;

        // This will get run every server tick (currently a tickrate of 28)
        socket.ReceivedMatchState += newState =>
        {
            switch (newState.OpCode)
            {
                case (ushort)OP_CODES.PLAYER_POS:
                    string stringJson = Encoding.Default.GetString(newState.State);
                    var incomingData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(stringJson); // Haven't been able to figure out how to change 'dynamic' to be a class

                    /* The current structure of JSON we receive (there's an optimisation to be made here and not to use JSON but leave that for later) is 
                     * a dictonary where the key is the 'user_id' of the player. The value is their properties such as their x,y,z values, health etc.
                     * Example:
                     * 
                     * {"84de12f5-4ae7-484b-b4b8-f4b3d86633b8":
                     *   {
                     *     "node":"nakama1",
                     *     "reason":0,
                     *     "session_id":"54926f7d-6a47-11ec-8e38-7106fdcb5b46",
                     *     "user_id":"84de12f5-4ae7-484b-b4b8-f4b3d86633b8", <-- TODO: Remove as the user_id is already the key
                     *     "username":"dfbc3663-a54d-40b9-9aca-c3cea7f57b15",
                     *     "x":35.699997,
                     *     "y":16.889925,
                     *     "z":-11.600002
                     *   }
                     * }
                     * 
                     * This way, we can easier access any user's properties with their 'user_id' in the global varible 'presences'.
                     */
                    foreach (var presence in incomingData)
                    {
                        // Check to see if the incoming data is a user in our current dictionary of players
                        if (currentPresences.ContainsKey(presence.Key))
                        {
                            // Update Body position with new position data
                            Body.SetTransform(currentPresences[presence.Key].m_Body, new Transform(new Vector3((float)presence.Value.x, (float)presence.Value.y, (float)presence.Value.z), new Quaternion(0, 0, 0, 1)));
                        }
                    }
                    break;
                default:
                    return;
            }
        };

        socket.ReceivedMatchPresence += player =>
        {
            if (player.Joins.Any())
            {
                foreach (var presence in player.Joins)
                {
                    if (presence.UserId != session.UserId)
                    {
                        Log.General("A player has joined the game with id of {0}", presence.UserId);
                        CreatePlayer(presence);
                        Body.SetPosition(currentPresences[presence.UserId].m_Body, new Vector3((float)0, (float)0, (float)0));
                        Body.SetTransform(currentPresences[presence.UserId].m_Body, new Transform(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1)));
                    }
                }
            };
        };

        Log.General("Sockets loaded");
    }

    /*
     * this function will be called every time the player is done updating (camera position updates, health updates, etc)
     */
    static void OnPostPlayerUpdate()
    {
        // if the user is in the menu / doing something else, don't run this
        if (!Game.IsPlaying())
            return;

        foreach (var presence in currentPresences)
        {
            // If it's not the local player, load in their vox player model
            if (presence.Key != session.UserId && !presence.Value.voxelLoaded)
            {
                Shape.LoadVox(presence.Value.m_Shape, "Assets/Vox/character_lee.vox", "", 1.0f / 2);
                Shape.SetCollisionFilter(presence.Value.m_Shape, 0, 0);
                Body.SetTransform(presence.Value.m_Body, new Transform(new Vector3(50, 10, 10), new Quaternion(0, 0, 0, 1)));
                presence.Value.voxelLoaded = true;

                Log.General("{0}: Voxel Loaded", presence.Key);
            }
        }

        if (socket == null || session == null || matchId == null)
        {
            return;
        }

        Vector2 playerInput = Player.GetPlayerMovementInput();
        Transform playerTransform = Player.GetCameraTransform();

        var newState = new Dictionary<string, dynamic> {
            { "user_id", session.UserId },
            { "currentX", playerTransform.Position.X },
            { "currentY", playerTransform.Position.Y },
            { "currentZ", playerTransform.Position.Z },
            { "rotationX", playerTransform.Rotation.X },
            { "rotationY", playerTransform.Rotation.Y },
            { "rotationZ", playerTransform.Rotation.Z },
            { "rotationW", playerTransform.Rotation.W }
        }.ToJson();

        // Every local game tick, send client's position data to Nakama
        socket.SendMatchStateAsync(matchId, 1, newState);
    }

    static async void JoinGame()
    {
        if (client == null || socket == null)
        {
            Log.Error("No client or socket found when authenticating");
            return;
        }

        /*
         * We currently create 1 match on startup of our Nakama server.
         * This is just for testing purposes, in the future there'll be a server browser.
         * 
         * This function sends an api request to fetch the match id and then joins that match
         */
        IApiRpc response = await client.RpcAsync(session, "get_match_id");
        matchId = response.Payload.Trim(new char[] { '"' });

        IMatch match = await socket.JoinMatchAsync(matchId);

        Log.General("Joined game with id: {0}", matchId);
        Log.General("Current local user id: {0}", session.UserId);

        // All the players already in the game
        // TODO: Figure out spawning of each player
        //       Remember that the current local player is included in here too! Make sure you add a guard check for that (user_id != session.user_id)
        foreach (IUserPresence presence in match.Presences)
        {
            if (presence.UserId != session.UserId)
            {
                Log.General("Found user id '{0}' already in the game", presence.UserId);
                CreatePlayer(presence);
            }
        }
    }

    static async void Authenticate()
    {
        if (client == null)
        {
            Log.Error("No client found when authenticating");
            return;
        }

        Log.General("Attempting to authenticate with device id of: {0}", deviceId);

        session = await client.AuthenticateDeviceAsync(deviceId, deviceId);
        socket = Socket.From(client);
        await socket.ConnectAsync(session);

        Log.General("Nakama session and socket created");
        InitListeners();
    }

    static void LogCurrentPresences()
    {
        foreach(var presence in currentPresences)
        {
            Log.General("{0}: {1}", presence.Key, presence.Value);
        }
    }

    /*
     * always instantiate and keep around the callback instances
     * otherwise the GC is going to collect them
     */

    private static dBindCallback LogCurrentPresencesCallbackFunc = new dBindCallback(LogCurrentPresences);
    private static dBindCallback JoinGameCallbackFunc = new dBindCallback(JoinGame);
    private static dCallback PostPlayerUpdateCallbackFunc = new dCallback(OnPostPlayerUpdate);

    private static CBind? LogCurrentPresencesBind;
    private static CBind? JoinGameBind;
    private static CCallback? PostPlayerUpdateCallback;

    /*
     * function that initializes our mod
     * it must always be a public static void function, otherwise the mod won't be loaded
     * it can be named whatever you want though, as long as you define it on the mod's info.json
     */
    public static void Init()
    {
        if (SteamAPI.Init()) {
            Log.General("SteamAPI initialized");
        } else {
            Log.Error("SteamAPI failed to initialize");
        }

        LogCurrentPresencesBind = new CBind(EKeyCode.VK_B, LogCurrentPresences);
        JoinGameBind = new CBind(EKeyCode.VK_N, JoinGame);
        PostPlayerUpdateCallback = new CCallback(ECallbackType.PostPlayerUpdate, PostPlayerUpdateCallbackFunc);

        // Creates Nakama client
        client = new Client("http", "127.0.0.1", 7350, "defaultkey");
        Authenticate();
    }

    // TODO:
    public static void Shutdown()
    {
        SteamAPI.Shutdown();
        Log.General("Teardown Nakama shutting down");
    }

    public static void Reload()
    {
        Init();
        Log.General("Teardown Nakama succesfully reloaded");
    }
}