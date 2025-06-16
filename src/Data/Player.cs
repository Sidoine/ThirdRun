namespace ThirdRun.Data
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using MonogameRPG.Map;
    using ThirdRun.Characters;

    /// <summary>
    /// Représente un joueur dans le jeu, avec des informations sur son personnage et son inventaire.
    /// </summary>
    public class Player
    {
        public List<Character> Characters { get; set; }

        public Player(Map map, ContentManager content)
        {
            Characters = new List<Character>();
             // Création d'un personnage de test au centre de la carte
            var hero = new Character("Héros", CharacterClass.Guerrier, 30, 1, content)
            {
                Position = new Vector2(
                    map.GridWidth / 2 * map.TileWidth + map.TileWidth / 2,
                    map.GridHeight / 2 * map.TileHeight + map.TileHeight / 2)
            };
            Characters.Add(hero);
        }
    }
}