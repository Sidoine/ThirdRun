using System.Collections.Generic;
using System.Linq;
using System;
using MonogameRPG.Monsters;

namespace ThirdRun.Data.Dungeons
{
    /// <summary>
    /// Repository containing all predefined dungeon definitions with their level ranges and map configurations
    /// </summary>
    public static class DungeonRepository
    {
        // Helper method to get MonsterType by name
        private static MonsterType GetMonsterType(string name)
        {
            var monsterType = MonsterTemplateRepository.GetMonsterTypeByName(name);
            if (monsterType == null)
            {
                throw new InvalidOperationException($"Monster type '{name}' not found in MonsterTemplateRepository");
            }
            return monsterType;
        }
        private static readonly List<Dungeon> Dungeons = new List<Dungeon>
        {
            new Dungeon("Caverne des Gobelins", 1, 3, new List<DungeonMapDefinition>
            {
                new DungeonMapDefinition("Entrée de la caverne avec quelques gobelins", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Gobelin"), 3)
                }),
                new DungeonMapDefinition("Tunnels sombres infestés de gobelins", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Gobelin"), 2),
                    new MonsterSpawn(GetMonsterType("Gobelin Voleur"), 2)
                }),
                new DungeonMapDefinition("Chambre du Chef Gobelin", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Gobelin"), 3),
                    new MonsterSpawn(GetMonsterType("Chef Gobelin"), 1, true) // Boss
                })
            }),

            new Dungeon("Forêt Maudite", 4, 6, new List<DungeonMapDefinition>
            {
                new DungeonMapDefinition("Lisière de la forêt avec loups et araignées", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Loup"), 2),
                    new MonsterSpawn(GetMonsterType("Araignée Géante"), 2)
                }),
                new DungeonMapDefinition("Cœur de la forêt avec créatures corrompues", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Loup Alpha"), 1),
                    new MonsterSpawn(GetMonsterType("Araignée Géante"), 2),
                    new MonsterSpawn(GetMonsterType("Chauve-souris Géante"), 2)
                }),
                new DungeonMapDefinition("Clairière du Druide Corrompu", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Loup"), 2),
                    new MonsterSpawn(GetMonsterType("Veuve Noire Géante"), 2),
                    new MonsterSpawn(GetMonsterType("Druide Corrompu"), 1, true) // Boss
                })
            }),

            new Dungeon("Crypte Ancienne", 7, 9, new List<DungeonMapDefinition>
            {
                new DungeonMapDefinition("Couloirs hantés par des squelettes", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Squelette"), 4)
                }),
                new DungeonMapDefinition("Salle des tombeaux avec zombies", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Squelette"), 2),
                    new MonsterSpawn(GetMonsterType("Squelette Guerrier"), 3)
                }),
                new DungeonMapDefinition("Chambre du Seigneur Liche", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Squelette Guerrier"), 2),
                    new MonsterSpawn(GetMonsterType("Squelette Archer"), 2),
                    new MonsterSpawn(GetMonsterType("Seigneur Liche"), 1, true) // Boss
                })
            }),

            new Dungeon("Pic du Dragon", 10, 13, new List<DungeonMapDefinition>
            {
                new DungeonMapDefinition("Sentier montagneux avec orcs", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Orc"), 3),
                    new MonsterSpawn(GetMonsterType("Orc Guerrier"), 2)
                }),
                new DungeonMapDefinition("Grotte avec trolls des montagnes", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Orc Élite"), 2),
                    new MonsterSpawn(GetMonsterType("Troll des Cavernes"), 1, true) // Mini-boss
                }),
                new DungeonMapDefinition("Sommet avec élémentaires de feu", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Élémentaire de Feu"), 4),
                    new MonsterSpawn(GetMonsterType("Sorcière Noire"), 2)
                }),
                new DungeonMapDefinition("Antre du Dragon Rouge", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Élémentaire de Feu"), 2),
                    new MonsterSpawn(GetMonsterType("Dragon Rouge"), 1, true) // Boss
                })
            }),

            new Dungeon("Citadelle du Chaos", 14, 16, new List<DungeonMapDefinition>
            {
                new DungeonMapDefinition("Remparts gardés par des démons mineurs", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Orc Élite"), 4),
                    new MonsterSpawn(GetMonsterType("Sorcière Noire"), 2)
                }),
                new DungeonMapDefinition("Cour intérieure avec gardes démoniaques", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Troll des Cavernes"), 2),
                    new MonsterSpawn(GetMonsterType("Liche"), 1, true), // Mini-boss
                    new MonsterSpawn(GetMonsterType("Sorcière Noire"), 2)
                }),
                new DungeonMapDefinition("Tour centrale avec élémentaires supérieurs", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Élémentaire de Feu"), 3),
                    new MonsterSpawn(GetMonsterType("Élémentaire d'Eau"), 3),
                    new MonsterSpawn(GetMonsterType("Dragon Jeune"), 1, true) // Mini-boss
                }),
                new DungeonMapDefinition("Sanctuaire du Seigneur du Chaos", new List<MonsterSpawn>
                {
                    new MonsterSpawn(GetMonsterType("Dragon Jeune"), 2),
                    new MonsterSpawn(GetMonsterType("Seigneur du Chaos"), 1, true) // Final Boss
                })
            })
        };

        /// <summary>
        /// Get all available dungeons
        /// </summary>
        public static IReadOnlyCollection<Dungeon> GetAllDungeons()
        {
            return Dungeons.AsReadOnly();
        }

        /// <summary>
        /// Find the most appropriate dungeon for a given character level
        /// </summary>
        public static Dungeon? GetDungeonForLevel(int level)
        {
            return Dungeons.FirstOrDefault(dungeon => dungeon.IsAppropriateForLevel(level));
        }

        /// <summary>
        /// Get all dungeons that are appropriate for a given level range
        /// </summary>
        public static IEnumerable<Dungeon> GetDungeonsForLevelRange(int minLevel, int maxLevel)
        {
            return Dungeons.Where(dungeon => 
                dungeon.MaxLevel >= minLevel && dungeon.MinLevel <= maxLevel);
        }
    }
}