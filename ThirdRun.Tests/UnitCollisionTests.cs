using Xunit;
using Microsoft.Xna.Framework;
using MonogameRPG.Map;
using MonogameRPG.Monsters;
using MonogameRPG;
using System.Collections.Generic;
using System.Linq;

namespace ThirdRun.Tests
{
    public class UnitCollisionTests
    {
        [Fact]
        public void Map_UnitGrid_InitializedWithMapGeneration()
        {
            // Arrange
            var map = new Map(Point.Zero);
            
            // Act
            map.GenerateRandomMap();
            
            // Assert
            Assert.NotNull(map.UnitGrid);
            Assert.Equal(Map.GridWidth, map.UnitGrid.GetLength(0));
            Assert.Equal(Map.GridHeight, map.UnitGrid.GetLength(1));
            
            // Initially all tiles should be empty
            for (int x = 0; x < Map.GridWidth; x++)
            {
                for (int y = 0; y < Map.GridHeight; y++)
                {
                    Assert.Null(map.UnitGrid[x, y]);
                }
            }
        }
        
        [Fact]
        public void Map_WorldPositionToTileCoordinates_ConvertsCorrectly()
        {
            // Arrange
            var map = new Map(Point.Zero);
            map.GenerateRandomMap();
            
            // Act & Assert
            // Test corner cases
            var result1 = map.WorldPositionToTileCoordinates(Vector2.Zero);
            Assert.True(result1.HasValue);
            Assert.Equal(0, result1.Value.X);
            Assert.Equal(0, result1.Value.Y);
            
            // Test center of first tile
            var result2 = map.WorldPositionToTileCoordinates(new Vector2(Map.TileWidth / 2, Map.TileHeight / 2));
            Assert.True(result2.HasValue);
            Assert.Equal(0, result2.Value.X);
            Assert.Equal(0, result2.Value.Y);
            
            // Test second tile
            var result3 = map.WorldPositionToTileCoordinates(new Vector2(Map.TileWidth, Map.TileHeight));
            Assert.True(result3.HasValue);
            Assert.Equal(1, result3.Value.X);
            Assert.Equal(1, result3.Value.Y);
            
            // Test out of bounds (use very large negative values)
            var result4 = map.WorldPositionToTileCoordinates(new Vector2(-1000, -1000));
            Assert.False(result4.HasValue);
            
            // Test out of bounds (beyond map size)
            var result5 = map.WorldPositionToTileCoordinates(new Vector2(Map.GridWidth * Map.TileWidth + 10, Map.GridHeight * Map.TileHeight + 10));
            Assert.False(result5.HasValue);
        }
        
        [Fact]
        public void Map_UpdateUnitPosition_TracksUnitCorrectly()
        {
            // Arrange
            var map = new Map(Point.Zero);
            map.GenerateRandomMap();
            
            var mockUnit = new MockUnit();
            mockUnit.Position = new Vector2(Map.TileWidth / 2, Map.TileHeight / 2); // Center of tile (0,0)
            
            // Act
            map.UpdateUnitPosition(mockUnit);
            
            // Assert
            var unitAtTile = map.GetUnitAtTile(0, 0);
            Assert.Equal(mockUnit, unitAtTile);
            
            var unitAtPosition = map.GetUnitAtWorldPosition(mockUnit.Position);
            Assert.Equal(mockUnit, unitAtPosition);
        }
        
        [Fact]
        public void Map_UpdateUnitPosition_ClearsOldPosition()
        {
            // Arrange
            var map = new Map(Point.Zero);
            map.GenerateRandomMap();
            
            var mockUnit = new MockUnit();
            var oldPosition = new Vector2(Map.TileWidth / 2, Map.TileHeight / 2); // Tile (0,0)
            var newPosition = new Vector2(Map.TileWidth * 1.5f, Map.TileHeight * 1.5f); // Tile (1,1)
            
            mockUnit.Position = oldPosition;
            map.UpdateUnitPosition(mockUnit);
            
            // Act - move unit to new position
            mockUnit.Position = newPosition;
            map.UpdateUnitPosition(mockUnit, oldPosition);
            
            // Assert
            Assert.Null(map.GetUnitAtTile(0, 0)); // Old position should be empty
            Assert.Equal(mockUnit, map.GetUnitAtTile(1, 1)); // New position should have unit
        }
        
        [Fact]
        public void Map_SpawnMonsters_UpdatesUnitGrid()
        {
            // Arrange
            var map = new Map(Point.Zero);
            map.GenerateRandomMap();
            
            // Act
            map.SpawnMonsters();
            
            // Assert
            var monsters = map.GetMonsters();
            foreach (var monster in monsters)
            {
                var tileCoords = map.WorldPositionToTileCoordinates(monster.Position);
                Assert.True(tileCoords.HasValue);
                var unitAtTile = map.GetUnitAtTile(tileCoords.Value.X, tileCoords.Value.Y);
                Assert.Equal(monster, unitAtTile);
            }
        }
        
        [Fact]
        public void Map_RefreshUnitGrid_RebuildsGrid()
        {
            // Arrange
            var map = new Map(Point.Zero);
            map.GenerateRandomMap();
            
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var character = new Character("TestChar", CharacterClass.Guerrier, 100, 10, worldMap);
            character.Position = new Vector2(Map.TileWidth / 2, Map.TileHeight / 2);
            map.SetCharacters(new List<Character> { character });
            
            // Manually corrupt the grid
            map.UnitGrid[0, 0] = null;
            
            // Act
            map.RefreshUnitGrid();
            
            // Assert
            Assert.Equal(character, map.GetUnitAtTile(0, 0));
        }
        
        [Fact]
        public void WorldMap_FindPathAStar_AvoidsOccupiedTiles()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var character1 = new Character("Char1", CharacterClass.Guerrier, 100, 10, worldMap);
            var character2 = new Character("Char2", CharacterClass.Mage, 80, 8, worldMap);
            
            // Position characters so char2 blocks a direct path
            character1.Position = new Vector2(Map.TileWidth / 2, Map.TileHeight / 2); // (0,0)
            character2.Position = new Vector2(Map.TileWidth / 2, Map.TileHeight * 1.5f); // (0,1) - blocks vertical movement
            
            worldMap.SetCharacters(new List<Character> { character1, character2 });
            
            // Act - try to find path from char1 to position directly below char2
            var targetPosition = new Vector2(Map.TileWidth / 2, Map.TileHeight * 2.5f); // (0,2)
            var path = worldMap.FindPathAStar(character1.Position, targetPosition);
            
            // Assert - should find a path that goes around char2
            Assert.NotEmpty(path);
            
            // The path should not go through char2's position
            var char2TilePosition = new Vector2(Map.TileWidth / 2, Map.TileHeight * 1.5f);
            Assert.DoesNotContain(char2TilePosition, path);
        }
        
        [Fact]
        public void WorldMap_FindPathAStar_AllowsMovementToTarget()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var character = new Character("Attacker", CharacterClass.Guerrier, 100, 10, worldMap);
            var target = new Character("Target", CharacterClass.Mage, 80, 8, worldMap);
            
            character.Position = new Vector2(Map.TileWidth / 2, Map.TileHeight / 2); // (0,0)
            target.Position = new Vector2(Map.TileWidth / 2, Map.TileHeight * 1.5f); // (0,1)
            
            worldMap.SetCharacters(new List<Character> { character, target });
            
            // Act - find path to target's position (should be allowed for combat)
            var path = worldMap.FindPathAStar(character.Position, target.Position);
            
            // Assert - should find a direct path to the target
            Assert.NotEmpty(path);
            Assert.Equal(target.Position, path.Last());
        }
    }
    
    // Mock Unit class for testing
    public class MockUnit : Unit
    {
        public MockUnit()
        {
            Position = Vector2.Zero;
            CurrentHealth = 100;
        }
    }
}