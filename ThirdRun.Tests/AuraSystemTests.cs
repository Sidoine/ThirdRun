using System;
using MonogameRPG;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests
{
    public class AuraSystemTests
    {
        private (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            return (worldMap.CurrentMap, worldMap);
        }

        private class TestUnit : Unit
        {
            public TestUnit(MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap, int health = 100, int attackPower = 10) 
                : base(map, worldMap)
            {
                CurrentHealth = health;
                MaxHealth = health;
                Characteristics.SetValue(Characteristic.MeleeAttackPower, attackPower);
                Position = Vector2.Zero;
            }
        }

        [Fact]
        public void Aura_Creation_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var aura = new Aura("Test Aura", "Test Description", 30f, 5, true);
            aura.AddModifier(Characteristic.MeleeAttackPower, 10);

            // Assert
            Assert.Equal("Test Aura", aura.Name);
            Assert.Equal("Test Description", aura.Description);
            Assert.Equal(30f, aura.BaseDuration);
            Assert.Equal(5, aura.MaxStacks);
            Assert.True(aura.IsDebuff);
            Assert.Equal(10, aura.CharacteristicModifiers[Characteristic.MeleeAttackPower]);
        }

        [Fact]
        public void AuraEffect_Creation_InitializesCorrectly()
        {
            // Arrange
            var aura = new Aura("Test Aura", "Test Description", 30f, 3, false);
            aura.AddModifier(Characteristic.MeleeAttackPower, 5);
            var appliedTime = 10f;

            // Act
            var auraEffect = new AuraEffect(aura, appliedTime, 2);

            // Assert
            Assert.Equal(aura, auraEffect.Aura);
            Assert.Equal(30f, auraEffect.RemainingDuration);
            Assert.Equal(2, auraEffect.Stacks);
            Assert.Equal(appliedTime, auraEffect.AppliedTime);
        }

        [Fact]
        public void AuraEffect_AddStacks_IncreasesStacksAndRefreshesDuration()
        {
            // Arrange
            var aura = new Aura("Test Aura", "Test Description", 30f, 5, false);
            var auraEffect = new AuraEffect(aura, 10f, 2);
            auraEffect.RemainingDuration = 15f; // Simulate partial duration

            // Act
            auraEffect.AddStacks(2);

            // Assert
            Assert.Equal(4, auraEffect.Stacks);
            Assert.Equal(30f, auraEffect.RemainingDuration); // Duration should be refreshed
        }

        [Fact]
        public void AuraEffect_AddStacks_RespectsMaxStacks()
        {
            // Arrange
            var aura = new Aura("Test Aura", "Test Description", 30f, 3, false);
            var auraEffect = new AuraEffect(aura, 10f, 2);

            // Act
            auraEffect.AddStacks(5); // Try to add more than max allows

            // Assert
            Assert.Equal(3, auraEffect.Stacks); // Should be capped at MaxStacks
        }

        [Fact]
        public void AuraEffect_GetCharacteristicModifier_ReturnsCorrectValue()
        {
            // Arrange
            var aura = new Aura("Test Aura", "Test Description", 30f, 5, false);
            aura.AddModifier(Characteristic.MeleeAttackPower, 3);
            aura.AddModifier(Characteristic.Armor, 2);
            var auraEffect = new AuraEffect(aura, 10f, 4);

            // Act & Assert
            Assert.Equal(12, auraEffect.GetCharacteristicModifier(Characteristic.MeleeAttackPower)); // 3 * 4 stacks
            Assert.Equal(8, auraEffect.GetCharacteristicModifier(Characteristic.Armor)); // 2 * 4 stacks
            Assert.Equal(0, auraEffect.GetCharacteristicModifier(Characteristic.Health)); // Not modified
        }

        [Fact]
        public void Unit_ApplyAura_AddsNewAuraEffect()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            var aura = new Aura("Test Aura", "Test Description", 30f, 5, false);

            // Act
            unit.ApplyAura(aura, 2);

            // Assert
            Assert.Single(unit.ActiveAuras);
            Assert.Equal("Test Aura", unit.ActiveAuras[0].Aura.Name);
            Assert.Equal(2, unit.ActiveAuras[0].Stacks);
        }

        [Fact]
        public void Unit_ApplyAura_StacksExistingAura()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            var aura = new Aura("Test Aura", "Test Description", 30f, 5, false);

            // Act
            unit.ApplyAura(aura, 2);
            unit.ApplyAura(aura, 1);

            // Assert
            Assert.Single(unit.ActiveAuras);
            Assert.Equal(3, unit.ActiveAuras[0].Stacks);
        }

        [Fact]
        public void Unit_RemoveAura_RemovesAuraByName()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            var aura1 = new Aura("Aura 1", "Test Description", 30f, 5, false);
            var aura2 = new Aura("Aura 2", "Test Description", 30f, 5, false);
            
            unit.ApplyAura(aura1);
            unit.ApplyAura(aura2);

            // Act
            unit.RemoveAura("Aura 1");

            // Assert
            Assert.Single(unit.ActiveAuras);
            Assert.Equal("Aura 2", unit.ActiveAuras[0].Aura.Name);
        }

        [Fact]
        public void Unit_GetTotalCharacteristic_IncludesAuraBonuses()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap, 100, 10);
            var aura = new Aura("Attack Boost", "Test Description", 30f, 5, false);
            aura.AddModifier(Characteristic.MeleeAttackPower, 5);
            
            unit.ApplyAura(aura, 3);

            // Act
            var totalAttackPower = unit.GetTotalCharacteristic(Characteristic.MeleeAttackPower);

            // Assert
            Assert.Equal(25, totalAttackPower); // 10 base + (5 * 3 stacks)
        }

        [Fact]
        public void Unit_UpdateAuras_RemovesExpiredAuras()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            unit.UpdateGameTime(10f);
            
            var aura = new Aura("Short Aura", "Test Description", 5f, 1, false);
            unit.ApplyAura(aura);

            // Act - simulate 6 seconds passing
            unit.UpdateAuras(6f);

            // Assert
            Assert.Empty(unit.ActiveAuras);
        }

        [Fact]
        public void Unit_UpdateAuras_KeepsActiveAuras()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            unit.UpdateGameTime(10f);
            
            var aura = new Aura("Long Aura", "Test Description", 10f, 1, false);
            unit.ApplyAura(aura);

            // Act - simulate 3 seconds passing
            unit.UpdateAuras(3f);

            // Assert
            Assert.Single(unit.ActiveAuras);
            Assert.Equal(7f, unit.ActiveAuras[0].RemainingDuration);
        }
    }
}