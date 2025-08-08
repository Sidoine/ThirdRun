using MonogameRPG;
using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    public class SelfHealAbility : Ability
    {
        public SelfHealAbility() 
            : base("Self Heal", 0f, 1f, TargetType.Self, 5f) // No range (self), 1 second cast time, 5 second cooldown
        {
        }
        
        protected override void Execute(Unit caster, Unit? target)
        {
            var healAmount = caster.Characteristics.GetValue(Characteristic.HealingPower);
            caster.CurrentHealth += healAmount;
            
            // Don't heal above max health
            if (caster.CurrentHealth > caster.MaxHealth)
                caster.CurrentHealth = caster.MaxHealth;
        }
    }
}