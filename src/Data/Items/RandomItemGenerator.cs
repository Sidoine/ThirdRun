using System;
using System.Collections.Generic;
using ThirdRun.Utils;

namespace ThirdRun.Items
{
    public static class RandomItemGenerator
    {
        public static Item GenerateRandomItem(int monsterLevel, Random random)
        {
            int itemLevel = CalculateItemLevel(monsterLevel, random);
            int itemType = random.Next(0, 3); // 0: weapon, 1: armor, 2: potion

            return itemType switch
            {
                0 => GenerateRandomWeapon(itemLevel, random),
                1 => GenerateRandomArmor(itemLevel, random),
                2 => GenerateRandomPotion(itemLevel, random),
                _ => GenerateRandomWeapon(itemLevel, random)
            };
        }

        private static int CalculateItemLevel(int monsterLevel, Random random)
        {
            // Item level is monster level +/- 1, minimum 1
            int variation = random.Next(-1, 2); // -1, 0, or 1
            return Math.Max(1, monsterLevel + variation);
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