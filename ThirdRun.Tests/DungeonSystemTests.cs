using System;
using System.Linq;
using Xunit;
using ThirdRun.Data.Dungeons;
using MonogameRPG.Map;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests
{
    public class DungeonSystemTests
    {
        [Fact]
        public void Character_Level_CalculatedCorrectlyFromExperience()
        {
            var random = new Random(12345);
            var worldMap = new WorldMap(random);
            worldMap.Initialize();
            
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            
            // Level 1: 0-199 experience
            character.GainExperience(new MonogameRPG.Monsters.Monster(new MonogameRPG.Monsters.MonsterType("TestMonster", 10, 5, "test", 1), worldMap.CurrentMap, worldMap, random));
            Assert.Equal(1, character.Level);
            
            // Add more experience to reach level 2 (200+ experience)
            for (int i = 0; i < 20; i++)
            {
                character.GainExperience(new MonogameRPG.Monsters.Monster(new MonogameRPG.Monsters.MonsterType("TestMonster", 10, 5, "test", 1), worldMap.CurrentMap, worldMap, random));
            }
            Assert.True(character.Level >= 2);
        }

        [Fact]
        public void DungeonRepository_HasCorrectLevelRanges()
        {
            var dungeons = DungeonRepository.GetAllDungeons();
            
            Assert.True(dungeons.Count >= 5);
            
            // Check that level ranges don't overlap
            var sortedDungeons = dungeons.OrderBy(d => d.MinLevel).ToList();
            for (int i = 0; i < sortedDungeons.Count - 1; i++)
            {
                Assert.True(sortedDungeons[i].MaxLevel < sortedDungeons[i + 1].MinLevel, 
                    $"Dungeon level ranges overlap: {sortedDungeons[i].Name} (max {sortedDungeons[i].MaxLevel}) and {sortedDungeons[i + 1].Name} (min {sortedDungeons[i + 1].MinLevel})");
            }
        }

        [Fact]
        public void DungeonRepository_FindsDungeonForLevel()
        {
            var level1Dungeon = DungeonRepository.GetDungeonForLevel(1);
            Assert.NotNull(level1Dungeon);
            Assert.True(level1Dungeon.IsAppropriateForLevel(1));

            var level5Dungeon = DungeonRepository.GetDungeonForLevel(5);
            Assert.NotNull(level5Dungeon);
            Assert.True(level5Dungeon.IsAppropriateForLevel(5));

            var level10Dungeon = DungeonRepository.GetDungeonForLevel(10);
            Assert.NotNull(level10Dungeon);
            Assert.True(level10Dungeon.IsAppropriateForLevel(10));
        }

        [Fact]
        public void Dungeon_HasBossOnFinalMap()
        {
            var dungeons = DungeonRepository.GetAllDungeons();
            
            foreach (var dungeon in dungeons)
            {
                Assert.True(dungeon.Maps.Count > 0, $"Dungeon {dungeon.Name} should have at least one map");
                
                var finalMap = dungeon.Maps.Last();
                Assert.True(finalMap.HasBoss, $"Final map of dungeon {dungeon.Name} should have a boss");
            }
        }

        [Fact]
        public void WorldMap_EnterDungeon_CreatesAppropriateLevel()
        {
            var random = new Random(12345);
            var worldMap = new WorldMap(random);
            worldMap.Initialize();
            
            // Create characters at different levels
            var characters = new[]
            {
                new Character("Char1", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap),
                new Character("Char2", CharacterClass.Mage, 80, 8, worldMap.CurrentMap, worldMap),
                new Character("Char3", CharacterClass.Prêtre, 90, 9, worldMap.CurrentMap, worldMap),
                new Character("Char4", CharacterClass.Chasseur, 85, 7, worldMap.CurrentMap, worldMap)
            };
            
            // Give different experience levels
            for (int i = 0; i < 10; i++)
            {
                characters[0].GainExperience(new MonogameRPG.Monsters.Monster(new MonogameRPG.Monsters.MonsterType("TestMonster", 10, 5, "test", 1), worldMap.CurrentMap, worldMap, random));
            }
            
            worldMap.SetCharacters(characters.ToList());
            
            Assert.False(worldMap.IsInDungeon);
            
            worldMap.EnterDungeon();
            
            Assert.True(worldMap.IsInDungeon);
            Assert.NotNull(worldMap.CurrentDungeon);
        }

        [Fact]
        public void WorldMap_ExitDungeon_ReturnsToNormalMap()
        {
            var random = new Random(12345);
            var worldMap = new WorldMap(random);
            worldMap.Initialize();
            
            var characters = new[]
            {
                new Character("Char1", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap)
            };
            
            worldMap.SetCharacters(characters.ToList());
            
            var originalPosition = worldMap.CurrentMapPosition;
            
            worldMap.EnterDungeon();
            Assert.True(worldMap.IsInDungeon);
            
            worldMap.ExitDungeon();
            Assert.False(worldMap.IsInDungeon);
            Assert.Null(worldMap.CurrentDungeon);
            Assert.Equal(originalPosition, worldMap.CurrentMapPosition);
        }

        [Fact]
        public void WorldMap_CannotEnterDungeonFromTown()
        {
            var random = new Random(12345);
            var worldMap = new WorldMap(random);
            worldMap.Initialize();
            
            var characters = new[]
            {
                new Character("Char1", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap)
            };
            
            worldMap.SetCharacters(characters.ToList());
            
            // Enter town mode
            worldMap.ToggleTownMode();
            Assert.True(worldMap.IsInTown);
            
            // Try to enter dungeon - should fail
            worldMap.EnterDungeon();
            Assert.False(worldMap.IsInDungeon);
        }
    }
}