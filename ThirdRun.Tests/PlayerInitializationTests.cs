using MonogameRPG.Map;
using System.Linq;
using ThirdRun.Data;

namespace ThirdRun.Tests
{
    public class PlayerInitializationTests
    {
        [Fact]
        public void Player_ShouldStartWithExactlyFourCharacters()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            // Act
            var player = new Player(worldMap);
            
            // Assert
            Assert.Equal(4, player.Characters.Count);
        }
        
        [Fact]
        public void Player_ShouldStartWithAllFourRequiredClasses()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            // Act
            var player = new Player(worldMap);
            var characterClasses = player.Characters.Select(c => c.Class).ToList();
            
            // Assert
            Assert.Contains(CharacterClass.Guerrier, characterClasses);
            Assert.Contains(CharacterClass.Chasseur, characterClasses);
            Assert.Contains(CharacterClass.Mage, characterClasses);
            Assert.Contains(CharacterClass.Prêtre, characterClasses);
        }
        
        [Fact]
        public void Player_ShouldStartWithEachClassOnlyOnce()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            // Act
            var player = new Player(worldMap);
            var characterClasses = player.Characters.Select(c => c.Class).ToList();
            
            // Assert
            Assert.Equal(1, characterClasses.Count(c => c == CharacterClass.Guerrier));
            Assert.Equal(1, characterClasses.Count(c => c == CharacterClass.Chasseur));
            Assert.Equal(1, characterClasses.Count(c => c == CharacterClass.Mage));
            Assert.Equal(1, characterClasses.Count(c => c == CharacterClass.Prêtre));
        }
        
        [Fact]
        public void Player_CharactersShouldHaveValidNames()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            // Act
            var player = new Player(worldMap);
            
            // Assert
            Assert.All(player.Characters, character => 
            {
                Assert.NotNull(character.Name);
                Assert.NotEmpty(character.Name);
            });
        }
    }
}