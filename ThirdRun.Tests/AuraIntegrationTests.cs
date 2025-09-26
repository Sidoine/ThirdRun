using System;
using System.Linq;
using MonogameRPG;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;
using MonogameRPG.Monsters;

namespace ThirdRun.Tests
{
    public class AuraIntegrationTests
    {
        private (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            return (worldMap.CurrentMap, worldMap);
        }

        [Fact]
        public void PriestGroupBuffAbility_Works()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var priest = new Character("Priest", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var warrior = new Character("Warrior", CharacterClass.Guerrier, 100, 12, map, worldMap);
            var mage = new Character("Mage", CharacterClass.Mage, 80, 8, map, worldMap);
            
            priest.Position = new Vector2(0, 0);
            warrior.Position = new Vector2(40, 0);  // Within group buff range
            mage.Position = new Vector2(60, 0);     // Within group buff range
            
            var characters = new System.Collections.Generic.List<Character> { priest, warrior, mage };
            map.SetCharacters(characters);
            
            // Record original attack power
            var priestOriginalAttack = priest.AttackPower;
            var warriorOriginalAttack = warrior.AttackPower;
            var mageOriginalAttack = mage.AttackPower;
            
            priest.UpdateGameTime(10f);
            
            // Act - Priest should use group buff ability
            priest.UseAbilities();
            
            // Assert - All characters should have the attack power buff
            Assert.Single(priest.ActiveAuras);
            Assert.Single(warrior.ActiveAuras);
            Assert.Single(mage.ActiveAuras);
            
            // Check that the aura is the correct one
            Assert.Equal("Blessing of Strength", priest.ActiveAuras[0].Aura.Name);
            Assert.Equal("Blessing of Strength", warrior.ActiveAuras[0].Aura.Name);
            Assert.Equal("Blessing of Strength", mage.ActiveAuras[0].Aura.Name);
            
            // Check that effective attack power is increased (base + aura bonus)
            Assert.Equal(priestOriginalAttack + 5, priest.GetEffectiveAttackPower()); // +5 from aura
            Assert.Equal(warriorOriginalAttack + 5, warrior.GetEffectiveAttackPower());
            Assert.Equal(mageOriginalAttack + 5, mage.GetEffectiveAttackPower());
        }
        
        [Fact]
        public void HunterRegenerationBuff_WorksOnFriendlyTarget()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var hunter = new Character("Hunter", CharacterClass.Chasseur, 100, 10, map, worldMap);
            var warrior = new Character("Warrior", CharacterClass.Guerrier, 100, 12, map, worldMap);
            
            // Damage the warrior so hunter will want to heal them
            warrior.CurrentHealth = 50;
            
            hunter.Position = new Vector2(0, 0);
            warrior.Position = new Vector2(60, 0); // Within regeneration range
            
            var characters = new System.Collections.Generic.List<Character> { hunter, warrior };
            map.SetCharacters(characters);
            
            var originalWarriorMaxHealth = warrior.MaxHealth;
            
            hunter.UpdateGameTime(10f);
            
            // Act - Hunter should use regeneration on damaged warrior
            hunter.UseAbilities();
            
            // Assert - Warrior should have regeneration aura
            Assert.Single(warrior.ActiveAuras);
            Assert.Equal("Regeneration", warrior.ActiveAuras[0].Aura.Name);
            Assert.False(warrior.ActiveAuras[0].Aura.IsDebuff);
            
            // Check that effective max health is increased slightly
            Assert.Equal(originalWarriorMaxHealth + 2, warrior.GetEffectiveMaxHealth()); // +2 from regeneration
        }
        
        [Fact]
        public void AuraStacking_WorksCorrectly()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var target = new Character("Target", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var buffAura = new Aura("Test Buff", "Test", 30f, 3, false);
            buffAura.AddModifier(Characteristic.MeleeAttackPower, 5);
            
            target.UpdateGameTime(10f);
            
            // Act - Apply the same aura multiple times
            target.ApplyAura(buffAura, 1);
            target.ApplyAura(buffAura, 1); 
            target.ApplyAura(buffAura, 1); 
            
            // Assert - Should have one aura effect with 3 stacks
            Assert.Single(target.ActiveAuras);
            Assert.Equal(3, target.ActiveAuras[0].Stacks);
            
            // Check that stacking works correctly
            var originalAttack = target.AttackPower;
            Assert.Equal(originalAttack + 15, target.GetEffectiveAttackPower()); // 5 * 3 stacks
        }
        
        [Fact]
        public void AuraExpiration_WorksCorrectly()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new Character("Unit", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var shortAura = new Aura("Short Aura", "Test", 2f, 1, false); // 2 second duration
            shortAura.AddModifier(Characteristic.MeleeAttackPower, 5);
            
            unit.UpdateGameTime(10f);
            unit.ApplyAura(shortAura);
            
            // Assert - Aura should be active initially
            Assert.Single(unit.ActiveAuras);
            Assert.Equal(15, unit.GetEffectiveAttackPower()); // 10 base + 5 aura
            
            // Act - Simulate 3 seconds passing (longer than aura duration)
            unit.UpdateGameTime(13f);
            
            // Assert - Aura should be expired and removed
            Assert.Empty(unit.ActiveAuras);
            Assert.Equal(10, unit.GetEffectiveAttackPower()); // Back to base value
        }
    }
}