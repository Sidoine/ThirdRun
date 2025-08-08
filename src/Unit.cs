using Microsoft.Xna.Framework;
using ThirdRun.Data;

namespace MonogameRPG
{
    public abstract class Unit
    {
        public Vector2 Position { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int AttackPower { get; set; }
        public CharacteristicValues Characteristics { get; private set; }
        public bool IsDead => CurrentHealth <= 0;

        protected Unit()
        {
            Characteristics = new CharacteristicValues();
        }
    }
}
