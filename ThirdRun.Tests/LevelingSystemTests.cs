using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonogameRPG;
using MonogameRPG.Monsters;
using ThirdRun.Data;
using Xunit;

namespace ThirdRun.Tests
{
    public class LevelingSystemTests
    {
        private (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            return (worldMap.CurrentMap, worldMap);
        }

        private Character CreateTestCharacter(string name, CharacterClass characterClass, MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap)
        {
            return new Character(name, characterClass, 100, 10, map, worldMap)
            {
                Position = Vector2.Zero
            };
        }

        private Monster CreateTestMonster(int level, MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap)
        {
            var monsterType = new MonsterType($"Test Monster Level {level}", 50, 5, "test_texture", level);
            
            return new Monster(monsterType, map, worldMap, new Random(12345))
            {
                Position = Vector2.Zero
            };
        }

        [Fact]
        public void Character_StartsAtLevel1()
        {
            // Arrange & Act
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = CreateTestCharacter("TestChar", CharacterClass.Guerrier, map, worldMap);

            // Assert
            Assert.Equal(1, character.Level);
            Assert.Equal(0, character.Experience);
        }

        [Fact]
        public void Monster_GetExperienceValue_ProportionalToLevel()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();

            // Act & Assert
            var level1Monster = CreateTestMonster(1, map, worldMap);
            Assert.Equal(10, level1Monster.GetExperienceValue()); // 10 * 1 = 10

            var level3Monster = CreateTestMonster(3, map, worldMap);
            Assert.Equal(30, level3Monster.GetExperienceValue()); // 10 * 3 = 30

            var level5Monster = CreateTestMonster(5, map, worldMap);
            Assert.Equal(50, level5Monster.GetExperienceValue()); // 10 * 5 = 50
        }

        [Fact]
        public void Character_GetExperienceRequiredForNextLevel_CalculatesCorrectly()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = CreateTestCharacter("TestChar", CharacterClass.Guerrier, map, worldMap);

            // Assert
            Assert.Equal(100, character.GetExperienceRequiredForNextLevel()); // 100 * 1 = 100 for level 2
        }

        [Fact]
        public void Character_LevelsUp_WhenEnoughExperienceGained()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = CreateTestCharacter("TestChar", CharacterClass.Guerrier, map, worldMap);
            var initialHealth = character.MaxHealth;
            var initialAttack = character.AttackPower;

            // Create a monster that gives enough XP for level up (100 XP needed)
            var monster = CreateTestMonster(10, map, worldMap); // 10 * 10 = 100 XP

            // Act
            character.GainExperience(monster);

            // Assert
            Assert.Equal(2, character.Level);
            Assert.Equal(100, character.Experience);
            
            // Check characteristic increases for Guerrier class
            Assert.Equal(initialHealth + 8, character.MaxHealth); // +8 health for Guerrier
            Assert.Equal(initialAttack + 3, character.AttackPower); // +3 attack for Guerrier
        }

        [Fact]
        public void Character_LevelsUpMultipleTimes_WithEnoughExperience()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = CreateTestCharacter("TestChar", CharacterClass.Guerrier, map, worldMap);
            var initialHealth = character.MaxHealth;
            var initialAttack = character.AttackPower;

            // Create a monster that gives enough XP for 2 level ups 
            // Level 1->2 needs 100 XP, Level 2->3 needs 200 XP = 300 total
            var monster = CreateTestMonster(30, map, worldMap); // 10 * 30 = 300 XP

            // Act
            character.GainExperience(monster);

            // Assert
            Assert.Equal(3, character.Level);
            Assert.Equal(300, character.Experience);
            
            // Check characteristic increases for 2 levels (Guerrier class)
            Assert.Equal(initialHealth + 16, character.MaxHealth); // +8 health per level * 2
            Assert.Equal(initialAttack + 6, character.AttackPower); // +3 attack per level * 2
        }

        [Theory]
        [InlineData(CharacterClass.Guerrier, 8, 3)]
        [InlineData(CharacterClass.Chasseur, 6, 3)]
        [InlineData(CharacterClass.Prêtre, 5, 2)]
        [InlineData(CharacterClass.Mage, 4, 2)]
        public void Character_LevelUpIncreases_VaryByClass(CharacterClass characterClass, int expectedHealthIncrease, int expectedAttackIncrease)
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = CreateTestCharacter("TestChar", characterClass, map, worldMap);
            var initialHealth = character.MaxHealth;
            var initialAttack = character.AttackPower;

            // Create a monster that gives enough XP for level up
            var monster = CreateTestMonster(10, map, worldMap); // 100 XP

            // Act
            character.GainExperience(monster);

            // Assert
            Assert.Equal(2, character.Level);
            Assert.Equal(initialHealth + expectedHealthIncrease, character.MaxHealth);
            Assert.Equal(initialAttack + expectedAttackIncrease, character.AttackPower);
        }

        [Fact]
        public void PartyExperienceDistribution_SharesEquallyAmongLivingMembers()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            
            // Create a party of characters
            var warrior = CreateTestCharacter("Warrior", CharacterClass.Guerrier, map, worldMap);
            var mage = CreateTestCharacter("Mage", CharacterClass.Mage, map, worldMap);
            var priest = CreateTestCharacter("Priest", CharacterClass.Prêtre, map, worldMap);
            var hunter = CreateTestCharacter("Hunter", CharacterClass.Chasseur, map, worldMap);

            // Set up the party in the world map
            var characters = new List<Character> { warrior, mage, priest, hunter };
            worldMap.SetCharacters(characters);

            // Create a monster worth 100 XP
            var monster = CreateTestMonster(10, map, worldMap); // 10 * 10 = 100 XP

            // Act - warrior defeats the monster
            warrior.OnMonsterDefeated(monster);

            // Assert - all characters should get 25 XP each (100/4)
            Assert.Equal(25, warrior.Experience);
            Assert.Equal(25, mage.Experience);
            Assert.Equal(25, priest.Experience);
            Assert.Equal(25, hunter.Experience);
        }

        [Fact]
        public void PartyExperienceDistribution_ExcludesDeadMembers()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            
            var warrior = CreateTestCharacter("Warrior", CharacterClass.Guerrier, map, worldMap);
            var mage = CreateTestCharacter("Mage", CharacterClass.Mage, map, worldMap);
            var priest = CreateTestCharacter("Priest", CharacterClass.Prêtre, map, worldMap);
            
            // Kill the priest
            priest.CurrentHealth = 0;
            
            var characters = new List<Character> { warrior, mage, priest };
            worldMap.SetCharacters(characters);

            var monster = CreateTestMonster(6, map, worldMap); // 60 XP total

            // Act - warrior defeats the monster
            warrior.OnMonsterDefeated(monster);

            // Assert - only living characters (2) should get XP: 60/2 = 30 each
            Assert.Equal(30, warrior.Experience);
            Assert.Equal(30, mage.Experience);
            Assert.Equal(0, priest.Experience); // Dead character gets no XP
        }

        [Fact]
        public void Character_FullLeveling_RequiredExperienceScales()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = CreateTestCharacter("TestChar", CharacterClass.Guerrier, map, worldMap);

            // Act & Assert level progression
            // Level 1: needs 100 XP to reach level 2 (total 100 XP)
            Assert.Equal(100, character.GetExperienceRequiredForNextLevel());

            // Give exactly enough XP to level up
            var monster1 = CreateTestMonster(10, map, worldMap); // 100 XP
            character.GainExperience(monster1);
            Assert.Equal(2, character.Level);

            // Level 2: needs 200 XP to reach level 3 (total 300 XP)
            Assert.Equal(200, character.GetExperienceRequiredForNextLevel());

            // Level up again
            var monster2 = CreateTestMonster(20, map, worldMap); // 200 XP
            character.GainExperience(monster2);
            Assert.Equal(3, character.Level);
            Assert.Equal(300, character.Experience);
            
            // Level 3: needs 300 XP to reach level 4 (total 600 XP)
            Assert.Equal(300, character.GetExperienceRequiredForNextLevel());
        }
        
        [Fact]
        public void LevelingSystem_IntegrationDemo()
        {
            // This test demonstrates the full leveling system in action
            // It runs a simulation similar to actual gameplay
            
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            
            // Create a party of characters with different classes
            var warrior = CreateTestCharacter("Warrior", CharacterClass.Guerrier, map, worldMap);
            var mage = CreateTestCharacter("Mage", CharacterClass.Mage, map, worldMap);
            var priest = CreateTestCharacter("Priest", CharacterClass.Prêtre, map, worldMap);
            var hunter = CreateTestCharacter("Hunter", CharacterClass.Chasseur, map, worldMap);
            
            var party = new List<Character> { warrior, mage, priest, hunter };
            worldMap.SetCharacters(party);
            
            // Store initial stats
            var initialWarriorHealth = warrior.MaxHealth;
            var initialWarriorAttack = warrior.AttackPower;
            var initialMageHealth = mage.MaxHealth;
            var initialMageAttack = mage.AttackPower;
            
            // Act - Defeat exactly enough monsters to reach level 2
            // Each character needs 100 XP to reach level 2
            // We'll use level 10 monsters (100 XP each) split 4 ways = 25 XP per character per monster
            // So we need 4 monsters to give each character exactly 100 XP
            
            for (int i = 0; i < 4; i++)
            {
                var monster = CreateTestMonster(10, map, worldMap); // 100 XP each
                warrior.OnMonsterDefeated(monster);
            }
            
            // Assert - All characters should have leveled up to level 2
            Assert.All(party, character =>
            {
                Assert.Equal(2, character.Level);
                Assert.Equal(100, character.Experience); // Exactly 100 XP
            });
            
            // Check that different classes have different stat growth
            Assert.Equal(initialWarriorHealth + 8, warrior.MaxHealth); // Warrior gets +8 HP
            Assert.Equal(initialWarriorAttack + 3, warrior.AttackPower); // Warrior gets +3 ATK
            
            // Mage should have different stat growth
            Assert.Equal(initialMageHealth + 4, mage.MaxHealth); // Mage gets +4 HP
            Assert.Equal(initialMageAttack + 2, mage.AttackPower); // Mage gets +2 ATK
            
            // Verify experience requirements scale correctly
            Assert.Equal(200, warrior.GetExperienceRequiredForNextLevel()); // Level 2→3 needs 200 XP
            
            // Demonstrate that dead characters don't get XP
            priest.CurrentHealth = 0; // Kill the priest
            var finalMonster = CreateTestMonster(3, map, worldMap); // 30 XP
            warrior.OnMonsterDefeated(finalMonster);
            
            // Living characters should get 30/3 = 10 XP each (priest excluded)
            Assert.Equal(110, warrior.Experience); // 100 + 10 = 110
            Assert.Equal(110, mage.Experience);
            Assert.Equal(110, hunter.Experience);
            Assert.Equal(100, priest.Experience); // Priest still at 100 (got no XP from final monster)
        }
    }
}