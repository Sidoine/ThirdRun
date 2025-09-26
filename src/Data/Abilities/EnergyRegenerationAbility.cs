using MonogameRPG;
using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    /// <summary>
    /// An ability that generates energy (demonstrates negative resource cost)
    /// </summary>
    public class EnergyRegenerationAbility : Ability
    {
        public EnergyRegenerationAbility() 
            : base("Energy Regeneration", "Abilities/energy_regen", 0f, 1f, TargetType.Self, 8f,
                  new ResourceCost(ResourceType.Energy, -25f)) // No range (self), 1 second cast time, 8 second cooldown, generates 25 energy
        {
        }
        
        protected override void Execute(Unit caster, Unit? target)
        {
            // The resource generation is handled automatically by the base Ability.Use() method
            // This ability doesn't need additional effects beyond generating energy
        }
    }
}