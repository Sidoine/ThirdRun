using System;
using ThirdRun.Data;

namespace ThirdRun.Items
{
    public class Equipment : Item
    {
        public CharacteristicValues Characteristics { get; private set; }

        // Backward compatibility property that delegates to MeleeAttackPower characteristic
        public int BonusStats
        {
            get => Characteristics.GetValue(Characteristic.MeleeAttackPower);
            set => Characteristics.SetValue(Characteristic.MeleeAttackPower, value);
        }

        public Equipment(string name, string description, int value, int bonusStats, int itemLevel = 1) : base(name, description, value, itemLevel)
        {
            Characteristics = new CharacteristicValues();
            // Set the legacy bonusStats parameter as MeleeAttackPower characteristic for backward compatibility
            if (bonusStats > 0)
            {
                BonusStats = bonusStats;
            }
        }

        public void Equip(Character character)
        {
            // Apply characteristic bonuses
            var characteristicValues = Characteristics.GetAllValues();
            foreach (var kvp in characteristicValues)
            {
                character.Characteristics.AddValue(kvp.Key, kvp.Value);
            }
        }

        public void Unequip(Character character)
        {
            // Remove characteristic bonuses
            var characteristicValues = Characteristics.GetAllValues();
            foreach (var kvp in characteristicValues)
            {
                character.Characteristics.RemoveValue(kvp.Key, kvp.Value);
            }
        }
    }
}