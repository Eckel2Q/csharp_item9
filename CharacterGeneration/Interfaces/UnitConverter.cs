// CharacterGeneration\Objects\UnitConverter.cs
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CharacterGeneration.Objects;

namespace CharacterGeneration.Interfaces
{
    public class UnitConverter : JsonConverter<IUnit>
    {
        public override IUnit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;

                // Check if it has properties that indicate it's a Character
                if (root.TryGetProperty("class", out _) ||
                    root.TryGetProperty("race", out _) ||
                    root.TryGetProperty("level", out _) ||
                    root.TryGetProperty("equipment", out _))
                {
                    return JsonSerializer.Deserialize<Character>(root.GetRawText(), options);
                }
                else
                {
                    return JsonSerializer.Deserialize<Monster>(root.GetRawText(), options);
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, IUnit value, JsonSerializerOptions options)
        {
            if (value is Character character)
            {
                JsonSerializer.Serialize(writer, character, options);
            }
            else if (value is Monster monster)
            {
                JsonSerializer.Serialize(writer, monster, options);
            }
            else
            {
                throw new NotSupportedException($"Unit type {value.GetType()} is not supported");
            }
        }
    }
}