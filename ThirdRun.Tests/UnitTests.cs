using Microsoft.Xna.Framework;
using MonogameRPG;
using MonogameRPG.Map;

namespace ThirdRun.Tests;

public class UnitTests
{
    [Fact]
    public void Unit_IsDead_ReturnsTrueWhenHealthIsZero()
    {
        // Arrange
        var unit = new TestUnit
        {
            CurrentHealth = 0,
            MaxHealth = 100
        };
        
        // Act & Assert
        Assert.True(unit.IsDead);
    }
    
    [Fact] 
    public void Unit_IsDead_ReturnsTrueWhenHealthIsNegative()
    {
        // Arrange
        var unit = new TestUnit
        {
            CurrentHealth = -5,
            MaxHealth = 100
        };
        
        // Act & Assert
        Assert.True(unit.IsDead);
    }
    
    [Fact]
    public void Unit_IsDead_ReturnsFalseWhenHealthIsPositive()
    {
        // Arrange
        var unit = new TestUnit
        {
            CurrentHealth = 50,
            MaxHealth = 100
        };
        
        // Act & Assert
        Assert.False(unit.IsDead);
    }
    
    [Fact]
    public void Unit_Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var position = new Vector2(10, 20);
        int currentHealth = 80;
        int maxHealth = 100;
        int attackPower = 15;
        
        // Act
        var unit = new TestUnit
        {
            Position = position,
            CurrentHealth = currentHealth,
            MaxHealth = maxHealth,
            AttackPower = attackPower
        };
        
        // Assert
        Assert.Equal(position, unit.Position);
        Assert.Equal(currentHealth, unit.CurrentHealth);
        Assert.Equal(maxHealth, unit.MaxHealth);
        Assert.Equal(attackPower, unit.AttackPower);
    }
    
    // Test implementation of abstract Unit class
    private class TestUnit : Unit
    {
        public TestUnit() : base(new Map(Point.Zero), new WorldMap())
        {
            // No additional implementation needed for testing basic properties
        }
    }
}