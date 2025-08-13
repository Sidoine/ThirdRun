using MonogameRPG;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace ThirdRun.Tests
{
    public class UseAbilitiesDebugTest
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
        
        private class TestAbility : Ability
        {
            public bool WasExecuted { get; set; }
            public Unit? LastCaster { get; private set; }
            public Unit? LastTarget { get; private set; }
            public string DebugLog { get; set; } = "";
            
            public TestAbility(string name = "Test", float range = 50f, float castTime = 0f, TargetType targetType = TargetType.Enemy, float cooldown = 1f)
                : base(name, "test_icon", range, castTime, targetType, cooldown)
            {
            }
            
            protected override void Execute(Unit caster, Unit? target)
            {
                DebugLog += $"Execute called with caster={caster?.GetType().Name}, target={target?.GetType().Name}; ";
                WasExecuted = true;
                LastCaster = caster;
                LastTarget = target;
            }
        }
        
        [Fact]
        public void Debug_UseAbilities_ShouldFindAndExecuteAbility()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var unit = new TestUnit(map, worldMap);
            var target = new TestUnit(map, worldMap);
            target.Position = new Vector2(30, 0); // Within range
            
            var ability = new TestAbility("Test");
            unit.Abilities.Clear();
            unit.Abilities.Add(ability);
            
            map.AddUnit(unit);
            map.AddUnit(target);
            
            // Debug info
            var charactersCount = map.Characters.Count();
            var monstersCount = map.Monsters.Count();
            var npcsCount = map.NPCs.Count();
            
            // Since TestUnit is not a Character or Monster, it should fall into the "else" case
            // which looks for other units in the combined collections
            var allUnitsInCombined = map.Characters.Cast<Unit>()
                .Concat(map.Monsters.Cast<Unit>())
                .Concat(map.NPCs.Cast<Unit>())
                .Where(u => u != unit && !u.IsDead)
                .Count();
            
            Assert.Equal(0, charactersCount); // TestUnits are not Characters
            Assert.Equal(0, monstersCount);   // TestUnits are not Monsters  
            Assert.Equal(0, npcsCount);       // TestUnits are not NPCs
            Assert.Equal(0, allUnitsInCombined); // So this will be 0
            
            unit.UpdateGameTime(0f);
            unit.UseAbilities();
            
            // Since no targets were found, ability should not be executed
            Assert.False(ability.WasExecuted);
        }
        
        [Fact]
        public void Debug_UseAbilities_WithActualCharacterAndMonster()
        {
            var (map, worldMap) = CreateTestMapAndWorld();
            var character = new Character("Test", CharacterClass.Guerrier, 100, 10, map, worldMap);
            var monsterType = MonogameRPG.Monsters.MonsterTemplateRepository.CreateRandomMonsterType(new Random());
            var monster = new MonogameRPG.Monsters.Monster(monsterType, map, worldMap, new Random());
            
            character.Position = Vector2.Zero;
            monster.Position = new Vector2(30, 0); // Within range
            
            var ability = new TestAbility("Test");
            character.Abilities.Clear();
            character.Abilities.Add(ability);
            
            map.AddUnit(character);
            map.AddUnit(monster);
            
            var monstersCount = map.Monsters.Count();
            Assert.Equal(1, monstersCount);
            
            character.UpdateGameTime(0f);
            character.UseAbilities();
            
            // Now there should be a monster target, so ability should be executed
            Assert.True(ability.WasExecuted, $"Debug: {ability.DebugLog}");
        }
    }
}