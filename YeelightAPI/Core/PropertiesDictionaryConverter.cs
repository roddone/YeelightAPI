using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using YeelightAPI.Models;

namespace YeelightAPI.Core
{
    /// <summary>
    /// custom converter for Dictionaries with PROPERTIES as key
    /// </summary>
    public class PropertiesDictionaryConverter : JsonConverter
    {
        #region Public Methods

        /// <summary>
        /// Can convert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// Read from JSON
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            Dictionary<PROPERTIES, object> dict = new Dictionary<PROPERTIES, object>();

            foreach (JProperty child in jsonObject.Children())
            {
                //skip if the property is not in PROPERTIES enum
                if (Enum.TryParse<PROPERTIES>(child.Name, out PROPERTIES prop))
                {
                    //only integers and string
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

        /// <summary>
        /// Write to JSON
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(value);
            token.WriteTo(writer);
        }

        #endregion Public Methods
    }
}