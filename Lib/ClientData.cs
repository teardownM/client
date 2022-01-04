using SledgeLib;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class ClientDataConverter : JsonConverter  {
    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer) {
        Log.General("Writing json");
        var list = (UserID)value;

        writer.WriteStartObject();

        foreach (var item in list.ClientData) {
            writer.WritePropertyName(item.user_id);
            serializer.Serialize(writer, item);
        }

        writer.WriteEndObject();
    }

    public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
        var jo = serializer.Deserialize<JObject>(reader);
        var result = new UserID();
        result.ClientData = new List<IClientData>();

        foreach (var item in jo.Properties()) {
            var p = item.Value.ToObject<IClientData>();
            p.user_id = item.Name;
            result.ClientData.Add(p);
        }

        return result;
    }

    public override bool CanConvert(Type objectType) {
        Log.General("CanConvert");
        return objectType == typeof(UserID);
    }
}

[JsonConverter(typeof(ClientDataConverter))]
class UserID {
    public List<IClientData> ClientData { get; set; } = new();
}

public class IClientData {
    public string session_id { get; set; } = "";
    public string username { get; set; } = "";
    public string user_id { get; set; } = "";
    public string node { get; set; } = "";

    public int reason { get; set; }

    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
}