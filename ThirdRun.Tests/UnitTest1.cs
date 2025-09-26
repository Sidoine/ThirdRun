using System;
using Microsoft.Xna.Framework;
using ThirdRun.Utils;

namespace ThirdRun.Tests;

public class HelpersTests
{
    [Fact]
    public void RandomNumber_ReturnsValueWithinRange()
    {
        // Arrange
        int min = 5;
        int max = 10;
        var random = new Random(12345);
        
        // Act
        int result = random.Next(min, max);
        
        // Assert
        Assert.True(result >= min && result < max);
    }
    
    [Fact]
    public void CalculateDistance_ReturnsCorrectDistance()
    {
        // Arrange  
        Vector2 pointA = new Vector2(0, 0);
        Vector2 pointB = new Vector2(3, 4);
        float expectedDistance = 5.0f; // 3-4-5 triangle
        
        // Act
        float result = Helpers.CalculateDistance(pointA, pointB);
        
        // Assert
        Assert.True(Math.Abs(result - expectedDistance) < 0.1f); // Check within tolerance
    }
    
    [Fact]
    public void CalculateDistance_SamePoint_ReturnsZero()
    {
        // Arrange
        Vector2 point = new Vector2(5, 10);
        
        // Act
        float result = Helpers.CalculateDistance(point, point);
        
        // Assert
        Assert.Equal(0.0f, result);
    }
}