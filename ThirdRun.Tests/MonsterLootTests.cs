using System;
using MonogameRPG.Monsters;
using ThirdRun.Items;
using Xunit;

namespace ThirdRun.Tests;

public class MonsterLootTests
{
    [Fact]
    public void MonsterType_Constructor_ShouldCreateDefaultLootTable()
    {
        // Arrange & Act
        var monsterType = new MonsterType("Test Orc", 15, 3, "Monsters/orc", 2);
        
        // Assert
        Assert.NotNull(monsterType.LootTable);
        Assert.Equal(100, monsterType.LootTable.GetTotalWeight());
        Assert.Single(monsterType.LootTable.GetEntries());
        
        var entry = monsterType.LootTable.GetEntries()[0];
        Assert.IsType<RandomLootEntry>(entry);
        var randomEntry = (RandomLootEntry)entry;
        Assert.Equal(ItemRarity.Common, randomEntry.Rarity);
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

    [Fact]
    public void MonsterType_CanSetCustomLootTable()
    {
        // Arrange
        var monsterType = new MonsterType("Boss Orc", 100, 15, "Monsters/orc", 5);
        var customLootTable = new LootTable(
            new RandomLootEntry(50, ItemRarity.Rare),
            new UniqueLootEntry(10, UniqueItemRepository.ExcaliburSword),
            new RandomLootEntry(40, ItemRarity.Common)
        );

        // Act
        monsterType.LootTable = customLootTable;

        // Assert
        Assert.Equal(customLootTable, monsterType.LootTable);
        Assert.Equal(100, monsterType.LootTable.GetTotalWeight());
        Assert.Equal(3, monsterType.LootTable.GetEntries().Count);
    }

    [Fact] 
    public void MonsterType_DefaultLootTable_ShouldGenerateItems()
    {
        // Arrange
        var monsterType = new MonsterType("Test Monster", 20, 5, "Monsters/test", 3);
        var random = new Random(12345);

        // Act
        var item = monsterType.LootTable!.GenerateItem(monsterType.Level, random);

        // Assert
        Assert.NotNull(item);
        Assert.True(item.ItemLevel >= 1);
    }
}