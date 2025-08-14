using System;
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
        private readonly Random random;

        public Character? Target { get; private set; } // Current target being chased

        public Monster(MonsterType type, MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap, Random random) : base(map, worldMap)
        {
            this.random = random;
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





        /// <summary>
        /// Updates monster AI behavior based on current state and nearby characters
        /// </summary>
        public void Update()
        {
            if (IsDead) return;
            if (Map == null) return;

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
            if (Map == null) return;
            
            var characters = Map.Characters.Where(c => !c.IsDead);
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
            if (Map == null) return;
            
            float wakeUpRadius = 150f; // Slightly larger than aggro radius
            var nearbyMonsters = Map.Monsters.Where(m => m != this && m.State == MonsterState.Sleeping);
            
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
                // In range of an available ability, use abilities
                UseAbilities();
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
            if (Map == null)
            {
                State = MonsterState.Sleeping;
                Target = null;
                return;
            }

            var nearestCharacter = Map.Characters
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
            if (Target == null || Map == null) return;

            // Use the sophisticated pathfinding from the base class
            MoveTo(Target.Position);
        }

        public Item DropLoot()
        {
            // Use the monster type's loot table if available, otherwise fall back to random generation
            if (Type.LootTable != null)
            {
                return Type.LootTable.GenerateItem(Level, random);
            }
            else
            {
                // Fallback to random generation for backward compatibility
                return RandomItemGenerator.GenerateRandomItem(Level, random);
            }
        }

        public int GetExperienceValue()
        {
            // Valeur d'expérience par défaut, à adapter selon le type de monstre
            return 10;
        }
    }
}