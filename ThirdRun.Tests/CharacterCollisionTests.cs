using Xunit;
using Microsoft.Xna.Framework;
using MonogameRPG.Map;
using MonogameRPG.Monsters;
using MonogameRPG;
using System.Collections.Generic;
using System.Linq;

namespace ThirdRun.Tests
{
    public class CharacterCollisionTests
    {
        [Fact]
        public void Character_MoveTo_StopsWhenCollidingWithOtherUnit()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var character1 = new Character("Hero", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            var character2 = new Character("Blocker", CharacterClass.Mage, 80, 8, worldMap.CurrentMap, worldMap);
            
            // Position character1 at tile (0,0) and character2 at tile (1,0)
            character1.Position = new Vector2(Map.TileWidth / 2, Map.TileHeight / 2);
            character2.Position = new Vector2(Map.TileWidth * 1.5f, Map.TileHeight / 2);
            
            worldMap.SetCharacters(new List<Character> { character1, character2 });
            
            var initialPosition = character1.Position;
            
            // Act - try to move character1 towards character2 (should be blocked)
            var targetPosition = character2.Position; // Direct collision
            
            // Simulate several movement attempts (since MoveTo moves gradually)
            for (int i = 0; i < 50; i++) // Try many times to ensure movement would complete if unblocked
            {
                var oldPosition = character1.Position;
                character1.Move(new List<Monster>());  // Empty monsters list, so it should try to move to next map
                
                // If position didn't change, character is blocked
                if (character1.Position == oldPosition)
                    break;
            }
            
            // Assert - character1 should not have reached character2's position
            var distance = Vector2.Distance(character1.Position, character2.Position);
            Assert.True(distance >= Map.TileWidth, 
                $"Character should be blocked by collision. Distance: {distance}, TileWidth: {Map.TileWidth}");
        }
        
        [Fact]  
        public void Character_MoveTo_AllowsMovementWhenNoCollision()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var character = new Character("Hero", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            character.Position = new Vector2(Map.TileWidth / 2, Map.TileHeight / 2); // Center of tile (0,0)
            
            worldMap.SetCharacters(new List<Character> { character });
            
            var initialPosition = character.Position;
            
            // Act - move to an empty area (several tiles away)
            var targetPosition = new Vector2(Map.TileWidth * 5.5f, Map.TileHeight * 5.5f); // Center of tile (5,5)
            
            // Simulate movement for a reasonable time
            var movementAttempts = 0;
            var maxAttempts = 100;
            
            while (Vector2.Distance(character.Position, targetPosition) > Map.TileWidth / 2 && movementAttempts < maxAttempts)
            {
                // Create a mock monster far away to trigger movement towards targetPosition
                var mockMonster = new Monster(MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(1, 1), worldMap.CurrentMap, worldMap);
                mockMonster.Position = targetPosition;
                
                character.Move(new List<Monster> { mockMonster });
                movementAttempts++;
            }
            
            // Assert - character should have moved significantly from initial position
            var distanceMoved = Vector2.Distance(initialPosition, character.Position);
            Assert.True(distanceMoved > Map.TileWidth, 
                $"Character should have moved when no collision. Distance moved: {distanceMoved}");
        }
        
        [Fact]
        public void Character_MoveTo_UpdatesUnitPositionCorrectly()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var character = new Character("Hero", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
            var initialTilePosition = new Vector2(Map.TileWidth / 2, Map.TileHeight / 2); // Center of tile (0,0)
            character.Position = initialTilePosition;
            
            worldMap.SetCharacters(new List<Character> { character });
            
            // Verify initial position tracking
            var initialUnitAtTile = worldMap.CurrentMap.GetUnitAtTile(0, 0);
            Assert.Equal(character, initialUnitAtTile);
            
            // Act - move character to a different tile
            var targetTilePosition = new Vector2(Map.TileWidth * 2.5f, Map.TileHeight * 2.5f); // Center of tile (2,2)
            var mockMonster = new Monster(MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(1, 1), worldMap.CurrentMap, worldMap);
            mockMonster.Position = targetTilePosition;
            
            // Simulate movement
            for (int i = 0; i < 50; i++)
            {
                character.Move(new List<Monster> { mockMonster });
                
                // Check if we've moved to a different tile
                var currentTileCoords = worldMap.CurrentMap.WorldPositionToTileCoordinates(character.Position);
                if (currentTileCoords.HasValue && (currentTileCoords.Value.X != 0 || currentTileCoords.Value.Y != 0))
                {
                    break;
                }
            }
            
            // Assert - unit tracking should be updated
            var currentTileCoords2 = worldMap.CurrentMap.WorldPositionToTileCoordinates(character.Position);
            Assert.True(currentTileCoords2.HasValue);
            
            var unitAtNewTile = worldMap.CurrentMap.GetUnitAtTile(currentTileCoords2.Value.X, currentTileCoords2.Value.Y);
            Assert.Equal(character, unitAtNewTile);
            
            // Original tile should now be empty (unless character hasn't moved)
            var unitAtOriginalTile = worldMap.CurrentMap.GetUnitAtTile(0, 0);
            if (currentTileCoords2.Value.X != 0 || currentTileCoords2.Value.Y != 0)
            {
                Assert.NotEqual(character, unitAtOriginalTile);
            }
        }
    }
}