using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using System.Linq;
using MonogameRPG.Monsters;

namespace MonogameRPG
{
    public abstract class Unit
    {
        public Vector2 Position { get; set; }
        public int CurrentHealth { get; set; }
        public CharacteristicValues Characteristics { get; private set; }
        public bool IsDead => CurrentHealth <= 0;
        public int Level { get; protected set; } = 1;
        
        // Map properties for movement and pathfinding
        public MonogameRPG.Map.Map? Map { get; set; }
        protected MonogameRPG.Map.WorldMap WorldMap { get; set; }
        
        // Ability system
        public List<Ability> Abilities { get; private set; }
        public Ability DefaultAbility { get; protected set; }
        
        // Aura system
        public List<AuraEffect> ActiveAuras { get; private set; }
        
        // Resource system
        public ResourceManager Resources { get; private set; }
        
        // Global cooldown system
        private float LastAbilityUsedTime { get; set; } = -2f; // Allow immediate first use
        private const float GlobalCooldownDuration = 1.5f;

        // Properties that delegate to characteristics
        public int MaxHealth 
        { 
            get => Characteristics.GetValue(Characteristic.Health);
            set => Characteristics.SetValue(Characteristic.Health, value);
        }
        
        public int AttackPower 
        { 
            get => Characteristics.GetValue(Characteristic.MeleeAttackPower);
            set => Characteristics.SetValue(Characteristic.MeleeAttackPower, value);
        }

        protected Unit(MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap)
        {
            Characteristics = new CharacteristicValues();
            Abilities = new List<Ability>();
            DefaultAbility = new MeleeAttackAbility();
            Abilities.Add(DefaultAbility);
            ActiveAuras = new List<AuraEffect>();
            Resources = new ResourceManager();
            Map = map;
            WorldMap = worldMap;
        }
        
        // Game time tracking - this should be set by the game loop
        protected float CurrentGameTime { get; set; }
        
        public void UpdateGameTime(float gameTime)
        {
            var deltaTime = gameTime - CurrentGameTime;
            CurrentGameTime = gameTime;
            
            // Update auras (remove expired ones)
            if (deltaTime > 0)
            {
                UpdateAuras(deltaTime);
                
                // Update resources (replenish over time)
                Resources.UpdateResources(deltaTime);
            }
        }
        
        public bool CanUseAbility(Ability ability, Unit? target)
        {
            return ability.CanUse(this, target, CurrentGameTime);
        }
        
        public void UseAbility(Ability ability, Unit? target)
        {
            ability.Use(this, target, CurrentGameTime);
            
            // Check if target was defeated and handle post-combat effects
            if (target != null && target.IsDead)
            {
                OnTargetDefeated(target);
            }
        }
        
        /// <summary>
        /// Checks if unit is on global cooldown
        /// </summary>
        public bool IsOnGlobalCooldown()
        {
            return CurrentGameTime < LastAbilityUsedTime + GlobalCooldownDuration;
        }
        
        /// <summary>
        /// Uses abilities in order on the nearest relevant target.
        /// Checks range, cooldown, and global cooldown before using an ability.
        /// Using an ability triggers a 1.5s global cooldown.
        /// </summary>
        public void UseAbilities()
        {
            // Check global cooldown first
            if (IsOnGlobalCooldown())
                return;
                
            // Try each ability in order
            foreach (var ability in Abilities)
            {
                // Check if ability is on cooldown
                if (ability.IsOnCooldown(CurrentGameTime))
                    continue;
                    
                // Find nearest relevant target for this ability
                Unit? target = FindNearestTargetForAbility(ability);
                
                // Check if we can use this ability on the target
                if (target != null && CanUseAbility(ability, target))
                {
                    // Use the ability and set global cooldown
                    UseAbility(ability, target);
                    LastAbilityUsedTime = CurrentGameTime;
                    return; // Only use one ability per call
                }
                
                // For self-targeting abilities, try using on self
                if (ability.TargetType == ThirdRun.Data.Abilities.TargetType.Self && CanUseAbility(ability, this))
                {
                    UseAbility(ability, this);
                    LastAbilityUsedTime = CurrentGameTime;
                    return; // Only use one ability per call
                }
                
                // For group-targeting abilities, use without a specific target
                if (ability.TargetType == ThirdRun.Data.Abilities.TargetType.Group && CanUseAbility(ability, null))
                {
                    UseAbility(ability, null);
                    LastAbilityUsedTime = CurrentGameTime;
                    return; // Only use one ability per call
                }
            }
        }
        
        /// <summary>
        /// Finds the nearest target relevant for the specified ability
        /// </summary>
        private Unit? FindNearestTargetForAbility(Ability ability)
        {
            if (Map == null) return null;
            
            List<Unit> potentialTargets = new List<Unit>();
            
            // Collect potential targets based on ability target type
            switch (ability.TargetType)
            {
                case ThirdRun.Data.Abilities.TargetType.Enemy:
                    // If this is a Character, target Monsters
                    if (this is Character)
                    {
                        potentialTargets.AddRange(Map.Monsters.Where(m => !m.IsDead));
                    }
                    // If this is a Monster, target Characters  
                    else if (this is MonogameRPG.Monsters.Monster)
                    {
                        potentialTargets.AddRange(Map.Characters.Where(c => !c.IsDead));
                    }
                    // For testing purposes, if neither Character nor Monster, target all other units
                    else
                    {
                        // Combine all unit types for tests
                        potentialTargets.AddRange(Map.Characters.Cast<Unit>().Where(u => u != this && !u.IsDead));
                        potentialTargets.AddRange(Map.Monsters.Cast<Unit>().Where(u => u != this && !u.IsDead));
                        potentialTargets.AddRange(Map.NPCs.Cast<Unit>().Where(u => u != this && !u.IsDead));
                    }
                    break;
                    
                case ThirdRun.Data.Abilities.TargetType.Friendly:
                    // If this is a Character, target other Characters
                    if (this is Character)
                    {
                        potentialTargets.AddRange(Map.Characters.Where(c => !c.IsDead && c != this));
                    }
                    // If this is a Monster, target other Monsters
                    else if (this is MonogameRPG.Monsters.Monster)
                    {
                        potentialTargets.AddRange(Map.Monsters.Where(m => !m.IsDead && m != this));
                    }
                    // For testing purposes, consider same type as friendly
                    else
                    {
                        // Find units of the same type
                        var allUnits = Map.Characters.Cast<Unit>()
                            .Concat(Map.Monsters.Cast<Unit>())
                            .Concat(Map.NPCs.Cast<Unit>());
                        potentialTargets.AddRange(allUnits.Where(u => u != this && !u.IsDead && u.GetType() == this.GetType()));
                    }
                    break;
                    
                case ThirdRun.Data.Abilities.TargetType.Self:
                    return this; // Self-targeting handled separately
                    
                case ThirdRun.Data.Abilities.TargetType.Group:
                    return null; // Group targeting doesn't need a specific target
            }
            
            // Find the nearest target within range
            Unit? nearestTarget = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var target in potentialTargets)
            {
                float distance = Vector2.Distance(Position, target.Position);
                if (distance <= ability.Range && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = target;
                }
            }
            
            return nearestTarget;
        }
        
        // Convenience method for default attack behavior
        public void Attack(Unit target)
        {
            UseAbility(DefaultAbility, target);
        }
        
        /// <summary>
        /// Called when this unit defeats another unit. Override in derived classes for specific behavior.
        /// </summary>
        protected virtual void OnTargetDefeated(Unit target)
        {
            // Default implementation does nothing. Derived classes can override for specific behavior.
        }
        
        /// <summary>
        /// Applies an aura to this unit. If the aura already exists, adds stacks and refreshes duration.
        /// </summary>
        public void ApplyAura(Aura aura, int stacks = 1)
        {
            var existingAura = ActiveAuras.FirstOrDefault(ae => ae.Aura.Name == aura.Name);
            if (existingAura != null)
            {
                existingAura.AddStacks(stacks);
            }
            else
            {
                ActiveAuras.Add(new AuraEffect(aura, CurrentGameTime, stacks));
            }
        }
        
        /// <summary>
        /// Removes an aura from this unit by name
        /// </summary>
        public void RemoveAura(string auraName)
        {
            ActiveAuras.RemoveAll(ae => ae.Aura.Name == auraName);
        }
        
        /// <summary>
        /// Checks if this unit has an aura with the specified name
        /// </summary>
        public bool HasAura(string auraName)
        {
            return ActiveAuras.Any(ae => ae.Aura.Name == auraName);
        }
        
        /// <summary>
        /// Checks if this unit has any debuff auras
        /// </summary>
        public bool HasAnyDebuff()
        {
            return ActiveAuras.Any(ae => ae.Aura.IsDebuff);
        }
        
        /// <summary>
        /// Updates all active auras, removing expired ones
        /// Call this regularly to manage aura durations
        /// </summary>
        public void UpdateAuras(float deltaTime)
        {
            // Update all auras and remove expired ones
            for (int i = ActiveAuras.Count - 1; i >= 0; i--)
            {
                if (!ActiveAuras[i].Update(CurrentGameTime, deltaTime))
                {
                    ActiveAuras.RemoveAt(i);
                }
            }
        }
        
        /// <summary>
        /// Gets the total modifier for a characteristic including base characteristics and aura bonuses
        /// </summary>
        public int GetTotalCharacteristic(Characteristic characteristic)
        {
            int baseValue = Characteristics.GetValue(characteristic);
            int auraBonus = 0;
            
            foreach (var auraEffect in ActiveAuras)
            {
                auraBonus += auraEffect.GetCharacteristicModifier(characteristic);
            }
            
            return baseValue + auraBonus;
        }
        
        /// <summary>
        /// Gets the effective max health including aura bonuses
        /// </summary>
        public int GetEffectiveMaxHealth()
        {
            return GetTotalCharacteristic(Characteristic.Health);
        }
        
        /// <summary>
        /// Gets the effective attack power including aura bonuses
        /// </summary>
        public int GetEffectiveAttackPower()
        {
            return GetTotalCharacteristic(Characteristic.MeleeAttackPower);
        }
        
        /// <summary>
        /// Gets the attack range from the first available ability that targets enemies
        /// </summary>
        /// <returns>Attack range in pixels, or 0 if no suitable ability is available</returns>
        protected float GetAttackRange()
        {
            // Find the first ability that is not on cooldown and targets enemies
            var availableAbility = Abilities.FirstOrDefault(ability => 
                ability.TargetType == ThirdRun.Data.Abilities.TargetType.Enemy && 
                !ability.IsOnCooldown(CurrentGameTime));
                
            return availableAbility?.Range ?? 0f;
        }
        
        /// <summary>
        /// Moves the unit towards the specified position using pathfinding
        /// </summary>
        /// <param name="position">Target position to move to</param>
        protected void MoveTo(Vector2 position)
        {
            if (WorldMap == null || Map == null) return;
            
            // Utiliser A* pour trouver le chemin sur la carte actuelle
            var path = WorldMap.FindPathAStar(Position, position);
            if (path.Count > 1)
            {
                Vector2 next = path[1]; // [0] = position actuelle
                Vector2 direction = next - Position;
                if (direction.Length() > 1f)
                {
                    direction.Normalize();
                    
                    // Calculate the new position we want to move to
                    Vector2 newPosition = Position + direction * 2f; // Vitesse de déplacement (pixels par frame)
                    
                    // Collision detection: check if the new position would collide with another unit
                    if (!WouldCollideWithOtherUnit(newPosition))
                    {
                        var oldPosition = Position;
                        Position = newPosition;
                        
                        // Update unit position tracking on the current map
                        Map.UpdateUnitPosition(this, oldPosition);
                    }
                    // If collision would occur, don't move (unit stops)
                }
                
                // Check for map transitions
                var mapAtPosition = WorldMap.GetMapAtPosition(Position);
                if (mapAtPosition != null && mapAtPosition != Map)
                {
                    Map.RemoveUnit(this);
                    mapAtPosition.AddUnit(this);
                    Map = mapAtPosition;
                    WorldMap.UpdateCurrentMap();
                }
            }
        }
        
        /// <summary>
        /// Checks if moving to the specified position would cause a collision with another unit
        /// </summary>
        /// <param name="newPosition">The position we want to move to</param>
        /// <returns>True if collision would occur, false otherwise</returns>
        private bool WouldCollideWithOtherUnit(Vector2 newPosition)
        {
            if (Map == null) return true;
            
            // Convert new position to tile coordinates
            var tileCoords = Map.WorldPositionToTileCoordinates(newPosition);
            if (!tileCoords.HasValue)
            {
                // Position is outside the map, consider it a collision
                return true;
            }
            
            // Check if there's another unit at this tile position
            var unitAtTile = Map.GetUnitAtTile(tileCoords.Value.X, tileCoords.Value.Y);
            if (unitAtTile != null && unitAtTile != this)
            {
                // There's another unit occupying this tile
                return true;
            }
            
            // No collision detected
            return false;
        }
        

    }
}
