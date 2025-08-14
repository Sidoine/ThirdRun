using System.Linq;
using MonogameRPG;
using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    /// <summary>
    /// Ability that removes all debuffs from a friendly target
    /// </summary>
    public class DispelDebuffAbility : Ability
    {
        public DispelDebuffAbility() 
            : base("Dispel Magic", "Abilities/dispel", 64f, 1.5f, TargetType.Friendly, 5f,
                  new ResourceCost(ResourceType.Energy, 15f)) // Range of ~2 tiles, 1.5 second cast time, 5 second cooldown, 15 energy cost
        {
        }
        
        public override bool CanUse(Unit caster, Unit? target, float currentTime)
        {
            // First check the base conditions (cooldown, range, etc.)
            if (!base.CanUse(caster, target, currentTime))
                return false;
                
            // Only allow dispelling if the target has debuffs to remove
            if (target != null && !target.HasAnyDebuff())
                return false;
                
            return true;
        }
        
        protected override void Execute(Unit caster, Unit? target)
        {
            if (target == null)
                return;
                
            // Remove all debuff auras from the target
            var debuffsToRemove = target.ActiveAuras.Where(ae => ae.Aura.IsDebuff).ToList();
            foreach (var debuff in debuffsToRemove)
            {
                target.RemoveAura(debuff.Aura.Name);
            }
        }
    }
}