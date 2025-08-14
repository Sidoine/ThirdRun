using System;
using System.Collections.Generic;
using ThirdRun.Utils;

namespace ThirdRun.Items
{
    public static class RandomItemGenerator
    {
        public static Item GenerateRandomItem(int monsterLevel, Random random)
        {
            return GenerateRandomItem(monsterLevel, random, ItemRarity.Common);
        }

        public static Item GenerateRandomItem(int monsterLevel, Random random, ItemRarity rarity)
        {
            int itemLevel = CalculateItemLevel(monsterLevel, random, rarity);
            int itemType = random.Next(0, 3); // 0: weapon, 1: armor, 2: potion

            return itemType switch
            {
                0 => GenerateRandomWeapon(itemLevel, random),
                1 => GenerateRandomArmor(itemLevel, random),
                2 => GenerateRandomPotion(itemLevel, random),
                _ => GenerateRandomWeapon(itemLevel, random)
            };
        }

        private static int CalculateItemLevel(int monsterLevel, Random random, ItemRarity rarity)
        {
            // Base item level is monster level +/- 1, minimum 1
            int variation = random.Next(-1, 2); // -1, 0, or 1
            int baseLevel = Math.Max(1, monsterLevel + variation);
            
            // Apply rarity boost to item level
            int rarityBoost = rarity switch
            {
                ItemRarity.Common => 0,
                ItemRarity.Rare => random.Next(1, 3),    // +1 to +2 levels
                ItemRarity.Epic => random.Next(2, 5),    // +2 to +4 levels
                _ => 0
            };
            
            return baseLevel + rarityBoost;
        }

        private static Weapon GenerateRandomWeapon(int itemLevel, Random random)
        {
            var template = ItemTemplateRepository.GetRandomWeaponTemplate(random);
            string prefix = ItemTemplateRepository.GetRandomWeaponPrefix(random);
            
            return (Weapon)template.CreateItem(itemLevel, random, prefix);
        }

        private static Armor GenerateRandomArmor(int itemLevel, Random random)
        {
            var template = ItemTemplateRepository.GetRandomArmorTemplate(random);
            string prefix = ItemTemplateRepository.GetRandomArmorPrefix(random);
            
            return (Armor)template.CreateItem(itemLevel, random, prefix);
        }

        private static Potion GenerateRandomPotion(int itemLevel, Random random)
        {
            var template = ItemTemplateRepository.GetRandomPotionTemplate(random);
            
            return (Potion)template.CreateItem(itemLevel, random);
        }
    }
}