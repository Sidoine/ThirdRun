using System;
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
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            // Act
            var player = new Player(worldMap, new Random(12345));
            
            // Assert
            Assert.Equal(4, player.Characters.Count);
        }
        
        [Fact]
        public void Player_ShouldStartWithAllFourRequiredClasses()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            // Act
            var player = new Player(worldMap, new Random(12345));
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
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            // Act
            var player = new Player(worldMap, new Random(12345));
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
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            // Act
            var player = new Player(worldMap, new Random(12345));
            
            // Assert
            Assert.All(player.Characters, character => 
            {
                Assert.NotNull(character.Name);
                Assert.NotEmpty(character.Name);
            });
        }
        
        [Fact]
        public void Player_WarriorShouldStartWithExactlyFiveRandomItems()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            
            // Act
            var player = new Player(worldMap, new Random(12345));
            var warrior = player.Characters.First(c => c.Class == CharacterClass.Guerrier);
            
            // Assert
            Assert.Equal(5, warrior.Inventory.GetItems().Count);
            Assert.All(warrior.Inventory.GetItems(), item => 
            {
                Assert.NotNull(item);
                Assert.NotNull(item.Name);
                Assert.NotEmpty(item.Name);
                // All items should have image paths since they're generated from templates
                Assert.NotNull(item.ImagePath);
                Assert.NotEmpty(item.ImagePath);
            });
        }
    }
}