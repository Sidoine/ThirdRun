using System;
using System.Collections.Generic;
using ThirdRun.Utils;

namespace ThirdRun.Items
{
    public static class RandomItemGenerator
    {
        private static readonly string[] WeaponNames = {
            "Épée", "Hache", "Arc", "Dague", "Marteau", "Lance", "Faux", "Masse"
        };

        private static readonly string[] ArmorNames = {
            "Casque", "Plastron", "Gants", "Bottes", "Bouclier", "Cotte de mailles", "Robe", "Cape"
        };

        private static readonly string[] WeaponPrefixes = {
            "Rouillé", "Solide", "Magique", "Enchanté", "Légendaire", "Ancien", "Béni", "Maudit"
        };

        private static readonly string[] ArmorPrefixes = {
            "Usé", "Renforcé", "Magique", "Enchanté", "Légendaire", "Ancien", "Béni", "Maudit"
        };

        private static readonly string[] PotionTypes = {
            "Potion de Soin", "Potion de Magie", "Potion de Force", "Potion d'Agilité", 
            "Potion de Résistance", "Élixir de Vie", "Philtre de Guérison", "Breuvage Mystique"
        };

        public static Item GenerateRandomItem(int monsterLevel)
        {
            int itemLevel = CalculateItemLevel(monsterLevel);
            int itemType = Helpers.RandomNumber(0, 3); // 0: weapon, 1: armor, 2: potion

            return itemType switch
            {
                0 => GenerateRandomWeapon(itemLevel),
                1 => GenerateRandomArmor(itemLevel),
                2 => GenerateRandomPotion(itemLevel),
                _ => GenerateRandomWeapon(itemLevel)
            };
        }

        private static int CalculateItemLevel(int monsterLevel)
        {
            // Item level is monster level +/- 1, minimum 1
            int variation = Helpers.RandomNumber(-1, 2); // -1, 0, or 1
            return Math.Max(1, monsterLevel + variation);
        }

        private static Weapon GenerateRandomWeapon(int itemLevel)
        {
            string prefix = WeaponPrefixes[Helpers.RandomNumber(0, WeaponPrefixes.Length)];
            string weaponType = WeaponNames[Helpers.RandomNumber(0, WeaponNames.Length)];
            string name = $"{prefix} {weaponType}";

            // Stats scale with item level
            int baseValue = itemLevel * Helpers.RandomNumber(15, 25);
            int bonusStats = itemLevel + Helpers.RandomNumber(0, 3);
            int damage = itemLevel * 2 + Helpers.RandomNumber(1, 6);

            string description = $"Une {weaponType.ToLower()} {prefix.ToLower()} de niveau {itemLevel}";

            return new Weapon(name, description, baseValue, bonusStats, damage, itemLevel);
        }

        private static Armor GenerateRandomArmor(int itemLevel)
        {
            string prefix = ArmorPrefixes[Helpers.RandomNumber(0, ArmorPrefixes.Length)];
            string armorType = ArmorNames[Helpers.RandomNumber(0, ArmorNames.Length)];
            string name = $"{prefix} {armorType}";

            // Stats scale with item level
            int baseValue = itemLevel * Helpers.RandomNumber(12, 20);
            int bonusStats = itemLevel + Helpers.RandomNumber(0, 2);
            int defense = itemLevel * 2 + Helpers.RandomNumber(1, 5);

            string description = $"Un {armorType.ToLower()} {prefix.ToLower()} de niveau {itemLevel}";

            return new Armor(name, description, baseValue, bonusStats, defense, itemLevel);
        }

        private static Potion GenerateRandomPotion(int itemLevel)
        {
            string potionType = PotionTypes[Helpers.RandomNumber(0, PotionTypes.Length)];
            string name = $"{potionType} (Niv. {itemLevel})";

            // Stats scale with item level
            int baseValue = itemLevel * Helpers.RandomNumber(8, 15);
            int healAmount = itemLevel * 3 + Helpers.RandomNumber(5, 15);

            string description = $"Une potion magique de niveau {itemLevel} qui restaure la santé";

            return new Potion(name, description, baseValue, healAmount, itemLevel);
        }
    }
}