using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonogameRPG;
using MonogameRPG.Monsters;
using ThirdRun.Data;

namespace ThirdRun.Tests
{
    public class LevelingIntegrationDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== ThirdRun Leveling System Integration Demo ===\n");
            
            // Create a test world
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            
            // Create a party of characters
            var warrior = new Character("Warrior", CharacterClass.Guerrier, 35, 12, worldMap.CurrentMap, worldMap) { Position = Vector2.Zero };
            var mage = new Character("Mage", CharacterClass.Mage, 25, 8, worldMap.CurrentMap, worldMap) { Position = Vector2.Zero };
            var priest = new Character("Priest", CharacterClass.Prêtre, 28, 6, worldMap.CurrentMap, worldMap) { Position = Vector2.Zero };
            var hunter = new Character("Hunter", CharacterClass.Chasseur, 30, 10, worldMap.CurrentMap, worldMap) { Position = Vector2.Zero };
            
            var party = new List<Character> { warrior, mage, priest, hunter };
            worldMap.SetCharacters(party);
            
            Console.WriteLine("Initial Party Stats:");
            PrintPartyStats(party);
            Console.WriteLine();
            
            // Create test monsters of various levels
            var level1Monster = CreateMonster("Goblin", 1, worldMap);
            var level3Monster = CreateMonster("Orc", 3, worldMap);
            var level5Monster = CreateMonster("Troll", 5, worldMap);
            
            // Simulate combat scenarios
            Console.WriteLine("=== Combat Scenario 1: Warrior defeats Level 1 Goblin ===");
            Console.WriteLine($"Monster gives {level1Monster.GetExperienceValue()} XP total, divided among {party.Count} party members = {level1Monster.GetExperienceValue() / party.Count} XP each");
            warrior.OnMonsterDefeated(level1Monster);
            PrintPartyStats(party);
            Console.WriteLine();
            
            Console.WriteLine("=== Combat Scenario 2: Mage defeats Level 3 Orc ===");
            Console.WriteLine($"Monster gives {level3Monster.GetExperienceValue()} XP total, divided among {party.Count} party members = {level3Monster.GetExperienceValue() / party.Count} XP each");
            mage.OnMonsterDefeated(level3Monster);
            PrintPartyStats(party);
            Console.WriteLine();
            
            Console.WriteLine("=== Combat Scenario 3: Hunter defeats Level 5 Troll ===");
            Console.WriteLine($"Monster gives {level5Monster.GetExperienceValue()} XP total, divided among {party.Count} party members = {level5Monster.GetExperienceValue() / party.Count} XP each");
            hunter.OnMonsterDefeated(level5Monster);
            PrintPartyStats(party);
            Console.WriteLine();
            
            // Let's kill enough monsters to trigger level-ups
            Console.WriteLine("=== Fighting More Monsters to Trigger Level-Ups ===");
            for (int i = 0; i < 4; i++)
            {
                var monster = CreateMonster($"Enemy {i + 1}", 5, worldMap);
                Console.WriteLine($"Defeating another Level 5 monster ({monster.GetExperienceValue()} XP)...");
                warrior.OnMonsterDefeated(monster);
            }
            
            Console.WriteLine("\nFinal Party Stats:");
            PrintPartyStats(party);
            
            Console.WriteLine("\n=== Level Requirements Explanation ===");
            Console.WriteLine($"Experience required for Warrior to reach next level: {warrior.GetExperienceRequiredForNextLevel()} XP");
            Console.WriteLine("Level progression XP requirements:");
            Console.WriteLine("  Level 1 → 2: 100 XP needed (10 level-1 monsters)");
            Console.WriteLine("  Level 2 → 3: 200 XP needed (6.7 level-3 monsters)"); 
            Console.WriteLine("  Level 3 → 4: 300 XP needed (6 level-5 monsters)");
            Console.WriteLine("  Level 4 → 5: 400 XP needed (8 level-5 monsters)");
            
            Console.WriteLine("\n=== Class-Specific Stat Growth ===");
            Console.WriteLine("Per-level stat increases by class:");
            Console.WriteLine("  Guerrier (Warrior): +8 HP, +3 ATK");
            Console.WriteLine("  Chasseur (Hunter): +6 HP, +3 ATK");
            Console.WriteLine("  Prêtre (Priest): +5 HP, +2 ATK");
            Console.WriteLine("  Mage: +4 HP, +2 ATK");
        }
        
        private static Monster CreateMonster(string name, int level, MonogameRPG.Map.WorldMap worldMap)
        {
            var monsterType = new MonsterType(name, 50, 10, "test_texture", level);
            return new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345)) { Position = Vector2.Zero };
        }
        
        private static void PrintPartyStats(List<Character> party)
        {
            foreach (var character in party)
            {
                Console.WriteLine($"  {character.Name} ({character.Class}): Level {character.Level}, XP {character.Experience}, HP {character.MaxHealth}, ATK {character.AttackPower}");
            }
        }
    }
}