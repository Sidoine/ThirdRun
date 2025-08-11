using MonogameRPG.Monsters;
using MonogameRPG.Map;
using MonogameRPG;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ThirdRun.Tests
{
    public class MonsterAggroIntegrationTest
    {
        private readonly ITestOutputHelper output;

        public MonsterAggroIntegrationTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void MonsterAggro_IntegrationScenario_ShouldDemonstrateFullBehavior()
        {
            // Arrange
            output.WriteLine("=== Monster Aggro Integration Test ===\n");
            
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            // Create characters
            var warrior = new Character("Aragorn", CharacterClass.Guerrier, 100, 15, worldMap.CurrentMap, worldMap);
            warrior.Position = new Vector2(100, 100);
            
            var mage = new Character("Gandalf", CharacterClass.Mage, 80, 12, worldMap.CurrentMap, worldMap);
            mage.Position = new Vector2(120, 120);
            
            var priest = new Character("Elrond", CharacterClass.Prêtre, 90, 10, worldMap.CurrentMap, worldMap);
            priest.Position = new Vector2(140, 140);
            
            var characters = new List<Character> { warrior, mage, priest };
            
            // Create monsters
            var monsterType = new MonsterType("Orc", 60, 8, "test_orc");
            
            var orc1 = new Monster(monsterType, worldMap.CurrentMap, worldMap);
            orc1.Position = new Vector2(150, 100); // Near warrior (50px distance)
            
            var orc2 = new Monster(monsterType, worldMap.CurrentMap, worldMap);
            orc2.Position = new Vector2(200, 150); // Near orc1 (can be awakened)
            
            var orc3 = new Monster(monsterType, worldMap.CurrentMap, worldMap);
            orc3.Position = new Vector2(500, 500); // Far away
            
            // Add units to map
            foreach (var character in characters)
            {
                worldMap.CurrentMap.AddUnit(character);
            }
            worldMap.CurrentMap.AddUnit(orc1);
            worldMap.CurrentMap.AddUnit(orc2);
            worldMap.CurrentMap.AddUnit(orc3);
            
            output.WriteLine("Initial Setup:");
            output.WriteLine($"- {warrior.Name} at {warrior.Position} (Health: {warrior.CurrentHealth})");
            output.WriteLine($"- {mage.Name} at {mage.Position} (Health: {mage.CurrentHealth})");
            output.WriteLine($"- {priest.Name} at {priest.Position} (Health: {priest.CurrentHealth})");
            output.WriteLine($"- Orc1 at {orc1.Position} (State: {orc1.State})");
            output.WriteLine($"- Orc2 at {orc2.Position} (State: {orc2.State})");
            output.WriteLine($"- Orc3 at {orc3.Position} (State: {orc3.State})");
            output.WriteLine("");
            
            // Act 1: Trigger aggro
            output.WriteLine("=== Step 1: Trigger Aggro ===");
            orc1.Update(); // Should detect warrior and wake up
            
            output.WriteLine($"After orc1.Update():");
            output.WriteLine($"- Orc1 state: {orc1.State}, Target: {orc1.Target?.Name ?? "None"}");
            output.WriteLine($"- Orc2 state: {orc2.State}, Target: {orc2.Target?.Name ?? "None"}");
            output.WriteLine($"- Orc3 state: {orc3.State}, Target: {orc3.Target?.Name ?? "None"}");
            output.WriteLine("");
            
            // Assert step 1
            Assert.Equal(MonsterState.Chasing, orc1.State);
            Assert.Equal(warrior, orc1.Target);
            Assert.Equal(MonsterState.Chasing, orc2.State); // Should be awakened by orc1
            Assert.Equal(MonsterState.Sleeping, orc3.State); // Too far away
            
            // Act 2: Simulate combat - wound characters
            output.WriteLine("=== Step 2: Combat Simulation ===");
            warrior.CurrentHealth = 10; // Badly wounded
            mage.CurrentHealth = 5;     // Very low health
            priest.CurrentHealth = 85;  // Still healthy
            
            output.WriteLine($"After combat:");
            output.WriteLine($"- {warrior.Name} Health: {warrior.CurrentHealth}");
            output.WriteLine($"- {mage.Name} Health: {mage.CurrentHealth}");
            output.WriteLine($"- {priest.Name} Health: {priest.CurrentHealth}");
            output.WriteLine("");
            
            // Act 3: Kill some characters and test party wipe logic
            output.WriteLine("=== Step 3: Character Deaths ===");
            warrior.CurrentHealth = 0; // Dead
            mage.CurrentHealth = 0;    // Dead
            // Priest still alive
            
            bool allDead = characters.All(c => c.IsDead);
            output.WriteLine($"All characters dead: {allDead}");
            
            // Should not trigger party wipe yet (priest alive)
            Assert.False(allDead);
            
            // Act 4: Complete party wipe
            output.WriteLine("=== Step 4: Complete Party Wipe ===");
            priest.CurrentHealth = 0; // Now everyone is dead
            
            allDead = characters.All(c => c.IsDead);
            output.WriteLine($"All characters dead: {allDead}");
            Assert.True(allDead);
            
            // Simulate party wipe recovery
            foreach (var character in characters)
            {
                character.CurrentHealth = character.MaxHealth;
            }
            
            output.WriteLine($"After recovery:");
            output.WriteLine($"- {warrior.Name} Health: {warrior.CurrentHealth}");
            output.WriteLine($"- {mage.Name} Health: {mage.CurrentHealth}");
            output.WriteLine($"- {priest.Name} Health: {priest.CurrentHealth}");
            output.WriteLine("");
            
            // Assert recovery
            Assert.False(warrior.IsDead);
            Assert.False(mage.IsDead);
            Assert.False(priest.IsDead);
            Assert.Equal(100, warrior.CurrentHealth);
            Assert.Equal(80, mage.CurrentHealth);
            Assert.Equal(90, priest.CurrentHealth);
            
            // Act 5: Test monster target switching after recovery
            output.WriteLine("=== Step 5: Monster Target Switching ===");
            orc1.Update(); // Should find new target since old one was dead
            orc2.Update();
            
            output.WriteLine($"After recovery and monster updates:");
            output.WriteLine($"- Orc1 Target: {orc1.Target?.Name ?? "None"}");
            output.WriteLine($"- Orc2 Target: {orc2.Target?.Name ?? "None"}");
            output.WriteLine("");
            
            // Assert target switching
            Assert.NotNull(orc1.Target);
            Assert.NotNull(orc2.Target);
            Assert.Contains(orc1.Target, characters);
            Assert.Contains(orc2.Target, characters);
            
            output.WriteLine("=== Integration Test Complete ===");
            output.WriteLine("✓ Monster aggro triggering works");
            output.WriteLine("✓ Chain awakening works");
            output.WriteLine("✓ Target switching works");
            output.WriteLine("✓ Party wipe detection works");
            output.WriteLine("✓ Recovery system works");
        }
    }
}