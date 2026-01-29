// CharacterCreatationTest\Objects\CharacterCreatorTests.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CharacterGeneration.Interfaces;
using CharacterGeneration.Objects;

namespace CharacterGeneration.Tests
{
    [TestClass]
    public class CharacterCreatorTests
    {
        private Dictionary<AbilityScore, int> standardHuman = new Dictionary<AbilityScore, int> {
            { AbilityScore.Strength, 16 },
            { AbilityScore.Dexterity, 14 },
            { AbilityScore.Constitution, 15 },
            { AbilityScore.Charisma, 12 },
            { AbilityScore.Wisdom, 13 },
            { AbilityScore.Intelligence, 10 }
        };

        private Dictionary<AbilityScore, int> standardArrayAssignment = new Dictionary<AbilityScore, int> {
            { AbilityScore.Strength, 15 },
            { AbilityScore.Dexterity, 14 },
            { AbilityScore.Constitution, 13 },
            { AbilityScore.Intelligence, 12 },
            { AbilityScore.Wisdom, 10 },
            { AbilityScore.Charisma, 8 }
        };

        private Dictionary<AbilityScore, int> validPointBuy = new Dictionary<AbilityScore, int> {
            { AbilityScore.Strength, 15 }, // 9 points
            { AbilityScore.Dexterity, 14 }, // 7 points
            { AbilityScore.Constitution, 13 }, // 5 points
            { AbilityScore.Intelligence, 12 }, // 4 points
            { AbilityScore.Wisdom, 10 }, // 2 points
            { AbilityScore.Charisma, 8 } // 0 points
        }; // Total: 27 points

        [TestMethod]
        public void CreateCharacter_Manual_ReturnsIUnitWithCorrectProperties()
        {
            // Act
            IUnit character = CharacterCreator.CreateCharacter("Aragorn", CharacterRace.Human, standardHuman, experience: 1000);

            // Assert
            Assert.IsNotNull(character);
            Assert.AreEqual("Aragorn", character.Name);
            Assert.AreEqual("Human", character.Type);
            Assert.AreEqual(3, character.GetAbilityModifier(AbilityScore.Strength)); // (16-10)/2 = 3
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Dexterity)); // (14-10)/2 = 2
        }

        [TestMethod]
        public void CreateRandomCharacter_GeneratesValidAbilityScores()
        {
            // Act
            IUnit character = CharacterCreator.CreateRandomCharacter("Random", CharacterRace.Elf);

            // Assert
            Assert.IsNotNull(character);
            Assert.AreEqual("Random", character.Name);
            Assert.AreEqual("Elf", character.Type);

            // Check that all ability scores are within reasonable range (3-18 for 4d6 drop lowest)
            foreach (AbilityScore ability in Enum.GetValues(typeof(AbilityScore)))
            {
                int modifier = character.GetAbilityModifier(ability);
                // Modifier range should be roughly -4 to +4 for scores 3-18
                Assert.IsTrue(modifier >= -4 && modifier <= +4,
                    $"Ability modifier for {ability} is {modifier}, which seems out of range");
            }
        }

        [TestMethod]
        public void CreateRandomCharacter_MultipleCreations_ProducesDifferentResults()
        {
            // Act
            var character1 = CharacterCreator.CreateRandomCharacter("Random1", CharacterRace.Human);
            var character2 = CharacterCreator.CreateRandomCharacter("Random2", CharacterRace.Human);

            // Assert - At least one ability should be different (very high probability)
            bool foundDifference = false;
            foreach (AbilityScore ability in Enum.GetValues(typeof(AbilityScore)))
            {
                if (character1.GetAbilityModifier(ability) != character2.GetAbilityModifier(ability))
                {
                    foundDifference = true;
                    break;
                }
            }
            Assert.IsTrue(foundDifference, "Random characters should have different ability scores");
        }

        [TestMethod]
        public void CreateStandardArrayCharacter_ValidAssignment_ReturnsCorrectCharacter()
        {
            // Act
            IUnit character = CharacterCreator.CreateStandardArrayCharacter("Standard", CharacterRace.Dwarf, standardArrayAssignment);

            // Assert
            Assert.IsNotNull(character);
            Assert.AreEqual("Standard", character.Name);
            Assert.AreEqual("Dwarf", character.Type);
            Assert.AreEqual(4, character.GetAbilityModifier(AbilityScore.Strength)); // 18 -> +4
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Dexterity)); // 14 -> +2
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Constitution)); // 13 -> +1
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Intelligence)); // 12 -> +1
            Assert.AreEqual(0, character.GetAbilityModifier(AbilityScore.Wisdom)); // 10 -> +0
            Assert.AreEqual(-1, character.GetAbilityModifier(AbilityScore.Charisma)); // 8 -> -1
        }

        [TestMethod]
        public void CreateStandardArrayCharacter_InvalidAssignment_ThrowsException()
        {
            // Arrange
            var invalidAssignment = new Dictionary<AbilityScore, int> {
                { AbilityScore.Strength, 16 }, // Invalid - not in standard array
                { AbilityScore.Dexterity, 14 },
                { AbilityScore.Constitution, 13 },
                { AbilityScore.Intelligence, 12 },
                { AbilityScore.Wisdom, 10 },
                { AbilityScore.Charisma, 8 }
            };

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                CharacterCreator.CreateStandardArrayCharacter("Invalid", CharacterRace.Human, invalidAssignment));
        }

        [TestMethod]
        public void CreateStandardArrayCharacter_DuplicateValues_ThrowsException()
        {
            // Arrange
            var duplicateAssignment = new Dictionary<AbilityScore, int> {
                { AbilityScore.Strength, 15 },
                { AbilityScore.Dexterity, 15 }, // Duplicate 15
                { AbilityScore.Constitution, 13 },
                { AbilityScore.Intelligence, 12 },
                { AbilityScore.Wisdom, 10 },
                { AbilityScore.Charisma, 8 }
            };

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                CharacterCreator.CreateStandardArrayCharacter("Duplicate", CharacterRace.Human, duplicateAssignment));
        }

        [TestMethod]
        public void CreatePointBuyCharacter_ValidAssignment_ReturnsCorrectCharacter()
        {
            // Act
            IUnit character = CharacterCreator.CreatePointBuyCharacter("PointBuy", CharacterRace.Halfling, validPointBuy);

            // Assert
            Assert.IsNotNull(character);
            Assert.AreEqual("PointBuy", character.Name);
            Assert.AreEqual("Halfling", character.Type);
            Assert.AreEqual(3, character.GetAbilityModifier(AbilityScore.Strength)); // 16 -> +2
            Assert.AreEqual(-1, character.GetAbilityModifier(AbilityScore.Charisma)); // 8 -> -1
        }

        [TestMethod]
        public void CreatePointBuyCharacter_TooManyPoints_ThrowsException()
        {
            // Arrange - 28 points total (1 over limit)
            var tooManyPoints = new Dictionary<AbilityScore, int> {
                { AbilityScore.Strength, 15 }, // 9 points
                { AbilityScore.Dexterity, 14 }, // 7 points
                { AbilityScore.Constitution, 13 }, // 5 points
                { AbilityScore.Intelligence, 12 }, // 4 points
                { AbilityScore.Wisdom, 11 }, // 3 points (changed from 10->2 points)
                { AbilityScore.Charisma, 8 } // 0 points
            }; // Total: 28 points

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                CharacterCreator.CreatePointBuyCharacter("TooMany", CharacterRace.Human, tooManyPoints));
        }

        [TestMethod]
        public void CreatePointBuyCharacter_TooFewPoints_ThrowsException()
        {
            // Arrange - 25 points total (2 under limit)
            var tooFewPoints = new Dictionary<AbilityScore, int> {
                { AbilityScore.Strength, 15 }, // 9 points
                { AbilityScore.Dexterity, 14 }, // 7 points
                { AbilityScore.Constitution, 13 }, // 5 points
                { AbilityScore.Intelligence, 12 }, // 4 points
                { AbilityScore.Wisdom, 8 }, // 0 points (changed from 10->2 points)
                { AbilityScore.Charisma, 8 } // 0 points
            }; // Total: 25 points

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                CharacterCreator.CreatePointBuyCharacter("TooFew", CharacterRace.Human, tooFewPoints));
        }

        [TestMethod]
        public void CreatePointBuyCharacter_InvalidScore_ThrowsException()
        {
            // Arrange
            var invalidScore = new Dictionary<AbilityScore, int> {
                { AbilityScore.Strength, 16 }, // Invalid score (not in point buy table)
                { AbilityScore.Dexterity, 14 },
                { AbilityScore.Constitution, 13 },
                { AbilityScore.Intelligence, 12 },
                { AbilityScore.Wisdom, 10 },
                { AbilityScore.Charisma, 8 }
            };

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                CharacterCreator.CreatePointBuyCharacter("Invalid", CharacterRace.Human, invalidScore));
        }

        [TestMethod]
        public void GetPointCost_ValidScores_ReturnsCorrectCosts()
        {
            // Act & Assert
            Assert.AreEqual(0, CharacterCreator.GetPointCost(8));
            Assert.AreEqual(1, CharacterCreator.GetPointCost(9));
            Assert.AreEqual(2, CharacterCreator.GetPointCost(10));
            Assert.AreEqual(3, CharacterCreator.GetPointCost(11));
            Assert.AreEqual(4, CharacterCreator.GetPointCost(12));
            Assert.AreEqual(5, CharacterCreator.GetPointCost(13));
            Assert.AreEqual(7, CharacterCreator.GetPointCost(14));
            Assert.AreEqual(9, CharacterCreator.GetPointCost(15));
        }

        [TestMethod]
        public void GetPointCost_InvalidScore_ReturnsNegativeOne()
        {
            // Act & Assert
            Assert.AreEqual(-1, CharacterCreator.GetPointCost(7));
            Assert.AreEqual(-1, CharacterCreator.GetPointCost(16));
            Assert.AreEqual(-1, CharacterCreator.GetPointCost(20));
        }

        [TestMethod]
        public void CalculateTotalPointCost_ValidPointBuy_Returns27()
        {
            // Act
            int totalCost = CharacterCreator.CalculateTotalPointCost(validPointBuy);

            // Assert
            Assert.AreEqual(27, totalCost);
        }

        [TestMethod]
        public void CalculateTotalPointCost_IsValidPointBuy()
        {
            // Act
            bool isValid = CharacterCreator.IsValidPointBuy(validPointBuy);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void CalculateTotalPointCost_IsValidPointBuy_ReturnsFalse()
        {
            // Act
            bool isValid = CharacterCreator.IsValidPointBuy(standardHuman);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void CalculateTotalPointCost_IsValidStandardArray()
        {
            // Act
            bool isValid = CharacterCreator.IsValidStandardArray(standardArrayAssignment);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void CalculateTotalPointCost_IsValidStandardArray_ReturnsFalse()
        {
            // Act
            bool isValid = CharacterCreator.IsValidStandardArray(standardHuman);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void AllCreationMethods_ReturnIUnitInterface()
        {
            // Act
            var manual = CharacterCreator.CreateCharacter("Manual", CharacterRace.Human, standardHuman);
            var random = CharacterCreator.CreateRandomCharacter("Random", CharacterRace.Elf);
            var standardArray = CharacterCreator.CreateStandardArrayCharacter("Standard", CharacterRace.Dwarf, standardArrayAssignment);
            var pointBuy = CharacterCreator.CreatePointBuyCharacter("PointBuy", CharacterRace.Halfling, validPointBuy);

            // Assert
            Assert.IsInstanceOfType(manual, typeof(IUnit));
            Assert.IsInstanceOfType(random, typeof(IUnit));
            Assert.IsInstanceOfType(standardArray, typeof(IUnit));
            Assert.IsInstanceOfType(pointBuy, typeof(IUnit));
        }

        [TestMethod]
        public void IUnitInterface_AllMethodsWork()
        {
            // Arrange
            IUnit character = CharacterCreator.CreateCharacter("Test", CharacterRace.Human, standardHuman);
            var sword = new Weapon("Longsword", "A sharp blade", 3, 0, AbilityScore.Strength, 8, false, 0, 0, WeaponType.Martial);

            // Act & Assert
            Assert.AreEqual("Test", character.Name);
            Assert.AreEqual("Human", character.Type);
            Assert.AreEqual(3, character.GetAbilityModifier(AbilityScore.Strength));
            Assert.AreEqual(12, character.ArmorClass); // 10 + 2 (dex modifier)

            // Attack and SkillCheck will have random components, just verify they don't throw
            int attackResult = character.Attack(sword);
            int skillResult = character.SkillCheck(Skill.Athletics);

            Assert.IsTrue(attackResult > 0);
            Assert.IsTrue(skillResult > 0);
        }

        [TestMethod]
        public void CreateStandardArrayCharacter_ValidAssignment_ReturnsCorrectCharacter_Fighter()
        {
            // Act
            IUnit character = CharacterCreator.CreateStandardArrayCharacter("Standard", CharacterRace.Dwarf, standardArrayAssignment, characterClass: CharacterClass.Fighter);

            // Assert
            Assert.IsNotNull(character);
            Assert.AreEqual("Standard", character.Name);
            Assert.AreEqual("Dwarf", character.Type);
            Assert.AreEqual(4, character.GetAbilityModifier(AbilityScore.Strength)); // 15 + 2 (Fighter) + 1 (Dwarf) = 18 -> +4
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Dexterity)); // 14 -> +2
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Constitution)); // 13 -> +1
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Intelligence)); // 12 -> +1
            Assert.AreEqual(0, character.GetAbilityModifier(AbilityScore.Wisdom)); // 10 -> +0
            Assert.AreEqual(-1, character.GetAbilityModifier(AbilityScore.Charisma)); // 8 -> -1
        }

        [TestMethod]
        public void CreateStandardArrayCharacter_ValidAssignment_ReturnsCorrectCharacter_Wizard()
        {
            // Act
            IUnit character = CharacterCreator.CreateStandardArrayCharacter("Standard", CharacterRace.Gnome, standardArrayAssignment, characterClass: CharacterClass.Wizard);

            // Assert
            Assert.IsNotNull(character);
            Assert.AreEqual("Standard", character.Name);
            Assert.AreEqual("Gnome", character.Type);
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Strength)); // 15 -> +2
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Dexterity)); // 14 -> +2
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Constitution)); // 13 -> +1
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Intelligence)); // 12 + 2 (Wizard) + 1 (Gnome) = 15 -> +2
            Assert.AreEqual(0, character.GetAbilityModifier(AbilityScore.Wisdom)); // 10 -> +0
            Assert.AreEqual(-1, character.GetAbilityModifier(AbilityScore.Charisma)); // 8 -> -1
        }

        [TestMethod]
        public void CreateStandardArrayCharacter_ValidAssignment_ReturnsCorrectCharacter_Cleric()
        {
            // Act
            IUnit character = CharacterCreator.CreateStandardArrayCharacter("Standard", CharacterRace.Dragonborn, standardArrayAssignment, characterClass: CharacterClass.Cleric);

            // Assert
            Assert.IsNotNull(character);
            Assert.AreEqual("Standard", character.Name);
            Assert.AreEqual("Dragonborn", character.Type);
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Strength)); // 15 -> +2
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Dexterity)); // 14 -> +2
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Constitution)); // 13 -> +1
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Intelligence)); // 12 -> +1
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Wisdom)); // 10 + 2 (Cleric) + 1 (Dragonborn) = 13 -> +1
            Assert.AreEqual(-1, character.GetAbilityModifier(AbilityScore.Charisma)); // 8 -> -1
        }

        [TestMethod]
        public void CreateStandardArrayCharacter_ValidAssignment_ReturnsCorrectCharacter_Bard()
        {
            // Act
            IUnit character = CharacterCreator.CreateStandardArrayCharacter("Standard", CharacterRace.Halfling, standardArrayAssignment, characterClass: CharacterClass.Bard);

            // Assert
            Assert.IsNotNull(character);
            Assert.AreEqual("Standard", character.Name);
            Assert.AreEqual("Halfling", character.Type);
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Strength)); // 15 -> +2
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Dexterity)); // 14 -> +2
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Constitution)); // 13 -> +1
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Intelligence)); // 12 -> +1
            Assert.AreEqual(0, character.GetAbilityModifier(AbilityScore.Wisdom)); // 10 -> +0
            Assert.AreEqual(0, character.GetAbilityModifier(AbilityScore.Charisma)); // 8 + 2 (Bard) + 1 (Halfling) = 11 -> +0
        }

        [TestMethod]
        public void CreateStandardArrayCharacter_ValidAssignment_ReturnsCorrectCharacter_Barbarian()
        {
            // Act
            IUnit character = CharacterCreator.CreateStandardArrayCharacter("Standard", CharacterRace.Human, standardArrayAssignment, characterClass: CharacterClass.Barbarian);

            // Assert
            Assert.IsNotNull(character);
            Assert.AreEqual("Standard", character.Name);
            Assert.AreEqual("Human", character.Type);
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Strength)); // 15 -> +2
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Dexterity)); // 14 -> +2
            Assert.AreEqual(3, character.GetAbilityModifier(AbilityScore.Constitution)); // 13 + 2 (Barbarian) + 1 (Human) = 16 -> +3
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Intelligence)); // 12 -> +1
            Assert.AreEqual(0, character.GetAbilityModifier(AbilityScore.Wisdom)); // 10 -> +0
            Assert.AreEqual(-1, character.GetAbilityModifier(AbilityScore.Charisma)); // 8 -> -1
        }

        [TestMethod]
        public void CreateStandardArrayCharacter_ValidAssignment_ReturnsCorrectCharacter_Thief()
        {
            // Act
            IUnit character = CharacterCreator.CreateStandardArrayCharacter("Standard", CharacterRace.Elf, standardArrayAssignment, characterClass: CharacterClass.Thief);

            // Assert
            Assert.IsNotNull(character);
            Assert.AreEqual("Standard", character.Name);
            Assert.AreEqual("Elf", character.Type);
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Strength)); // 15 -> +2
            Assert.AreEqual(3, character.GetAbilityModifier(AbilityScore.Dexterity)); // 14 + 2 (Thief) + 1 (Elf) = 17 -> +3
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Constitution)); // 13 -> +1
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Intelligence)); // 12 -> +1
            Assert.AreEqual(0, character.GetAbilityModifier(AbilityScore.Wisdom)); // 10 -> +0
            Assert.AreEqual(-1, character.GetAbilityModifier(AbilityScore.Charisma)); // 8 -> -1
        }

    }
}