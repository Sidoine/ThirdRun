using System;
using System.Collections.Generic;
using ThirdRun.Utils;

namespace ThirdRun.Items
{
    public static class RandomItemGenerator
    {
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
            var template = ItemTemplateRepository.GetRandomWeaponTemplate();
            string prefix = ItemTemplateRepository.GetRandomWeaponPrefix();
            
            return (Weapon)template.CreateItem(itemLevel, prefix);
        }

        private static Armor GenerateRandomArmor(int itemLevel)
        {
            var template = ItemTemplateRepository.GetRandomArmorTemplate();
            string prefix = ItemTemplateRepository.GetRandomArmorPrefix();
            
            return (Armor)template.CreateItem(itemLevel, prefix);
        }

        private static Potion GenerateRandomPotion(int itemLevel)
        {
            var template = ItemTemplateRepository.GetRandomPotionTemplate();
            
            return (Potion)template.CreateItem(itemLevel);
        }
    }
}