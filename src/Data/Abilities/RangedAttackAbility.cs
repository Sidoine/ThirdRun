using MonogameRPG;
using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    public class RangedAttackAbility : Ability
    {
        public RangedAttackAbility() 
            : base("Ranged Attack", "Abilities/ranged_attack", 128f, 0f, TargetType.Enemy, 1.5f) // Range of ~4 tiles, instant cast, 1.5 second cooldown
        {
        }
        
        protected override void Execute(Unit caster, Unit? target)
        {
            if (target == null)
                return;
                
            var damage = caster.Characteristics.GetValue(Characteristic.RangedAttackPower);
            target.CurrentHealth -= damage;
            
            if (target.CurrentHealth < 0)
                target.CurrentHealth = 0;
        }
    }
}