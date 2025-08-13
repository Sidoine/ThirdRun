using System;
using Microsoft.Xna.Framework;
using ThirdRun.Data;
using ThirdRun.Characters;
using MonogameRPG;

namespace ThirdRun.Tests
{
    public class ResourceBarDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== Character Portrait Resource Bars Demo ===");
            Console.WriteLine();
            
            // Create test characters
            var (map, worldMap) = CreateTestMapAndWorld();
            var warrior = new Character("Warrior", CharacterClass.Guerrier, 100, 15, map, worldMap);
            var mage = new Character("Mage", CharacterClass.Mage, 80, 12, map, worldMap);
            
            // Display initial state
            Console.WriteLine("Initial Character States:");
            DisplayCharacterBars(warrior);
            DisplayCharacterBars(mage);
            
            // Simulate combat - take damage and use energy
            Console.WriteLine("\nAfter Combat Simulation:");
            warrior.CurrentHealth = 60; // Took 40 damage
            warrior.Resources.GetResource(ResourceType.Energy)!.TryConsume(30f); // Used abilities
            
            mage.CurrentHealth = 45; // Took 35 damage  
            mage.Resources.GetResource(ResourceType.Energy)!.TryConsume(50f); // Used more abilities
            
            DisplayCharacterBars(warrior);
            DisplayCharacterBars(mage);
            
            // Show different percentage levels
            Console.WriteLine("\nVarious Resource Levels:");
            ShowBarVisualization("Health", 100, 100, "████████████████████"); // 100%
            ShowBarVisualization("Energy", 100, 100, "████████████████████");
            
            ShowBarVisualization("Health", 75, 100, "███████████████░░░░░"); // 75%
            ShowBarVisualization("Energy", 60, 100, "████████████░░░░░░░░");
            
            ShowBarVisualization("Health", 25, 100, "█████░░░░░░░░░░░░░░░"); // 25%
            ShowBarVisualization("Energy", 10, 100, "██░░░░░░░░░░░░░░░░░░");
        }
        
        private static void DisplayCharacterBars(Character character)
        {
            var energy = character.Resources.GetResource(ResourceType.Energy)!;
            
            Console.WriteLine($"{character.Name} ({character.Class}):");
            Console.WriteLine($"  Health: {character.CurrentHealth}/{character.GetEffectiveMaxHealth()} " +
                            CreateBar(character.CurrentHealth, character.GetEffectiveMaxHealth()));
            Console.WriteLine($"  Energy: {energy.CurrentValue:F0}/{energy.MaxValue:F0} " +
                            CreateBar(energy.CurrentValue, energy.MaxValue));
            Console.WriteLine();
        }
        
        private static void ShowBarVisualization(string type, float current, float max, string visual)
        {
            float percent = current / max * 100f;
            Console.WriteLine($"{type}: {current:F0}/{max:F0} ({percent:F0}%) [{visual}]");
        }
        
        private static string CreateBar(float current, float max, int width = 20)
        {
            if (max <= 0) return "[" + new string('?', width) + "]";
            
            float percent = MathHelper.Clamp(current / max, 0f, 1f);
            int filled = (int)(width * percent);
            int empty = width - filled;
            
            return "[" + new string('█', filled) + new string('░', empty) + "]";
        }
        
        private static (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            var map = worldMap.CurrentMap;
            return (map, worldMap);
        }
    }
}