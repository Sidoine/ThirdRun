using Xunit;
using Microsoft.Xna.Framework;
using MonogameRPG.Map;
using System.Linq;

namespace ThirdRun.Tests
{
    public class MapGenerationIntegrationTests
    {
        [Fact]
        public void Map_GenerateRandomMap_UsesAdvancedGenerator()
        {
            // Arrange
            var map = new Map(Point.Zero);
            
            // Act
            map.GenerateRandomMap(3);
            
            // Assert
            Assert.NotNull(map.Tiles);
            Assert.Equal(Map.GridWidth, map.Tiles.GetLength(0));
            Assert.Equal(Map.GridHeight, map.Tiles.GetLength(1));
            
            // Verify all tiles are initialized
            for (int x = 0; x < Map.GridWidth; x++)
            {
                for (int y = 0; y < Map.GridHeight; y++)
                {
                    Assert.NotNull(map.Tiles[x, y]);
                    Assert.NotNull(map.Tiles[x, y].Name);
                }
            }
        }
        
        [Fact]
        public void Map_GenerateRandomMap_ProducesVarietyOfTileTypes()
        {
            // Arrange
            var map = new Map(Point.Zero);
            
            // Act
            map.GenerateRandomMap(2);
            
            // Assert
            var tileTypes = new HashSet<TileTypeEnum>();
            for (int x = 0; x < Map.GridWidth; x++)
            {
                for (int y = 0; y < Map.GridHeight; y++)
                {
                    tileTypes.Add(map.Tiles[x, y].Type);
                }
            }
            
            // Should have at least 3 different tile types with advanced generation
            Assert.True(tileTypes.Count >= 3, 
                $"Expected at least 3 different tile types, but got {tileTypes.Count}: {string.Join(", ", tileTypes)}");
        }
        
        [Fact]
        public void Map_GenerateRandomMap_HasWalkableTiles()
        {
            // Arrange
            var map = new Map(Point.Zero);
            
            // Act
            map.GenerateRandomMap(2);
            
            // Assert
            bool hasWalkableTiles = false;
            for (int x = 0; x < Map.GridWidth; x++)
            {
                for (int y = 0; y < Map.GridHeight; y++)
                {
                    if (map.Tiles[x, y].IsWalkable)
                    {
                        hasWalkableTiles = true;
                        break;
                    }
                }
                if (hasWalkableTiles) break;
            }
            
            Assert.True(hasWalkableTiles, "Generated map should have walkable tiles for monster spawning");
        }
        
        [Fact]
        public void Map_GenerateRandomMap_CreatesMonsterSpawnPoints()
        {
            // Arrange
            var map = new Map(Point.Zero);
            int requestedSpawnCount = 4;
            
            // Act
            map.GenerateRandomMap(requestedSpawnCount);
            var spawnPoints = map.GetMonsterSpawnPoints();
            
            // Assert
            Assert.NotNull(spawnPoints);
            Assert.True(spawnPoints.Count > 0, "Should create at least some monster spawn points");
            Assert.True(spawnPoints.Count <= requestedSpawnCount, "Should not exceed requested spawn count");
            
            // Verify spawn points are on walkable tiles
            foreach (var spawnPoint in spawnPoints)
            {
                int tileX = (int)spawnPoint.X;
                int tileY = (int)spawnPoint.Y;
                Assert.True(tileX >= 0 && tileX < Map.GridWidth, "Spawn point X should be within map bounds");
                Assert.True(tileY >= 0 && tileY < Map.GridHeight, "Spawn point Y should be within map bounds");
                Assert.True(map.Tiles[tileX, tileY].IsWalkable, "Monster spawn points should be on walkable tiles");
            }
        }
        
        [Fact]
        public void Map_WithDifferentWorldPositions_GeneratesDifferentMaps()
        {
            // Arrange
            var map1 = new Map(new Point(0, 0));
            var map2 = new Map(new Point(1, 0));
            
            // Act
            map1.GenerateRandomMap(2);
            map2.GenerateRandomMap(2);
            
            // Assert
            bool foundDifference = false;
            for (int x = 0; x < Map.GridWidth && !foundDifference; x++)
            {
                for (int y = 0; y < Map.GridHeight && !foundDifference; y++)
                {
                    if (map1.Tiles[x, y].Type != map2.Tiles[x, y].Type)
                    {
                        foundDifference = true;
                    }
                }
            }
            
            Assert.True(foundDifference, "Maps with different world positions should generate different terrain");
        }
        
        [Fact]
        public void Map_SameWorldPosition_GeneratesSameMap()
        {
            // Arrange
            var map1 = new Map(new Point(2, 3));
            var map2 = new Map(new Point(2, 3));
            
            // Act
            map1.GenerateRandomMap(3);
            map2.GenerateRandomMap(3);
            
            // Assert
            for (int x = 0; x < Map.GridWidth; x++)
            {
                for (int y = 0; y < Map.GridHeight; y++)
                {
                    Assert.Equal(map1.Tiles[x, y].Type, map2.Tiles[x, y].Type);
                    Assert.Equal(map1.Tiles[x, y].IsWalkable, map2.Tiles[x, y].IsWalkable);
                }
            }
        }
        
        [Theory]
        [InlineData(TileTypeEnum.Tree)]
        [InlineData(TileTypeEnum.Building)]
        [InlineData(TileTypeEnum.Road)]
        public void Map_AdvancedGeneration_CanProduceNewTileTypes(TileTypeEnum expectedTileType)
        {
            // Arrange & Act
            bool foundExpectedTile = false;
            
            // Generate multiple maps to increase chances of finding the expected tile type
            for (int mapAttempt = 0; mapAttempt < 10 && !foundExpectedTile; mapAttempt++)
            {
                var map = new Map(new Point(mapAttempt, mapAttempt));
                map.GenerateRandomMap(2);
                
                for (int x = 0; x < Map.GridWidth && !foundExpectedTile; x++)
                {
                    for (int y = 0; y < Map.GridHeight && !foundExpectedTile; y++)
                    {
                        if (map.Tiles[x, y].Type == expectedTileType)
                        {
                            foundExpectedTile = true;
                        }
                    }
                }
            }
            
            // Assert - With advanced generation, we should be able to find these tile types
            // Note: This test may occasionally fail due to randomness, but should generally pass
            Assert.True(foundExpectedTile, 
                $"Advanced map generation should be capable of producing {expectedTileType} tiles");
        }
    }
}