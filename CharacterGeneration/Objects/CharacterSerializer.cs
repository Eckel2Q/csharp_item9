// CharacterGeneration\Objects\CharacterSerializer.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CharacterGeneration.Interfaces;

namespace CharacterGeneration.Objects
{
    public static class CharacterSerializer
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(), new EquipmentConverter(), new UnitConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

        };

        public static void SerializeUnit(IUnit unit, string filePath)
        {
            try
            {
                List<IUnit> units = new List<IUnit>();

                // Load existing units if file exists
                if (File.Exists(filePath))
                {
                    units = DeserializeUnits(filePath);
                }

                // Add new unit
                units.Add(unit);

                // Save all units
                string jsonString = JsonSerializer.Serialize(units, JsonOptions);
                File.WriteAllText(filePath, jsonString);

                Console.WriteLine($"Unit '{unit.Name}' saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving unit: {ex.Message}");
            }
        }

        public static List<IUnit> DeserializeUnits(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File {filePath} does not exist.");
                    return new List<IUnit>();
                }

                string jsonString = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    return new List<IUnit>();
                }

                var units = JsonSerializer.Deserialize<List<IUnit>>(jsonString, JsonOptions);
                return units ?? new List<IUnit>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading units: {ex.Message}");
                return new List<IUnit>();
            }
        }

        public static void DisplayUnitNames(string filePath)
        {
            var units = DeserializeUnits(filePath);

            if (units.Count == 0)
            {
                Console.WriteLine("No units found in the file.");
                return;
            }

            Console.WriteLine($"Units found in {filePath}:");
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                unit.PrintUnitInformation();
            }
        }
    }
}