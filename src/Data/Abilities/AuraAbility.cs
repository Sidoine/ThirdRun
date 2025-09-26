using MonogameRPG;
using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    /// <summary>
    /// Base class for abilities that apply auras to targets
    /// </summary>
    public abstract class AuraAbility : Ability
    {
        protected Aura Aura { get; }
        protected int StacksToApply { get; }

        protected AuraAbility(string name, string iconPath, float range, float castTime, 
                              TargetType targetType, float cooldown, Aura aura, int stacksToApply = 1)
            : base(name, iconPath, range, castTime, targetType, cooldown)
        {
            Aura = aura;
            StacksToApply = stacksToApply;
        }

        public override bool CanUse(Unit caster, Unit? target, float currentTime)
        {
            // First check the base conditions (cooldown, range, etc.)
            if (!base.CanUse(caster, target, currentTime))
                return false;
                
            // Only allow applying aura if the target doesn't already have it
            if (target != null && target.HasAura(Aura.Name))
                return false;
                
            return true;
        }

        protected override void Execute(Unit caster, Unit? target)
        {
            if (target != null)
            {
                target.ApplyAura(Aura, StacksToApply);
            }
        }
    }
}