using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;

namespace MonogameRPG
{
    public abstract class Unit
    {
        public Vector2 Position { get; set; }
        public int CurrentHealth { get; set; }
        public CharacteristicValues Characteristics { get; private set; }
        public bool IsDead => CurrentHealth <= 0;
        
        // Ability system
        public List<Ability> Abilities { get; private set; }
        public Ability DefaultAbility { get; protected set; }

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

        protected Unit()
        {
            Characteristics = new CharacteristicValues();
            Abilities = new List<Ability>();
            DefaultAbility = new MeleeAttackAbility();
            Abilities.Add(DefaultAbility);
        }
        
        // Game time tracking - this should be set by the game loop
        protected float CurrentGameTime { get; set; }
        
        public void UpdateGameTime(float gameTime)
        {
            CurrentGameTime = gameTime;
        }
        
        public bool CanUseAbility(Ability ability, Unit? target)
        {
            return ability.CanUse(this, target, CurrentGameTime);
        }
        
        public void UseAbility(Ability ability, Unit? target)
        {
            ability.Use(this, target, CurrentGameTime);
        }
        
        // Convenience method for default attack behavior
        public void Attack(Unit target)
        {
            UseAbility(DefaultAbility, target);
        }
        
        /// <summary>
        /// Moves the unit to the specified position without collision detection
        /// </summary>
        /// <param name="newPosition">The new position for the unit</param>
        public void MoveToPosition(Vector2 newPosition)
        {
            Position = newPosition;
        }
    }
}
