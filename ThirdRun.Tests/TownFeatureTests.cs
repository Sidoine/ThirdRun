using Xunit;
using ThirdRun.UI;
using ThirdRun.Data.NPCs;
using MonogameRPG.Map;
using Microsoft.Xna.Framework;

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
        public void Map_ShouldSupportTownConversion()
        {
            // Arrange
            var map = new Map(Point.Zero);
            map.GenerateRandomMap();
            map.SpawnMonsters();
            
            // Initially should not be a town and have monsters
            Assert.False(map.IsTownZone);
            Assert.NotEmpty(map.GetMonsters());
            Assert.Empty(map.NPCs);
            
            // Act - Convert to town
            map.ConvertToTown();
            
            // Assert
            Assert.True(map.IsTownZone);
            Assert.Empty(map.GetMonsters()); // No monsters in town
            Assert.NotEmpty(map.NPCs); // Should have NPCs
            
            // Act - Convert back to hostile
            map.ConvertToHostile();
            
            // Assert
            Assert.False(map.IsTownZone);
            Assert.NotEmpty(map.GetMonsters()); // Monsters are back
            Assert.Empty(map.NPCs); // No NPCs in hostile zone
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
    }
}