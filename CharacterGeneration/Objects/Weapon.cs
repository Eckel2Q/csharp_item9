// CharacterGeneration\Objects\Weapon.cs
using CharacterGeneration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterGeneration.Objects
{
    public class Weapon : IEquipment
    {
        private int damage_modifier;
        private int weight;
        private string name;
        private string description;

        public AbilityScore AbilityModifier { get; set; }
        public WeaponType Type { get; set; }
        private int dieType;
        private bool isRanged;
        private int minRange;
        private int maxRange;
        private bool isActive;

        // Constructor
        public Weapon(string name, string description, int weight, int damageModifier,
              AbilityScore abilityModifier, int dieType, bool isRanged,
              int minRange, int maxRange, WeaponType type)
        {
            this.name = name;
            this.description = description;
            this.weight = weight;
            damage_modifier = damageModifier;
            this.dieType = dieType;
            this.isRanged = isRanged;
            this.minRange = minRange;
            this.maxRange = maxRange;
            AbilityModifier = abilityModifier;
            Type = type;
        }

        // IEquipment properties
        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public string SType
        {
            get; set;
        } = "Weapon";

        // Weapon-specific properties
        public int DamageModifier
        {
            get { return damage_modifier; }
            set { damage_modifier = value; }
        }

        public int DieType
        {
            get { return dieType; }
            set { dieType = value; }
        }

        public bool IsRanged
        {
            get { return isRanged; }
            set { isRanged = value; }
        }

        public int MinRange
        {
            get { return minRange; }
            set { minRange = value; }
        }

        public int MaxRange
        {
            get { return maxRange; }
            set { maxRange = value; }
        }

        
    }
}