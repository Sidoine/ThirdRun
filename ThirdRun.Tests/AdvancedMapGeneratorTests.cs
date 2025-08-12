using System;
using Xunit;
using Microsoft.Xna.Framework;
using MonogameRPG.Map;
using ThirdRun.Data.Map;
using System.Linq;

namespace ThirdRun.Tests
{
    public class AdvancedMapGeneratorTests
    {
        [Fact]
        public void GenerateMap_ReturnsCorrectDimensions()
        {
            // Arrange
            var generator = new AdvancedMapGenerator(0);
            int width = 10;
            int height = 8;
            
            // Act
            var tiles = generator.GenerateMap(width, height, Point.Zero);
            
            // Assert
            Assert.Equal(width, tiles.GetLength(0));
            Assert.Equal(height, tiles.GetLength(1));
        }
        
        [Fact]
        public void GenerateMap_AllTilesAreInitialized()
        {
            // Arrange
            var generator = new AdvancedMapGenerator(123);
            int width = 5;
            int height = 5;
            
            // Act
            var tiles = generator.GenerateMap(width, height, Point.Zero);
            
            // Assert
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Assert.NotNull(tiles[x, y]);
                    Assert.NotNull(tiles[x, y].Name);
                }
            }
        }
        
        [Fact]
        public void GenerateMap_ProducesVarietyOfTileTypes()
        {
            // Arrange
            var generator = new AdvancedMapGenerator(456);
            int width = 25;
            int height = 18;
            
            // Act
            var tiles = generator.GenerateMap(width, height, Point.Zero);
            
            // Assert
            var tileTypes = new HashSet<TileTypeEnum>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tileTypes.Add(tiles[x, y].Type);
                }
            }
            
            // Should have at least 3 different tile types
            Assert.True(tileTypes.Count >= 3, 
                $"Expected at least 3 different tile types, but got {tileTypes.Count}: {string.Join(", ", tileTypes)}");
        }
        
        [Fact]
        public void GenerateMap_WithSameSeed_ProducesSameMap()
        {
            // Arrange
            var generator1 = new AdvancedMapGenerator(789);
            var generator2 = new AdvancedMapGenerator(789);
            int width = 10;
            int height = 8;
            var position = new Point(1, 1);
            
            // Act
            var tiles1 = generator1.GenerateMap(width, height, position);
            var tiles2 = generator2.GenerateMap(width, height, position);
            
            // Assert
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Assert.Equal(tiles1[x, y].Type, tiles2[x, y].Type);
                }
            }
        }
        
        [Fact]
        public void GenerateMap_WithDifferentSeeds_ProducesDifferentMaps()
        {
            // Arrange
            var generator1 = new AdvancedMapGenerator(111);
            var generator2 = new AdvancedMapGenerator(222);
            int width = 15;
            int height = 12;
            
            // Act
            var tiles1 = generator1.GenerateMap(width, height, Point.Zero);
            var tiles2 = generator2.GenerateMap(width, height, Point.Zero);
            
            // Assert
            bool foundDifference = false;
            for (int x = 0; x < width && !foundDifference; x++)
            {
                for (int y = 0; y < height && !foundDifference; y++)
                {
                    if (tiles1[x, y].Type != tiles2[x, y].Type)
                    {
                        foundDifference = true;
                    }
                }
            }
            
            Assert.True(foundDifference, "Maps with different seeds should produce different terrain");
        }
        
        [Fact]
        public void GenerateMap_HasWalkableTiles()
        {
            // Arrange
            var generator = new AdvancedMapGenerator(333);
            int width = 20;
            int height = 15;
            
            // Act
            var tiles = generator.GenerateMap(width, height, Point.Zero);
            
            // Assert
            bool hasWalkableTiles = false;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tiles[x, y].IsWalkable)
                    {
                        hasWalkableTiles = true;
                        break;
                    }
                }
                if (hasWalkableTiles) break;
            }
            
            Assert.True(hasWalkableTiles, "Generated map should have at least some walkable tiles");
        }
        
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        public void GenerateMap_WithDifferentWorldPositions_ProducesDifferentMaps(int worldX, int worldY)
        {
            // Arrange
            var generator = new AdvancedMapGenerator(444);
            int width = 10;
            int height = 8;
            var position1 = Point.Zero;
            var position2 = new Point(worldX, worldY);
            
            // Act
            var tiles1 = generator.GenerateMap(width, height, position1);
            var tiles2 = generator.GenerateMap(width, height, position2);
            
            // Assert
            if (position1 != position2)
            {
                bool foundDifference = false;
                for (int x = 0; x < width && !foundDifference; x++)
                {
                    for (int y = 0; y < height && !foundDifference; y++)
                    {
                        if (tiles1[x, y].Type != tiles2[x, y].Type)
                        {
                            foundDifference = true;
                        }
                    }
                }
                
                Assert.True(foundDifference, $"Maps at different world positions ({position1} vs {position2}) should produce different terrain");
            }
        }
    }
}