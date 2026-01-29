// CharacterCreatationTest\Objects\CharacterTests.cs
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CharacterGeneration;
using CharacterGeneration.Objects;

namespace CharacterGeneration.Tests
{
    [TestClass]
    public class CharacterTests
    {
        protected Dictionary<AbilityScore, int> standardHuman = new Dictionary<AbilityScore, int> {
            { AbilityScore.Strength, 16 },
            { AbilityScore.Dexterity, 14 },
            { AbilityScore.Constitution, 15 },
            { AbilityScore.Charisma, 12 },
            { AbilityScore.Wisdom, 13 },
            { AbilityScore.Intelligence, 10 }
        };
        protected Dictionary<AbilityScore, int> weakHuman = new Dictionary<AbilityScore, int> {
            { AbilityScore.Strength, 8 },
            { AbilityScore.Dexterity, 6 },
            { AbilityScore.Constitution, 9 },
            { AbilityScore.Charisma, 7 },
            { AbilityScore.Wisdom, 5 },
            { AbilityScore.Intelligence, 3 }
        };
        [TestMethod]
        public void Constructor_SetsAllProperties()
        {
            // Arrange & Act
            
            var character = new Character("Aragorn", CharacterRace.Human, CharacterClass.Fighter, standardHuman, experience: 1000, applyClassBonuses: false);

            // Assert
            Assert.AreEqual("Aragorn", character.Name);
            Assert.AreEqual(CharacterRace.Human, character.Race);
            Assert.AreEqual(16, character.GetAbilityScore(AbilityScore.Strength));
            Assert.AreEqual(14, character.GetAbilityScore(AbilityScore.Dexterity));
            Assert.AreEqual(15, character.GetAbilityScore(AbilityScore.Constitution));
            Assert.AreEqual(12, character.GetAbilityScore(AbilityScore.Charisma));
            Assert.AreEqual(13, character.GetAbilityScore(AbilityScore.Wisdom));
            Assert.AreEqual(10, character.GetAbilityScore(AbilityScore.Intelligence));
            Assert.AreEqual(3, character.Level); // Level should be calculated from 1000 exp
            Assert.AreEqual(1000, character.Experience);
        }

        [TestMethod]
        public void AbilityModifiers_CalculateCorrectly()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, experience: 1000,applyClassBonuses: false);
            character.SetAbilityScore(AbilityScore.Intelligence, 8);

            // Act & Assert
            Assert.AreEqual(3, character.GetAbilityModifier(AbilityScore.Strength));      // (16-10)/2 = 3
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Dexterity));     // (14-10)/2 = 2
            Assert.AreEqual(2, character.GetAbilityModifier(AbilityScore.Constitution));  // (15-10)/2 = 2.5 -> 2
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Charisma));      // (12-10)/2 = 1
            Assert.AreEqual(1, character.GetAbilityModifier(AbilityScore.Wisdom));        // (13-10)/2 = 1.5 -> 1
            Assert.AreEqual(-1, character.GetAbilityModifier(AbilityScore.Intelligence)); // (8-10)/2 = -1
        }

        [TestMethod]
        public void AbilityModifiers_HandleNegativeValues_RoundDown()
        {
            // Arrange
            var character = new Character("Weak", CharacterRace.Human, CharacterClass.Fighter, weakHuman, applyClassBonuses: false);

            // Act & Assert
            Assert.AreEqual(-1, character.GetAbilityModifier(AbilityScore.Strength));      // (8-10)/2 = -1
            Assert.AreEqual(-2, character.GetAbilityModifier(AbilityScore.Dexterity));     // (6-10)/2 = -2
            Assert.AreEqual(-1, character.GetAbilityModifier(AbilityScore.Constitution));  // (9-10)/2 = -0.5 -> -1 (rounds down)
            Assert.AreEqual(-2, character.GetAbilityModifier(AbilityScore.Charisma));      // (7-10)/2 = -1.5 -> -2 (rounds down)
            Assert.AreEqual(-3, character.GetAbilityModifier(AbilityScore.Wisdom));        // (5-10)/2 = -2.5 -> -3 (rounds down)
            Assert.AreEqual(-4, character.GetAbilityModifier(AbilityScore.Intelligence));  // (3-10)/2 = -3.5 -> -4 (rounds down)
        }

        [TestMethod]
        public void Level_SetLevel_UpdatesExperience()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);

            // Act
            character.Level = 5;

            // Assert
            Assert.AreEqual(5, character.Level);
            Assert.AreEqual(6500, character.Experience); // Min exp for level 5
        }

        [TestMethod]
        public void Experience_SetExperience_UpdatesLevel()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);

            // Act
            character.Experience = 15000;

            // Assert
            Assert.AreEqual(6, character.Level); // 15000 exp = level 6
            Assert.AreEqual(15000, character.Experience);
        }

        [TestMethod]
        public void Experience_BoundaryValues_CalculateCorrectLevel()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);

            // Test various boundary values
            character.Experience = 0;
            Assert.AreEqual(1, character.Level);

            character.Experience = 299;
            Assert.AreEqual(1, character.Level);

            character.Experience = 300;
            Assert.AreEqual(2, character.Level);

            character.Experience = 2700;
            Assert.AreEqual(4, character.Level);

            character.Experience = 355000;
            Assert.AreEqual(20, character.Level);
        }

        [TestMethod]
        public void Level_BoundaryValues_CalculateCorrectExperiance()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);


            // Test various boundary values
            character.Level = 10;
            Assert.AreEqual(64000, character.Experience);
            character.Level = 9;
            Assert.AreEqual(63999, character.Experience);
            character.Level = 11;
            Assert.AreEqual(85000, character.Experience);
            character.Level = 10;
            Assert.AreEqual(84999, character.Experience);
            character.Level = 0;
            Assert.AreEqual(299, character.Experience);
            character.Level = 10;
            Assert.AreEqual(64000, character.Experience);



        }

        [TestMethod]
        public void Level_ClampsBetween1And20()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);

            // Act & Assert
            character.Level = 0;
            Assert.AreEqual(1, character.Level);

            character.Level = 25;
            Assert.AreEqual(20, character.Level);
        }

        [TestMethod]
        public void ArmorClass_NoArmor_Returns10PlusDexModifier()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // Dex 14 = +2

            // Act & Assert
            Assert.AreEqual(12, character.ArmorClass); // 10 + 2
        }

        [TestMethod]
        public void ArmorClass_WithBaseArmor_UsesArmorBase()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // Dex 14 = +2
            var chainMail = new Armor("Chain Mail", "Medium armor", 20, ArmorType.Medium, 0, 16);

            // Act
            character.AddEquipment(chainMail);
            character.EquipItem(chainMail);
            

            // Assert
            Assert.AreEqual(16, character.ArmorClass); // Armor base 16, modifier 0
        }

        [TestMethod]
        public void ArmorClass_WithShield_StacksModifier()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // Dex 14 = +2
            var chainMail = new Armor("Chain Mail", "Medium armor", 20, ArmorType.Medium, 0, 16);
            var shield = new Armor("Shield", "Protective shield", 6, ArmorType.Medium, 2, 0);

            // Act
            character.AddEquipment(chainMail);
            character.AddEquipment(shield);
            character.EquipItem(chainMail);
            character.EquipItem(shield); 

            // Assert
            Assert.AreEqual(18, character.ArmorClass); // 16 (base) + 0 (chain modifier) + 2 (shield modifier)
        }

        [TestMethod]
        public void ArmorClass_OnlyShield_UsesCharacterBaseWithModifier()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // Dex 14 = +2
            var shield = new Armor("Shield", "Protective shield", 6, ArmorType.Medium, 2, 0);

            // Act
            character.AddEquipment(shield);
            character.EquipItem(shield);

            // Assert
            Assert.AreEqual(14, character.ArmorClass); // 10 + 2 (dex) + 2 (shield)
        }

        [TestMethod]
        public void ArmorClass_MultipleArmorModifiers_StackCorrectly()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);
            var plateArmor = new Armor("Plate Armor", "Heavy armor", 65, ArmorType.Heavy, 0, 18);
            var shield = new Armor("Shield", "Shield", 6, ArmorType.Medium, 2, 0);
            var magicCloak = new Armor("Cloak of Protection", "Magic cloak", 1, ArmorType.None, 1, 0);

            // Act
            character.AddEquipment(plateArmor);
            character.AddEquipment(shield);
            character.AddEquipment(magicCloak);
            character.EquipItem(plateArmor);
            character.EquipItem(shield);
            character.EquipItem(magicCloak);

            // Assert
            Assert.AreEqual(21, character.ArmorClass); // 18 (base) + 0 + 2 + 1 = 21
        }


        [TestMethod]
        public void CarryingCapacity_EqualsStrengthTimesFifteen()
        {
            // Arrange
            var character = new Character("Strong", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);
            character.SetAbilityScore(AbilityScore.Strength, 18);

            // Act & Assert
            Assert.AreEqual(270, character.CarryingCapacity);
        }

        [TestMethod]
        public void AddEquipment_WithinCapacity_ReturnsTrue()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // 240 capacity
            var sword = new Weapon("Longsword", "A sharp blade", 3, 0, AbilityScore.Strength, 8, false, 0, 0, WeaponType.Martial);

            // Act
            bool result = character.AddEquipment(sword);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, character.GetEquipment().Count);
            Assert.AreEqual(3, character.CurrentWeight);
        }

        [TestMethod]
        public void AddEquipment_ExceedsCapacity_ReturnsFalse()
        {
            // Arrange
            var character = new Character("Weak", CharacterRace.Human, CharacterClass.Fighter, weakHuman, applyClassBonuses: false); // 120 capacity
            var heavyArmor = new Armor("Plate Mail", "Heavy armor", 130, ArmorType.Heavy, 8, 18);

            // Act
            bool result = character.AddEquipment(heavyArmor);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, character.GetEquipment().Count);
            Assert.AreEqual(0, character.CurrentWeight);
        }

        [TestMethod]
        public void AddEquipment_AdditionalArmor_ReturnsTrue()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // 240 capacity
            var heavyArmor = new Armor("Plate Mail", "Heavy armor", 100, ArmorType.Heavy, 8, 18);
            var moreArmor = new Armor("Leather", "Light armor", 10, ArmorType.Light, 0, 11);

            // Act
            bool result = character.AddEquipment(heavyArmor);
            result = character.AddEquipment(moreArmor);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, character.GetEquipment().Count);
            Assert.AreEqual(110, character.CurrentWeight);
        }

        [TestMethod]
        public void CanCarryWeight_WithinCapacity_ReturnsTrue()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // 240 capacity

            // Act & Assert
            Assert.IsTrue(character.CanCarryWeight(50));
            Assert.IsTrue(character.CanCarryWeight(240));
        }

        [TestMethod]
        public void CanCarryWeight_ExceedsCapacity_ReturnsFalse()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // 240 capacity

            // Act & Assert
            Assert.IsFalse(character.CanCarryWeight(241));
        }

        [TestMethod]
        public void CurrentWeight_MultipleItems_CalculatesCorrectly()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // 300 capacity
            character.SetAbilityScore(AbilityScore.Strength, 20);
            var sword = new Weapon("Longsword", "A sharp blade", 3, 0, AbilityScore.Strength, 8, false, 0, 0, WeaponType.Martial);
            var armor = new Armor("Chain Mail", "Medium armor", 20, ArmorType.Medium, 3, 16);

            // Act
            character.AddEquipment(sword);
            character.AddEquipment(armor);

            // Assert
            Assert.AreEqual(23, character.CurrentWeight);
        }

        [TestMethod]
        public void RaceBonuses_AppliedCorrectly_WithClassBonuses()
        {
            // Arrange & Act
            var humanFighter = new Character("Human Fighter", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: true);
            var dwarfFighter = new Character("Dwarf Fighter", CharacterRace.Dwarf, CharacterClass.Fighter, standardHuman, applyClassBonuses: true);
            var elfThief = new Character("Elf Thief", CharacterRace.Elf, CharacterClass.Thief, standardHuman, applyClassBonuses: true);
            var gnomeWizard = new Character("Gnome Wizard", CharacterRace.Gnome, CharacterClass.Wizard, standardHuman, applyClassBonuses: true);
            var halflingBard = new Character("Halfling Bard", CharacterRace.Halfling, CharacterClass.Bard, standardHuman, applyClassBonuses: true);
            var dragonbornCleric = new Character("Dragonborn Cleric", CharacterRace.Dragonborn, CharacterClass.Cleric, standardHuman, applyClassBonuses: true);

            // Assert - Class bonuses (+2) and Race bonuses (+1)
            Assert.AreEqual(18, humanFighter.GetAbilityScore(AbilityScore.Strength)); // 16 + 2 (Fighter)
            Assert.AreEqual(16, humanFighter.GetAbilityScore(AbilityScore.Constitution)); // 15 + 1 (Human)

            Assert.AreEqual(19, dwarfFighter.GetAbilityScore(AbilityScore.Strength)); // 16 + 2 (Fighter) + 1 (Dwarf)
            Assert.AreEqual(15, dwarfFighter.GetAbilityScore(AbilityScore.Constitution)); // 15 (no bonus)

            Assert.AreEqual(16, elfThief.GetAbilityScore(AbilityScore.Strength)); // 16 (no bonus)
            Assert.AreEqual(17, elfThief.GetAbilityScore(AbilityScore.Dexterity)); // 14 + 2 (Thief) + 1 (Elf)

            Assert.AreEqual(13, gnomeWizard.GetAbilityScore(AbilityScore.Intelligence)); // 10 + 2 (Wizard) + 1 (Gnome)

            Assert.AreEqual(15, halflingBard.GetAbilityScore(AbilityScore.Charisma)); // 12 + 2 (Bard) + 1 (Halfling)

            Assert.AreEqual(16, dragonbornCleric.GetAbilityScore(AbilityScore.Wisdom)); // 13 + 2 (Cleric) + 1 (Dragonborn)
        }

        [TestMethod]
        public void RaceBonuses_NotApplied_WithoutClassBonuses()
        {
            // Arrange & Act
            var humanFighter = new Character("Human Fighter", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);

            // Assert - No bonuses applied
            Assert.AreEqual(16, humanFighter.GetAbilityScore(AbilityScore.Strength)); // Original value
            Assert.AreEqual(15, humanFighter.GetAbilityScore(AbilityScore.Constitution)); // Original value
        }


        [TestMethod]
        public void Add_And_Remove_Skill_Proficency()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);

            // Act
            character.AddSkillProficiency(Skill.Arcana);

            // Assert
            Assert.IsTrue(character.IsProficientInSkill(Skill.Arcana));
            Assert.IsFalse(character.IsProficientInSkill(Skill.Medicine));

            // Act
            character.RemoveSkillProficiency(Skill.Arcana);

            // Assert
            Assert.IsFalse(character.IsProficientInSkill(Skill.Arcana));
            Assert.IsFalse(character.IsProficientInSkill(Skill.Medicine));
        }

        [TestMethod]
        public void Attack_Damage_Calculations()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Wizard, standardHuman, applyClassBonuses: false);

            var sword = new Weapon("Longsword", "A sharp blade", 3, 0, AbilityScore.Strength, 1, false, 0, 0, WeaponType.Martial);
            var dagger = new Weapon("Dagger", "A sharp blade", 3, 0, AbilityScore.Dexterity, 1, false, 0, 0, WeaponType.Simple);

            character.AddEquipment(dagger);
            character.AddEquipment(sword);

            // Act
            var daggerDamage = character.Attack(dagger);
            var swordDamage = character.Attack(sword);

            // Assert
            Assert.AreEqual(daggerDamage, 5);
            Assert.AreEqual(swordDamage, 4);

        }

        [TestMethod]
        public void AddEquipment_NonProficientArmor_CanCarryButNotUse()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Wizard, standardHuman, applyClassBonuses: false); // Wizards not proficient with heavy armor
            var plateArmor = new Armor("Plate Armor", "Heavy armor", 65, ArmorType.Heavy, 0, 18);

            // Act
            bool result = character.AddEquipment(plateArmor);

            // Assert
            Assert.IsTrue(result); // Can carry it
            Assert.AreEqual(1, character.GetEquipment().Count); // It's in inventory
            Assert.AreEqual(12, character.ArmorClass); // But AC is still 10 + Dex (not using the armor)
        }

        [TestMethod]
        public void AddEquipment_NonProficientShield_CanCarryButNotUse()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Wizard, standardHuman, applyClassBonuses: false); // Wizards not proficient with shields
            var shield = new Armor("Shield", "Protective shield", 6, ArmorType.Shield, 2, 0);

            // Act
            bool result = character.AddEquipment(shield);

            // Assert
            Assert.IsTrue(result); // Can carry it
            Assert.AreEqual(1, character.GetEquipment().Count); // It's in inventory
            Assert.AreEqual(12, character.ArmorClass); // But AC is still 10 + Dex (not using the shield)
        }

        [TestMethod]
        public void ArmorClass_ProficientShield_AddsToAC()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // Fighters are proficient with shields
            var shield = new Armor("Shield", "Protective shield", 6, ArmorType.Shield, 2, 0);

            // Act
            character.AddEquipment(shield);
            character.EquipItem(shield);

            // Assert
            Assert.AreEqual(14, character.ArmorClass); // 10 + 2 (dex) + 2 (shield)
        }

        [TestMethod]
        public void ArmorClass_ProficientArmorAndShield_BothAddToAC()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);
            var chainMail = new Armor("Chain Mail", "Medium armor", 20, ArmorType.Medium, 0, 16);
            var shield = new Armor("Shield", "Protective shield", 6, ArmorType.Shield, 2, 0);

            // Act
            character.AddEquipment(chainMail);
            character.AddEquipment(shield);
            character.EquipItem(shield);
            character.EquipItem(chainMail);

            // Assert
            Assert.AreEqual(18, character.ArmorClass); // 16 (base) + 0 (chain modifier) + 2 (shield modifier)
        }

        [TestMethod]
        public void ArmorClass_NonProficientArmorWithProficientShield_OnlyShieldCounts()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Bard, standardHuman, applyClassBonuses: false); // Bards not proficient with medium armor
            var chainMail = new Armor("Chain Mail", "Medium armor", 20, ArmorType.Medium, 0, 16);
            var shield = new Armor("Shield", "Protective shield", 6, ArmorType.Shield, 2, 0);

            // Act
            character.AddEquipment(chainMail);
            character.AddEquipment(shield);

            // Assert - Bards aren't proficient with shields either, so neither should count
            Assert.AreEqual(12, character.ArmorClass); // 10 + 2 (dex) - no armor or shield bonus
        }

        [TestMethod]
        public void ShieldProficiency_FighterClericBarbarian_AreProficient()
        {
            // Arrange & Act
            var fighter = new Character("Fighter", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);
            var cleric = new Character("Cleric", CharacterRace.Human, CharacterClass.Cleric, standardHuman, applyClassBonuses: false);
            var barbarian = new Character("Barbarian", CharacterRace.Human, CharacterClass.Barbarian, standardHuman, applyClassBonuses: false);

            // Assert
            Assert.IsTrue(fighter.IsProficientWithArmor(ArmorType.Shield));
            Assert.IsTrue(cleric.IsProficientWithArmor(ArmorType.Shield));
            Assert.IsTrue(barbarian.IsProficientWithArmor(ArmorType.Shield));
        }

        [TestMethod]
        public void ShieldProficiency_WizardBardThief_AreNotProficient()
        {
            // Arrange & Act
            var wizard = new Character("Wizard", CharacterRace.Human, CharacterClass.Wizard, standardHuman, applyClassBonuses: false);
            var bard = new Character("Bard", CharacterRace.Human, CharacterClass.Bard, standardHuman, applyClassBonuses: false);
            var thief = new Character("Thief", CharacterRace.Human, CharacterClass.Thief, standardHuman, applyClassBonuses: false);

            // Assert
            Assert.IsFalse(wizard.IsProficientWithArmor(ArmorType.Shield));
            Assert.IsFalse(bard.IsProficientWithArmor(ArmorType.Shield));
            Assert.IsFalse(thief.IsProficientWithArmor(ArmorType.Shield));
        }

        [TestMethod]
        public void AddEquipment_NonProficientWeapon_CanCarryAndUseWithoutProficiencyBonus()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Wizard, standardHuman, applyClassBonuses: false); // Wizards not proficient with martial weapons
            var sword = new Weapon("Longsword", "A sharp blade", 3, 0, AbilityScore.Strength, 8, false, 0, 0, WeaponType.Martial);

            // Act
            bool result = character.AddEquipment(sword);
            int damage = character.Attack(sword);

            // Assert
            Assert.IsTrue(result); // Can carry it
            Assert.AreEqual(1, character.GetEquipment().Count); // It's in inventory
                                                                // Damage should be: 1d8 (let's assume roll of 4) + 0 (weapon modifier) + 3 (str modifier) + 0 (no proficiency bonus)
                                                                // Since we can't control the die roll, we'll test that it's at least the minimum expected
            Assert.IsTrue(damage >= 4); // Minimum: 1 (min die) + 0 + 3 + 0
            Assert.IsTrue(damage <= 11); // Maximum: 8 (max die) + 0 + 3 + 0
        }

        [TestMethod]
        public void Attack_ProficientWeapon_IncludesProficiencyBonus()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false); // Fighters proficient with martial weapons
            var sword = new Weapon("Longsword", "A sharp blade", 3, 0, AbilityScore.Strength, 8, false, 0, 0, WeaponType.Martial);

            // Act
            character.AddEquipment(sword);
            int damage = character.Attack(sword);

            // Assert
            // Damage should include proficiency bonus (2 at level 1)
            // Minimum: 1 (min die) + 0 (weapon modifier) + 3 (str modifier) + 2 (proficiency bonus) = 6
            // Maximum: 8 (max die) + 0 (weapon modifier) + 3 (str modifier) + 2 (proficiency bonus) = 13
            Assert.IsTrue(damage >= 6);
            Assert.IsTrue(damage <= 13);
        }

        [TestMethod]
        public void AddEquipment_MultipleBaseArmors_SecondArmorRejected()
        {
            // Arrange
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);
            var plateArmor = new Armor("Plate Armor", "Heavy armor", 65, ArmorType.Heavy, 0, 18);
            var chainMail = new Armor("Chain Mail", "Medium armor", 20, ArmorType.Medium, 0, 16);

            // Act
            bool firstResult = character.AddEquipment(plateArmor);
            Assert.IsTrue(firstResult);
            firstResult = character.EquipItem(plateArmor);
            bool secondResult = character.AddEquipment(chainMail);
            Assert.IsTrue(secondResult);
            secondResult = character.EquipItem(chainMail);

            // Assert
            Assert.IsTrue(firstResult);
            Assert.IsFalse(secondResult); // Can't wear two base armors
            Assert.AreEqual(2, character.GetEquipment().Count);
            Assert.AreEqual(18, character.ArmorClass); // Using plate armor
        }

        [TestMethod]
        public void EquipAndUnequip_Equipment()
        {
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);
            var plateArmor = new Armor("Plate Armor", "Heavy armor", 65, ArmorType.Heavy, 0, 18);

            // Act
            bool firstResult = character.AddEquipment(plateArmor);
            Assert.IsTrue(firstResult);
            firstResult = character.EquipItem(plateArmor);


            // Assert
            Assert.IsTrue(firstResult);
            Assert.AreEqual(1, character.GetEquipment().Count);
            Assert.AreEqual(18, character.ArmorClass); // Using plate armor

            //Act
            firstResult = character.UnequipItem(plateArmor);
            // Assert
            Assert.IsTrue(firstResult);
            Assert.AreEqual(1, character.GetEquipment().Count);
            Assert.AreEqual(12, character.ArmorClass); // Not using armor
        }

        [TestMethod]
        public void EquipAlreadyEquiped()
        {
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);
            var plateArmor = new Armor("Plate Armor", "Heavy armor", 65, ArmorType.Heavy, 0, 18);


            // Act
            bool firstResult = character.AddEquipment(plateArmor);
            Assert.IsTrue(firstResult);
            firstResult = character.EquipItem(plateArmor);

            // Assert
            Assert.IsTrue(firstResult);
            Assert.AreEqual(1, character.GetEquipment().Count);
            Assert.AreEqual(18, character.ArmorClass); // Using plate armor

            // Act 
            firstResult = character.EquipItem(plateArmor); // should be counted as failure

            // Assert
            Assert.IsFalse(firstResult);
            Assert.AreEqual(1, character.GetEquipment().Count);
            Assert.AreEqual(18, character.ArmorClass); // Using plate armor
        }

        [TestMethod]
        public void Unequip_nonActiveItem()
        {
            var character = new Character("Test", CharacterRace.Human, CharacterClass.Fighter, standardHuman, applyClassBonuses: false);
            var plateArmor = new Armor("Plate Armor", "Heavy armor", 65, ArmorType.Heavy, 0, 18);


            // Act
            bool firstResult = character.AddEquipment(plateArmor);
            Assert.IsTrue(firstResult);
            firstResult = character.UnequipItem(plateArmor);

            // Assert
            Assert.IsFalse(firstResult);
            Assert.AreEqual(1, character.GetEquipment().Count);
            Assert.AreEqual(12, character.ArmorClass); // Not using armor
        }


    }
}