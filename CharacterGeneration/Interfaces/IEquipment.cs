// CharacterGeneration\Objects\Equipment.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterGeneration.Interfaces
{
    public interface IEquipment
    {
        int Weight { get; set; }
        string Name { get; set; }
        string Description { get; set; }

        bool IsActive { get; set; }

        string SType { get; }
    }
}