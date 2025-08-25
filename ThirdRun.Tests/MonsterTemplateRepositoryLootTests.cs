using System;
using System.Linq;
using MonogameRPG.Monsters;
using ThirdRun.Items;
using Xunit;

namespace ThirdRun.Tests;

public class MonsterTemplateRepositoryLootTests
{
    [Fact]
    public void CreateRandomMonsterType_ShouldHaveConfiguredLootTable()
    {
        // Arrange
        var random = new Random(12345);

        // Act
        var monsterType = MonsterTemplateRepository.CreateRandomMonsterType(random);

        // Assert
        Assert.NotNull(monsterType.LootTable);
        Assert.True(monsterType.LootTable.GetTotalWeight() > 0);
        Assert.True(monsterType.LootTable.GetEntries().Count > 0);
    }

    [Fact]
    public void CreateRandomMonsterTypeForLevel_LowLevel_ShouldHaveMostlyCommonLoot()
    {
        // Arrange
        var random = new Random(12345);

        // Act
        var monsterType = MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(1, 2, random);

        // Assert
        Assert.NotNull(monsterType.LootTable);
        Assert.True(monsterType.Level <= 2);
        
        // Should have at least one RandomLootEntry with Common rarity
        var entries = monsterType.LootTable.GetEntries();
        var commonEntries = entries.OfType<RandomLootEntry>()
            .Where(e => e.Rarity == ItemRarity.Common);
        Assert.True(commonEntries.Any());
    }

    [Fact]
    public void CreateRandomMonsterTypeForLevel_HighLevel_ShouldHaveMoreRareAndEpicLoot()
    {
        // Arrange
        var random = new Random(12345);

        // Act
        var monsterType = MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(5, 6, random);

        // Assert
        Assert.NotNull(monsterType.LootTable);
        Assert.True(monsterType.Level >= 5);
        
        // Should have rare and epic entries
        var entries = monsterType.LootTable.GetEntries();
        var rareEntries = entries.OfType<RandomLootEntry>()
            .Where(e => e.Rarity == ItemRarity.Rare);
        var epicEntries = entries.OfType<RandomLootEntry>()
            .Where(e => e.Rarity == ItemRarity.Epic);
            
        Assert.True(rareEntries.Any());
        Assert.True(epicEntries.Any());
    }

    [Fact]
    public void ConfigureLootTable_DragonType_ShouldHaveDragonSlayerAxe()
    {
        // Arrange
        var random = new Random(12345);

        // Act - Generate many monsters to increase chance of getting a dragon
        MonsterType? dragonType = null;
        for (int i = 0; i < 100 && dragonType == null; i++)
        {
            var monsterType = MonsterTemplateRepository.CreateRandomMonsterType(random);
            if (monsterType.Name.ToLowerInvariant().Contains("dragon"))
            {
                dragonType = monsterType;
            }
        }

        // Assert - If we found a dragon, it should have DragonSlayer axe in loot table
        if (dragonType != null)
        {
            var entries = dragonType.LootTable!.GetEntries();
            var uniqueEntries = entries.OfType<UniqueLootEntry>();
            var dragonSlayerEntry = uniqueEntries.FirstOrDefault(e => 
                e.UniqueItemTemplate == UniqueItemRepository.DragonSlayerAxe);
            
            Assert.NotNull(dragonSlayerEntry);
        }
    }

    [Fact]
    public void GeneratedLootTable_ShouldGenerateItems()
    {
        // Arrange
        var random = new Random(12345);
        var monsterType = MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(3, 4, random);

        // Act
        var items = new Item[10];
        for (int i = 0; i < 10; i++)
        {
            items[i] = monsterType.LootTable!.GenerateItem(monsterType.Level, random);
        }

        // Assert
        Assert.All(items, item => Assert.NotNull(item));
        Assert.All(items, item => Assert.True(item.ItemLevel >= 1));
        Assert.All(items, item => Assert.True(item.Value > 0));
    }
}