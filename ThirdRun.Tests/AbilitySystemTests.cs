using MonogameRPG;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;
using System;

namespace ThirdRun.Tests
{
    public class AbilitySystemTests
    {
        private (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var worldMap = new MonogameRPG.Map.WorldMap();
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
        
        private class TestAbility : Ability
        {
            public bool WasExecuted { get; set; }
            public Unit? LastCaster { get; private set; }
            public Unit? LastTarget { get; private set; }
            
            public TestAbility(float range = 50f, float castTime = 0f, TargetType targetType = TargetType.Enemy, float cooldown = 1f)
                : base("Test Ability", range, castTime, targetType, cooldown)
            {
            }
            
            protected override void Execute(Unit caster, Unit? target)
            {
                WasExecuted = true;
                LastCaster = caster;
                LastTarget = target;
            }
        }
        
        [Fact]
        public void Unit_ShouldHaveDefaultMeleeAbility()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            
            Assert.NotNull(unit.DefaultAbility);
            Assert.Equal("Melee Attack", unit.DefaultAbility.Name);
            Assert.Equal(TargetType.Enemy, unit.DefaultAbility.TargetType);
            Assert.Contains(unit.DefaultAbility, unit.Abilities);
        }
        
        [Fact]
        public void MeleeAttackAbility_ShouldDamageTarget()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var attacker = new TestUnit(map, worldMap, health: 100, attackPower: 25);
            var target = new TestUnit(map, worldMap, health: 100);
            
            attacker.UpdateGameTime(1f);
            attacker.UseAbility(attacker.DefaultAbility, target);
            
            Assert.Equal(75, target.CurrentHealth); // 100 - 25 = 75
        }
        
        [Fact]
        public void Ability_ShouldRespectCooldown()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            var ability = new TestAbility(cooldown: 2f);
            
            unit.UpdateGameTime(0f);
            unit.UseAbility(ability, new TestUnit(map, worldMap));
            Assert.True(ability.WasExecuted);
            
            // Reset for second test
            var testAbility = ability as TestAbility;
            testAbility!.WasExecuted = false;
            
            // Try to use again immediately - should be on cooldown
            unit.UseAbility(ability, new TestUnit(map, worldMap));
            Assert.False(ability.WasExecuted);
            
            // Wait for cooldown to expire
            unit.UpdateGameTime(3f);
            unit.UseAbility(ability, new TestUnit(map, worldMap));
            Assert.True(ability.WasExecuted);
        }
        
        [Fact]
        public void Ability_ShouldRespectRange()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap);
            var ability = new TestAbility(range: 50f);
            
            caster.Position = new Vector2(0, 0);
            target.Position = new Vector2(30, 0); // Within range
            
            caster.UpdateGameTime(0f);
            caster.UseAbility(ability, target);
            Assert.True((ability as TestAbility)!.WasExecuted);
            
            // Reset and test out of range
            var testAbility = ability as TestAbility;
            testAbility!.WasExecuted = false;
            
            target.Position = new Vector2(100, 0); // Out of range
            caster.UpdateGameTime(1f);
            caster.UseAbility(ability, target);
            Assert.False(ability.WasExecuted);
        }
        
        [Fact]
        public void Ability_ShouldValidateTargetType()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap);
            
            // Test self-target ability
            var selfAbility = new TestAbility(targetType: TargetType.Self);
            caster.UpdateGameTime(0f);
            
            // Should work with self as target
            caster.UseAbility(selfAbility, caster);
            Assert.True((selfAbility as TestAbility)!.WasExecuted);
            
            // Should not work with other as target
            var testAbility = selfAbility as TestAbility;
            testAbility!.WasExecuted = false;
            caster.UpdateGameTime(1f);
            caster.UseAbility(selfAbility, target);
            Assert.False(selfAbility.WasExecuted);
        }
        
        [Fact]
        public void Attack_ShouldUseDefaultAbility()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var attacker = new TestUnit(map, worldMap, attackPower: 15);
            var target = new TestUnit(map, worldMap, health: 50);
            
            attacker.UpdateGameTime(0f);
            attacker.Attack(target);
            
            Assert.Equal(35, target.CurrentHealth); // 50 - 15 = 35
        }
        
        [Fact]
        public void Ability_IsOnCooldown_ShouldReturnCorrectStatus()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var ability = new TestAbility(cooldown: 2f);
            var unit = new TestUnit(map, worldMap);
            
            unit.UpdateGameTime(0f);
            Assert.False(ability.IsOnCooldown(0f));
            
            unit.UseAbility(ability, new TestUnit(map, worldMap));
            Assert.True(ability.IsOnCooldown(1f));
            Assert.True(ability.IsOnCooldown(1.5f));
            Assert.False(ability.IsOnCooldown(2.1f));
        }
        
        [Fact]
        public void Ability_GetCooldownRemaining_ShouldReturnCorrectTime()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var ability = new TestAbility(cooldown: 3f);
            var unit = new TestUnit(map, worldMap);
            
            unit.UpdateGameTime(0f);
            unit.UseAbility(ability, new TestUnit(map, worldMap));
            
            Assert.True(Math.Abs(2f - ability.GetCooldownRemaining(1f)) < 0.1f);
            Assert.True(Math.Abs(0.5f - ability.GetCooldownRemaining(2.5f)) < 0.1f);
            Assert.True(Math.Abs(0f - ability.GetCooldownRemaining(4f)) < 0.1f);
        }
        
        [Fact]
        public void RangedAttackAbility_ShouldDamageTarget()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var attacker = new TestUnit(map, worldMap, health: 100, attackPower: 20);
            attacker.Characteristics.SetValue(Characteristic.RangedAttackPower, 15);
            var target = new TestUnit(map, worldMap, health: 80);
            
            var rangedAttack = new RangedAttackAbility();
            attacker.UpdateGameTime(0f);
            attacker.UseAbility(rangedAttack, target);
            
            Assert.Equal(65, target.CurrentHealth); // 80 - 15 = 65
        }
        
        [Fact]
        public void HealAbility_ShouldHealTarget()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var healer = new TestUnit(map, worldMap);
            healer.Characteristics.SetValue(Characteristic.HealingPower, 20);
            var target = new TestUnit(map, worldMap, health: 50);
            target.MaxHealth = 100;
            
            var healAbility = new HealAbility();
            healer.UpdateGameTime(0f);
            healer.UseAbility(healAbility, target);
            
            Assert.Equal(70, target.CurrentHealth); // 50 + 20 = 70
        }
        
        [Fact]
        public void SelfHealAbility_ShouldHealCaster()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap, health: 30);
            unit.MaxHealth = 100;
            unit.Characteristics.SetValue(Characteristic.HealingPower, 25);
            
            var selfHealAbility = new SelfHealAbility();
            unit.UpdateGameTime(0f);
            unit.UseAbility(selfHealAbility, unit);
            
            Assert.Equal(55, unit.CurrentHealth); // 30 + 25 = 55
        }
        
        [Fact]
        public void HealAbility_ShouldNotExceedMaxHealth()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var healer = new TestUnit(map, worldMap);
            healer.Characteristics.SetValue(Characteristic.HealingPower, 30);
            var target = new TestUnit(map, worldMap, health: 90);
            target.MaxHealth = 100;
            
            var healAbility = new HealAbility();
            healer.UpdateGameTime(0f);
            healer.UseAbility(healAbility, target);
            
            Assert.Equal(100, target.CurrentHealth); // Should cap at MaxHealth
        }
    }
}
