using ThirdRun.Items;
using ThirdRun.Characters;
using Microsoft.Xna.Framework;
using MonogameRPG;
using System.Collections.Generic;
using System.Linq;

namespace MonogameRPG.Monsters
{
    public enum MonsterState
    {
        Sleeping,   // Default state, monster doesn't move or attack
        Awake,      // Monster is awake but not actively chasing
        Chasing     // Monster is actively chasing and attacking when in range
    }

    public class Monster : Unit
    {
        public MonsterType Type { get; set; }
        public int Level => Type.Level;
        public MonsterState State { get; private set; } = MonsterState.Sleeping;
        public float AggroRadius { get; set; } = 100f; // Default aggro radius in pixels

        public Character? Target { get; private set; } // Current target being chased
        
        // Reference to the map for accessing other units
        private MonogameRPG.Map.Map? _currentMap;

        public Monster(MonsterType type)
        {
            Type = type;

            // Copy all characteristics from monster type
            var typeCharacteristics = type.Characteristics.GetAllValues();
            foreach (var kvp in typeCharacteristics)
            {
                Characteristics.SetValue(kvp.Key, kvp.Value);
            }

            // Set current health to max health from characteristics
            CurrentHealth = MaxHealth;
            Position = new Vector2(0, 0); // Initial position
        }

        public void SetCurrentMap(MonogameRPG.Map.Map map)
        {
            _currentMap = map;
            Map = map; // Also set the base class Map property
        }
        
        public void SetWorldMap(MonogameRPG.Map.WorldMap worldMap)
        {
            WorldMap = worldMap;
        }

        /// <summary>
        /// Gets the attack range from the first available ability that targets enemies
        /// </summary>
        /// <returns>Attack range in pixels, or 0 if no suitable ability is available</returns>
        private float GetAttackRange()
        {
            // Find the first ability that is not on cooldown and targets enemies
            var availableAbility = Abilities.FirstOrDefault(ability => 
                ability.TargetType == ThirdRun.Data.Abilities.TargetType.Enemy && 
                !ability.IsOnCooldown(CurrentGameTime));
                
            return availableAbility?.Range ?? 0f;
        }

        /// <summary>
        /// Updates monster AI behavior based on current state and nearby characters
        /// </summary>
        public void Update()
        {
            if (IsDead) return;
            if (_currentMap == null) return;

            switch (State)
            {
                case MonsterState.Sleeping:
                    CheckForAggroTrigger();
                    break;
                case MonsterState.Awake:
                case MonsterState.Chasing:
                    UpdateChasing();
                    break;
            }
        }

        /// <summary>
        /// Checks if any character is within aggro radius and wakes up if so
        /// </summary>
        private void CheckForAggroTrigger()
        {
            if (_currentMap == null) return;
            
            var characters = _currentMap.Characters.Where(c => !c.IsDead);
            foreach (var character in characters)
            {
                float distance = Vector2.Distance(Position, character.Position);
                if (distance <= AggroRadius)
                {
                    WakeUp(character);
                    break;
                }
            }
        }

        /// <summary>
        /// Wakes up this monster and nearby monsters, starts chasing the target
        /// </summary>
        public void WakeUp(Character trigger)
        {
            if (State == MonsterState.Sleeping)
            {
                State = MonsterState.Awake;
                Target = trigger;
                
                // Wake up nearby monsters (chain awakening)
                WakeUpNearbyMonsters();
                
                // Immediately start chasing
                State = MonsterState.Chasing;
            }
        }

        /// <summary>
        /// Wakes up other sleeping monsters within a certain radius
        /// </summary>
        private void WakeUpNearbyMonsters()
        {
            if (_currentMap == null) return;
            
            float wakeUpRadius = 150f; // Slightly larger than aggro radius
            var nearbyMonsters = _currentMap.Monsters.Where(m => m != this && m.State == MonsterState.Sleeping);
            
            foreach (var monster in nearbyMonsters)
            {
                float distance = Vector2.Distance(Position, monster.Position);
                if (distance <= wakeUpRadius)
                {
                    monster.State = MonsterState.Awake;
                    monster.Target = Target; // Share the same target
                    monster.State = MonsterState.Chasing;
                }
            }
        }

        /// <summary>
        /// Updates chasing behavior - moves toward target and attacks when in range
        /// </summary>
        private void UpdateChasing()
        {
            if (Target == null || Target.IsDead)
            {
                // Find new target or go back to sleep
                FindNewTarget();
                return;
            }

            float distanceToTarget = Vector2.Distance(Position, Target.Position);
            float attackRange = GetAttackRange();
            
            if (attackRange > 0 && distanceToTarget <= attackRange)
            {
                // In range of an available ability, attack
                AttackTarget();
            }
            else
            {
                // Not in range or no available abilities, move toward target
                MoveTowardTarget();
            }
        }



        /// <summary>
        /// Finds a new target or returns to sleep if no characters are nearby
        /// </summary>
        private void FindNewTarget()
        {
            if (_currentMap == null)
            {
                State = MonsterState.Sleeping;
                Target = null;
                return;
            }

            var nearestCharacter = _currentMap.Characters
                .Where(c => !c.IsDead)
                .OrderBy(c => Vector2.Distance(Position, c.Position))
                .FirstOrDefault();

            if (nearestCharacter != null)
            {
                float distance = Vector2.Distance(Position, nearestCharacter.Position);
                if (distance <= AggroRadius * 1.5f) // Slightly larger search radius when already awake
                {
                    Target = nearestCharacter;
                    State = MonsterState.Chasing;
                }
                else
                {
                    // No characters nearby, go back to sleep
                    State = MonsterState.Sleeping;
                    Target = null;
                }
            }
            else
            {
                // No living characters, go back to sleep
                State = MonsterState.Sleeping;
                Target = null;
            }
        }

        /// <summary>
        /// Moves toward the current target
        /// </summary>
        private void MoveTowardTarget()
        {
            if (Target == null || _currentMap == null) return;

            // Use the sophisticated pathfinding from the base class
            MoveTo(Target.Position);
        }

        /// <summary>
        /// Attacks the current target using the first available ability that targets enemies
        /// </summary>
        private void AttackTarget()
        {
            if (Target == null) return;

            // Find the first ability that is not on cooldown and targets enemies
            var availableAbility = Abilities.FirstOrDefault(ability => 
                ability.TargetType == ThirdRun.Data.Abilities.TargetType.Enemy && 
                !ability.IsOnCooldown(CurrentGameTime));

            if (availableAbility != null)
            {
                UseAbility(availableAbility, Target);
            }
        }

        public void Attack(Character target)
        {
            // Use the inherited Attack method from Unit which uses the default ability
            Attack((Unit)target);
        }

        public Item DropLoot()
        {
            // Generate a random item based on monster level
            return RandomItemGenerator.GenerateRandomItem(Level);
        }

        public int GetExperienceValue()
        {
            // Valeur d'expérience par défaut, à adapter selon le type de monstre
            return 10;
        }
    }
}