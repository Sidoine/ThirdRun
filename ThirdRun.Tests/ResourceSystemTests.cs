using System;
using MonogameRPG;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;
using Xunit;

namespace ThirdRun.Tests
{
    public class ResourceSystemTests
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
        public void Resource_InitializesCorrectly()
        {
            // Arrange & Act
            var energy = new Resource(ResourceType.Energy, 100f, 10f);

            // Assert
            Assert.Equal(ResourceType.Energy, energy.Type);
            Assert.Equal(100f, energy.MaxValue);
            Assert.Equal(100f, energy.CurrentValue); // Starts at max by default
            Assert.Equal(10f, energy.ReplenishRate);
        }

        [Fact]
        public void Resource_ReplenishesOverTime()
        {
            // Arrange
            var energy = new Resource(ResourceType.Energy, 100f, 10f, 50f); // Start with 50 energy

            // Act - 2 seconds elapsed, should replenish 20 energy (10 per second)
            energy.Replenish(2f);

            // Assert
            Assert.Equal(70f, energy.CurrentValue);
        }

        [Fact]
        public void Resource_ReplenishDoesNotExceedMax()
        {
            // Arrange
            var energy = new Resource(ResourceType.Energy, 100f, 10f, 95f); // Start with 95 energy

            // Act - 2 seconds elapsed, would add 20 but max is 100
            energy.Replenish(2f);

            // Assert
            Assert.Equal(100f, energy.CurrentValue); // Capped at max
        }

        [Fact]
        public void Resource_TryConsume_Success()
        {
            // Arrange
            var energy = new Resource(ResourceType.Energy, 100f, 10f, 50f); // Start with 50 energy

            // Act
            var result = energy.TryConsume(30f);

            // Assert
            Assert.True(result);
            Assert.Equal(20f, energy.CurrentValue);
        }

        [Fact]
        public void Resource_TryConsume_InsufficientResource()
        {
            // Arrange
            var energy = new Resource(ResourceType.Energy, 100f, 10f, 20f); // Start with 20 energy

            // Act
            var result = energy.TryConsume(30f); // Try to consume more than available

            // Assert
            Assert.False(result);
            Assert.Equal(20f, energy.CurrentValue); // Should remain unchanged
        }

        [Fact]
        public void Resource_TryGenerate_Success()
        {
            // Arrange
            var energy = new Resource(ResourceType.Energy, 100f, 10f, 70f); // Start with 70 energy

            // Act
            var result = energy.TryGenerate(20f);

            // Assert
            Assert.True(result);
            Assert.Equal(90f, energy.CurrentValue);
        }

        [Fact]
        public void Resource_TryGenerate_WouldExceedMax()
        {
            // Arrange
            var energy = new Resource(ResourceType.Energy, 100f, 10f, 90f); // Start with 90 energy

            // Act
            var result = energy.TryGenerate(20f); // Would exceed max of 100

            // Assert
            Assert.False(result);
            Assert.Equal(90f, energy.CurrentValue); // Should remain unchanged
        }

        [Fact]
        public void ResourceManager_InitializesWithDefaultEnergy()
        {
            // Arrange & Act
            var manager = new ResourceManager();

            // Assert
            var energy = manager.GetResource(ResourceType.Energy);
            Assert.NotNull(energy);
            Assert.Equal(100f, manager.GetCurrentValue(ResourceType.Energy));
            Assert.Equal(100f, manager.GetMaxValue(ResourceType.Energy));
        }

        [Fact]
        public void ResourceManager_UpdatesResourcesOverTime()
        {
            // Arrange
            var manager = new ResourceManager();
            // Start with 50 energy
            manager.GetResource(ResourceType.Energy)!.CurrentValue = 50f;

            // Act - 3 seconds elapsed, should replenish 30 energy (10 per second)
            manager.UpdateResources(3f);

            // Assert
            Assert.Equal(80f, manager.GetCurrentValue(ResourceType.Energy));
        }

        [Fact]
        public void Unit_HasResourceManagerAndReplenishesResources()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            // Start with partial energy
            character.Resources.GetResource(ResourceType.Energy)!.CurrentValue = 40f;
            
            // Act - Update game time with 2 seconds elapsed
            character.UpdateGameTime(0f);
            character.UpdateGameTime(2f);

            // Assert - Should have replenished 20 energy (10 per second)
            Assert.Equal(60f, character.Resources.GetCurrentValue(ResourceType.Energy));
        }

        [Fact]
        public void ResourceCost_PropertiesWork()
        {
            // Arrange & Act
            var energyCost = new ResourceCost(ResourceType.Energy, 25f);
            var energyGeneration = new ResourceCost(ResourceType.Energy, -15f);

            // Assert
            Assert.Equal(ResourceType.Energy, energyCost.ResourceType);
            Assert.True(energyCost.IsCost);
            Assert.False(energyCost.IsGeneration);
            Assert.Equal(25f, energyCost.AbsoluteAmount);

            Assert.Equal(ResourceType.Energy, energyGeneration.ResourceType);
            Assert.False(energyGeneration.IsCost);
            Assert.True(energyGeneration.IsGeneration);
            Assert.Equal(15f, energyGeneration.AbsoluteAmount);
        }

        [Fact]
        public void Ability_CannotUseWithInsufficientResources()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            target.Position = new Vector2(30, 0); // Within range
            
            // Set energy to less than melee attack cost (10)
            character.Resources.GetResource(ResourceType.Energy)!.CurrentValue = 5f;
            character.UpdateGameTime(0f);

            // Act & Assert
            var meleeAbility = character.Abilities.Find(a => a is MeleeAttackAbility);
            Assert.NotNull(meleeAbility);
            Assert.False(character.CanUseAbility(meleeAbility, target));
        }

        [Fact]
        public void Ability_CanUseWithSufficientResources()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            target.Position = new Vector2(30, 0); // Within range
            
            // Set energy to enough for melee attack cost (10)
            character.Resources.GetResource(ResourceType.Energy)!.CurrentValue = 50f;
            character.UpdateGameTime(0f);

            // Act & Assert
            var meleeAbility = character.Abilities.Find(a => a is MeleeAttackAbility);
            Assert.NotNull(meleeAbility);
            Assert.True(character.CanUseAbility(meleeAbility, target));
        }

        [Fact]
        public void Ability_ConsumesResourcesOnUse()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            target.Position = new Vector2(30, 0); // Within range
            
            character.Resources.GetResource(ResourceType.Energy)!.CurrentValue = 50f;
            character.UpdateGameTime(0f);

            // Act
            var meleeAbility = character.Abilities.Find(a => a is MeleeAttackAbility);
            Assert.NotNull(meleeAbility);
            character.UseAbility(meleeAbility, target);

            // Assert - Should have consumed 10 energy
            Assert.Equal(40f, character.Resources.GetCurrentValue(ResourceType.Energy));
        }

        [Fact]
        public void EnergyRegenerationAbility_CannotUseWhenFull()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            var energyRegen = new EnergyRegenerationAbility();
            character.Abilities.Add(energyRegen);
            
            // Character starts with full energy (100)
            character.UpdateGameTime(0f);

            // Act & Assert - Cannot use because it would exceed max
            Assert.False(character.CanUseAbility(energyRegen, character));
        }

        [Fact]
        public void EnergyRegenerationAbility_GeneratesEnergyWhenUsed()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            var energyRegen = new EnergyRegenerationAbility();
            character.Abilities.Add(energyRegen);
            
            // Set energy to partial so it can generate more
            character.Resources.GetResource(ResourceType.Energy)!.CurrentValue = 50f;
            character.UpdateGameTime(0f);

            // Act
            character.UseAbility(energyRegen, character);

            // Assert - Should have generated 25 energy
            Assert.Equal(75f, character.Resources.GetCurrentValue(ResourceType.Energy));
        }

        [Fact]
        public void EnergyRegenerationAbility_CannotUseIfWouldExceedMax()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            var energyRegen = new EnergyRegenerationAbility();
            character.Abilities.Add(energyRegen);
            
            // Set energy to 80, so adding 25 would exceed 100
            character.Resources.GetResource(ResourceType.Energy)!.CurrentValue = 80f;
            character.UpdateGameTime(0f);

            // Act & Assert
            Assert.False(character.CanUseAbility(energyRegen, character));
        }

        [Fact]
        public void IntegrationTest_ResourceSystemWorksTogether()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            target.Position = new Vector2(30, 0);

            var energyRegen = new EnergyRegenerationAbility();
            character.Abilities.Add(energyRegen);
            
            // Start with low energy
            character.Resources.GetResource(ResourceType.Energy)!.CurrentValue = 15f;
            character.UpdateGameTime(0f);

            // Act 1 - Use melee attack (costs 10 energy)
            var meleeAbility = character.Abilities.Find(a => a is MeleeAttackAbility);
            Assert.NotNull(meleeAbility);
            character.UseAbility(meleeAbility, target);
            Assert.Equal(5f, character.Resources.GetCurrentValue(ResourceType.Energy));

            // Act 2 - Wait for energy regeneration (5 seconds = 50 energy replenished)
            character.UpdateGameTime(5f);
            Assert.Equal(55f, character.Resources.GetCurrentValue(ResourceType.Energy));

            // Act 3 - Use energy regeneration ability (generates 25 energy)
            character.UseAbility(energyRegen, character);
            Assert.Equal(80f, character.Resources.GetCurrentValue(ResourceType.Energy));

            // Act 4 - Use multiple abilities
            character.UseAbility(meleeAbility, target); // -10 energy = 70
            Assert.Equal(70f, character.Resources.GetCurrentValue(ResourceType.Energy));
        }
    }
}