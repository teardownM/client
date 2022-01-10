// using Nakama;
// using Newtonsoft.Json;

// public static class SledgeMPAPI {
//     public static async Task<string?> GetServerLevel() {
//         if (Server.MatchID == null)
//             return null;

//         IApiRpc response = await Client.m_Connection!.RpcAsync(Client.m_Session, "rpc_get_map");
//         JsonConvert.DeserializeObject<Server>(response.Payload.Substring(1, response.Payload.Length - 2));

//         return "";
//     }

//     public static void GetSteamID() {
//         if (Server.MatchID == null)
//             return;
//     }

//     public static void GetSteamID64() {
//         if (Server.MatchID == null)
//             return;
//     }

//     public static void GetSteamName(string steamID) {
//         if (Server.MatchID == null)
//             return;
//     }
// }

// /*
// function GetLevel()
//     -> Send OPCODE.GET_MAP
//     -> Wait for response
//     -> Deserialize response
//     -> Return level
// end
// */