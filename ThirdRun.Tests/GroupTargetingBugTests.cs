using System;
using System.Linq;
using MonogameRPG;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests
{
    public class GroupTargetingBugTests
    {
        private (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            return (worldMap.CurrentMap, worldMap);
        }

        [Fact]
        public void Debug_UseAbilities_DetailedFlow()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var ally = new Character("Ally", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            caster.Position = new Vector2(0, 0);
            ally.Position = new Vector2(50, 0);
            
            var characters = new System.Collections.Generic.List<Character> { caster, ally };
            map.SetCharacters(characters);
            
            var groupAbility = new AttackPowerBuffAbility();
            caster.Abilities.Clear(); // Remove the default melee attack
            caster.Abilities.Add(groupAbility);
            caster.UpdateGameTime(10f);

            // Pre-conditions
            Assert.Empty(caster.ActiveAuras);
            Assert.Empty(ally.ActiveAuras);
            Assert.False(caster.IsOnGlobalCooldown());
            Assert.False(groupAbility.IsOnCooldown(10f));
            
            // Act
            caster.UseAbilities();

            // Assert
            Assert.Single(caster.ActiveAuras);
            Assert.Single(ally.ActiveAuras);
        }
    }
}