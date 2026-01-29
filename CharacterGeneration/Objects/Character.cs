// CharacterGeneration\Objects\Character.cs
using CharacterGeneration.Interfaces;
using CharacterGeneration.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CharacterGeneration
{

    public enum AbilityScore
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    }

    public enum Skill
    {
        Athletics,
        Acrobatics,
        SleightOfHand,
        Stealth,
        Arcana,
        History,
        Investigation,
        Nature,
        Religion,
        AnimalHandling,
        Insight,
        Medicine,
        Perception,
        Survival,
        Deception,
        Intimidation,
        Performance,
        Persuasion
    }

    public enum WeaponType
    {
        Simple,
        Martial
    }

    public enum CharacterClass
    {
        Wizard,
        Cleric,
        Fighter,
        Thief,
        Bard,
        Barbarian
    }

    public enum ArmorType
    {
        Light,
        Medium,
        Heavy,
        Shield,  // Add Shield as its own type
        None
    }

    public enum CharacterRace
    {
        Human,
        Dwarf,
        Elf,
        Halfling,
        Dragonborn,
        Gnome
    }

    public class Character : IUnit
    {
        private string name;
        private CharacterRace race;
        private CharacterClass characterClass;
        private int level;
        private int experience;
        private List<IEquipment> equipment;

        public string Type => Race.ToString(); // Type maps to Race
        // Experience to Level mapping
        private static readonly int[] ExperienceThresholds = {
            0, 300, 900, 2700, 6500, 14000, 23000, 34000, 48000, 64000,
            85000, 100000, 120000, 140000, 165000, 195000, 225000, 265000, 305000, 355000
        };

        private Dictionary<AbilityScore, int> abilityScores;
        private HashSet<Skill> proficientSkills = new HashSet<Skill>();
        private HashSet<WeaponType> proficientWeapons = new HashSet<WeaponType>();
        private HashSet<ArmorType> proficientArmor = new HashSet<ArmorType>();
        // Updated constructor

        // Add parameterless constructor for deserialization
        public Character()
        {
            this.abilityScores = new Dictionary<AbilityScore, int>();
            this.equipment = new List<IEquipment>();
            this.proficientSkills = new HashSet<Skill>();
            this.proficientWeapons = new HashSet<WeaponType>();
            this.proficientArmor = new HashSet<ArmorType>();
        }
        public Character(string name, CharacterRace race, CharacterClass characterClass, Dictionary<AbilityScore, int> abilityScores,
                        int level = 1, int experience = 0, bool applyClassBonuses = true)
        {
            this.name = name;
            this.race = race;
            this.characterClass = characterClass;
            this.abilityScores = new Dictionary<AbilityScore, int>();
            this.equipment = new List<IEquipment>();
            // Ensure all ability scores are set
            foreach (AbilityScore ability in Enum.GetValues(typeof(AbilityScore)))
            {
                if (!abilityScores.ContainsKey(ability))
                    this.abilityScores[ability] = 10; // Default to 10
                else
                    this.abilityScores[ability] = abilityScores[ability];  //create a deep copy
            }

            // Apply class bonuses if requested
            if (applyClassBonuses)
            {
                ApplyClassBonuses();
                ApplyRaceBonuses();
            }
            proficientArmor.Add(ArmorType.None);
            // Apply class proficiencies
            ApplyClassProficiencies();

            // Set level and experience with proper synchronization
            if (experience > 0)
            {
                this.experience = experience;
                this.level = CalculateLevelFromExperience(experience);
            }
            else
            {
                this.level = level;
                this.experience = GetMinExperienceForLevel(level);
            }
        }

        // Properties for basic stats
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public CharacterRace Race
        {
            get { return race; }
            set { race = value; }
        }

        public CharacterClass Class
        {
            get { return characterClass; }
            set { characterClass = value; }
        }

        [JsonPropertyName("abilityScores")]
        public Dictionary<AbilityScore, int> AbilityScores
        {
            get { return abilityScores; }
            set { abilityScores = value; }
        }

        [JsonPropertyName("proficientSkills")]
        public HashSet<Skill> ProficientSkills
        {
            get { return proficientSkills; }
            set { proficientSkills = value; }
        }

        [JsonPropertyName("proficientWeapons")]
        public HashSet<WeaponType> ProficientWeapons
        {
            get { return proficientWeapons; }
            set { proficientWeapons = value; }
        }

        [JsonPropertyName("proficientArmor")]
        public HashSet<ArmorType> ProficientArmor
        {
            get { return proficientArmor; }
            set { proficientArmor = value; }
        }

        [JsonPropertyName("equipment")]
        public List<IEquipment> Equipment
        {
            get { return equipment; }
            set { equipment = value; }
        }

        public int HitPoints
        {
            get
            {
                int classHitDie = GetClassHitDie();
                int constitutionModifier = GetAbilityModifier(AbilityScore.Constitution);
                return (classHitDie + constitutionModifier) * Level;
            }
        }
        private void ApplyClassBonuses()
        {
            switch (characterClass)
            {
                case CharacterClass.Wizard:
                    abilityScores[AbilityScore.Intelligence] += 2;
                    break;
                case CharacterClass.Cleric:
                    abilityScores[AbilityScore.Wisdom] += 2;
                    break;
                case CharacterClass.Fighter:
                    abilityScores[AbilityScore.Strength] += 2;
                    break;
                case CharacterClass.Thief:
                    abilityScores[AbilityScore.Dexterity] += 2;
                    break;
                case CharacterClass.Bard:
                    abilityScores[AbilityScore.Charisma] += 2;
                    break;
                case CharacterClass.Barbarian:
                    abilityScores[AbilityScore.Constitution] += 2;
                    break;
            }
        }

        private void ApplyRaceBonuses()
        {
            switch (race)
            {
                case CharacterRace.Human:
                    abilityScores[AbilityScore.Constitution] += 1;
                    break;
                case CharacterRace.Dwarf:
                    abilityScores[AbilityScore.Strength] += 1;
                    break;
                case CharacterRace.Elf:
                    abilityScores[AbilityScore.Dexterity] += 1;
                    break;
                case CharacterRace.Gnome:
                    abilityScores[AbilityScore.Intelligence] += 1;
                    break;
                case CharacterRace.Halfling:
                    abilityScores[AbilityScore.Charisma] += 1;
                    break;
                case CharacterRace.Dragonborn:
                    abilityScores[AbilityScore.Wisdom] += 1;
                    break;
            }
        }


        private void ApplyClassProficiencies()
        {
            switch (characterClass)
            {
                case CharacterClass.Wizard:
                    proficientWeapons.Add(WeaponType.Simple);
                    proficientSkills.Add(Skill.Arcana);
                    proficientSkills.Add(Skill.Investigation);
                    break;
                case CharacterClass.Cleric:
                    proficientWeapons.Add(WeaponType.Simple);
                    proficientArmor.Add(ArmorType.Light);
                    proficientArmor.Add(ArmorType.Medium);
                    proficientArmor.Add(ArmorType.Shield);  // Add shield proficiency
                    proficientSkills.Add(Skill.Religion);
                    proficientSkills.Add(Skill.Insight);
                    break;
                case CharacterClass.Fighter:
                    proficientWeapons.Add(WeaponType.Simple);
                    proficientWeapons.Add(WeaponType.Martial);
                    proficientArmor.Add(ArmorType.Light);
                    proficientArmor.Add(ArmorType.Medium);
                    proficientArmor.Add(ArmorType.Heavy);
                    proficientArmor.Add(ArmorType.Shield);  // Add shield proficiency
                    proficientSkills.Add(Skill.Athletics);
                    proficientSkills.Add(Skill.Perception);
                    break;
                case CharacterClass.Thief:
                    proficientWeapons.Add(WeaponType.Simple);
                    proficientWeapons.Add(WeaponType.Martial);
                    proficientArmor.Add(ArmorType.Light);
                    proficientSkills.Add(Skill.Stealth);
                    proficientSkills.Add(Skill.Deception);
                    proficientSkills.Add(Skill.SleightOfHand);
                    break;
                case CharacterClass.Bard:
                    proficientWeapons.Add(WeaponType.Simple);
                    proficientArmor.Add(ArmorType.Light);
                    proficientSkills.Add(Skill.Performance);
                    proficientSkills.Add(Skill.Persuasion);
                    break;
                case CharacterClass.Barbarian:
                    proficientWeapons.Add(WeaponType.Simple);
                    proficientWeapons.Add(WeaponType.Martial);
                    proficientArmor.Add(ArmorType.Light);
                    proficientArmor.Add(ArmorType.Medium);
                    proficientArmor.Add(ArmorType.Shield);  // Add shield proficiency
                    proficientSkills.Add(Skill.Intimidation);
                    proficientSkills.Add(Skill.Acrobatics);
                    proficientSkills.Add(Skill.Athletics);
                    break;
            }
        }

        private int GetClassHitDie()
        {
            switch (characterClass)
            {
                case CharacterClass.Wizard:
                case CharacterClass.Bard:
                    return 4;
                case CharacterClass.Cleric:
                case CharacterClass.Thief:
                    return 5;
                case CharacterClass.Fighter:
                    return 6;
                case CharacterClass.Barbarian:
                    return 7;
                default:
                    return 4;
            }
        }

        // Methods to manage armor proficiencies
        public void AddArmorProficiency(ArmorType armorType)
        {
            proficientArmor.Add(armorType);
        }

        public void RemoveArmorProficiency(ArmorType armorType)
        {
            proficientArmor.Remove(armorType);
        }

        public bool IsProficientWithArmor(ArmorType armorType)
        {
            return proficientArmor.Contains(armorType);
        }

        // Updated ability score access
        public int GetAbilityScore(AbilityScore ability)
        {
            return abilityScores.TryGetValue(ability, out int score) ? score : 10;
        }

        public void SetAbilityScore(AbilityScore ability, int value)
        {
            abilityScores[ability] = value;
        }

        public int GetAbilityModifier(AbilityScore ability)
        {
            int score = GetAbilityScore(ability);
            return (int)Math.Floor((score - 10) / 2.0);
        }

        public int Level
        {
            get { return level; }
            set
            {
                int oldLevel = level;
                level = Math.Max(1, Math.Min(20, value)); // Clamp between 1-20
                if (oldLevel < level)
                {
                    experience = GetMinExperienceForLevel(level); //if we are coming from below, use the lowest exp value
                }
                else if (oldLevel > level)
                {
                    experience = GetMaxExperianceForLevel(level); //if we are coming from above, use the highest exp value
                }
                else
                {
                    //do nothing.  Don't change experiance if level isn't changing.
                }

            }
        }

        public int Experience
        {
            get { return experience; }
            set
            {
                experience = Math.Max(0, value);
                level = CalculateLevelFromExperience(experience);
            }
        }

        // Equipment management
        public int CarryingCapacity
        {
            get { return GetAbilityScore(AbilityScore.Strength) * 15; }
        }

        public int CurrentWeight
        {
            get { return equipment.Sum(e => e.Weight); }
        }

        // Armor Class calculation - only count armor/shields if proficient
        public int ArmorClass
        {
            get
            {
                var armors = equipment.OfType<Armor>()
                    .Where(a => a.IsActive)  // Only count if active
                    .ToList();

                if (!armors.Any())
                {
                    // No usable armor: 10 + Dex modifier
                    return 10 + GetAbilityModifier(AbilityScore.Dexterity);
                }

                // Find armor with base AC (should only be one)
                var baseArmor = armors.FirstOrDefault(a => a.ArmorBase > 0);
                int baseAC;

                if (baseArmor != null)
                {
                    // Use armor's base AC
                    baseAC = baseArmor.ArmorBase;
                }
                else
                {
                    // No base armor found, use character's base AC
                    baseAC = 10 + GetAbilityModifier(AbilityScore.Dexterity);
                }

                // Add all armor modifiers from usable armor
                int totalModifier = armors.Sum(a => a.ArmorModifier);

                return baseAC + totalModifier;
            }
        }

        public bool CanCarryWeight(int additionalWeight)
        {
            return CurrentWeight + additionalWeight <= CarryingCapacity;
        }

        private bool AlreadyWearingArmor()
        {
            return equipment.OfType<Armor>()
                .Where(a => a.IsActive)  // Only count if active
                .FirstOrDefault(a => a.ArmorBase > 0) != null;
        }

        public bool AddEquipment(IEquipment item)
        {
            if (CanCarryWeight(item.Weight))
            {
                equipment.Add(item);
                return true;
            }
            return false;
        }

        public bool EquipItem(IEquipment item)
        {
            if (this.equipment.Contains(item))
            {
                if (item.GetType() == typeof(Armor))
                {
                    Armor armor = (Armor)item;
                    if(armor.ArmorBase > 0 && AlreadyWearingArmor())
                    {
                        return false;
                    }
                    armor.IsActive = true; 
                }
                else if (item.GetType() == typeof(Weapon))
                {
                    Weapon weapon = (Weapon)item;
                    weapon.IsActive = true;
                }
            }
            
            return item.IsActive;
        }

        public bool UnequipItem(IEquipment item)
        {
            if (item.IsActive)
            {
                item.IsActive = false;
                return true;
            }
            return false;
        }


        public bool RemoveEquipment(IEquipment item)
        {
            return equipment.Remove(item);
        }

        public List<IEquipment> GetEquipment()
        {
            return new List<IEquipment>(equipment); // Return a copy to prevent external modification
        }

        // Helper methods for level/experience conversion
        private int CalculateLevelFromExperience(int exp)
        {
            for (int i = ExperienceThresholds.Length - 1; i >= 0; i--)
            {
                if (exp >= ExperienceThresholds[i])
                {
                    return i + 1;
                }
            }
            return 1;
        }

        private int GetMinExperienceForLevel(int lvl)
        {
            if (lvl < 1) return 0;
            if (lvl > 20) return ExperienceThresholds[19];
            return ExperienceThresholds[lvl - 1];
        }

        private int GetMaxExperianceForLevel(int lvl)
        {
            if (lvl < 1) return 0;
            if (lvl > 19) return ExperienceThresholds[19]; //level 20 experiance doesn't matter any more
            return ExperienceThresholds[lvl] - 1; // subtract 1 from the next levels highest value.
        }

        public int ProficiencyBonus
        {
            get { return 1 + (int)Math.Ceiling(Level / 4.0); }
        }

        // Methods to manage proficiencies
        public void AddSkillProficiency(Skill skill)
        {
            proficientSkills.Add(skill);
        }

        public void RemoveSkillProficiency(Skill skill)
        {
            proficientSkills.Remove(skill);
        }

        public bool IsProficientInSkill(Skill skill)
        {
            return proficientSkills.Contains(skill);
        }

        public void AddWeaponProficiency(WeaponType weaponType)
        {
            proficientWeapons.Add(weaponType);
        }

        public void RemoveWeaponProficiency(WeaponType weaponType)
        {
            proficientWeapons.Remove(weaponType);
        }

        public bool IsProficientWithWeapon(WeaponType weaponType)
        {
            return proficientWeapons.Contains(weaponType);
        }

        public int Attack(Weapon weapon)
        {
            // Base damage from weapon's die
            int damage = RollDie(weapon.DieType);

            // Add weapon's damage modifier
            damage += weapon.DamageModifier;

            // Add appropriate ability modifier
            damage += GetAbilityModifier(weapon.AbilityModifier);

            // Add proficiency bonus if proficient with this weapon type
            if (IsProficientWithWeapon(weapon.Type))
            {
                damage += ProficiencyBonus;
            }

            // Ensure minimum damage of 1
            return Math.Max(1, damage);
        }

        // Helper method for rolling dice
        private int RollDie(int sides)
        {
            return new Random().Next(1, sides + 1);
        }

        public int SkillCheck(Skill skill, AbilityScore ability = default)
        {
            // If no ability specified, use the default for the skill
            if (ability == default)
            {
                ability = GetDefaultAbilityForSkill(skill);
            }

            int result = RollDie(20); // d20 roll
            result += GetAbilityModifier(ability);

            if (IsProficientInSkill(skill))
            {
                result += ProficiencyBonus;
            }

            return result;
        }

        // Add a method to get default ability for each skill
        private AbilityScore GetDefaultAbilityForSkill(Skill skill)
        {
            switch (skill)
            {
                case Skill.Athletics: return AbilityScore.Strength;
                case Skill.Acrobatics:
                case Skill.SleightOfHand:
                case Skill.Stealth: return AbilityScore.Dexterity;
                case Skill.Arcana:
                case Skill.History:
                case Skill.Investigation:
                case Skill.Nature:
                case Skill.Religion: return AbilityScore.Intelligence;
                case Skill.AnimalHandling:
                case Skill.Insight:
                case Skill.Medicine:
                case Skill.Perception:
                case Skill.Survival: return AbilityScore.Wisdom;
                case Skill.Deception:
                case Skill.Intimidation:
                case Skill.Performance:
                case Skill.Persuasion: return AbilityScore.Charisma;
                default: return AbilityScore.Intelligence;
            }
        }

        public void PrintUnitInformation()
        {
            PrintCharacterDetails(this);
        }

        // Helper method to display character information
        public static void PrintCharacterDetails(Character character)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine($"Character: {character.Name}");
            Console.WriteLine($"Race: {character.Race}");
            Console.WriteLine($"Class: {character.Class}");
            Console.WriteLine($"Level: {character.Level}");
            Console.WriteLine($"Hit Points: {character.HitPoints}");
            Console.WriteLine($"Armor Class: {character.ArmorClass}");
            Console.WriteLine($"Proficiency Bonus: +{character.ProficiencyBonus}");

            Console.WriteLine("\nAbility Scores:");
            foreach (AbilityScore ability in Enum.GetValues<AbilityScore>())
            {
                int score = character.GetAbilityScore(ability);
                int modifier = character.GetAbilityModifier(ability);
                string modifierStr = modifier >= 0 ? $"+{modifier}" : modifier.ToString();
                Console.WriteLine($"  {ability}: {score} ({modifierStr})");
            }

            Console.WriteLine("\nProficient Skills:");
            foreach (var skill in character.ProficientSkills)
            {
                Console.WriteLine($"  - {skill}");
            }

            Console.WriteLine("\nWeapon Proficiencies:");
            foreach (var weapon in character.ProficientWeapons)
            {
                Console.WriteLine($"  - {weapon} weapons");
            }

            Console.WriteLine("\nArmor Proficiencies:");
            foreach (var armor in character.ProficientArmor)
            {
                Console.WriteLine($"  - {armor} armor");
            }
            Console.WriteLine(new string('=', 40));
        }

        // Helper method to get random race
        public static CharacterRace GetRandomRace()
        {
            var races = Enum.GetValues<CharacterRace>();
            var random = new Random();
            return races[random.Next(races.Length)];
        }

        // Helper method to get random class
        public static CharacterClass GetRandomClass()
        {
            var classes = Enum.GetValues<CharacterClass>();
            var random = new Random();
            return classes[random.Next(classes.Length)];
        }

    }
}