using System;
using System.Linq;
using MonogameRPG;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests
{
    public class AuraAbilitySystemTests
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
        public void TargetType_Group_IsAddedToEnum()
        {
            // Assert that Group targeting exists
            var groupTargetType = TargetType.Group;
            Assert.Equal(3, (int)groupTargetType); // Should be the 4th enum value (0-based)
        }

        [Fact]
        public void Ability_CanUse_GroupTargeting_AlwaysReturnsTrue()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var ability = new AttackPowerBuffAbility();
            caster.UpdateGameTime(10f);

            // Act & Assert
            Assert.True(ability.CanUse(caster, null, 10f));
        }

        [Fact]
        public void AttackPowerBuffAbility_Properties_AreCorrect()
        {
            // Arrange & Act
            var ability = new AttackPowerBuffAbility();

            // Assert
            Assert.Equal("Blessing of Strength", ability.Name);
            Assert.Equal("Abilities/attack_buff", ability.IconPath);
            Assert.Equal(96f, ability.Range);
            Assert.Equal(1.5f, ability.CastTime);
            Assert.Equal(TargetType.Group, ability.TargetType);
            Assert.Equal(15f, ability.Cooldown);
        }

        [Fact]
        public void WeaknessDebuffAbility_Properties_AreCorrect()
        {
            // Arrange & Act
            var ability = new WeaknessDebuffAbility();

            // Assert
            Assert.Equal("Curse of Weakness", ability.Name);
            Assert.Equal("Abilities/weakness_debuff", ability.IconPath);
            Assert.Equal(64f, ability.Range);
            Assert.Equal(2f, ability.CastTime);
            Assert.Equal(TargetType.Enemy, ability.TargetType);
            Assert.Equal(8f, ability.Cooldown);
        }

        [Fact]
        public void RegenerationBuffAbility_Properties_AreCorrect()
        {
            // Arrange & Act
            var ability = new RegenerationBuffAbility();

            // Assert
            Assert.Equal("Regeneration", ability.Name);
            Assert.Equal("Abilities/regeneration", ability.IconPath);
            Assert.Equal(80f, ability.Range);
            Assert.Equal(1f, ability.CastTime);
            Assert.Equal(TargetType.Friendly, ability.TargetType);
            Assert.Equal(12f, ability.Cooldown);
        }

        [Fact]
        public void AuraAbility_Execute_AppliesAuraToTarget()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap);
            var ability = new WeaknessDebuffAbility();

            // Act
            ability.Use(caster, target, 10f);

            // Assert
            Assert.Single(target.ActiveAuras);
            Assert.Equal("Curse of Weakness", target.ActiveAuras[0].Aura.Name);
        }

        [Fact] 
        public void Unit_UseAbilities_SupportsGroupTargeting()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var ally = new Character("Ally", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            caster.Position = new Vector2(0, 0);
            ally.Position = new Vector2(50, 0);
            
            var characters = new List<Character> { caster, ally };
            map.SetCharacters(characters);
            
            // Note: Priest now automatically gets AttackPowerBuffAbility from InitializeClassAbilities()
            caster.UpdateGameTime(10f);

            // Act
            caster.UseAbilities();

            // Assert - Both units should have the buff
            Assert.Single(caster.ActiveAuras);
            Assert.Single(ally.ActiveAuras);
            Assert.Equal("Blessing of Strength", caster.ActiveAuras[0].Aura.Name);
            Assert.Equal("Blessing of Strength", ally.ActiveAuras[0].Aura.Name);
        }

        [Fact]
        public void AttackPowerBuff_AppliesCorrectAura()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            // Position them within group buff range (96)
            caster.Position = new Vector2(0, 0);
            target.Position = new Vector2(50, 0); 
            
            // Set characters on map so group targeting can find them
            var characters = new List<Character> { caster, target };
            map.SetCharacters(characters);
            
            var ability = new AttackPowerBuffAbility();

            // Act
            ability.Use(caster, null, 10f); // Group abilities use null target

            // Assert - Both caster and target should have the aura
            Assert.Single(caster.ActiveAuras);
            Assert.Single(target.ActiveAuras);
            
            var casterAura = caster.ActiveAuras[0];
            var targetAura = target.ActiveAuras[0];
            
            Assert.Equal("Blessing of Strength", casterAura.Aura.Name);
            Assert.Equal("Blessing of Strength", targetAura.Aura.Name);
            Assert.Equal(30f, casterAura.RemainingDuration);
            Assert.Equal(30f, targetAura.RemainingDuration);
            Assert.Equal(1, casterAura.Stacks);
            Assert.Equal(1, targetAura.Stacks);
            Assert.False(casterAura.Aura.IsDebuff);
            Assert.False(targetAura.Aura.IsDebuff);
            
            // Check characteristic modification
            var attackBonus = targetAura.GetCharacteristicModifier(Characteristic.MeleeAttackPower);
            Assert.Equal(5, attackBonus); // 5 per stack, 1 stack
        }

        [Fact]
        public void WeaknessDebuff_AppliesCorrectAura()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap, 100, 10);
            var ability = new WeaknessDebuffAbility();

            // Act
            ability.Use(caster, target, 10f);

            // Assert
            var auraEffect = target.ActiveAuras[0];
            Assert.Equal("Curse of Weakness", auraEffect.Aura.Name);
            Assert.Equal(20f, auraEffect.RemainingDuration);
            Assert.Equal(1, auraEffect.Stacks);
            Assert.True(auraEffect.Aura.IsDebuff);
            
            // Check characteristic modifications
            var attackPenalty = auraEffect.GetCharacteristicModifier(Characteristic.MeleeAttackPower);
            var armorPenalty = auraEffect.GetCharacteristicModifier(Characteristic.Armor);
            Assert.Equal(-3, attackPenalty); // -3 per stack, 1 stack
            Assert.Equal(-2, armorPenalty); // -2 per stack, 1 stack
        }
    }
}