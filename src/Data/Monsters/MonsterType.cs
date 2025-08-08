using Microsoft.Xna.Framework;
using ThirdRun.Data;

namespace MonogameRPG.Monsters
{
    public class MonsterType
    {
        public string Name { get; set; }
        public int BaseHealth { get; set; }
        public int BaseAttack { get; set; }
        public string TexturePath { get; set; }
        public int Level { get; set; }
        public CharacteristicValues Characteristics { get; private set; }

        public MonsterType(string name, int baseHealth, int baseAttack, string texturePath, int level = 1)
        {
            Name = name;
            BaseHealth = baseHealth;
            BaseAttack = baseAttack;
            TexturePath = texturePath;
            Level = level;
            Characteristics = new CharacteristicValues();
        }
    }
}
