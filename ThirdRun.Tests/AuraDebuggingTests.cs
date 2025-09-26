using System;
using System.Linq;
using MonogameRPG;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests
{
    public class AuraDebuggingTests
    {
        private (MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) CreateTestMapAndWorld()
        {
            var random = new Random(12345);
            var worldMap = new MonogameRPG.Map.WorldMap(random);
            worldMap.Initialize();
            return (worldMap.CurrentMap, worldMap);
        }

        [Fact]
        public void Debug_GroupTargeting_Step1_AbilityCanBeUsed()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            caster.Position = new Vector2(0, 0);
            caster.UpdateGameTime(10f);
            
            var ability = new AttackPowerBuffAbility();

            // Act & Assert
            Assert.True(ability.CanUse(caster, null, 10f));
        }

        [Fact]
        public void Debug_GroupTargeting_Step2_AbilityExecutesTogetherCorrectly()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var ally = new Character("Ally", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            caster.Position = new Vector2(0, 0);
            ally.Position = new Vector2(50, 0); // Within range
            
            var characters = new System.Collections.Generic.List<Character> { caster, ally };
            map.SetCharacters(characters);
            
            var ability = new AttackPowerBuffAbility();

            // Act
            ability.Use(caster, null, 10f);

            // Assert
            Assert.Single(caster.ActiveAuras);
            Assert.Single(ally.ActiveAuras);
        }

        [Fact]
        public void Debug_GroupTargeting_Step3_CharactersAreOnMap()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var ally = new Character("Ally", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            var characters = new System.Collections.Generic.List<Character> { caster, ally };
            map.SetCharacters(characters);

            // Act & Assert
            Assert.Equal(2, map.Characters.Count());
            Assert.Contains(caster, map.Characters);
            Assert.Contains(ally, map.Characters);
        }

        [Fact]
        public void Debug_GroupTargeting_Step4_GetGroupTargetsWorks()
        {
            // Arrange
            var (map, worldMap) = CreateTestMapAndWorld();
            var caster = new Character("Caster", CharacterClass.Prêtre, 100, 10, map, worldMap);
            var ally = new Character("Ally", CharacterClass.Guerrier, 100, 10, map, worldMap);
            
            caster.Position = new Vector2(0, 0);
            ally.Position = new Vector2(50, 0); // Within 96 unit range
            
            var characters = new System.Collections.Generic.List<Character> { caster, ally };
            map.SetCharacters(characters);
            
            var ability = new AttackPowerBuffAbility(); // Range 96f

            // Act
            var targets = ability.GetGroupTargets(caster);

            // Assert
            Assert.Equal(2, targets.Count());
        }
    }
}