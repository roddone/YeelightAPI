using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using YeelightAPI.Models;

namespace YeelightAPI.Core
{
    /// <summary>
    /// custom converter for Dictionaries with PROPERTIES as key
    /// </summary>
    public class PropertiesDictionaryConverter : JsonConverter<Dictionary<PROPERTIES, object>>
    {
        #region Public Methods

        /// <summary>
        /// Read from JSON
        /// </summary>
        public override Dictionary<PROPERTIES, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject token");
            }

            var dict = new Dictionary<PROPERTIES, object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dict;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Expected PropertyName token");
                }

                string propertyName = reader.GetString();
                reader.Read();

                // skip if the property is not in PROPERTIES enum
                if (Enum.TryParse<PROPERTIES>(propertyName, out PROPERTIES prop))
                {
                    object value;
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.Number:
                            if (reader.TryGetInt64(out long longValue))
                            {
                                value = (decimal)longValue;
                            }
                            else if (reader.TryGetDecimal(out decimal decimalValue))
                            {
                                value = decimalValue;
                            }
                            else
                            {
                                value = reader.GetDouble();
                            }
                            break;
                        case JsonTokenType.String:
                            value = reader.GetString();
                            break;
                        case JsonTokenType.True:
                            value = true;
                            break;
                        case JsonTokenType.False:
                            value = false;
                            break;
                        case JsonTokenType.Null:
                            value = null;
                            break;
                        default:
                            // Skip complex types
                            reader.Skip();
                            continue;
                    }
                    dict.Add(prop, value);
                }
                else
                {
                    // Skip values for properties not in the enum
                    reader.Skip();
                }
            }

            throw new JsonException("Expected EndObject token");
        }

        /// <summary>
        /// Write to JSON
        /// </summary>
        public override void Write(Utf8JsonWriter writer, Dictionary<PROPERTIES, object> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kvp in value)
            {
                writer.WritePropertyName(kvp.Key.ToString());
                JsonSerializer.Serialize(writer, kvp.Value, options);
            }
            writer.WriteEndObject();
        }

        #endregion Public Methods
    }
}
