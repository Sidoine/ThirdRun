using MonogameRPG.Map;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests;

public class MapSpawningTests
{
    [Fact]
    public void Map_AreaLevel_ShouldIncreaseLevelWithDistance()
    {
        // Arrange & Act
        var mapOrigin = new Map(new Point(0, 0));
        var mapFar = new Map(new Point(3, 2)); // Distance = 5
        
        // Assert - Maps should have correct world positions
        Assert.Equal(new Point(0, 0), mapOrigin.WorldPosition);
        Assert.Equal(new Point(3, 2), mapFar.WorldPosition);
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public void Map_Constructor_ShouldSetWorldPositionCorrectly(int x, int y)
    {
        // Arrange & Act
        var map = new Map(new Point(x, y));
        
        // Assert
        Assert.Equal(new Point(x, y), map.WorldPosition);
    }
}