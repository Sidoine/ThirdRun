using ThirdRun.Items;
using ThirdRun.Characters;
using Microsoft.Xna.Framework;
using MonogameRPG;

namespace MonogameRPG.Monsters
{
    public class Monster : Unit
    {
        public MonsterType Type { get; set; }
        public int Level => Type.Level;

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

        public void Attack(Character target)
        {
            target.CurrentHealth -= AttackPower;
            if (target.CurrentHealth < 0) target.CurrentHealth = 0;
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