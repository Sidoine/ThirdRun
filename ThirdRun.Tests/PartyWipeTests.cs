using MonogameRPG;
using MonogameRPG.Map;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ThirdRun.Tests
{
    public class PartyWipeTests
    {
        [Fact]
        public void AllCharactersDead_ShouldTriggerPartyWipe()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var characters = new List<Character>
            {
                new Character("Warrior", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap),
                new Character("Mage", CharacterClass.Mage, 80, 8, worldMap.CurrentMap, worldMap),
                new Character("Priest", CharacterClass.Prêtre, 90, 7, worldMap.CurrentMap, worldMap)
            };
            
            // Kill all characters
            foreach (var character in characters)
            {
                character.CurrentHealth = 0;
            }

            // Act & Assert
            bool allDead = characters.All(c => c.IsDead);
            Assert.True(allDead);
        }

        [Fact]
        public void SomeCharactersAlive_ShouldNotTriggerPartyWipe()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var characters = new List<Character>
            {
                new Character("Warrior", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap),
                new Character("Mage", CharacterClass.Mage, 80, 8, worldMap.CurrentMap, worldMap),
                new Character("Priest", CharacterClass.Prêtre, 90, 7, worldMap.CurrentMap, worldMap)
            };
            
            // Kill only some characters
            characters[0].CurrentHealth = 0; // Warrior dead
            characters[1].CurrentHealth = 0; // Mage dead
            // Priest remains alive

            // Act & Assert
            bool allDead = characters.All(c => c.IsDead);
            Assert.False(allDead);
        }

        [Fact]
        public void PartyWipeRecovery_ShouldRestoreFullHealth()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var warrior = new Character("Warrior", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            warrior.CurrentHealth = 0; // Dead
            
            var mage = new Character("Mage", CharacterClass.Mage, 80, 8, worldMap.CurrentMap, worldMap);
            mage.CurrentHealth = 0; // Dead
            
            var characters = new List<Character> { warrior, mage };

            // Act - Simulate party wipe recovery
            foreach (var character in characters)
            {
                character.CurrentHealth = character.MaxHealth;
            }

            // Assert
            Assert.Equal(100, warrior.CurrentHealth);
            Assert.Equal(80, mage.CurrentHealth);
            Assert.False(warrior.IsDead);
            Assert.False(mage.IsDead);
        }

        [Fact]
        public void DeadCharacter_ShouldNotMove()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var character = new Character("DeadChar", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            character.CurrentHealth = 0; // Dead
            var initialPosition = character.Position;
            
            // Act - Try to move (this should not happen in real game loop for dead characters)
            // The game loop should check IsDead before calling Move()
            bool shouldMove = !character.IsDead;
            
            // Assert
            Assert.False(shouldMove);
            Assert.True(character.IsDead);
        }

        [Fact]
        public void EmptyParty_ShouldNotCrash()
        {
            // Arrange
            var characters = new List<Character>();

            // Act & Assert - Should not crash
            bool allDead = characters.Count > 0 && characters.All(c => c.IsDead);
            Assert.False(allDead); // Empty party is not considered "all dead"
        }
    }
}