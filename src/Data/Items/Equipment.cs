using System;
using ThirdRun.Data;

namespace ThirdRun.Items
{
    public class Equipment : Item
    {
        public int BonusStats { get; set; }
        public CharacteristicValues Characteristics { get; private set; }

        public Equipment(string name, string description, int value, int bonusStats, int itemLevel = 1) : base(name, description, value, itemLevel)
        {
            BonusStats = bonusStats;
            Characteristics = new CharacteristicValues();
        }

        public void Equip(Character character)
        {
            character.AttackPower += BonusStats; // Keep existing behavior
            
            // Apply characteristic bonuses
            var characteristicValues = Characteristics.GetAllValues();
            foreach (var kvp in characteristicValues)
            {
                character.Characteristics.AddValue(kvp.Key, kvp.Value);
            }
        }

        public void Unequip(Character character)
        {
            character.AttackPower -= BonusStats; // Remove existing bonuses
            
            // Remove characteristic bonuses
            var characteristicValues = Characteristics.GetAllValues();
            foreach (var kvp in characteristicValues)
            {
                character.Characteristics.RemoveValue(kvp.Key, kvp.Value);
            }
        }
    }
}