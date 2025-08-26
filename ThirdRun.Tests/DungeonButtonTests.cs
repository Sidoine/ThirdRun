using System;
using Xunit;
using ThirdRun.Data;
using MonogameRPG.Map;
using Microsoft.Xna.Framework;
using System.Linq;

namespace ThirdRun.Tests
{
    public class DungeonButtonTests
    {
        [Fact]
        public void UIManager_HasDungeonState()
        {
            // Verify the UIManager.State class has the new IsInDungeon property
            var state = new ThirdRun.UI.UiManager.State();
            
            Assert.False(state.IsInDungeon);
            state.IsInDungeon = true;
            Assert.True(state.IsInDungeon);
        }

        [Fact] 
        public void EnterDungeon_RequiresValidCharacters()
        {
            var random = new Random(12345);
            var worldMap = new WorldMap(random);
            worldMap.Initialize();
            
            // Test with no characters
            Assert.False(worldMap.IsInDungeon);
            worldMap.EnterDungeon(); // Should not crash with no characters
            
            // Test with characters
            var characters = new[]
            {
                new Character("TestChar", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap)
            };
            
            worldMap.SetCharacters(characters.ToList());
            worldMap.EnterDungeon();
            
            Assert.True(worldMap.IsInDungeon);
            Assert.NotNull(worldMap.CurrentDungeon);
        }

        [Fact]
        public void DungeonSystem_Integration_WorksCorrectly()
        {
            var random = new Random(12345);
            var worldMap = new WorldMap(random);
            worldMap.Initialize();
            
            var characters = new[]
            {
                new Character("Warrior", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap),
                new Character("Mage", CharacterClass.Mage, 80, 8, worldMap.CurrentMap, worldMap)
            };
            
            // Give some experience to increase level
            for (int i = 0; i < 15; i++)
            {
                characters[0].GainExperience(new MonogameRPG.Monsters.Monster(
                    new MonogameRPG.Monsters.MonsterType("TestMonster", 10, 5, "test", 2), 
                    worldMap.CurrentMap, worldMap, random));
            }
            
            worldMap.SetCharacters(characters.ToList());
            
            // Test full dungeon flow
            Assert.False(worldMap.IsInDungeon);
            
            // Enter dungeon
            worldMap.EnterDungeon();
            Assert.True(worldMap.IsInDungeon);
            Assert.NotNull(worldMap.CurrentDungeon);
            
            // Verify appropriate dungeon was selected
            var meanLevel = (characters[0].Level + characters[1].Level) / 2;
            Assert.True(worldMap.CurrentDungeon.IsAppropriateForLevel(meanLevel));
            
            // Exit dungeon
            worldMap.ExitDungeon();
            Assert.False(worldMap.IsInDungeon);
            Assert.Null(worldMap.CurrentDungeon);
        }

        [Fact]
        public void ButtonsPanel_DungeonButton_Integration()
        {
            // Verify that the ButtonsPanel class exists and can be instantiated
            // This tests the structural integrity of our button changes
            var random = new Random(12345);
            var worldMap = new WorldMap(random);
            worldMap.Initialize();
            var gameState = new GameState
            {
                Player = new ThirdRun.Data.Player(worldMap, random),
                WorldMap = worldMap
            };
            
            // We can't actually instantiate the ButtonsPanel without MonoGame setup
            // but we can verify the types and methods we need exist
            Assert.NotNull(gameState.WorldMap);
            Assert.NotNull(typeof(ThirdRun.UI.Panels.ButtonsPanel));
            
            // Verify the WorldMap has the new dungeon methods
            var worldMapType = typeof(WorldMap);
            Assert.NotNull(worldMapType.GetMethod("EnterDungeon"));
            Assert.NotNull(worldMapType.GetMethod("ExitDungeon"));
            Assert.NotNull(worldMapType.GetProperty("IsInDungeon"));
            Assert.NotNull(worldMapType.GetProperty("CurrentDungeon"));
        }
    }
}