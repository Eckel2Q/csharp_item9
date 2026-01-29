using CharacterGeneration.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CharacterGeneration.Interfaces
{
    public class EquipmentConverter : JsonConverter<IEquipment>
    {
        public override IEquipment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                if (document.RootElement.TryGetProperty("sType", out JsonElement typeElement))
                { 
                    string type = typeElement.GetString();
                    switch (type)
                    {
                        case "Armor":
                            return JsonSerializer.Deserialize<Armor>(document.RootElement.GetRawText(), options);
                        case "Weapon":
                            return JsonSerializer.Deserialize<Weapon>(document.RootElement.GetRawText(), options);
                        default:
                            throw new JsonException($"Unknown equipment type: {type}");
                    }
                }
                else
                {
                    throw new JsonException("Missing 'sType' property in equipment JSON.");
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, IEquipment value, JsonSerializerOptions options) {
            switch (value)
            {
                case null:
                    JsonSerializer.Serialize(writer, (IEquipment)null, options); 
                    break;
                default:
                    var type = value.GetType();
                    JsonSerializer.Serialize(writer, value, type, options);
                    break;
            }
        }
    }
}
