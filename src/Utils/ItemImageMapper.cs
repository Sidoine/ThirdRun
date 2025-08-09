using System.Collections.Generic;

namespace ThirdRun.Utils
{
    /// <summary>
    /// Maps French item names to their corresponding image filenames in the Content directory
    /// </summary>
    public static class ItemImageMapper
    {
        // Map French weapon names to their image filenames
        private static readonly Dictionary<string, string> WeaponImageMap = new()
        {
            { "Épée", "Items/Weapons/epee" },
            { "Hache", "Items/Weapons/hache" },
            { "Arc", "Items/Weapons/arc" },
            { "Dague", "Items/Weapons/dague" },
            { "Marteau", "Items/Weapons/marteau" },
            { "Lance", "Items/Weapons/lance" },
            { "Faux", "Items/Weapons/faux" },
            { "Masse", "Items/Weapons/masse" }
        };

        // Map French armor names to their image filenames
        private static readonly Dictionary<string, string> ArmorImageMap = new()
        {
            { "Casque", "Items/Armors/casque" },
            { "Plastron", "Items/Armors/plastron" },
            { "Gants", "Items/Armors/gants" },
            { "Bottes", "Items/Armors/bottes" },
            { "Bouclier", "Items/Armors/bouclier" },
            { "Cotte de mailles", "Items/Armors/cotte_de_mailles" },
            { "Robe", "Items/Armors/robe" },
            { "Cape", "Items/Armors/cape" }
        };

        // Map potion types to their image filenames
        private static readonly Dictionary<string, string> PotionImageMap = new()
        {
            { "Potion de Soin", "Items/Potions/potion_soin" },
            { "Potion de Magie", "Items/Potions/potion_magie" },
            { "Potion de Force", "Items/Potions/potion_force" },
            { "Potion d'Agilité", "Items/Potions/potion_agilite" },
            { "Potion de Résistance", "Items/Potions/potion_resistance" }
        };

        /// <summary>
        /// Get the image path for an item name. 
        /// For items with prefixes (like "Rouillé Épée"), extracts the base name to find the image.
        /// </summary>
        /// <param name="itemName">The full item name</param>
        /// <returns>The content path for the item's image, or null if not found</returns>
        public static string? GetImagePath(string itemName)
        {
            // For potions, check if it starts with a known potion type
            foreach (var potion in PotionImageMap)
            {
                if (itemName.StartsWith(potion.Key))
                {
                    return potion.Value;
                }
            }

            // For weapons and armor, try exact match first
            if (WeaponImageMap.TryGetValue(itemName, out string? exactWeaponPath))
            {
                return exactWeaponPath;
            }
            
            if (ArmorImageMap.TryGetValue(itemName, out string? exactArmorPath))
            {
                return exactArmorPath;
            }

            // For prefixed items, extract the base name
            // Check if the item name contains any weapon name
            foreach (var weapon in WeaponImageMap)
            {
                if (itemName.EndsWith(weapon.Key))
                {
                    return weapon.Value;
                }
            }

            // Check if the item name contains any armor name
            foreach (var armor in ArmorImageMap)
            {
                if (itemName.EndsWith(armor.Key))
                {
                    return armor.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Get all available item image paths for loading
        /// </summary>
        /// <returns>Collection of all available item image paths</returns>
        public static IEnumerable<string> GetAllImagePaths()
        {
            foreach (var path in WeaponImageMap.Values)
                yield return path;
            
            foreach (var path in ArmorImageMap.Values)
                yield return path;
            
            foreach (var path in PotionImageMap.Values)
                yield return path;
        }
    }
}