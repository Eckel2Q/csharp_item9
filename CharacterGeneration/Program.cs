// CharacterGeneration\Program.cs
using CharacterGeneration;
using CharacterGeneration.Objects;
using System;

Console.WriteLine("Character Generation and Serialization Demo");
Console.WriteLine("==========================================");

string filePath = "characters.json";

while (true)
{
    // Get character name from user
    Console.WriteLine("\nEnter a character name (or 'quit' to exit):");
    string characterName = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(characterName))
    {
        Console.WriteLine("Please enter a valid name.");
        continue;
    }

    if (characterName.ToLower() == "quit")
    {
        break;
    }

    // Generate random character with random race and class
    var randomRace = Character.GetRandomRace();
    var randomClass = Character.GetRandomClass();

    Character currentCharacter = null;
    bool characterFinalized = false;

    while (!characterFinalized)
    {
        // Generate new random character
        currentCharacter = (Character)CharacterCreator.CreateRandomCharacter(
            characterName,
            randomRace,
            level: 1,
            characterClass: randomClass
        );

        // Display character info
        Character.PrintCharacterDetails(currentCharacter);

        // Ask user to save or reroll
        Console.WriteLine("\nWould you like to save this character? (y/n):");
        string response = Console.ReadLine()?.Trim().ToLower();

        if (response == "y" || response == "yes")
        {
            // Save character
            CharacterSerializer.SerializeUnit(currentCharacter, filePath);
            characterFinalized = true;

            // Display all characters
            Console.WriteLine("\nAll saved characters:");
            CharacterSerializer.DisplayUnitNames(filePath);
        }
        else if (response == "n" || response == "no")
        {
            // Generate new random race and class for reroll
            randomRace = Character.GetRandomRace();
            randomClass = Character.GetRandomClass();
            Console.WriteLine("\nRerolling character...");
        }
        else
        {
            Console.WriteLine("Please enter 'y' for yes or 'n' for no.");
        }
    }
}

Console.WriteLine("\nThanks for using the Character Generator!");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();




