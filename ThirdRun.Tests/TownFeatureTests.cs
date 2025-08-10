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
    }
}