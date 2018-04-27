using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using YeelightAPI.Models;

namespace YeelightAPI.Core
{
    internal class PropertiesDictionaryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj1 = JObject.Load(reader);
            Dictionary<PROPERTIES, object> dict = new Dictionary<PROPERTIES, object>();

            foreach (JProperty child in obj1.Children())
            {
                if (Enum.TryParse<PROPERTIES>(child.Name, out PROPERTIES prop))
                {
                    if (child.Value.Type == JTokenType.Integer)
                    {
                        dict.Add(prop, child.ToObject(typeof(decimal)));
                    }
                    else
                    {
                        dict.Add(prop, child.Value.ToString());
                    }
                }
            }

            return dict;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            t.WriteTo(writer);
        }
    }
}
