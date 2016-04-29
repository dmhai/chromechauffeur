using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ChromeChauffeur.Core.Infrastructure.Json
{
    public class JsonSerializer
    {
        public string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o, CreateSettings());
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, CreateSettings());
        }

        private static JsonSerializerSettings CreateSettings()
        {
            return new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
    }
}