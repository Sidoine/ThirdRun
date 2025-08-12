using System;
using ThirdRun.Items;

namespace ThirdRun.Tests;

public class EquipmentTests
{
    [Fact]
    public void Equipment_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        string name = "Test Equipment";
        string description = "Test equipment piece";
        int value = 200;
        int bonusStats = 10;
        
        // Act
        var equipment = new TestEquipment(name, description, value, bonusStats);
        
        // Assert
        Assert.Equal(name, equipment.Name);
        Assert.Equal(description, equipment.Description);
        Assert.Equal(value, equipment.Value);
        Assert.Equal(bonusStats, equipment.BonusStats);
    }
    
    [Fact]
    public void Equipment_InheritsFromItem()
    {
        // Arrange & Act
        var equipment = new TestEquipment("Test", "Test", 100, 5);
        
        // Assert
        Assert.IsAssignableFrom<Item>(equipment);
    }
    
    // Test class to test abstract Equipment class
    private class TestEquipment : Equipment
    {
        public TestEquipment(string name, string description, int value, int bonusStats) 
            : base(name, description, value, bonusStats)
        {
        }
    }
}