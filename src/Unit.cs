using Microsoft.Xna.Framework;
using ThirdRun.Data;

namespace MonogameRPG
{
    public abstract class Unit
    {
        public Vector2 Position { get; set; }
        public int CurrentHealth { get; set; }
        public CharacteristicValues Characteristics { get; private set; }
        public bool IsDead => CurrentHealth <= 0;

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
        }
    }
}
