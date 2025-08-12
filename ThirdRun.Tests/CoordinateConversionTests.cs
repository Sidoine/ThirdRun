using System;
using Microsoft.Xna.Framework;
using MonogameRPG.Map;
using ThirdRun.Data;
using Xunit;

namespace ThirdRun.Tests
{
    /// <summary>
    /// Tests for coordinate conversion logic in WorldMap.GetRelativeTileCoordinate method.
    /// This tests the core logic that was causing IndexOutOfRangeException issues.
    /// </summary>
    public class CoordinateConversionTests
    {
        [Fact]
        public void GetRelativeTileCoordinate_PositiveCoordinates_ShouldCalculateCorrectly()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();

            // Test cases for positive coordinates
            var testCases = new[]
            {
                // (absoluteX, absoluteY, expectedMapX, expectedMapY, expectedRelativeX, expectedRelativeY)
                (0, 0, 0, 0, 0, 0),                                          // Origin
                (24, 17, 0, 0, 24, 17),                                      // Edge of first map
                (25, 18, 1, 1, 0, 0),                                        // Start of adjacent map
                (49, 35, 1, 1, 24, 17),                                      // Edge of adjacent map
                (Map.GridWidth, Map.GridHeight, 1, 1, 0, 0),                 // Exactly at map boundary
                (Map.GridWidth - 1, Map.GridHeight - 1, 0, 0, Map.GridWidth - 1, Map.GridHeight - 1), // Last tile of origin map
            };

            foreach (var (absX, absY, expectedMapX, expectedMapY, expectedRelX, expectedRelY) in testCases)
            {
                // Act
                var (map, relativeX, relativeY) = worldMap.GetRelativeTileCoordinate(absX, absY);

                // Assert
                Assert.Equal(expectedRelX, relativeX);
                Assert.Equal(expectedRelY, relativeY);
                Assert.True(relativeX >= 0 && relativeX < Map.GridWidth, 
                    $"RelativeX {relativeX} should be within bounds [0, {Map.GridWidth}) for absolute coordinates ({absX}, {absY})");
                Assert.True(relativeY >= 0 && relativeY < Map.GridHeight, 
                    $"RelativeY {relativeY} should be within bounds [0, {Map.GridHeight}) for absolute coordinates ({absX}, {absY})");
            }
        }

        [Fact]
        public void GetRelativeTileCoordinate_NegativeCoordinates_ShouldCalculateCorrectly()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();

            // Test cases for negative coordinates - this is where the bug likely was
            var testCases = new[]
            {
                // (absoluteX, absoluteY, expectedRelativeX, expectedRelativeY)
                (-1, -1, Map.GridWidth - 1, Map.GridHeight - 1),    // One step back from origin
                (-1, 0, Map.GridWidth - 1, 0),                     // Negative X only
                (0, -1, 0, Map.GridHeight - 1),                    // Negative Y only
                (-25, -18, 0, 0),                                  // Full map step back
                (-26, -19, Map.GridWidth - 1, Map.GridHeight - 1), // One more step back
                (-Map.GridWidth, -Map.GridHeight, 0, 0),           // Exactly one map back
            };

            foreach (var (absX, absY, expectedRelX, expectedRelY) in testCases)
            {
                // Act
                var (map, relativeX, relativeY) = worldMap.GetRelativeTileCoordinate(absX, absY);

                // Assert
                Assert.Equal(expectedRelX, relativeX);
                Assert.Equal(expectedRelY, relativeY);
                Assert.True(relativeX >= 0 && relativeX < Map.GridWidth, 
                    $"RelativeX {relativeX} should be within bounds [0, {Map.GridWidth}) for absolute coordinates ({absX}, {absY})");
                Assert.True(relativeY >= 0 && relativeY < Map.GridHeight, 
                    $"RelativeY {relativeY} should be within bounds [0, {Map.GridHeight}) for absolute coordinates ({absX}, {absY})");
            }
        }

        [Fact]
        public void GetRelativeTileCoordinate_MixedCoordinates_ShouldCalculateCorrectly()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();

            // Test cases with mixed positive/negative coordinates
            var testCases = new[]
            {
                // (absoluteX, absoluteY, expectedRelativeX, expectedRelativeY)
                (10, -5, 10, Map.GridHeight - 5),     // Positive X, negative Y
                (-15, 12, Map.GridWidth - 15, 12),    // Negative X, positive Y
            };

            foreach (var (absX, absY, expectedRelX, expectedRelY) in testCases)
            {
                // Act
                var (map, relativeX, relativeY) = worldMap.GetRelativeTileCoordinate(absX, absY);

                // Assert
                Assert.Equal(expectedRelX, relativeX);
                Assert.Equal(expectedRelY, relativeY);
                Assert.True(relativeX >= 0 && relativeX < Map.GridWidth, 
                    $"RelativeX {relativeX} should be within bounds [0, {Map.GridWidth}) for absolute coordinates ({absX}, {absY})");
                Assert.True(relativeY >= 0 && relativeY < Map.GridHeight, 
                    $"RelativeY {relativeY} should be within bounds [0, {Map.GridHeight}) for absolute coordinates ({absX}, {absY})");
            }
        }

        [Fact]
        public void GetRelativeTileCoordinate_EdgeCases_ShouldHandleGracefully()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();

            // Test extreme edge cases
            var testCases = new[]
            {
                int.MaxValue,
                int.MinValue,
                -1000000,
                1000000,
                0,
                -Map.GridWidth,
                -Map.GridHeight,
                Map.GridWidth * 1000,
                Map.GridHeight * 1000
            };

            foreach (var x in testCases)
            {
                foreach (var y in testCases)
                {
                    // Act & Assert - Should not throw exceptions
                    var (map, relativeX, relativeY) = worldMap.GetRelativeTileCoordinate(x, y);
                    
                    // Verify relative coordinates are always in valid range
                    Assert.True(relativeX >= 0 && relativeX < Map.GridWidth,
                        $"RelativeX {relativeX} should be within bounds [0, {Map.GridWidth}) for absolute coordinates ({x}, {y})");
                    Assert.True(relativeY >= 0 && relativeY < Map.GridHeight,
                        $"RelativeY {relativeY} should be within bounds [0, {Map.GridHeight}) for absolute coordinates ({x}, {y})");
                }
            }
        }

        [Fact]
        public void GetNeighbors_WithNewCoordinateLogic_ShouldNotThrowIndexOutOfRangeException()
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();

            // Test the same edge cases that were causing issues before
            var testCells = new[]
            {
                new Point(-1, -1),         // Negative coordinates
                new Point(0, -1),          // Edge case at Y boundary
                new Point(-1, 0),          // Edge case at X boundary
                new Point(Map.GridWidth, 0),     // At X boundary
                new Point(0, Map.GridHeight),    // At Y boundary
                new Point(Map.GridWidth, Map.GridHeight), // At both boundaries
                new Point(1000, 1000),     // Very large coordinates
                new Point(-1000, -1000),   // Very negative coordinates
                new Point(Map.GridWidth - 1, Map.GridHeight - 1), // Valid edge case
            };

            foreach (var cell in testCells)
            {
                // Act & Assert - Should not throw IndexOutOfRangeException
                var neighbors = worldMap.GetNeighbors(cell);
                Assert.NotNull(neighbors);
                
                // Each neighbor should have valid coordinates when converted
                foreach (var (neighborPoint, cost) in neighbors)
                {
                    var (map, relX, relY) = worldMap.GetRelativeTileCoordinate(neighborPoint.X, neighborPoint.Y);
                    if (map != null)
                    {
                        Assert.True(relX >= 0 && relX < Map.GridWidth,
                            $"Neighbor relative X {relX} should be within bounds for point {neighborPoint}");
                        Assert.True(relY >= 0 && relY < Map.GridHeight,
                            $"Neighbor relative Y {relY} should be within bounds for point {neighborPoint}");
                    }
                }
            }
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(25, 18)] 
        [InlineData(-1, -1)]
        [InlineData(-25, -18)]
        [InlineData(100, -50)]
        [InlineData(-200, 75)]
        public void GetRelativeTileCoordinate_RoundTrip_ShouldBeConsistent(int absoluteX, int absoluteY)
        {
            // Arrange
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();

            // Act
            var (map, relativeX, relativeY) = worldMap.GetRelativeTileCoordinate(absoluteX, absoluteY);

            // Assert - The relative coordinates should always be valid
            Assert.True(relativeX >= 0 && relativeX < Map.GridWidth,
                $"RelativeX {relativeX} should be within bounds for absolute ({absoluteX}, {absoluteY})");
            Assert.True(relativeY >= 0 && relativeY < Map.GridHeight,
                $"RelativeY {relativeY} should be within bounds for absolute ({absoluteX}, {absoluteY})");
                
            // If we have a map, we should be able to reconstruct the map coordinates
            if (map != null)
            {
                int reconstructedAbsX = map.WorldPosition.X * Map.GridWidth + relativeX;
                int reconstructedAbsY = map.WorldPosition.Y * Map.GridHeight + relativeY;
                
                Assert.Equal(absoluteX, reconstructedAbsX);
                Assert.Equal(absoluteY, reconstructedAbsY);
            }
        }
    }
}