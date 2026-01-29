// CharacterGeneration\CharacterCreator.cs
using CharacterGeneration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CharacterGeneration.Objects
{
    public enum CreationMethod
    {
        Manual,
        Random,
        StandardArray,
        PointBuy
    }

    public class CharacterCreator
    {
        private static readonly Random random = new Random();

        // Standard array values that must be used exactly once each
        private static readonly int[] StandardArrayValues = { 15, 14, 13, 12, 10, 8 };

        // Point buy cost table
        private static readonly Dictionary<int, int> PointBuyCosts = new Dictionary<int, int>
        {
            { 8, 0 }, { 9, 1 }, { 10, 2 }, { 11, 3 }, { 12, 4 }, { 13, 5 }, { 14, 7 }, { 15, 9 }
        };

        // Method 1: Manual stat assignment (no class bonuses applied)
        public static IUnit CreateCharacter(string name, CharacterRace race, Dictionary<AbilityScore, int> abilityScores,
                                          int level = 1, int experience = 0, CharacterClass characterClass = CharacterClass.Fighter)
        {
            return new Character(name, race, characterClass, abilityScores, level, experience, applyClassBonuses: false);
        }

        // Method 2: Random stat generation (4d6, drop lowest) with class bonuses
        public static IUnit CreateRandomCharacter(string name, CharacterRace race, int level = 1, int experience = 0,
                                                CharacterClass characterClass = CharacterClass.Fighter)
        {
            var abilityScores = new Dictionary<AbilityScore, int>();

            foreach (AbilityScore ability in Enum.GetValues(typeof(AbilityScore)))
            {
                abilityScores[ability] = RollAbilityScore();
            }

            return new Character(name, race, characterClass, abilityScores, level, experience, applyClassBonuses: true);
        }

        // Method 3: Standard array (15,14,13,12,10,8 assigned to abilities) with class bonuses
        public static IUnit CreateStandardArrayCharacter(string name, CharacterRace race,
                                                       Dictionary<AbilityScore, int> abilityAssignments,
                                                       int level = 1, int experience = 0,
                                                       CharacterClass characterClass = CharacterClass.Fighter)
        {
            if (!IsValidStandardArray(abilityAssignments))
            {
                throw new ArgumentException("Standard array must use exactly the values: 15, 14, 13, 12, 10, 8");
            }

            return new Character(name, race, characterClass, abilityAssignments, level, experience, applyClassBonuses: true);
        }

        // Method 4: Point buy system (27 points total) with class bonuses
        public static IUnit CreatePointBuyCharacter(string name, CharacterRace race,
                                                  Dictionary<AbilityScore, int> abilityScores,
                                                  int level = 1, int experience = 0,
                                                  CharacterClass characterClass = CharacterClass.Fighter)
        {
            if (!IsValidPointBuy(abilityScores))
            {
                throw new ArgumentException("Point buy scores must total exactly 27 points and use values 8-15");
            }

            return new Character(name, race, characterClass, abilityScores, level, experience, applyClassBonuses: true);
        }

        // Helper method: Roll 4d6, drop lowest
        private static int RollAbilityScore()
        {
            var rolls = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                rolls.Add(random.Next(1, 7)); // 1d6
            }

            // Remove the lowest roll
            rolls.Remove(rolls.Min());

            return rolls.Sum();
        }

        // Helper method: Validate standard array
        public static bool IsValidStandardArray(Dictionary<AbilityScore, int> abilityScores)
        {
            if (abilityScores.Count != 6)
                return false;

            var values = abilityScores.Values.OrderBy(x => x).ToArray();
            var expectedValues = StandardArrayValues.OrderBy(x => x).ToArray();

            return values.SequenceEqual(expectedValues);
        }

        // Helper method: Validate point buy
        public static bool IsValidPointBuy(Dictionary<AbilityScore, int> abilityScores)
        {
            if (abilityScores.Count != 6)
                return false;

            int totalCost = 0;

            foreach (var score in abilityScores.Values)
            {
                if (!PointBuyCosts.ContainsKey(score))
                    return false; // Invalid score value

                totalCost += PointBuyCosts[score];
            }

            return totalCost == 27;
        }

        // Utility method to get point cost for a score
        public static int GetPointCost(int score)
        {
            return PointBuyCosts.TryGetValue(score, out int cost) ? cost : -1;
        }

        // Utility method to get total point cost for a set of scores
        public static int CalculateTotalPointCost(Dictionary<AbilityScore, int> abilityScores)
        {
            return abilityScores.Values.Sum(score => PointBuyCosts.TryGetValue(score, out int cost) ? cost : 0);
        }

    }
}