using Xunit;
using ThirdRun.UI;
using ThirdRun.Data.NPCs;
using MonogameRPG.Map;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ThirdRun.Tests
{
    public class TownFeatureTests
    {
        [Fact]
        public void UIManager_ShouldHaveTownState()
        {
            // Arrange & Act
            var state = new UiManager.State();
            
            // Assert
            Assert.False(state.IsInTown); // Default should be false
            
            // Test we can set it
            state.IsInTown = true;
            Assert.True(state.IsInTown);
        }

        [Fact]
        public void NPC_ShouldHavePropertiesAndGreeting()
        {
            // Arrange
            var npc = new NPC("Marcus", NPCType.Merchant, Vector2.Zero);
            
            // Assert
            Assert.Equal("Marcus", npc.Name);
            Assert.Equal(NPCType.Merchant, npc.Type);
            Assert.Contains("Marcus", npc.GetGreeting());
            Assert.Contains("merchant", npc.GetGreeting());
        }

        [Fact]
        public void Map_ShouldSupportTownZone()
        {
            // Arrange
            var map = new Map(Point.Zero);
            map.GenerateRandomMap();
            map.SpawnMonsters();
            
            // Initially should not be a town and have monsters
            Assert.False(map.IsTownZone);
            Assert.NotEmpty(map.GetMonsters());
            Assert.Empty(map.NPCs);
            
            // Act - Create a town map
            var townMap = new Map(new Point(-999, -999));
            townMap.GenerateRandomMap();
            townMap.IsTownZone = true;
            townMap.SpawnNPCs();
            
            // Assert
            Assert.True(townMap.IsTownZone);
            Assert.Empty(townMap.GetMonsters()); // Town map starts with no monsters
            Assert.NotEmpty(townMap.NPCs); // Should have NPCs
        }

        [Fact]
        public void WorldMap_ShouldToggleTownMode()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            // Initially should not be in town
            Assert.False(worldMap.IsInTown);
            
            // Act
            worldMap.ToggleTownMode();
            
            // Assert
            Assert.True(worldMap.IsInTown);
            Assert.True(worldMap.CurrentMap.IsTownZone);
            
            // Act - Toggle back
            worldMap.ToggleTownMode();
            
            // Assert
            Assert.False(worldMap.IsInTown);
            Assert.False(worldMap.CurrentMap.IsTownZone);
        }

        [Fact]
        public void Map_TeleportCharacters_ShouldUpdateCharacterPositions()
        {
            // Arrange
            var map = new Map(Point.Zero);
            map.GenerateRandomMap();
            
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var characters = new List<Character>
            {
                new Character("Test1", CharacterClass.Guerrier, 100, 10, worldMap),
                new Character("Test2", CharacterClass.Mage, 80, 12, worldMap)
            };
            
            // Set initial positions that are likely outside the map bounds
            var originalPosition1 = new Vector2(10000, 10000);
            var originalPosition2 = new Vector2(20000, 20000);
            characters[0].Position = originalPosition1;
            characters[1].Position = originalPosition2;
            
            // Act - Teleport characters to the map
            map.TeleportCharacters(characters);
            
            // Assert - Positions should be changed to valid locations on the map
            Assert.NotEqual(originalPosition1, characters[0].Position);
            Assert.NotEqual(originalPosition2, characters[1].Position);
            
            // Positions should be within the map bounds
            var mapWidth = Map.GridWidth * Map.TileWidth;
            var mapHeight = Map.GridHeight * Map.TileHeight;
            
            Assert.True(characters[0].Position.X >= map.Position.X);
            Assert.True(characters[0].Position.X <= map.Position.X + mapWidth);
            Assert.True(characters[0].Position.Y >= map.Position.Y);
            Assert.True(characters[0].Position.Y <= map.Position.Y + mapHeight);
            
            Assert.True(characters[1].Position.X >= map.Position.X);
            Assert.True(characters[1].Position.X <= map.Position.X + mapWidth);
            Assert.True(characters[1].Position.Y >= map.Position.Y);
            Assert.True(characters[1].Position.Y <= map.Position.Y + mapHeight);
            
            // Characters should be added to the map
            Assert.Equal(2, map.Characters.Count());
            Assert.Contains(characters[0], map.Characters);
            Assert.Contains(characters[1], map.Characters);
        }
    }
}