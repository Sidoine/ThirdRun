using MonogameRPG;
using System.Collections.Generic;
using System.Linq;
using ThirdRun.Data;

namespace ThirdRun.Data.Abilities
{
    public abstract class Ability
    {
        public string Name { get; protected set; }
        public string IconPath { get; protected set; }
        public float Range { get; protected set; }
        public float CastTime { get; protected set; }
        public TargetType TargetType { get; protected set; }
        public float Cooldown { get; protected set; }
        public ResourceCost? ResourceCost { get; protected set; }
        
        // Track when the ability was last used for cooldown calculation
        public float LastUsedTime { get; private set; }
        
        protected Ability(string name, string iconPath, float range, float castTime, TargetType targetType, float cooldown, ResourceCost? resourceCost = null)
        {
            Name = name;
            IconPath = iconPath;
            Range = range;
            CastTime = castTime;
            TargetType = targetType;
            Cooldown = cooldown;
            ResourceCost = resourceCost;
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
        
        public virtual bool CanUse(Unit caster, Unit? target, float currentTime)
        {
            // Check cooldown
            if (IsOnCooldown(currentTime))
                return false;
            
            // Check resource requirements
            if (ResourceCost != null)
            {
                if (ResourceCost.IsCost)
                {
                    // For resource costs, check if we have enough
                    if (!caster.Resources.HasEnoughResource(ResourceCost.ResourceType, ResourceCost.AbsoluteAmount))
                        return false;
                }
                else if (ResourceCost.IsGeneration)
                {
                    // For resource generation, check if it would exceed maximum
                    if (caster.Resources.WouldExceedMaxResource(ResourceCost.ResourceType, ResourceCost.AbsoluteAmount))
                        return false;
                }
            }
                
            // Check if we have a valid target based on target type
            if (TargetType == TargetType.Self && target != caster)
                return false;
                
            if ((TargetType == TargetType.Enemy || TargetType == TargetType.Friendly) && target == null)
                return false;
                
            // Group targeting doesn't require a specific target
            if (TargetType == TargetType.Group)
                return true;
                
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
            
            // Handle resource cost/generation
            if (ResourceCost != null)
            {
                if (ResourceCost.IsCost)
                {
                    // Consume the resource
                    caster.Resources.TryConsumeResource(ResourceCost.ResourceType, ResourceCost.AbsoluteAmount);
                }
                else if (ResourceCost.IsGeneration)
                {
                    // Generate the resource
                    caster.Resources.TryGenerateResource(ResourceCost.ResourceType, ResourceCost.AbsoluteAmount);
                }
            }
            
            if (TargetType == TargetType.Group)
            {
                ExecuteGroup(caster);
            }
            else
            {
                Execute(caster, target);
            }
        }
        
        protected abstract void Execute(Unit caster, Unit? target);
        
        /// <summary>
        /// Executes the ability on all valid targets in range for group-targeted abilities.
        /// Override this for abilities that use Group targeting.
        /// </summary>
        protected virtual void ExecuteGroup(Unit caster)
        {
            // Default implementation - find all friendly units in range and execute on each
            if (caster.Map == null) return;
            
            var targets = GetGroupTargets(caster);
            foreach (var target in targets)
            {
                Execute(caster, target);
            }
        }
        
        /// <summary>
        /// Gets all valid targets for group targeting
        /// </summary>
        public List<Unit> GetGroupTargets(Unit caster)
        {
            var targets = new List<Unit>();
            if (caster.Map == null) return targets;
            
            // For group targeting, find all friendly units within range
            var potentialTargets = new List<Unit>();
            
            if (caster is Character)
            {
                potentialTargets.AddRange(caster.Map.Characters.Where(c => !c.IsDead));
            }
            else if (caster is MonogameRPG.Monsters.Monster)
            {
                potentialTargets.AddRange(caster.Map.Monsters.Where(m => !m.IsDead));
            }
            
            // Filter by range
            foreach (var target in potentialTargets)
            {
                float distance = Microsoft.Xna.Framework.Vector2.Distance(caster.Position, target.Position);
                if (distance <= Range)
                {
                    targets.Add(target);
                }
            }
            
            return targets;
        }
    }
}