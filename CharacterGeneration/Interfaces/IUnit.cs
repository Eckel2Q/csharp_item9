// CharacterGeneration\Interfaces\IUnit.cs
using CharacterGeneration.Objects;

namespace CharacterGeneration.Interfaces
{
    public interface IUnit
    {
        string Name { get; }
        string Type { get; } // Equivalent to Race
        int HitPoints { get; }
        int GetAbilityModifier(AbilityScore ability);
        int ArmorClass { get; }
        int Attack(Weapon weapon);
        int SkillCheck(Skill skill, AbilityScore ability = default);

        void PrintUnitInformation();
    }
}
