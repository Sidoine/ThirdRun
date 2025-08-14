using System;

namespace ThirdRun.Items
{
    /// <summary>
    /// Represents a unique item with fixed characteristics that doesn't change between drops
    /// </summary>
    public abstract class UniqueItem
    {
        public string Name { get; }
        public string Description { get; }
        public int Value { get; }
        public int ItemLevel { get; }
        public string? ImagePath { get; }

        protected UniqueItem(string name, string description, int value, int itemLevel, string? imagePath = null)
        {
            Name = name;
            Description = description;
            Value = value;
            ItemLevel = itemLevel;
            ImagePath = imagePath;
        }

        /// <summary>
        /// Create an instance of this unique item
        /// </summary>
        public abstract Item CreateItem();
    }

    /// <summary>
    /// Unique weapon with fixed stats
    /// </summary>
    public class UniqueWeapon : UniqueItem
    {
        public int Damage { get; }
        public int BonusStats { get; }

        public UniqueWeapon(string name, string description, int value, int itemLevel, int damage, int bonusStats, string? imagePath = null)
            : base(name, description, value, itemLevel, imagePath)
        {
            Damage = damage;
            BonusStats = bonusStats;
        }

        public override Item CreateItem()
        {
            return new Weapon(Name, Description, Value, BonusStats, Damage, ItemLevel)
            {
                ImagePath = ImagePath
            };
        }
    }

    /// <summary>
    /// Unique armor with fixed stats
    /// </summary>
    public class UniqueArmor : UniqueItem
    {
        public int Defense { get; }
        public int BonusStats { get; }

        public UniqueArmor(string name, string description, int value, int itemLevel, int defense, int bonusStats, string? imagePath = null)
            : base(name, description, value, itemLevel, imagePath)
        {
            Defense = defense;
            BonusStats = bonusStats;
        }

        public override Item CreateItem()
        {
            return new Armor(Name, Description, Value, BonusStats, Defense, ItemLevel)
            {
                ImagePath = ImagePath
            };
        }
    }

    /// <summary>
    /// Unique potion with fixed healing amount
    /// </summary>
    public class UniquePotion : UniqueItem
    {
        public int HealAmount { get; }

        public UniquePotion(string name, string description, int value, int itemLevel, int healAmount, string? imagePath = null)
            : base(name, description, value, itemLevel, imagePath)
        {
            HealAmount = healAmount;
        }

        public override Item CreateItem()
        {
            return new Potion(Name, Description, Value, HealAmount, ItemLevel)
            {
                ImagePath = ImagePath
            };
        }
    }
}