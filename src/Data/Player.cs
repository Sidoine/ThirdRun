namespace ThirdRun.Data
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using MonogameRPG.Map;
    using ThirdRun.Characters;
    using ThirdRun.Items;

    /// <summary>
    /// Représente un joueur dans le jeu, avec des informations sur son personnage et son inventaire.
    /// </summary>
    public class Player
    {
        public List<Character> Characters { get; set; }

        public Player(WorldMap worldMap)
        {
            Characters = new List<Character>();
            
            // Position de départ au centre de la carte
            Vector2 startPosition = new Vector2(
                MonogameRPG.Map.Map.GridWidth / 2 * MonogameRPG.Map.Map.TileWidth + MonogameRPG.Map.Map.TileWidth / 2,
                MonogameRPG.Map.Map.GridHeight / 2 * MonogameRPG.Map.Map.TileHeight + MonogameRPG.Map.Map.TileHeight / 2);
            
            // Création des quatre personnages requis
            var warrior = new Character("Guerrier", CharacterClass.Guerrier, 35, 12, worldMap)
            {
                Position = startPosition
            };
            
            var hunter = new Character("Chasseur", CharacterClass.Chasseur, 30, 10, worldMap)
            {
                Position = startPosition + new Vector2(20, 0) // Légèrement décalé
            };
            
            var mage = new Character("Mage", CharacterClass.Mage, 25, 8, worldMap)
            {
                Position = startPosition + new Vector2(0, 20) // Légèrement décalé
            };
            
            var priest = new Character("Prêtre", CharacterClass.Prêtre, 28, 6, worldMap)
            {
                Position = startPosition + new Vector2(20, 20) // Légèrement décalé
            };
            
            // Ajout d'objets de test dans l'inventaire du guerrier (leader du groupe)
            warrior.Inventory.AddItem(new Potion("Potion de soin", "Restore 20 HP", 50, 20));
            warrior.Inventory.AddItem(new Weapon("Épée de fer", "A sturdy iron sword", 100, 5, 15));
            warrior.Inventory.AddItem(new Armor("Armure de cuir", "Basic leather armor", 80, 3, 8));
            warrior.Inventory.AddItem(new Potion("Grande Potion", "Restore 50 HP", 120, 50));
            warrior.Inventory.AddItem(new Weapon("Dague", "A quick dagger", 60, 2, 8));
            warrior.Inventory.AddItem(new Item("Clé mystérieuse", "An old mysterious key", 200));
            warrior.Inventory.AddItem(new Item("Parchemin", "An ancient scroll", 150));
            warrior.Inventory.AddItem(new Armor("Bouclier", "A wooden shield", 90, 4, 12));
            warrior.Inventory.AddItem(new Item("Bottes de vitesse", "Increases movement speed", 70));
            warrior.Inventory.AddItem(new Item("Anneau de protection", "Increases defense", 110));
            warrior.Inventory.AddItem(new Item("Cape d'invisibilité", "Grants temporary invisibility", 300));
            warrior.Inventory.AddItem(new Item("Potion de mana", "Restores 30 mana", 80));
            warrior.Inventory.AddItem(new Item("Pierre de téléportation", "Teleports you to a safe location", 250));
            warrior.Inventory.AddItem(new Item("Fragment de cristal", "A piece of a magical crystal", 180));
            warrior.Inventory.AddItem(new Item("Livre de sorts", "Contains powerful spells", 400));
            warrior.Inventory.AddItem(new Item("Bâton magique", "A staff imbued with magic", 350));
            warrior.Inventory.AddItem(new Item("Gemme de feu", "A gem that radiates heat", 220));
            warrior.Inventory.AddItem(new Item("Gemme de glace", "A gem that radiates cold", 220));
            warrior.Inventory.AddItem(new Item("Gemme de foudre", "A gem that crackles with electricity", 220));
            warrior.Inventory.AddItem(new Item("Gemme de terre", "A gem that feels heavy and solid", 220));
            warrior.Inventory.AddItem(new Item("Potion de force", "Increases attack power temporarily", 90));
            warrior.Inventory.AddItem(new Item("Potion de défense", "Increases defense temporarily", 90));
            warrior.Inventory.AddItem(new Item("Potion de vitesse", "Increases movement speed temporarily", 90));
            warrior.Inventory.AddItem(new Item("Potion de chance", "Increases luck temporarily", 90));
            warrior.Inventory.AddItem(new Item("Potion de sagesse", "Increases intelligence temporarily", 90));
            
            Characters.Add(warrior);
            Characters.Add(hunter);
            Characters.Add(mage);
            Characters.Add(priest);
        }
    }
}