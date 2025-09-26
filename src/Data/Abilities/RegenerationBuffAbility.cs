using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    /// <summary>
    /// Ability that applies a regeneration buff to friendly units
    /// </summary>
    public class RegenerationBuffAbility : AuraAbility
    {
        public RegenerationBuffAbility() 
            : base("Regeneration", "Abilities/regeneration", 80f, 1f, TargetType.Friendly, 12f, CreateAura())
        {
        }

        private static Aura CreateAura()
        {
            var aura = new Aura("Regeneration", "Gradually restores health", 25f, 1, false);
            aura.AddModifier(Characteristic.Health, 2); // Small health boost
            return aura;
        }
    }
}