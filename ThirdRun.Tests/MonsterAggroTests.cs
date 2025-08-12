using System;
using MonogameRPG.Monsters;
using MonogameRPG.Map;
using MonogameRPG;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ThirdRun.Tests
{
    public class MonsterAggroTests
    {
        [Fact]
        public void Monster_StartsInSleepingState()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            var monsterType = new MonsterType("Test Monster", 50, 10, "test");
            var monster = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));

            // Assert
            Assert.Equal(MonsterState.Sleeping, monster.State);
        }

        [Fact]
        public void Monster_HasDefaultAggroRadius()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            var monsterType = new MonsterType("Test Monster", 50, 10, "test");
            var monster = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));

            // Assert
            Assert.True(monster.AggroRadius > 0);
            Assert.Equal(100f, monster.AggroRadius); // Default value
        }

        [Fact]
        public void Monster_WakesUpWhenCharacterEntersAggroRadius()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            var character = new Character("TestChar", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            character.Position = new Vector2(0, 0);
            
            var monsterType = new MonsterType("Test Monster", 50, 10, "test");
            var monster = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));
            monster.Position = new Vector2(50, 0); // Within default aggro radius (100)            // Add units to the map
            worldMap.CurrentMap.AddUnit(character);
            worldMap.CurrentMap.AddUnit(monster);

            // Act
            monster.Update(); // This should detect character and wake up

            // Assert
            Assert.Equal(MonsterState.Chasing, monster.State);
            Assert.Equal(character, monster.Target);
        }

        [Fact]
        public void Monster_DoesNotWakeUpWhenCharacterOutsideAggroRadius()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            var character = new Character("TestChar", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            character.Position = new Vector2(0, 0);
            
            var monsterType = new MonsterType("Test Monster", 50, 10, "test");
            var monster = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));
            monster.Position = new Vector2(200, 0); // Outside aggro radius (100)            // Add units to the map
            worldMap.CurrentMap.AddUnit(character);
            worldMap.CurrentMap.AddUnit(monster);

            // Act
            monster.Update();

            // Assert
            Assert.Equal(MonsterState.Sleeping, monster.State);
            Assert.Null(monster.Target);
        }

        [Fact]
        public void Monster_DoesNotWakeUpForDeadCharacters()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            var character = new Character("TestChar", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            character.Position = new Vector2(0, 0);
            character.CurrentHealth = 0; // Dead character
            
            var monsterType = new MonsterType("Test Monster", 50, 10, "test");
            var monster = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));
            monster.Position = new Vector2(50, 0); // Within aggro radius            // Add units to the map
            worldMap.CurrentMap.AddUnit(character);
            worldMap.CurrentMap.AddUnit(monster);

            // Act
            monster.Update();

            // Assert
            Assert.Equal(MonsterState.Sleeping, monster.State);
            Assert.Null(monster.Target);
        }

        [Fact]
        public void Monster_WakesUpNearbyMonstersWhenTriggered()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            var character = new Character("TestChar", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            character.Position = new Vector2(0, 0);
            
            var monsterType = new MonsterType("Test Monster", 50, 10, "test");
            
            var monster1 = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));
            monster1.Position = new Vector2(50, 0); // Within aggro radius
            
            var monster2 = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));
            monster2.Position = new Vector2(100, 0); // Within wake up radius of monster1
            
            var monster3 = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));
            monster3.Position = new Vector2(300, 0); // Too far away
            
            // Add units to the map
            worldMap.CurrentMap.AddUnit(character);
            worldMap.CurrentMap.AddUnit(monster1);
            worldMap.CurrentMap.AddUnit(monster2);
            worldMap.CurrentMap.AddUnit(monster3);

            // Act
            monster1.Update(); // Should wake up and trigger nearby monsters

            // Assert
            Assert.Equal(MonsterState.Chasing, monster1.State);
            Assert.Equal(MonsterState.Chasing, monster2.State); // Should be woken up
            Assert.Equal(MonsterState.Sleeping, monster3.State); // Should remain asleep
        }

        [Fact]
        public void Monster_FindsNewTargetWhenCurrentTargetDies()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            var character1 = new Character("Char1", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            character1.Position = new Vector2(0, 0);
            
            var character2 = new Character("Char2", CharacterClass.Mage, 80, 8, worldMap.CurrentMap, worldMap);
            character2.Position = new Vector2(50, 50);
            
            var monsterType = new MonsterType("Test Monster", 50, 10, "test");
            var monster = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));
            monster.Position = new Vector2(25, 25);            // Add units to the map
            worldMap.CurrentMap.AddUnit(character1);
            worldMap.CurrentMap.AddUnit(character2);
            worldMap.CurrentMap.AddUnit(monster);

            // Manually wake up monster with first character as target
            monster.WakeUp(character1);
            Assert.Equal(character1, monster.Target);

            // Act - kill the target
            character1.CurrentHealth = 0;
            monster.Update(); // Should find new target

            // Assert
            Assert.Equal(character2, monster.Target);
            Assert.Equal(MonsterState.Chasing, monster.State);
        }

        [Fact]
        public void Monster_GoesBackToSleepWhenNoTargetsAvailable()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            var character = new Character("TestChar", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            character.Position = new Vector2(0, 0);
            
            var monsterType = new MonsterType("Test Monster", 50, 10, "test");
            var monster = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));
            monster.Position = new Vector2(50, 0);            // Add units to the map
            worldMap.CurrentMap.AddUnit(character);
            worldMap.CurrentMap.AddUnit(monster);

            // Wake up monster
            monster.WakeUp(character);
            Assert.Equal(MonsterState.Chasing, monster.State);

            // Act - kill the character and move them far away
            character.CurrentHealth = 0;
            character.Position = new Vector2(1000, 1000);
            monster.Update();

            // Assert
            Assert.Equal(MonsterState.Sleeping, monster.State);
            Assert.Null(monster.Target);
        }

        [Fact]
        public void Monster_AttacksWhenInRange()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            var character = new Character("TestChar", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            character.Position = new Vector2(0, 0);
            
            var monsterType = new MonsterType("Test Monster", 50, 10, "test");
            var monster = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));
            monster.Position = new Vector2(25, 0); // Within melee attack range (32)            monster.UpdateGameTime(0f); // Set game time for cooldown checks
            
            // Add units to the map
            worldMap.CurrentMap.AddUnit(character);
            worldMap.CurrentMap.AddUnit(monster);

            // Wake up monster
            monster.WakeUp(character);
            
            var initialHealth = character.CurrentHealth;
            
            // Act - update to trigger attack
            monster.Update();

            // Assert - monster should still be chasing but character should take damage
            Assert.Equal(MonsterState.Chasing, monster.State);
            Assert.True(character.CurrentHealth < initialHealth, "Character should have taken damage from monster attack");
        }

        [Fact]
        public void DeadMonster_DoesNotUpdate()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            var character = new Character("TestChar", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            character.Position = new Vector2(0, 0);
            
            var monsterType = new MonsterType("Test Monster", 50, 10, "test");
            var monster = new Monster(monsterType, worldMap.CurrentMap, worldMap, new Random(12345));
            monster.Position = new Vector2(50, 0);
            monster.CurrentHealth = 0; // Dead monster            // Add units to the map
            worldMap.CurrentMap.AddUnit(character);
            worldMap.CurrentMap.AddUnit(monster);

            // Act
            monster.Update();

            // Assert
            Assert.Equal(MonsterState.Sleeping, monster.State); // Should remain in initial state
            Assert.Null(monster.Target);
        }
    }
}