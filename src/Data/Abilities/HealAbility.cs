using MonogameRPG;
using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    public class HealAbility : Ability
    {
        public HealAbility() 
            : base("Heal", "Abilities/heal", 64f, 2f, TargetType.Friendly, 3f) // Range of ~2 tiles, 2 second cast time, 3 second cooldown
        {
        }
        
        protected override void Execute(Unit caster, Unit? target)
        {
            if (target == null)
                return;
                
            var healAmount = caster.Characteristics.GetValue(Characteristic.HealingPower);
            target.CurrentHealth += healAmount;
            
            // Don't heal above max health
            if (target.CurrentHealth > target.MaxHealth)
                target.CurrentHealth = target.MaxHealth;
        }
    }
}