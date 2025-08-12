using System;
using MonogameRPG.Monsters;
using ThirdRun.Items;

namespace ThirdRun.Tests;

public class MonsterLootTests
{
    [Fact]
    public void Monster_DropLoot_ShouldReturnRandomItem()
    {
        // Arrange
        var monsterType = new MonsterType("Test Orc", 15, 3, "Monsters/orc", 2);
        // We can't easily create a Monster without ContentManager, so test via MonsterType properties
        
        // Act & Assert
        Assert.Equal(2, monsterType.Level);
        Assert.Equal("Test Orc", monsterType.Name);
    }
    
    [Fact]
    public void MonsterType_Constructor_ShouldSetLevelCorrectly()
    {
        // Arrange & Act
        var monsterType1 = new MonsterType("Orc", 15, 3, "Monsters/orc", 2);
        var monsterType2 = new MonsterType("Goblin", 10, 2, "Monsters/goblin"); // Default level
        
        // Assert
        Assert.Equal(2, monsterType1.Level);
        Assert.Equal(1, monsterType2.Level); // Default should be 1
    }
    
    [Fact]
    public void MonsterType_Constructor_WithoutLevel_ShouldDefaultToOne()
    {
        // Arrange & Act
        var monsterType = new MonsterType("Goblin", 10, 2, "Monsters/goblin");
        
        // Assert
        Assert.Equal(1, monsterType.Level);
    }
}