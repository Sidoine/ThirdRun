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

        protected override void Execute(Unit caster, Unit? target)
        {
            if (target != null)
            {
                target.ApplyAura(Aura, StacksToApply);
            }
        }
    }
}