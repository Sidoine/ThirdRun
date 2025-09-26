using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    /// <summary>
    /// Ability that applies a debuff to weaken enemies
    /// </summary>
    public class WeaknessDebuffAbility : AuraAbility
    {
        public WeaknessDebuffAbility() 
            : base("Curse of Weakness", "Abilities/weakness_debuff", 64f, 2f, TargetType.Enemy, 8f, CreateAura())
        {
        }

        private static Aura CreateAura()
        {
            var aura = new Aura("Curse of Weakness", "Reduces attack power and armor", 20f, 3, true);
            aura.AddModifier(Characteristic.MeleeAttackPower, -3);
            aura.AddModifier(Characteristic.Armor, -2);
            return aura;
        }
    }
}