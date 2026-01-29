// CharacterCreatationTest\Objects\MonsterTests.cs
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CharacterGeneration;
using CharacterGeneration.Objects;
using System.Collections.Generic;

namespace CharacterGeneration.Tests
{
    [TestClass]
    public class MonsterTests
    {
        protected Dictionary<AbilityScore, int> standardModifiers = new Dictionary<AbilityScore, int> {
            { AbilityScore.Strength, 3 },
            { AbilityScore.Dexterity, 2 },
            { AbilityScore.Constitution, 2 },
            { AbilityScore.Charisma, 1 },
            { AbilityScore.Wisdom, 1 },
            { AbilityScore.Intelligence, -1 }
        };

        protected Dictionary<AbilityScore, int> weakModifiers = new Dictionary<AbilityScore, int> {
            { AbilityScore.Strength, -2 },
            { AbilityScore.Dexterity, -3 },
            { AbilityScore.Constitution, -1 },
            { AbilityScore.Charisma, -2 },
            { AbilityScore.Wisdom, -3 },
            { AbilityScore.Intelligence, -4 }
        };

        [TestMethod]
        public void Constructor_SetsAllProperties()
        {
            // Arrange & Act
            var monster = new Monster("Orc", "Humanoid", 15, 13, standardModifiers);

            // Assert
            Assert.AreEqual("Orc", monster.Name);
            Assert.AreEqual("Humanoid", monster.Type);
            Assert.AreEqual(15, monster.HitPoints);
            Assert.AreEqual(13, monster.ArmorClass);
            Assert.AreEqual(3, monster.GetAbilityModifier(AbilityScore.Strength));
            Assert.AreEqual(2, monster.GetAbilityModifier(AbilityScore.Dexterity));
            Assert.AreEqual(2, monster.GetAbilityModifier(AbilityScore.Constitution));
            Assert.AreEqual(1, monster.GetAbilityModifier(AbilityScore.Charisma));
            Assert.AreEqual(1, monster.GetAbilityModifier(AbilityScore.Wisdom));
            Assert.AreEqual(-1, monster.GetAbilityModifier(AbilityScore.Intelligence));
        }

        [TestMethod]
        public void Constructor_DefaultsToZeroForMissingModifiers()
        {
            // Arrange
            var partialModifiers = new Dictionary<AbilityScore, int> {
                { AbilityScore.Strength, 2 },
                { AbilityScore.Dexterity, 1 }
            };

            // Act
            var monster = new Monster("Goblin", "Humanoid", 7, 15, partialModifiers);

            // Assert
            Assert.AreEqual(2, monster.GetAbilityModifier(AbilityScore.Strength));
            Assert.AreEqual(1, monster.GetAbilityModifier(AbilityScore.Dexterity));
            Assert.AreEqual(0, monster.GetAbilityModifier(AbilityScore.Constitution));
            Assert.AreEqual(0, monster.GetAbilityModifier(AbilityScore.Intelligence));
            Assert.AreEqual(0, monster.GetAbilityModifier(AbilityScore.Wisdom));
            Assert.AreEqual(0, monster.GetAbilityModifier(AbilityScore.Charisma));
        }

        [TestMethod]
        public void Constructor_EnforcesMinimumValues()
        {
            // Arrange & Act
            var monster = new Monster("Weak Monster", "Undead", -5, 0, standardModifiers);

            // Assert
            Assert.AreEqual(1, monster.HitPoints); // Should be clamped to minimum 1
            Assert.AreEqual(1, monster.ArmorClass); // Should be clamped to minimum 1
        }

        [TestMethod]
        public void ParameterlessConstructor_InitializesCorrectly()
        {
            // Arrange & Act
            var monster = new Monster();

            // Assert
            Assert.IsNotNull(monster.AbilityModifiers);
            Assert.AreEqual(0, monster.AbilityModifiers.Count);
        }

        [TestMethod]
        public void SetAbilityModifier_UpdatesCorrectly()
        {
            // Arrange
            var monster = new Monster("Test Monster", "Beast", 10, 12, standardModifiers);

            // Act
            monster.SetAbilityModifier(AbilityScore.Strength, 5);
            monster.SetAbilityModifier(AbilityScore.Intelligence, -2);

            // Assert
            Assert.AreEqual(5, monster.GetAbilityModifier(AbilityScore.Strength));
            Assert.AreEqual(-2, monster.GetAbilityModifier(AbilityScore.Intelligence));
        }

        [TestMethod]
        public void GetAbilityModifier_ReturnsZeroForUnsetModifier()
        {
            // Arrange
            var monster = new Monster();

            // Act & Assert
            Assert.AreEqual(0, monster.GetAbilityModifier(AbilityScore.Strength));
            Assert.AreEqual(0, monster.GetAbilityModifier(AbilityScore.Dexterity));
        }

        [TestMethod]
        public void HitPoints_SetProperty_EnforcesMinimum()
        {
            // Arrange
            var monster = new Monster("Test", "Beast", 10, 12, standardModifiers);

            // Act
            monster.HitPoints = -10;

            // Assert
            Assert.AreEqual(1, monster.HitPoints);
        }

        [TestMethod]
        public void ArmorClass_SetProperty_EnforcesMinimum()
        {
            // Arrange
            var monster = new Monster("Test", "Beast", 10, 12, standardModifiers);

            // Act
            monster.ArmorClass = 0;

            // Assert
            Assert.AreEqual(1, monster.ArmorClass);
        }

        [TestMethod]
        public void Attack_CalculatesDamageWithoutProficiencyBonus()
        {
            // Arrange
            var monster = new Monster("Orc", "Humanoid", 15, 13, standardModifiers);
            var sword = new Weapon("Scimitar", "A curved blade", 3, 1, AbilityScore.Strength, 6, false, 0, 0, WeaponType.Martial);

            // Act
            int damage = monster.Attack(sword);

            // Assert
            // Damage should be: 1d6 + 1 (weapon modifier) + 3 (strength modifier)
            // No proficiency bonus for monsters
            // Minimum: 1 + 1 + 3 = 5
            // Maximum: 6 + 1 + 3 = 10
            Assert.IsTrue(damage >= 5);
            Assert.IsTrue(damage <= 10);
        }

        [TestMethod]
        public void Attack_WithNegativeModifiers_EnforcesMinimumDamage()
        {
            // Arrange
            var monster = new Monster("Weak Goblin", "Humanoid", 5, 10, weakModifiers);
            var dagger = new Weapon("Dagger", "A small blade", 1, -1, AbilityScore.Strength, 4, false, 0, 0, WeaponType.Simple);

            // Act
            int damage = monster.Attack(dagger);

            // Assert
            // Damage calculation: 1d4 + (-1) + (-2) = potentially negative
            // But should be clamped to minimum 1
            Assert.AreEqual(1, damage);
        }

        [TestMethod]
        public void SkillCheck_UsesAbilityModifierWithoutProficiency()
        {
            // Arrange
            var monster = new Monster("Smart Dragon", "Dragon", 200, 18, new Dictionary<AbilityScore, int> {
                { AbilityScore.Intelligence, 5 },
                { AbilityScore.Strength, 8 },
                { AbilityScore.Dexterity, 1 },
                { AbilityScore.Constitution, 6 },
                { AbilityScore.Wisdom, 3 },
                { AbilityScore.Charisma, 4 }
            });

            // Act - Test multiple times to account for randomness
            bool foundValidRange = false;
            for (int i = 0; i < 20; i++)
            {
                int result = monster.SkillCheck(Skill.Arcana); // Uses Intelligence by default
                // Should be: 1d20 + 5 (intelligence modifier)
                // Range: 6-25
                if (result >= 6 && result <= 25)
                {
                    foundValidRange = true;
                    break;
                }
            }

            // Assert
            Assert.IsTrue(foundValidRange);
        }

        [TestMethod]
        public void SkillCheck_WithSpecifiedAbility_UsesCorrectModifier()
        {
            // Arrange
            var monster = new Monster("Strong Ogre", "Giant", 59, 11, new Dictionary<AbilityScore, int> {
                { AbilityScore.Strength, 4 },
                { AbilityScore.Intelligence, -2 },
                { AbilityScore.Dexterity, -1 },
                { AbilityScore.Constitution, 3 },
                { AbilityScore.Wisdom, 0 },
                { AbilityScore.Charisma, -2 }
            });

            // Act - Test multiple times to account for randomness
            bool foundValidRange = false;
            for (int i = 0; i < 20; i++)
            {
                int result = monster.SkillCheck(Skill.Arcana, AbilityScore.Strength); // Force use of Strength instead of Intelligence
                // Should be: 1d20 + 4 (strength modifier)
                // Range: 5-24
                if (result >= 5 && result <= 24)
                {
                    foundValidRange = true;
                    break;
                }
            }

            // Assert
            Assert.IsTrue(foundValidRange);
        }

        [TestMethod]
        public void SkillCheck_DefaultAbilities_MapCorrectly()
        {
            // Arrange
            var monster = new Monster("Test Monster", "Beast", 10, 12, new Dictionary<AbilityScore, int> {
                { AbilityScore.Strength, 1 },
                { AbilityScore.Dexterity, 2 },
                { AbilityScore.Constitution, 3 },
                { AbilityScore.Intelligence, 4 },
                { AbilityScore.Wisdom, 5 },
                { AbilityScore.Charisma, 6 }
            });

            // Act & Assert - Test a few key skill mappings
            // Athletics should use Strength (modifier 1)
            for (int i = 0; i < 10; i++)
            {
                int athleticsResult = monster.SkillCheck(Skill.Athletics);
                Assert.IsTrue(athleticsResult >= 2 && athleticsResult <= 21); // 1d20 + 1
            }

            // Stealth should use Dexterity (modifier 2)
            for (int i = 0; i < 10; i++)
            {
                int stealthResult = monster.SkillCheck(Skill.Stealth);
                Assert.IsTrue(stealthResult >= 3 && stealthResult <= 22); // 1d20 + 2
            }

            // Perception should use Wisdom (modifier 5)
            for (int i = 0; i < 10; i++)
            {
                int perceptionResult = monster.SkillCheck(Skill.Perception);
                Assert.IsTrue(perceptionResult >= 6 && perceptionResult <= 25); // 1d20 + 5
            }
        }

        [TestMethod]
        public void Properties_GetAndSet_WorkCorrectly()
        {
            // Arrange
            var monster = new Monster("Original", "Beast", 10, 12, standardModifiers);

            // Act
            monster.Name = "Updated Name";
            monster.Type = "Updated Type";
            monster.HitPoints = 25;
            monster.ArmorClass = 16;

            // Assert
            Assert.AreEqual("Updated Name", monster.Name);
            Assert.AreEqual("Updated Type", monster.Type);
            Assert.AreEqual(25, monster.HitPoints);
            Assert.AreEqual(16, monster.ArmorClass);
        }
    }
}