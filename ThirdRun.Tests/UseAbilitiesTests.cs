using MonogameRPG;
using MonogameRPG.Monsters;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace ThirdRun.Tests
{
    public class UseAbilitiesTests
    {
        private (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            
            // Clear any pre-existing monsters that might be spawned by default
            var existingMonsters = worldMap.CurrentMap.Monsters.ToList();
            foreach (var monster in existingMonsters)
            {
                worldMap.CurrentMap.RemoveUnit(monster);
            }
            
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
            
            public TestAbility(string name = "Test", float range = 50f, float castTime = 0f, TargetType targetType = TargetType.Enemy, float cooldown = 1f)
                : base(name, "test_icon", range, castTime, targetType, cooldown)
            {
            }
            
            protected override void Execute(Unit caster, Unit? target)
            {
                WasExecuted = true;
                LastCaster = caster;
                LastTarget = target;
                
                // Simulate damage for enemy targets
                if (target != null && TargetType == TargetType.Enemy)
                {
                    target.CurrentHealth -= 10;
                    if (target.CurrentHealth < 0)
                        target.CurrentHealth = 0;
                }
            }
        }
        
        [Fact]
        public void UseAbilities_ShouldRespectGlobalCooldown()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var monsterType = MonogameRPG.Monsters.MonsterTemplateRepository.CreateRandomMonsterType(new Random());
            var target = new MonogameRPG.Monsters.Monster(monsterType, map, worldMap, new Random());
            target.Position = new Vector2(30, 0); // Within range
            
            var ability1 = new TestAbility("First", cooldown: 0.1f);
            var ability2 = new TestAbility("Second", cooldown: 0.1f);
            character.Abilities.Clear();
            character.Abilities.Add(ability1);
            character.Abilities.Add(ability2);
            
            map.AddUnit(character);
            map.AddUnit(target);
            
            character.UpdateGameTime(0f);
            character.UseAbilities();
            
            // First ability should be used
            Assert.True(ability1.WasExecuted);
            Assert.False(ability2.WasExecuted);
            
            // Reset and try again immediately - should be on global cooldown
            ability1.WasExecuted = false;
            ability2.WasExecuted = false;
            character.UpdateGameTime(0.5f); // Less than 1.5s global cooldown
            character.UseAbilities();
            
            Assert.False(ability1.WasExecuted);
            Assert.False(ability2.WasExecuted);
            
            // After global cooldown expires
            character.UpdateGameTime(2f); // More than 1.5s global cooldown
            character.UseAbilities();
            
            Assert.True(ability1.WasExecuted);
        }
        
        [Fact]
        public void UseAbilities_ShouldUseAbilitiesInOrder()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var monsterType = MonogameRPG.Monsters.MonsterTemplateRepository.CreateRandomMonsterType(new Random());
            var target = new MonogameRPG.Monsters.Monster(monsterType, map, worldMap, new Random());
            target.Position = new Vector2(30, 0); // Within range
            
            var ability1 = new TestAbility("First");
            var ability2 = new TestAbility("Second");
            var ability3 = new TestAbility("Third");
            
            character.Abilities.Clear();
            character.Abilities.Add(ability1);
            character.Abilities.Add(ability2);
            character.Abilities.Add(ability3);
            
            // Put ability2 on cooldown so it should be skipped
            character.UpdateGameTime(0f);
            character.UseAbility(ability2, target);
            ability2.WasExecuted = false; // Reset the flag for the test
            
            map.AddUnit(character);
            map.AddUnit(target);
            
            character.UpdateGameTime(0.5f); // Move time forward, but ability2 still on cooldown (1s default)
            character.UseAbilities();
            
            // ability1 should be used since it's first and available
            Assert.True(ability1.WasExecuted);
            Assert.False(ability2.WasExecuted); // Should stay false, was already used
            Assert.False(ability3.WasExecuted); // Should not be reached
        }
        
        [Fact]
        public void UseAbilities_ShouldSkipAbilitiesOnCooldown()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var monsterType = MonogameRPG.Monsters.MonsterTemplateRepository.CreateRandomMonsterType(new Random());
            var target = new MonogameRPG.Monsters.Monster(monsterType, map, worldMap, new Random());
            target.Position = new Vector2(30, 0); // Within range
            
            var ability1 = new TestAbility("First", cooldown: 5f);
            var ability2 = new TestAbility("Second", cooldown: 1f);
            
            // Put ability1 on cooldown
            character.UpdateGameTime(0f);
            character.UseAbility(ability1, target);
            ability1.WasExecuted = false; // Reset for test
            
            character.Abilities.Clear();
            character.Abilities.Add(ability1); // On cooldown
            character.Abilities.Add(ability2); // Available
            
            map.AddUnit(character);
            map.AddUnit(target);
            
            // Wait for global cooldown but not ability1 cooldown
            character.UpdateGameTime(2f);
            character.UseAbilities();
            
            Assert.False(ability1.WasExecuted); // Should be skipped due to cooldown
            Assert.True(ability2.WasExecuted);  // Should be used
        }
        
        [Fact] 
        public void UseAbilities_ShouldFindNearestEnemyTarget()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var monsterType1 = MonogameRPG.Monsters.MonsterTemplateRepository.CreateRandomMonsterType(new Random());
            var monsterType2 = MonogameRPG.Monsters.MonsterTemplateRepository.CreateRandomMonsterType(new Random());
            var monster1 = new Monster(monsterType1, map, worldMap, new Random());
            var monster2 = new Monster(monsterType2, map, worldMap, new Random());
            
            character.Position = new Vector2(0, 0);
            monster1.Position = new Vector2(25, 0); // Closer
            monster2.Position = new Vector2(45, 0); // Further
            
            map.AddUnit(character);
            map.AddUnit(monster1);
            map.AddUnit(monster2);
            
            int monster1InitialHealth = monster1.CurrentHealth;
            int monster2InitialHealth = monster2.CurrentHealth;
            
            character.UpdateGameTime(0f);
            character.UseAbilities();
            
            // monster1 should be damaged (it's closer)
            Assert.True(monster1.CurrentHealth < monster1InitialHealth);
            // monster2 should not be damaged 
            Assert.Equal(monster2InitialHealth, monster2.CurrentHealth);
        }
        
        [Fact]
        public void UseAbilities_ShouldRespectRange()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap);
            target.Position = new Vector2(100, 0); // Out of range
            
            var ability = new TestAbility("Test", range: 50f);
            unit.Abilities.Clear();
            unit.Abilities.Add(ability);
            
            map.AddUnit(unit);
            map.AddUnit(target);
            
            unit.UpdateGameTime(0f);
            unit.UseAbilities();
            
            // Ability should not be used due to range
            Assert.False(ability.WasExecuted);
        }
        
        [Fact]
        public void UseAbilities_ShouldHandleSelfTargeting()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            
            var selfAbility = new TestAbility("Self", targetType: TargetType.Self);
            unit.Abilities.Clear();
            unit.Abilities.Add(selfAbility);
            
            map.AddUnit(unit);
            
            unit.UpdateGameTime(0f);
            unit.UseAbilities();
            
            Assert.True(selfAbility.WasExecuted);
            Assert.Equal(unit, selfAbility.LastTarget);
        }
        
        [Fact]
        public void IsOnGlobalCooldown_ShouldReturnCorrectStatus()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            character.UpdateGameTime(0f);
            Assert.False(character.IsOnGlobalCooldown());
            
            // Use an ability to trigger global cooldown
            var monsterType = MonogameRPG.Monsters.MonsterTemplateRepository.CreateRandomMonsterType(new Random());
            var target = new MonogameRPG.Monsters.Monster(monsterType, map, worldMap, new Random());
            target.Position = new Vector2(10, 0);
            map.AddUnit(character);
            map.AddUnit(target);
            
            character.UseAbilities();
            
            // Should be on global cooldown
            Assert.True(character.IsOnGlobalCooldown());
            
            // After global cooldown expires
            character.UpdateGameTime(2f);
            Assert.False(character.IsOnGlobalCooldown());
        }
    }
}