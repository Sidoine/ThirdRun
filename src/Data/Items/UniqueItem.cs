using System;
using ThirdRun.Data;

namespace ThirdRun.Items
{
    /// <summary>
    /// Represents a unique item with fixed characteristics that doesn't change between drops
    /// </summary>
    public class UniqueItem
    {
        public string Name { get; }
        public string Description { get; }
        public int ItemLevel { get; }
        public string? ImagePath { get; }
        public ItemSlot ItemSlot { get; }
        public CharacteristicValues Characteristics { get; }

        public UniqueItem(string name, string description, int itemLevel, ItemSlot itemSlot, CharacteristicValues characteristics, string? imagePath = null)
        {
            Name = name;
            Description = description;
            ItemLevel = itemLevel;
            ItemSlot = itemSlot;
            Characteristics = new CharacteristicValues();
            // Copy characteristics to avoid shared reference issues
            foreach (var kvp in characteristics.GetAllValues())
            {
                Characteristics.SetValue(kvp.Key, kvp.Value);
            }
            ImagePath = imagePath;
        }

        /// <summary>
        /// Create an instance of this unique item
        /// </summary>
        public Item CreateItem()
        {
            // Calculate value based on item level (unique items are more valuable)
            int baseValue = ItemLevel * 100; // Base value significantly higher for unique items

            switch (ItemSlot)
            {
                case ItemSlot.Weapon:
                    var weapon = new Weapon(Name, Description, baseValue, 0, 0, ItemLevel)
                    {
                        ImagePath = ImagePath
                    };
                    // Apply characteristics
                    weapon.Characteristics.Clear();
                    foreach (var kvp in Characteristics.GetAllValues())
                    {
                        weapon.Characteristics.SetValue(kvp.Key, kvp.Value);
                    }
                    // Set damage from characteristics if available, otherwise default
                    weapon.Damage = Characteristics.GetValue(Characteristic.MeleeAttackPower);
                    return weapon;

                case ItemSlot.Armor:
                    var armor = new Armor(Name, Description, baseValue, 0, 0, ItemLevel)
                    {
                        ImagePath = ImagePath
                    };
                    // Apply characteristics
                    armor.Characteristics.Clear();
                    foreach (var kvp in Characteristics.GetAllValues())
                    {
                        armor.Characteristics.SetValue(kvp.Key, kvp.Value);
                    }
                    // Set defense from characteristics if available, otherwise default
                    armor.Defense = Characteristics.GetValue(Characteristic.Armor);
                    return armor;

                case ItemSlot.Potion:
                    return new Potion(Name, Description, baseValue, Characteristics.GetValue(Characteristic.HealingPower), ItemLevel)
                    {
                        ImagePath = ImagePath
                    };

                default:
                    throw new ArgumentException($"Unsupported ItemSlot: {ItemSlot}");
            }
        }
    }
}