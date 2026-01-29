using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CharacterGeneration;
using CharacterGeneration.Objects;

namespace CharacterGeneration.Tests
{
    [TestClass]
    public class SpellTests
    {
        private Dictionary<AbilityScore, int> standardStats = new Dictionary<AbilityScore, int> {
            { AbilityScore.Strength, 10 },
            { AbilityScore.Dexterity, 10 },
            { AbilityScore.Constitution, 10 },
            { AbilityScore.Charisma, 10 },
            { AbilityScore.Wisdom, 10 },
            { AbilityScore.Intelligence, 10 }
        };

        [TestMethod]
        public void AddSpell_ValidClassAndLevel_ReturnsTrue()
        {
            // Arrange
            var character = new Character("Wizard", CharacterRace.Human, CharacterClass.Wizard, standardStats, experience: 0, applyClassBonuses: false);
            character.Level = 1; // Max spell level = (1/2) + 1 = 1
            
            var spell = new Spell("Magic Missile", 1, "Damage", new List<CharacterClass> { CharacterClass.Wizard });

            // Act
            bool result = character.AddSpell(spell);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, character.Spells.Count);
        }

        [TestMethod]
        public void AddSpell_InvalidClass_ReturnsFalse()
        {
            // Arrange
            var character = new Character("Fighter", CharacterRace.Human, CharacterClass.Fighter, standardStats, experience: 0, applyClassBonuses: false);
            
            var spell = new Spell("Magic Missile", 1, "Damage", new List<CharacterClass> { CharacterClass.Wizard });

            // Act
            bool result = character.AddSpell(spell);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, character.Spells.Count);
        }

        [TestMethod]
        public void AddSpell_InvalidLevel_ReturnsFalse()
        {
            // Arrange
            var character = new Character("Wizard", CharacterRace.Human, CharacterClass.Wizard, standardStats, experience: 0, applyClassBonuses: false);
            character.Level = 1; // Max spell level = (1/2) + 1 = 1
            
            var spell = new Spell("Fireball", 3, "Big Damage", new List<CharacterClass> { CharacterClass.Wizard });

            // Act
            bool result = character.AddSpell(spell);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, character.Spells.Count);
        }

        [TestMethod]
        public void AddSpell_InvalidClass_Override_ReturnsTrue()
        {
            // Arrange
            var character = new Character("Fighter", CharacterRace.Human, CharacterClass.Fighter, standardStats, experience: 0, applyClassBonuses: false);
            
            var spell = new Spell("Magic Missile", 1, "Damage", new List<CharacterClass> { CharacterClass.Wizard });

            // Act
            bool result = character.AddSpell(spell, overrideRules: true);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, character.Spells.Count);
        }

        [TestMethod]
        public void AddSpell_InvalidLevel_Override_ReturnsTrue()
        {
            // Arrange
            var character = new Character("Wizard", CharacterRace.Human, CharacterClass.Wizard, standardStats, experience: 0, applyClassBonuses: false);
            character.Level = 1; 
            
            var spell = new Spell("Fireball", 3, "Big Damage", new List<CharacterClass> { CharacterClass.Wizard });

            // Act
            bool result = character.AddSpell(spell, overrideRules: true);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, character.Spells.Count);
        }

        [TestMethod]
        public void Monster_AddSpell_AlwaysReturnsTrue()
        {
            // Arrange
            var monster = new Monster("Goblin Mage", "Goblin", 10, 12, new Dictionary<AbilityScore, int>());
            var spell = new Spell("Fireball", 3, "Big Damage", new List<CharacterClass> { CharacterClass.Wizard });

            // Act
            bool result = monster.AddSpell(spell);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, monster.Spells.Count);
        }

        [TestMethod]
        public void AddSpell_HigherLevelCharacter_CanLearnHigherLevelSpells()
        {
             // Arrange
            var character = new Character("Wizard", CharacterRace.Human, CharacterClass.Wizard, standardStats, experience: 0, applyClassBonuses: false);
            character.Level = 5; // Max spell level = (5/2) + 1 = 2 + 1 = 3
            
            var spell = new Spell("Fireball", 3, "Big Damage", new List<CharacterClass> { CharacterClass.Wizard });

            // Act
            bool result = character.AddSpell(spell);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, character.Spells.Count);
        }
    }
}
