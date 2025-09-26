using MonogameRPG;
using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    public class MeleeAttackAbility : Ability
    {
        public MeleeAttackAbility() 
            : base("Melee Attack", "Abilities/melee_attack", 32f, 0f, TargetType.Enemy, 1f, 
                  new ResourceCost(ResourceType.Energy, 10f)) // Range of ~1 tile, instant cast, 1 second cooldown, 10 energy cost
        {
        }
        
        protected override void Execute(Unit caster, Unit? target)
        {
            if (target == null)
                return;
                
            var damage = caster.Characteristics.GetValue(Characteristic.MeleeAttackPower);
            target.CurrentHealth -= damage;
            
            if (target.CurrentHealth < 0)
                target.CurrentHealth = 0;
        }
    }
}