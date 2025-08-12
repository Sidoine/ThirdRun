namespace ThirdRun.Data
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using MonogameRPG.Map;
    using ThirdRun.Characters;
    using ThirdRun.Items;
    using ThirdRun.Utils;

    /// <summary>
    /// Représente un joueur dans le jeu, avec des informations sur son personnage et son inventaire.
    /// </summary>
    public class Player
    {
        public List<Character> Characters { get; set; }

        public Player(WorldMap worldMap, int? seed = null)
        {
            // Set up seeded randomization if provided
            if (seed.HasValue)
            {
                Helpers.SetRandomSeed(seed.Value + 1000); // Offset seed to avoid conflicts with WorldMap
            }
            
            Characters = new List<Character>();
            
            // Position de départ au centre de la carte
            Vector2 startPosition = new Vector2(
                MonogameRPG.Map.Map.GridWidth / 2 * MonogameRPG.Map.Map.TileWidth + MonogameRPG.Map.Map.TileWidth / 2,
                MonogameRPG.Map.Map.GridHeight / 2 * MonogameRPG.Map.Map.TileHeight + MonogameRPG.Map.Map.TileHeight / 2);
            
            // Création des quatre personnages requis
            var warrior = new Character("Guerrier", CharacterClass.Guerrier, 35, 12, worldMap.CurrentMap, worldMap)
            {
                Position = startPosition
            };
            
            var hunter = new Character("Chasseur", CharacterClass.Chasseur, 30, 10, worldMap.CurrentMap, worldMap)
            {
                Position = startPosition + new Vector2(20, 0) // Légèrement décalé
            };
            
            var mage = new Character("Mage", CharacterClass.Mage, 25, 8, worldMap.CurrentMap, worldMap)
            {
                Position = startPosition + new Vector2(0, 20) // Légèrement décalé
            };
            
            var priest = new Character("Prêtre", CharacterClass.Prêtre, 28, 6, worldMap.CurrentMap, worldMap)
            {
                Position = startPosition + new Vector2(20, 20) // Légèrement décalé
            };
            
            // Ajout d'objets générés aléatoirement dans l'inventaire du guerrier (leader du groupe)
            for (int i = 0; i < 5; i++)
            {
                var randomItem = RandomItemGenerator.GenerateRandomItem(5); // Niveau 5 pour les objets de départ
                warrior.Inventory.AddItem(randomItem);
            }
            
            Characters.Add(warrior);
            Characters.Add(hunter);
            Characters.Add(mage);
            Characters.Add(priest);
        }
    }
}