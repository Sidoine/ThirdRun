using System.Linq;
using Xunit;
using MonogameRPG.Map;
using ThirdRun.Data.Abilities;
using System;

namespace ThirdRun.Tests
{
    /// <summary>
    /// Test to verify that removing the Techniques property didn't break Character functionality
    /// and that the Abilities system is still working properly.
    /// </summary>
    public class TechniquesRemovalTest
    {
        [Fact]
        public void Character_ShouldHaveAbilitiesFromBaseClass()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            // Act
            var character = new Character("Test", CharacterClass.Chasseur, 100, 10, worldMap.CurrentMap, worldMap);
            
            // Assert - Character should have abilities from Unit base class
            Assert.NotNull(character.Abilities);
            Assert.True(character.Abilities.Count > 0);
            
            // Should have default melee ability plus class-specific abilities
            Assert.Contains(character.Abilities, a => a is MeleeAttackAbility);
            Assert.Contains(character.Abilities, a => a is RangedAttackAbility); // Chasseur specific
        }

        [Fact] 
        public void Character_ShouldNotHaveTechniquesProperty()
        {
            // Arrange & Act - Use reflection to check that Techniques property no longer exists
            var characterType = typeof(Character);
            var techniquesProperty = characterType.GetProperty("Techniques");
            
            // Assert
            Assert.Null(techniquesProperty);
        }
    }
}