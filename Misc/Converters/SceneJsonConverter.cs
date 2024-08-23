using LF2Clone.Base;
using Newtonsoft.Json;

namespace LF2Clone.Misc.Converters
{
    public class SceneJsonConverter : JsonConverter<Scene>
    {

        public override Scene? ReadJson(JsonReader reader, Type objectType, Scene? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        } // no reading, only writing should be overriden

        public override void WriteJson(JsonWriter writer, Scene? value, JsonSerializer serializer)
        {
            
        }
    }
}
