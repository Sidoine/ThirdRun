using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    /// <summary>
    /// Ability that applies a buff to increase attack power of friendly units
    /// </summary>
    public class AttackPowerBuffAbility : AuraAbility
    {
        public AttackPowerBuffAbility() 
            : base("Blessing of Strength", "Abilities/attack_buff", 96f, 1.5f, TargetType.Group, 15f, CreateAura())
        {
        }

        private static Aura CreateAura()
        {
            var aura = new Aura("Blessing of Strength", "Increases attack power", 30f, 5, false);
            aura.AddModifier(Characteristic.MeleeAttackPower, 5);
            return aura;
        }
    }
}