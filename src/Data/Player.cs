namespace ThirdRun.Data
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using MonogameRPG.Map;
    using ThirdRun.Characters;
    using ThirdRun.Items;

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
            
            // Ajout d'objets de test dans l'inventaire
            hero.Inventory.AddItem(new Potion("Potion de soin", "Restore 20 HP", 50, 20));
            hero.Inventory.AddItem(new Weapon("Épée de fer", "A sturdy iron sword", 100, 5, 15));
            hero.Inventory.AddItem(new Armor("Armure de cuir", "Basic leather armor", 80, 3, 8));
            hero.Inventory.AddItem(new Potion("Grande Potion", "Restore 50 HP", 120, 50));
            hero.Inventory.AddItem(new Weapon("Dague", "A quick dagger", 60, 2, 8));
            hero.Inventory.AddItem(new Item("Clé mystérieuse", "An old mysterious key", 200));
            hero.Inventory.AddItem(new Item("Parchemin", "An ancient scroll", 150));
            hero.Inventory.AddItem(new Armor("Bouclier", "A wooden shield", 90, 4, 12));
            
            Characters.Add(hero);
        }
    }
}