// CharacterGeneration\Objects\Monster.cs
using CharacterGeneration.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CharacterGeneration.Objects
{
    public class Monster : IUnit
    {
        private string name;
        private string type;
        private int hitPoints;
        private int armorClass;
        private Dictionary<AbilityScore, int> abilityModifiers;
        private List<Spell> spells = new List<Spell>();

        // Parameterless constructor for deserialization
        public Monster()
        {
            this.abilityModifiers = new Dictionary<AbilityScore, int>();
        }

        public Monster(string name, string type, int hitPoints, int armorClass, Dictionary<AbilityScore, int> abilityModifiers)
        {
            this.name = name;
            this.type = type;
            this.hitPoints = Math.Max(1, hitPoints); // Minimum 1 HP
            this.armorClass = Math.Max(1, armorClass); // Minimum 1 AC
            this.abilityModifiers = new Dictionary<AbilityScore, int>();

            // Ensure all ability modifiers are set
            foreach (AbilityScore ability in Enum.GetValues(typeof(AbilityScore)))
            {
                if (!abilityModifiers.ContainsKey(ability))
                    this.abilityModifiers[ability] = 0; // Default to 0 modifier
                else
                    this.abilityModifiers[ability] = abilityModifiers[ability];
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public int HitPoints
        {
            get { return hitPoints; }
            set { hitPoints = Math.Max(1, value); }
        }

        public int ArmorClass
        {
            get { return armorClass; }
            set { armorClass = Math.Max(1, value); }
        }

        [JsonPropertyName("abilityModifiers")]
        public Dictionary<AbilityScore, int> AbilityModifiers
        {
            get { return abilityModifiers; }
            set { abilityModifiers = value; }
        }

        public int GetAbilityModifier(AbilityScore ability)
        {
            return abilityModifiers.TryGetValue(ability, out int modifier) ? modifier : 0;
        }

        public void SetAbilityModifier(AbilityScore ability, int modifier)
        {
            abilityModifiers[ability] = modifier;
        }

        public int Attack(Weapon weapon)
        {
            // Base damage from weapon's die
            int damage = RollDie(weapon.DieType);

            // Add weapon's damage modifier
            damage += weapon.DamageModifier;

            // Add appropriate ability modifier
            damage += GetAbilityModifier(weapon.AbilityModifier);

            // Monsters don't have proficiency bonuses - they use their raw stats

            // Ensure minimum damage of 1
            return Math.Max(1, damage);
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

            // Monsters don't have skill proficiencies - they use their raw modifiers

            return result;
        }

        // Helper method for rolling dice
        private int RollDie(int sides)
        {
            return new Random().Next(1, sides + 1);
        }

        // Helper method to get default ability for each skill
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

        public List<Spell> Spells => new List<Spell>(spells);

        public bool AddSpell(Spell spell, bool overrideRules = false)
        {
            // Monsters can just have any spells added.
            spells.Add(spell);
            return true;
        }

        public void PrintUnitInformation()
        {
            PrintMonsterDetails(this);
        }

        // Helper method to display monster information
        public static void PrintMonsterDetails(Monster monster)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine($"Monster: {monster.Name}");
            Console.WriteLine($"Type: {monster.Type}");
            Console.WriteLine($"Hit Points: {monster.HitPoints}");
            Console.WriteLine($"Armor Class: {monster.ArmorClass}");

            Console.WriteLine("\nAbility Modifiers:");
            foreach (AbilityScore ability in Enum.GetValues<AbilityScore>())
            {
                int modifier = monster.GetAbilityModifier(ability);
                string modifierStr = modifier >= 0 ? $"+{modifier}" : modifier.ToString();
                Console.WriteLine($"  {ability}: {modifierStr}");
            }

            if (monster.spells.Count > 0)
            {
                Console.WriteLine("\nSpells:");
                foreach (var spell in monster.spells)
                {
                    Console.WriteLine($"  - Level {spell.Level} {spell.Name}: {spell.Effect}");
                }
            }

            Console.WriteLine(new string('=', 40));
        }
    }
}