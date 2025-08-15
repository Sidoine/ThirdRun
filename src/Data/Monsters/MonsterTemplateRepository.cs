using System;
using System.Collections.Generic;
using System.Linq;
using ThirdRun.Utils;

namespace MonogameRPG.Monsters
{
    /// <summary>
    /// Repository containing all predefined monster templates with their associated images
    /// </summary>
    public static class MonsterTemplateRepository
    {
        // Monster templates organized by category
        private static readonly MonsterTemplate[] HumanoidTemplates = 
        {
            new("Orc Faible", "Un orc affaibli et malade", "Monsters/orc", 15, 3, 1),
            new("Orc", "Un orc standard, guerrier brutal", "Monsters/orc", 20, 4, 2),
            new("Orc Guerrier", "Un orc expérimenté au combat", "Monsters/orc", 25, 6, 3),
            new("Orc Chef", "Le leader d'un groupe d'orcs", "Monsters/orc", 35, 8, 4),
            new("Orc Élite", "Un orc d'élite, redoutable", "Monsters/orc", 50, 12, 5),
            new("Gobelin", "Petite créature fourbe et agile", "Monsters/goblin", 12, 2, 1),
            new("Gobelin Voleur", "Gobelin rapide spécialisé dans le vol", "Monsters/goblin", 18, 4, 2),
            new("Gobelin Chaman", "Gobelin magique avec des pouvoirs", "Monsters/goblin", 22, 6, 3),
            new("Troll", "Géant primitif à la peau épaisse", "Monsters/troll", 80, 15, 4),
            new("Troll des Cavernes", "Troll massif vivant sous terre", "Monsters/troll", 120, 20, 5),
        };

        private static readonly MonsterTemplate[] UndeadTemplates = 
        {
            new("Squelette", "Os animés par la magie noire", "Monsters/skeleton", 18, 4, 1),
            new("Squelette Guerrier", "Squelette armé et dangereux", "Monsters/skeleton", 25, 7, 2),
            new("Squelette Archer", "Squelette tirant des flèches", "Monsters/skeleton", 20, 8, 3),
            new("Liche", "Sorcier mort-vivant très puissant", "Monsters/lich", 60, 18, 5),
        };

        private static readonly MonsterTemplate[] BeastTemplates = 
        {
            new("Loup", "Prédateur sauvage aux crocs acérés", "Monsters/wolf", 25, 6, 2),
            new("Loup Alpha", "Chef de meute, plus grand et fort", "Monsters/wolf", 40, 10, 3),
            new("Ours", "Mammifère imposant aux griffes redoutables", "Monsters/bear", 55, 12, 3),
            new("Ours Grizzly", "Ours géant particulièrement agressif", "Monsters/bear", 85, 18, 4),
        };

        private static readonly MonsterTemplate[] MagicalTemplates = 
        {
            new("Sorcière", "Magicienne maléfique aux sorts puissants", "Monsters/witch", 35, 12, 3),
            new("Sorcière Noire", "Maîtresse des arts interdits", "Monsters/witch", 50, 16, 4),
            new("Élémentaire de Feu", "Créature de flammes pures", "Monsters/fire_elemental", 45, 14, 3),
            new("Élémentaire d'Eau", "Être aquatique mystique", "Monsters/water_elemental", 40, 11, 3),
            new("Dragon Jeune", "Jeune dragon encore petit mais redoutable", "Monsters/dragon", 100, 25, 5),
        };

        private static readonly MonsterTemplate[] BossTemplates = 
        {
            // Early game bosses (Levels 1-3)
            new("Chef Gobelin", "Chef des gobelins, plus fort et intelligent", "Monsters/goblin_chief", 45, 12, 3),
            
            // Mid-early game bosses (Levels 4-6)
            new("Druide Corrompu", "Druide transformé par la magie noire", "Monsters/corrupted_druid", 70, 18, 5),
            
            // Mid game bosses (Levels 7-9)
            new("Seigneur Liche", "Liche ancienne aux pouvoirs redoutables", "Monsters/lich_lord", 120, 28, 8),
            
            // Late mid game bosses (Levels 10-13)
            new("Dragon Rouge", "Dragon adulte cracheur de feu", "Monsters/red_dragon", 200, 45, 12),
            
            // End game bosses (Levels 14-16)
            new("Seigneur du Chaos", "Maître des forces chaotiques", "Monsters/chaos_lord", 300, 60, 15)
        };

        private static readonly MonsterTemplate[] CreatureTemplates = 
        {
            new("Araignée Géante", "Arachnide venimeuse de grande taille", "Monsters/spider", 28, 8, 2),
            new("Veuve Noire Géante", "Araignée mortelle au venin paralysant", "Monsters/spider", 35, 12, 3),
            new("Chauve-souris Géante", "Mammifère volant agressif", "Monsters/bat", 15, 5, 1),
            new("Vampire Chauve-souris", "Chauve-souris assoiffée de sang", "Monsters/bat", 22, 8, 2),
        };

        /// <summary>
        /// Get all monster templates
        /// </summary>
        private static IEnumerable<MonsterTemplate> GetAllTemplates()
        {
            return HumanoidTemplates
                .Concat(UndeadTemplates)
                .Concat(BeastTemplates)
                .Concat(MagicalTemplates)
                .Concat(CreatureTemplates)
                .Concat(BossTemplates);
        }

        /// <summary>
        /// Get a random monster template of any type
        /// </summary>
        public static MonsterTemplate GetRandomTemplate(Random random)
        {
            var allTemplates = GetAllTemplates().ToArray();
            return allTemplates[random.Next(0, allTemplates.Length)];
        }

        /// <summary>
        /// Get a random monster template for a specific level range
        /// </summary>
        public static MonsterTemplate GetRandomTemplateForLevel(int minLevel, int maxLevel, Random random)
        {
            var suitableTemplates = GetAllTemplates()
                .Where(t => t.Level >= minLevel && t.Level <= maxLevel)
                .ToArray();
            
            if (suitableTemplates.Length == 0)
            {
                // Fallback to any template if none match the level range
                return GetRandomTemplate(random);
            }
            
            return suitableTemplates[random.Next(0, suitableTemplates.Length)];
        }

        /// <summary>
        /// Get templates by category
        /// </summary>
        public static IReadOnlyCollection<MonsterTemplate> GetHumanoidTemplates() => HumanoidTemplates;
        public static IReadOnlyCollection<MonsterTemplate> GetUndeadTemplates() => UndeadTemplates;
        public static IReadOnlyCollection<MonsterTemplate> GetBeastTemplates() => BeastTemplates;
        public static IReadOnlyCollection<MonsterTemplate> GetMagicalTemplates() => MagicalTemplates;
        public static IReadOnlyCollection<MonsterTemplate> GetCreatureTemplates() => CreatureTemplates;
        public static IReadOnlyCollection<MonsterTemplate> GetBossTemplates() => BossTemplates;

        /// <summary>
        /// Get all available monster image paths for content loading
        /// </summary>
        public static IEnumerable<string> GetAllImagePaths()
        {
            return GetAllTemplates().Select(t => t.ImagePath).Distinct();
        }

        /// <summary>
        /// Get all monster templates
        /// </summary>
        public static IReadOnlyCollection<MonsterTemplate> GetAllMonsterTemplates()
        {
            return GetAllTemplates().ToList();
        }

        /// <summary>
        /// Create a MonsterType from a random template
        /// </summary>
        public static MonsterType CreateRandomMonsterType(Random random)
        {
            return GetRandomTemplate(random).ToMonsterType();
        }

        /// <summary>
        /// Create a MonsterType from a random template for a specific level
        /// </summary>
        public static MonsterType CreateRandomMonsterTypeForLevel(int minLevel, int maxLevel, Random random)
        {
            return GetRandomTemplateForLevel(minLevel, maxLevel, random).ToMonsterType();
        }
    }
}