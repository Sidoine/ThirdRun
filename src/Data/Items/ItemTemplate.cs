using System;

namespace ThirdRun.Items
{
    /// <summary>
    /// Base class for item templates that define the properties and image of items
    /// </summary>
    public abstract class ItemTemplate
    {
        public string BaseName { get; }
        public string Description { get; }
        public string ImagePath { get; }
        
        protected ItemTemplate(string baseName, string description, string imagePath)
        {
            BaseName = baseName;
            Description = description;
            ImagePath = imagePath;
        }

        /// <summary>
        /// Create an actual item instance from this template
        /// </summary>
        public abstract Item CreateItem(int itemLevel, Random random, string? prefix = null);
    }

    /// <summary>
    /// Template for weapons with specific images and properties
    /// </summary>
    public class WeaponTemplate : ItemTemplate
    {
        public int BaseDamage { get; }
        
        public WeaponTemplate(string baseName, string description, string imagePath, int baseDamage)
            : base(baseName, description, imagePath)
        {
            BaseDamage = baseDamage;
        }

        public override Item CreateItem(int itemLevel, Random random, string? prefix = null)
        {
            string name = prefix != null ? $"{prefix} {BaseName}" : BaseName;
            int baseValue = itemLevel * random.Next(15, 25);
            int bonusStats = itemLevel + random.Next(0, 3);
            int damage = BaseDamage * itemLevel + random.Next(1, 6);
            
            string description = $"Une {BaseName.ToLower()} {(prefix?.ToLower() ?? "")} de niveau {itemLevel}".Trim();

            return new Weapon(name, description, baseValue, bonusStats, damage, itemLevel)
            {
                ImagePath = ImagePath
            };
        }
    }

    /// <summary>
    /// Template for armor pieces with specific images and properties
    /// </summary>
    public class ArmorTemplate : ItemTemplate
    {
        public int BaseDefense { get; }
        
        public ArmorTemplate(string baseName, string description, string imagePath, int baseDefense)
            : base(baseName, description, imagePath)
        {
            BaseDefense = baseDefense;
        }

        public override Item CreateItem(int itemLevel, Random random, string? prefix = null)
        {
            string name = prefix != null ? $"{prefix} {BaseName}" : BaseName;
            int baseValue = itemLevel * random.Next(12, 20);
            int bonusStats = itemLevel + random.Next(0, 2);
            int defense = BaseDefense * itemLevel + random.Next(1, 5);
            
            string description = $"Un {BaseName.ToLower()} {(prefix?.ToLower() ?? "")} de niveau {itemLevel}".Trim();

            return new Armor(name, description, baseValue, bonusStats, defense, itemLevel)
            {
                ImagePath = ImagePath
            };
        }
    }

    /// <summary>
    /// Template for potions with specific images and properties
    /// </summary>
    public class PotionTemplate : ItemTemplate
    {
        public int BaseHealAmount { get; }
        
        public PotionTemplate(string baseName, string description, string imagePath, int baseHealAmount)
            : base(baseName, description, imagePath)
        {
            BaseHealAmount = baseHealAmount;
        }

        public override Item CreateItem(int itemLevel, Random random, string? prefix = null)
        {
            string name = $"{BaseName} (Niv. {itemLevel})";
            int baseValue = itemLevel * random.Next(8, 15);
            int healAmount = BaseHealAmount * itemLevel + random.Next(5, 15);
            
            string description = $"Une potion magique de niveau {itemLevel} qui restaure la santé";

            return new Potion(name, description, baseValue, healAmount, itemLevel)
            {
                ImagePath = ImagePath
            };
        }
    }
}