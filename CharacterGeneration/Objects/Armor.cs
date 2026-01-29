// CharacterGeneration\Objects\Armor.cs
using CharacterGeneration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterGeneration.Objects
{
    public class Armor : IEquipment
    {
        private int armorModifier;
        private int armorBase;
        private ArmorType armorType;
        private int weight;
        private string name;
        private string description;
        private bool isActive;

        // Constructor
        public Armor(string name, string description, int weight, ArmorType armorType, int armorModifier, int armorBase)
        {
            this.name = name;
            this.description = description;
            this.weight = weight;
            this.armorType = armorType;
            this.armorModifier = armorModifier;
            this.armorBase = armorBase;
            this.isActive = false; //default to not active
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

        public bool IsActive {
            get { return isActive; }
            set { isActive = value; }
        }

        public string SType
        {
            get; set;
        } = "Armor";

        // Armor-specific properties
        public int ArmorModifier
        {
            get { return armorModifier; }
            set { armorModifier = value; }
        }

        public int ArmorBase
        {
            get { return armorBase; }
            set { armorBase = value; }
        }

        public ArmorType ArmorType
        {
            get { return armorType; }
            set { armorType = value; }
        }
    }
}