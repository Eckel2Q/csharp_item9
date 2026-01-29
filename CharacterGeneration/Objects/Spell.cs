using System;
using System.Collections.Generic;
using CharacterGeneration;

namespace CharacterGeneration.Objects
{
    public class Spell
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public string Effect { get; set; }
        public List<CharacterClass> AllowedClasses { get; set; } = new List<CharacterClass>();

        public Spell(string name, int level, string effect, List<CharacterClass> allowedClasses = null)
        {
            Name = name;
            Level = level;
            Effect = effect;
            if (allowedClasses != null)
            {
                AllowedClasses = allowedClasses;
            }
        }

        public Spell() { }
    }
}
