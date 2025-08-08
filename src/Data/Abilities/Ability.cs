using MonogameRPG;

namespace ThirdRun.Data.Abilities
{
    public abstract class Ability
    {
        public string Name { get; protected set; }
        public float Range { get; protected set; }
        public float CastTime { get; protected set; }
        public TargetType TargetType { get; protected set; }
        public float Cooldown { get; protected set; }
        
        // Track when the ability was last used for cooldown calculation
        public float LastUsedTime { get; private set; }
        
        protected Ability(string name, float range, float castTime, TargetType targetType, float cooldown)
        {
            Name = name;
            Range = range;
            CastTime = castTime;
            TargetType = targetType;
            Cooldown = cooldown;
            LastUsedTime = -cooldown; // Allow immediate first use
        }
        
        public bool IsOnCooldown(float currentTime)
        {
            return currentTime < LastUsedTime + Cooldown;
        }
        
        public float GetCooldownRemaining(float currentTime)
        {
            var remaining = (LastUsedTime + Cooldown) - currentTime;
            return remaining > 0 ? remaining : 0;
        }
        
        public bool CanUse(Unit caster, Unit? target, float currentTime)
        {
            // Check cooldown
            if (IsOnCooldown(currentTime))
                return false;
                
            // Check if we have a valid target based on target type
            if (TargetType == TargetType.Self && target != caster)
                return false;
                
            if ((TargetType == TargetType.Enemy || TargetType == TargetType.Friendly) && target == null)
                return false;
                
            // Check range if we have a target
            if (target != null && target != caster)
            {
                var distance = Microsoft.Xna.Framework.Vector2.Distance(caster.Position, target.Position);
                if (distance > Range)
                    return false;
            }
            
            return true;
        }
        
        public void Use(Unit caster, Unit? target, float currentTime)
        {
            if (!CanUse(caster, target, currentTime))
                return;
                
            LastUsedTime = currentTime;
            Execute(caster, target);
        }
        
        protected abstract void Execute(Unit caster, Unit? target);
    }
}