using System;
using System.Linq;
using MonogameRPG;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;
using Xunit;

namespace ThirdRun.Tests
{
    public class AbilityConditionIntegrationTests
    {
        private (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            return (worldMap.CurrentMap, worldMap);
        }

        [Fact]
        public void UseAbilities_SkipsHealWhenTargetFullyHealed()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var healer = new Character("Healer", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            healer.Position = new Vector2(0, 0);
            target.Position = new Vector2(32, 0); // Within heal range
            
            // Ensure target is at full health
            target.CurrentHealth = target.MaxHealth;
            
            var healAbility = new TestableHealAbility();
            healer.Abilities.Clear();
            healer.Abilities.Add(healAbility);
            
            map.AddUnit(healer);
            map.AddUnit(target);
            
            healer.UpdateGameTime(10f);
            
            // Act
            healer.UseAbilities();
            
            // Assert - heal should not be used because target is at full health
            Assert.False(healAbility.WasExecuted);
        }

        [Fact]
        public void UseAbilities_UsesHealWhenTargetNeedsHealing()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var healer = new Character("Healer", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            healer.Position = new Vector2(0, 0);
            target.Position = new Vector2(32, 0); // Within heal range
            
            // Damage the target so it needs healing
            target.CurrentHealth = target.MaxHealth - 20;
            
            var healAbility = new TestableHealAbility();
            healer.Abilities.Clear();
            healer.Abilities.Add(healAbility);
            
            map.AddUnit(healer);
            map.AddUnit(target);
            
            healer.UpdateGameTime(10f);
            
            // Act
            healer.UseAbilities();
            
            // Assert - heal should be used because target needs healing
            Assert.True(healAbility.WasExecuted);
            Assert.Equal(target, healAbility.LastTarget);
        }

        [Fact]
        public void UseAbilities_SkipsAuraWhenTargetAlreadyHasIt()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            caster.Position = new Vector2(0, 0);
            target.Position = new Vector2(32, 0); // Within aura range
            
            // Pre-apply the aura to the target
            var existingAura = new Aura("Test Buff", "Test", 10f, 1, false);
            target.ApplyAura(existingAura);
            
            var auraAbility = new TestableAuraAbility();
            caster.Abilities.Clear();
            caster.Abilities.Add(auraAbility);
            
            map.AddUnit(caster);
            map.AddUnit(target);
            
            caster.UpdateGameTime(10f);
            
            // Act
            caster.UseAbilities();
            
            // Assert - aura ability should not be used because target already has the aura
            Assert.False(auraAbility.WasExecuted);
        }

        [Fact]
        public void UseAbilities_UsesAuraWhenTargetDoesNotHaveIt()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            caster.Position = new Vector2(0, 0);
            target.Position = new Vector2(32, 0); // Within aura range
            
            var auraAbility = new TestableAuraAbility();
            caster.Abilities.Clear();
            caster.Abilities.Add(auraAbility);
            
            map.AddUnit(caster);
            map.AddUnit(target);
            
            caster.UpdateGameTime(10f);
            
            // Act
            caster.UseAbilities();
            
            // Assert - aura ability should be used because target doesn't have the aura
            Assert.True(auraAbility.WasExecuted);
            Assert.Equal(target, auraAbility.LastTarget);
        }

        [Fact]
        public void UseAbilities_SkipsDispelWhenTargetHasNoDebuffs()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            caster.Position = new Vector2(0, 0);
            target.Position = new Vector2(32, 0); // Within dispel range
            
            // Apply only a buff (not a debuff)
            var buff = new Aura("Strength", "Test buff", 10f, 1, false);
            target.ApplyAura(buff);
            
            var dispelAbility = new TestableDispelAbility();
            caster.Abilities.Clear();
            caster.Abilities.Add(dispelAbility);
            
            map.AddUnit(caster);
            map.AddUnit(target);
            
            caster.UpdateGameTime(10f);
            
            // Act
            caster.UseAbilities();
            
            // Assert - dispel should not be used because target has no debuffs
            Assert.False(dispelAbility.WasExecuted);
        }

        [Fact]
        public void UseAbilities_UsesDispelWhenTargetHasDebuffs()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            caster.Position = new Vector2(0, 0);
            target.Position = new Vector2(32, 0); // Within dispel range
            
            // Apply a debuff
            var debuff = new Aura("Weakness", "Test debuff", 10f, 1, true);
            target.ApplyAura(debuff);
            
            var dispelAbility = new TestableDispelAbility();
            caster.Abilities.Clear();
            caster.Abilities.Add(dispelAbility);
            
            map.AddUnit(caster);
            map.AddUnit(target);
            
            caster.UpdateGameTime(10f);
            
            // Act
            caster.UseAbilities();
            
            // Assert - dispel should be used because target has debuffs
            Assert.True(dispelAbility.WasExecuted);
            Assert.Equal(target, dispelAbility.LastTarget);
            Assert.False(target.HasAnyDebuff()); // Debuff should be removed
        }

        [Fact]
        public void UseAbilities_SkipsAllAbilitiesWhenNoValidTargets()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            caster.Position = new Vector2(0, 0);
            target.Position = new Vector2(32, 0);
            
            // Make target invalid for all abilities
            target.CurrentHealth = target.MaxHealth; // No heal needed
            var existingAura = new Aura("Test Buff", "Test", 10f, 1, false);
            target.ApplyAura(existingAura); // Already has aura
            // No debuffs for dispel
            
            var healAbility = new TestableHealAbility();
            var auraAbility = new TestableAuraAbility();
            var dispelAbility = new TestableDispelAbility();
            
            caster.Abilities.Clear();
            caster.Abilities.Add(healAbility);
            caster.Abilities.Add(auraAbility);
            caster.Abilities.Add(dispelAbility);
            
            map.AddUnit(caster);
            map.AddUnit(target);
            
            caster.UpdateGameTime(10f);
            
            // Act
            caster.UseAbilities();
            
            // Assert - no abilities should be used
            Assert.False(healAbility.WasExecuted);
            Assert.False(auraAbility.WasExecuted);
            Assert.False(dispelAbility.WasExecuted);
            Assert.False(caster.IsOnGlobalCooldown()); // Global cooldown should not be triggered
        }

        // Helper classes for testing

        private class TestableHealAbility : HealAbility
        {
            public bool WasExecuted { get; private set; }
            public Unit? LastTarget { get; private set; }

            protected override void Execute(Unit caster, Unit? target)
            {
                WasExecuted = true;
                LastTarget = target;
                base.Execute(caster, target);
            }
        }

        private class TestableAuraAbility : AuraAbility
        {
            public bool WasExecuted { get; private set; }
            public Unit? LastTarget { get; private set; }

            public TestableAuraAbility() 
                : base("Test Buff", "test", 64f, 0f, TargetType.Friendly, 1f, 
                      new Aura("Test Buff", "Test", 10f, 1, false))
            {
            }

            protected override void Execute(Unit caster, Unit? target)
            {
                WasExecuted = true;
                LastTarget = target;
                base.Execute(caster, target);
            }
        }

        private class TestableDispelAbility : DispelDebuffAbility
        {
            public bool WasExecuted { get; private set; }
            public Unit? LastTarget { get; private set; }

            protected override void Execute(Unit caster, Unit? target)
            {
                WasExecuted = true;
                LastTarget = target;
                base.Execute(caster, target);
            }
        }
    }
}