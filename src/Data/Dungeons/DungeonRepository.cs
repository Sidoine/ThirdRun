using System.Collections.Generic;
using System.Linq;
using System;

namespace ThirdRun.Data.Dungeons
{
    /// <summary>
    /// Repository containing all predefined dungeon definitions with their level ranges and map configurations
    /// </summary>
    public static class DungeonRepository
    {
        private static readonly List<Dungeon> Dungeons = new List<Dungeon>
        {
            new Dungeon("Caverne des Gobelins", 1, 3, new List<DungeonMapDefinition>
            {
                new DungeonMapDefinition("Entrée de la caverne avec quelques gobelins", false, 3, 1, 2),
                new DungeonMapDefinition("Tunnels sombres infestés de gobelins", false, 4, 1, 3),
                new DungeonMapDefinition("Chambre du Chef Gobelin", true, 5, 2, 3)
            }),

            new Dungeon("Forêt Maudite", 4, 6, new List<DungeonMapDefinition>
            {
                new DungeonMapDefinition("Lisière de la forêt avec loups et araignées", false, 4, 3, 4),
                new DungeonMapDefinition("Cœur de la forêt avec créatures corrompues", false, 5, 3, 5),
                new DungeonMapDefinition("Clairière du Druide Corrompu", true, 6, 4, 5)
            }),

            new Dungeon("Crypte Ancienne", 7, 9, new List<DungeonMapDefinition>
            {
                new DungeonMapDefinition("Couloirs hantés par des squelettes", false, 4, 5, 6),
                new DungeonMapDefinition("Salle des tombeaux avec zombies", false, 5, 5, 7),
                new DungeonMapDefinition("Chambre du Seigneur Liche", true, 6, 6, 8)
            }),

            new Dungeon("Pic du Dragon", 10, 13, new List<DungeonMapDefinition>
            {
                new DungeonMapDefinition("Sentier montagneux avec orcs", false, 5, 8, 9),
                new DungeonMapDefinition("Grotte avec trolls des montagnes", true, 6, 8, 10),
                new DungeonMapDefinition("Sommet avec élémentaires de feu", false, 6, 9, 11),
                new DungeonMapDefinition("Antre du Dragon Rouge", true, 8, 10, 12)
            }),

            new Dungeon("Citadelle du Chaos", 14, 16, new List<DungeonMapDefinition>
            {
                new DungeonMapDefinition("Remparts gardés par des démons mineurs", false, 6, 12, 13),
                new DungeonMapDefinition("Cour intérieure avec gardes démoniaques", true, 7, 12, 14),
                new DungeonMapDefinition("Tour centrale avec élémentaires supérieurs", false, 7, 13, 14),
                new DungeonMapDefinition("Sanctuaire du Seigneur du Chaos", true, 8, 14, 15)
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