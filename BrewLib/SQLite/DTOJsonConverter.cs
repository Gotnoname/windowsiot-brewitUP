using System;
using Newtonsoft.Json;
using BrewLib.Interfaces;

namespace BrewLib.Databse
{
    public class DTOJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType.FullName == typeof(IStep).FullName
                || objectType.FullName == typeof(BrewProfile).FullName)                
            {
                return true;
            }
            return false;
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (objectType.FullName == typeof(IStep).FullName)
            {
                return serializer.Deserialize(reader, typeof(IStep));
            }
            else if (objectType.FullName == typeof(BrewProfile).FullName)
            {
                return serializer.Deserialize(reader, typeof(BrewProfile));
            }
            throw new NotSupportedException(string.Format("Type {0} unexpected.", objectType));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
