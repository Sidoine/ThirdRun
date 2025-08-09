using System;
using System.Collections.Generic;
using System.Linq;

namespace ThirdRun.Items
{
    /// <summary>
    /// Repository containing all predefined item templates with their associated images
    /// </summary>
    public static class ItemTemplateRepository
    {
        // Weapon templates mapped to their images
        private static readonly WeaponTemplate[] WeaponTemplates = 
        {
            new("Épée", "Une épée classique", "Items/Weapons/epee", 2),
            new("Hache", "Une hache de guerre", "Items/Weapons/hache", 3),
            new("Arc", "Un arc de chasse", "Items/Weapons/arc", 2),
            new("Dague", "Une dague rapide", "Items/Weapons/dague", 1),
            new("Marteau", "Un marteau lourd", "Items/Weapons/marteau", 3),
            new("Lance", "Une lance perçante", "Items/Weapons/lance", 2),
            new("Faux", "Une faux mortelle", "Items/Weapons/faux", 3),
            new("Masse", "Une masse d'armes", "Items/Weapons/masse", 3)
        };

        // Armor templates mapped to their images
        private static readonly ArmorTemplate[] ArmorTemplates = 
        {
            new("Casque", "Un casque de protection", "Items/Armors/casque", 2),
            new("Plastron", "Une armure de torse", "Items/Armors/plastron", 3),
            new("Gants", "Des gants de combat", "Items/Armors/gants", 1),
            new("Bottes", "Des bottes résistantes", "Items/Armors/bottes", 1),
            new("Bouclier", "Un bouclier défensif", "Items/Armors/bouclier", 2),
            new("Cotte de mailles", "Une cotte de mailles", "Items/Armors/cotte_de_mailles", 3),
            new("Robe", "Une robe magique", "Items/Armors/robe", 1),
            new("Cape", "Une cape protectrice", "Items/Armors/cape", 1)
        };

        // Potion templates mapped to their images (only ones with existing images)
        private static readonly PotionTemplate[] PotionTemplates = 
        {
            new("Potion de Soin", "Restaure la santé", "Items/Potions/potion_soin", 3),
            new("Potion de Magie", "Restaure la magie", "Items/Potions/potion_magie", 2),
            new("Potion de Force", "Augmente la force", "Items/Potions/potion_force", 2),
            new("Potion d'Agilité", "Augmente l'agilité", "Items/Potions/potion_agilite", 2),
            new("Potion de Résistance", "Augmente la résistance", "Items/Potions/potion_resistance", 2)
        };

        // Prefixes for weapons and armor
        private static readonly string[] WeaponPrefixes = {
            "Rouillé", "Solide", "Magique", "Enchanté", "Légendaire", "Ancien", "Béni", "Maudit"
        };

        private static readonly string[] ArmorPrefixes = {
            "Usé", "Renforcé", "Magique", "Enchanté", "Légendaire", "Ancien", "Béni", "Maudit"
        };

        /// <summary>
        /// Get a random weapon template
        /// </summary>
        public static WeaponTemplate GetRandomWeaponTemplate()
        {
            return WeaponTemplates[Utils.Helpers.RandomNumber(0, WeaponTemplates.Length)];
        }

        /// <summary>
        /// Get a random armor template
        /// </summary>
        public static ArmorTemplate GetRandomArmorTemplate()
        {
            return ArmorTemplates[Utils.Helpers.RandomNumber(0, ArmorTemplates.Length)];
        }

        /// <summary>
        /// Get a random potion template
        /// </summary>
        public static PotionTemplate GetRandomPotionTemplate()
        {
            return PotionTemplates[Utils.Helpers.RandomNumber(0, PotionTemplates.Length)];
        }

        /// <summary>
        /// Get a random weapon prefix
        /// </summary>
        public static string GetRandomWeaponPrefix()
        {
            return WeaponPrefixes[Utils.Helpers.RandomNumber(0, WeaponPrefixes.Length)];
        }

        /// <summary>
        /// Get a random armor prefix
        /// </summary>
        public static string GetRandomArmorPrefix()
        {
            return ArmorPrefixes[Utils.Helpers.RandomNumber(0, ArmorPrefixes.Length)];
        }

        /// <summary>
        /// Get all available item image paths for content loading
        /// </summary>
        public static IEnumerable<string> GetAllImagePaths()
        {
            foreach (var template in WeaponTemplates)
                yield return template.ImagePath;
            
            foreach (var template in ArmorTemplates)
                yield return template.ImagePath;
            
            foreach (var template in PotionTemplates)
                yield return template.ImagePath;
        }

        /// <summary>
        /// Get all weapon templates
        /// </summary>
        public static IReadOnlyCollection<WeaponTemplate> GetAllWeaponTemplates() => WeaponTemplates;

        /// <summary>
        /// Get all armor templates
        /// </summary>
        public static IReadOnlyCollection<ArmorTemplate> GetAllArmorTemplates() => ArmorTemplates;

        /// <summary>
        /// Get all potion templates
        /// </summary>
        public static IReadOnlyCollection<PotionTemplate> GetAllPotionTemplates() => PotionTemplates;
    }
}