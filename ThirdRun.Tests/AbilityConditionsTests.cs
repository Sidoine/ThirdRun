using System;
using System.Linq;
using MonogameRPG;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;
using Xunit;

namespace ThirdRun.Tests
{
    public class AbilityConditionsTests
    {
        private (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            return (worldMap.CurrentMap, worldMap);
        }

        [Fact]
        public void Unit_HasAura_ReturnsTrueWhenAuraExists()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            var aura = new Aura("Test Aura", "Test", 10f, 1);

            // Act
            unit.ApplyAura(aura);

            // Assert
            Assert.True(unit.HasAura("Test Aura"));
        }

        [Fact]
        public void Unit_HasAura_ReturnsFalseWhenAuraDoesNotExist()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);

            // Act & Assert
            Assert.False(unit.HasAura("Nonexistent Aura"));
        }

        [Fact]
        public void Unit_HasAnyDebuff_ReturnsTrueWhenDebuffExists()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            var debuff = new Aura("Weakness", "Weakens the unit", 10f, 1, true);

            // Act
            unit.ApplyAura(debuff);

            // Assert
            Assert.True(unit.HasAnyDebuff());
        }

        [Fact]
        public void Unit_HasAnyDebuff_ReturnsFalseWhenOnlyBuffsExist()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            var buff = new Aura("Strength", "Strengthens the unit", 10f, 1, false);

            // Act
            unit.ApplyAura(buff);

            // Assert
            Assert.False(unit.HasAnyDebuff());
        }

        [Fact]
        public void Unit_HasAnyDebuff_ReturnsFalseWhenNoAurasExist()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);

            // Act & Assert
            Assert.False(unit.HasAnyDebuff());
        }

        [Fact]
        public void HealAbility_CanUse_ReturnsFalseWhenTargetAtFullHealth()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap, 100, 10);
            var healAbility = new HealAbility();

            // Ensure target is at full health
            target.CurrentHealth = target.MaxHealth;

            // Act
            bool canUse = healAbility.CanUse(caster, target, 10f);

            // Assert
            Assert.False(canUse);
        }

        [Fact]
        public void HealAbility_CanUse_ReturnsTrueWhenTargetNeedsHealing()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap, 100, 10);
            var healAbility = new HealAbility();

            // Damage the target so it needs healing
            target.CurrentHealth = target.MaxHealth - 10;

            // Act
            bool canUse = healAbility.CanUse(caster, target, 10f);

            // Assert
            Assert.True(canUse);
        }

        [Fact]
        public void AuraAbility_CanUse_ReturnsFalseWhenTargetAlreadyHasAura()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap, 100, 10);
            var weaknessAbility = new WeaknessDebuffAbility();

            // Pre-apply the aura to the target
            target.ApplyAura(new Aura("Curse of Weakness", "Test", 10f, 1, true));

            // Act
            bool canUse = weaknessAbility.CanUse(caster, target, 10f);

            // Assert
            Assert.False(canUse);
        }

        [Fact]
        public void AuraAbility_CanUse_ReturnsTrueWhenTargetDoesNotHaveAura()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap, 100, 10);
            var weaknessAbility = new WeaknessDebuffAbility();

            // Ensure target is in range
            caster.Position = new Vector2(0, 0);
            target.Position = new Vector2(32, 0); // Within range of 64f

            // Act
            bool canUse = weaknessAbility.CanUse(caster, target, 10f);

            // Assert
            Assert.True(canUse);
        }

        [Fact]
        public void DispelDebuffAbility_Properties_AreCorrect()
        {
            // Arrange & Act
            var ability = new DispelDebuffAbility();

            // Assert
            Assert.Equal("Dispel Magic", ability.Name);
            Assert.Equal("Abilities/dispel", ability.IconPath);
            Assert.Equal(64f, ability.Range);
            Assert.Equal(1.5f, ability.CastTime);
            Assert.Equal(TargetType.Friendly, ability.TargetType);
            Assert.Equal(5f, ability.Cooldown);
        }

        [Fact]
        public void DispelDebuffAbility_CanUse_ReturnsFalseWhenTargetHasNoDebuffs()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap, 100, 10);
            var dispelAbility = new DispelDebuffAbility();

            // Apply only a buff (not a debuff)
            var buff = new Aura("Strength", "Strengthens the unit", 10f, 1, false);
            target.ApplyAura(buff);

            // Act
            bool canUse = dispelAbility.CanUse(caster, target, 10f);

            // Assert
            Assert.False(canUse);
        }

        [Fact]
        public void DispelDebuffAbility_CanUse_ReturnsTrueWhenTargetHasDebuffs()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap, 100, 10);
            var dispelAbility = new DispelDebuffAbility();

            // Apply a debuff
            var debuff = new Aura("Weakness", "Weakens the unit", 10f, 1, true);
            target.ApplyAura(debuff);

            // Ensure target is in range
            caster.Position = new Vector2(0, 0);
            target.Position = new Vector2(32, 0); // Within range of 64f

            // Act
            bool canUse = dispelAbility.CanUse(caster, target, 10f);

            // Assert
            Assert.True(canUse);
        }

        [Fact]
        public void DispelDebuffAbility_Execute_RemovesOnlyDebuffs()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap, 100, 10);
            var dispelAbility = new DispelDebuffAbility();

            // Apply both a buff and a debuff
            var buff = new Aura("Strength", "Strengthens the unit", 10f, 1, false);
            var debuff1 = new Aura("Weakness", "Weakens the unit", 10f, 1, true);
            var debuff2 = new Aura("Curse", "Cursed", 10f, 1, true);

            target.ApplyAura(buff);
            target.ApplyAura(debuff1);
            target.ApplyAura(debuff2);

            // Verify initial state
            Assert.Equal(3, target.ActiveAuras.Count);
            Assert.True(target.HasAura("Strength"));
            Assert.True(target.HasAura("Weakness"));
            Assert.True(target.HasAura("Curse"));

            // Act
            dispelAbility.Use(caster, target, 10f);

            // Assert - only debuffs should be removed, buff should remain
            Assert.Single(target.ActiveAuras);
            Assert.True(target.HasAura("Strength"));
            Assert.False(target.HasAura("Weakness"));
            Assert.False(target.HasAura("Curse"));
        }

        [Fact]
        public void AttackPowerBuffAbility_CanUse_ReturnsFalseWhenTargetAlreadyHasBuff()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap, 100, 10);
            var buffAbility = new AttackPowerBuffAbility();

            // Pre-apply the buff to the target
            var existingBuff = new Aura("Blessing of Strength", "Test", 10f, 1, false);
            target.ApplyAura(existingBuff);

            // Act
            bool canUse = buffAbility.CanUse(caster, target, 10f);

            // Assert
            Assert.False(canUse);
        }

        private class TestUnit : Unit
        {
            public TestUnit(MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap, int maxHealth = 100, int level = 1)
                : base(map, worldMap)
            {
                MaxHealth = maxHealth;
                CurrentHealth = maxHealth;
                Characteristics.SetValue(Characteristic.MeleeAttackPower, 10);
                Position = Vector2.Zero;
                
                // Initialize with a basic melee attack ability
                Abilities.Add(new MeleeAttackAbility());
            }
        }
    }
}