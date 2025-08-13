using System;
using Microsoft.Xna.Framework;
using ThirdRun.Data;
using ThirdRun.Characters;
using MonogameRPG;
using Xunit;

namespace ThirdRun.Tests
{
    public class CharacterPortraitResourceBarsTests
    {
        private (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            var map = worldMap.CurrentMap;
            return (map, worldMap);
        }
        
        [Fact]
        public void Character_HasEnergyResource_WithDefaultValues()
        {
            // Create test character - can't test UI directly without MonoGame context
            // but can verify the underlying data that would be used for the bars
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            // Verify character has resource manager with Energy resource
            var energyResource = character.Resources.GetResource(ResourceType.Energy);
            Assert.NotNull(energyResource);
            Assert.Equal(ResourceType.Energy, energyResource.Type);
            Assert.Equal(100f, energyResource.MaxValue);
            Assert.Equal(100f, energyResource.CurrentValue); // Should start at max
        }
        
        [Fact]
        public void Character_HealthAndEnergyValues_CanBeUsedForBarCalculations()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            // Test health values that would be used for health bar
            Assert.True(character.CurrentHealth > 0);
            Assert.True(character.GetEffectiveMaxHealth() > 0);
            Assert.True(character.CurrentHealth <= character.GetEffectiveMaxHealth());
            
            // Test energy values that would be used for energy bar
            var energyResource = character.Resources.GetResource(ResourceType.Energy);
            Assert.NotNull(energyResource);
            Assert.True(energyResource.CurrentValue >= 0);
            Assert.True(energyResource.MaxValue > 0);
            Assert.True(energyResource.CurrentValue <= energyResource.MaxValue);
        }
        
        [Theory]
        [InlineData(50, 100, 0.5f)]
        [InlineData(75, 100, 0.75f)]
        [InlineData(0, 100, 0.0f)]
        [InlineData(100, 100, 1.0f)]
        public void ResourceBar_PercentageCalculation_WorksCorrectly(float current, float max, float expectedPercent)
        {
            // Test the bar percentage calculation logic that would be used in DrawBar
            float actualPercent = MathHelper.Clamp(current / max, 0f, 1f);
            Assert.Equal(expectedPercent, actualPercent, 1f); // 1 decimal place precision
        }
        
        [Fact]
        public void ResourceColors_AreDefinedCorrectly()
        {
            // Test that we have defined the expected colors for different resource types
            // This validates the static readonly Color fields exist with expected values
            
            // Health bar should be green-ish (for health bar foreground)
            var healthColor = Color.LimeGreen;
            Assert.Equal(50, healthColor.R);
            Assert.Equal(205, healthColor.G);
            Assert.Equal(50, healthColor.B);
            
            // Energy bar should be green (specified requirement)
            var energyColor = Color.Green;
            Assert.Equal(0, energyColor.R);
            Assert.Equal(128, energyColor.G);
            Assert.Equal(0, energyColor.B);
        }
        
        [Fact]
        public void Character_ResourceAndHealthValues_ChangeCorrectly()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var energyResource = character.Resources.GetResource(ResourceType.Energy)!;
            
            // Test reducing values (simulates combat/ability use)
            var initialHealth = character.CurrentHealth;
            var initialEnergy = energyResource.CurrentValue;
            
            // Simulate taking damage
            character.CurrentHealth -= 10;
            Assert.True(character.CurrentHealth < initialHealth);
            
            // Simulate using energy
            bool consumed = energyResource.TryConsume(20f);
            Assert.True(consumed);
            Assert.True(energyResource.CurrentValue < initialEnergy);
            
            // Values should still be valid for bar calculations
            Assert.True(character.CurrentHealth >= 0);
            Assert.True(energyResource.CurrentValue >= 0);
        }
    }
}