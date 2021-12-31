using SledgeLib;

using Nakama;
using Nakama.TinyJson;

using Newtonsoft.Json;

using System.Text;
using System.Numerics;

class IPlayer : IUserPresence
{
    public float X { get; set; } // TODO: Can this be a vector instead? 
    public float Y { get; set; }
    public float Z { get; set; }
    public bool Persistence { get; set; }
    public string? SessionId { get; set; }
    public string? Status { get; set; }
    public string? Username  { get; set; }
    public string? UserId { get; set; }
}

public class TeardownNakama
{
    static readonly string deviceId = Guid.NewGuid().ToString(); // TODO - Make this steamId

    static IClient? client;
    static ISession? session;
    static ISocket? socket;
    static string? matchId;

    static Dictionary<string, IPlayer> currentPresences = new();

    private static uint m_VoxBody = 0;
    private static uint m_Shape = 0;
    enum OP_CODES : ushort
    {
        PLAYER_POS = 1,
        PLAYER_SPAWN = 2,
        SPAWN_ALL = 3
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
                        // Check to see if the incoming data is from the current local player.
                        if (presence.Key == session.UserId)
                        {
                            // Do nothing for now 
                            return;
                        } else
                        {
                            // Check to see if the incoming data is a user in our current dictionary of players
                            if (currentPresences.ContainsKey(presence.Key))
                            {
                                currentPresences[presence.Key].X = presence.Value.x;
                                currentPresences[presence.Key].Y = presence.Value.y;
                                currentPresences[presence.Key].Z = presence.Value.z;
                            }
                            else
                            {
                                // Is this check neccesary if we already add them when they join?  
                                IPlayer newPlayer = new IPlayer { X = presence.Value.x, Y = presence.Value.y, Z = presence.Value.z, SessionId = presence.Value.session_id, UserId = presence.Value.user_id };
                                currentPresences.Add(presence.Key, newPlayer);
                            }

                            // Set their character to move
                            Body.SetTransform(m_VoxBody, new Transform(new Vector3((float)presence.Value.x, (float)presence.Value.y, (float)presence.Value.z), new Quaternion(0, 0, 0, 1)));
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
                foreach (var presense in player.Joins)
                {
                    // Add user to our current dictionary of players
                    IPlayer newPlayer = new IPlayer { X = 0, Y = 0, Z = 0, Persistence = presense.Persistence, SessionId = presense.SessionId, Status = presense.Status, Username = presense.Username, UserId = presense.UserId };
                    currentPresences.Add(presense.UserId, newPlayer);

                    // Spawn them in
                    // TODO: Figure out how to dynamically spawn voxels (probably want to spawn in a sprite image rather than voxel?)
                    //       Currently it's just using the 1 m_VoxBody id. 
                    //Body.SetTransform(m_VoxBody, new Transform(new Vector3(50, 10, 10), new Quaternion(0, 0, 0, 1)));

                    Log.General("A player has joined the game with id of {0}", presense.UserId);
                }
            };
        };

        Log.General("Sockets loaded");
    }

    /*
     * this function will be called every time the player is done updating (camera position updates, health updates, etc)
     */
    static async void OnPostPlayerUpdate()
    {
        // if the user is in the menu / doing something else, don't run this
        if (!Game.IsPlaying())
            return;

        
        // I believe this is where we have to define our VoxBody's as anywhere else crashes the game? 
        if (m_VoxBody > 0)
        {

        } else
        {
            m_VoxBody = Body.Create();
            m_Shape = Shape.Create(m_VoxBody);

            // TODO: Change to be dynamic path
            Shape.LoadVox(m_Shape, "D:/Games/SteamLibrary/steamapps/common/Teardown/mods/assetpack/assets/props/container_red.vox", "body", 1.0f);
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
        await socket.SendMatchStateAsync(matchId, 1, newState);
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

        Log.General("Found match with id: {0}", matchId);

        IMatch match = await socket.JoinMatchAsync(matchId);

        Log.General("Joined game with id: {0}", matchId);

        // All the players already in the game
        // TODO: Figure out spawning of each player
        //       Remember that the current local player is included in here too! Make sure you add a guard check for that (user_id != session.user_id)
        foreach (IUserPresence presence in match.Presences)
        {
            Log.General("User id '{0}' is currently in the game", presence.UserId);
        }
    }

    static async void Authenticate()
    {
        if (client == null)
        {
            Log.Error("No client found when authenticating");
            return;
        }

        Log.General("Nakama authenticating with id: {0}", deviceId);

        session = await client.AuthenticateDeviceAsync(deviceId, deviceId);

        socket = Socket.From(client);
        await socket.ConnectAsync(session);
        Log.General("Nakama session and socket created");

        InitListeners();
    }

    /*
     * always instantiate and keep around the callback instances
     * otherwise the GC is going to collect them
     */

    private static dBindCallback ToggleNoclipCallbackFunc = new dBindCallback(JoinGame);
    private static dCallback PostPlayerUpdateCallbackFunc = new dCallback(OnPostPlayerUpdate);

    private static CBind? JoinGameBind;
    private static CCallback? PostPlayerUpdateCallback;

    /*
     * function that initializes our mod
     * it must always be a public static void function, otherwise the mod won't be loaded
     * it can be named whatever you want though, as long as you define it on the mod's info.json
     */
    public static void Init()
    {
        JoinGameBind = new CBind(EKeyCode.VK_N, JoinGame);
        PostPlayerUpdateCallback = new CCallback(ECallbackType.PostPlayerUpdate, PostPlayerUpdateCallbackFunc);

        // Creates Nakama client
        client = new Client("http", "127.0.0.1", 7350, "defaultkey");
        Authenticate();
    }

    // TODO:
    public static void Shutdown()
    {
        Log.General("Teardown Nakama shutting down");
    }

    public static void Reload()
    {
        Init();
        Log.General("Teardown Nakama succesfully reloaded");
    }
}