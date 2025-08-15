using System;
using System.Collections.Generic;
using System.Linq;
using ThirdRun.Items;
using Xunit;

namespace ThirdRun.Tests;

public class LootTableTests
{
    [Fact]
    public void LootTable_WithSingleEntry_ShouldAlwaysReturnThatItem()
    {
        // Arrange
        var uniqueWeapon = UniqueItemRepository.ExcaliburSword;
        var lootEntry = new UniqueLootEntry(100, uniqueWeapon);
        var lootTable = new LootTable(lootEntry);
        var random = new Random(12345);

        // Act
        var item1 = lootTable.GenerateItem(1, random);
        var item2 = lootTable.GenerateItem(5, random);

        // Assert
        Assert.IsType<Weapon>(item1);
        Assert.IsType<Weapon>(item2);
        Assert.Equal("Excalibur", item1.Name);
        Assert.Equal("Excalibur", item2.Name);
        Assert.Equal(500, item1.Value); // Level 5 * 100 = 500
        Assert.Equal(500, item2.Value); // Level 5 * 100 = 500
    }

    [Fact]
    public void LootTable_WithMultipleEntries_ShouldRespectWeights()
    {
        // Arrange
        var swordTemplate = ItemTemplateRepository.GetAllWeaponTemplates().First(t => t.BaseName == "Épée");
        var commonEntry = new RandomLootEntry(70, swordTemplate, ItemRarity.Common);  // 70% chance
        var rareEntry = new RandomLootEntry(25, swordTemplate, ItemRarity.Rare);      // 25% chance
        var epicEntry = new RandomLootEntry(5, swordTemplate, ItemRarity.Epic);       // 5% chance
        var lootTable = new LootTable(commonEntry, rareEntry, epicEntry);
        var random = new Random(12345);

        // Act - Generate many items to test probability distribution
        var generatedItems = new List<Item>();
        for (int i = 0; i < 1000; i++)
        {
            generatedItems.Add(lootTable.GenerateItem(3, random));
        }

        // Assert - Should get different item levels indicating different rarities
        var itemLevels = generatedItems.Select(item => item.ItemLevel).ToList();
        var minLevel = itemLevels.Min();
        var maxLevel = itemLevels.Max();
        
        Assert.True(minLevel >= 2, "Common items should have at least level 2 (monster level 3 - 1)");
        Assert.True(maxLevel >= 5, "Epic items should have boosted levels of 5 or higher");
        Assert.True(maxLevel > minLevel, "Should generate items of varying levels due to rarity");
    }

    [Fact]
    public void LootTable_GetTotalWeight_ShouldReturnSumOfAllWeights()
    {
        // Arrange
        var hammerTemplate = ItemTemplateRepository.GetAllWeaponTemplates().First(t => t.BaseName == "Marteau");
        var entry1 = new RandomLootEntry(30, hammerTemplate, ItemRarity.Common);
        var entry2 = new RandomLootEntry(50, hammerTemplate, ItemRarity.Rare);
        var entry3 = new UniqueLootEntry(20, UniqueItemRepository.ExcaliburSword);
        var lootTable = new LootTable(entry1, entry2, entry3);

        // Act
        var totalWeight = lootTable.GetTotalWeight();

        // Assert
        Assert.Equal(100, totalWeight);
    }

    [Fact]
    public void LootTable_WithZeroWeight_ShouldThrowException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new LootTable());
    }

    [Fact] 
    public void RandomLootEntry_WithDifferentRarities_ShouldGenerateAppropriateItems()
    {
        // Arrange
        var daggerTemplate = ItemTemplateRepository.GetAllWeaponTemplates().First(t => t.BaseName == "Dague");
        var commonEntry = new RandomLootEntry(100, daggerTemplate, ItemRarity.Common);
        var rareEntry = new RandomLootEntry(100, daggerTemplate, ItemRarity.Rare);
        var epicEntry = new RandomLootEntry(100, daggerTemplate, ItemRarity.Epic);
        var random = new Random(12345);
        int monsterLevel = 3;

        // Act
        var commonItem = commonEntry.GenerateItem(monsterLevel, random);
        var rareItem = rareEntry.GenerateItem(monsterLevel, random);
        var epicItem = epicEntry.GenerateItem(monsterLevel, random);

        // Assert - Rare and epic items should generally have higher levels than common
        Assert.True(commonItem.ItemLevel >= 2); // Monster level 3 +/- 1, minimum 1
        Assert.True(rareItem.ItemLevel >= commonItem.ItemLevel - 1); // Should be at least as good
        Assert.True(epicItem.ItemLevel >= rareItem.ItemLevel - 1); // Should be at least as good
    }

    [Fact]
    public void UniqueLootEntry_ShouldAlwaysGeneratesSameItem()
    {
        // Arrange
        var uniquePotion = UniqueItemRepository.ElixirOfLife;
        var entry = new UniqueLootEntry(100, uniquePotion);
        var random1 = new Random(12345);
        var random2 = new Random(67890);

        // Act
        var item1 = entry.GenerateItem(1, random1);
        var item2 = entry.GenerateItem(10, random2);

        // Assert - Should be identical regardless of monster level or random seed
        Assert.IsType<Potion>(item1);
        Assert.IsType<Potion>(item2);
        Assert.Equal("Élixir de Vie", item1.Name);
        Assert.Equal("Élixir de Vie", item2.Name);
        Assert.Equal(item1.Value, item2.Value);
        Assert.Equal(item1.ItemLevel, item2.ItemLevel);
        
        var potion1 = (Potion)item1;
        var potion2 = (Potion)item2;
        Assert.Equal(potion1.HealAmount, potion2.HealAmount);
    }

    [Fact]
    public void RandomLootEntry_WithSpecificTemplate_ShouldUseOnlyThatTemplate()
    {
        // Arrange
        var swordTemplate = ItemTemplateRepository.GetAllWeaponTemplates().First(t => t.BaseName == "Épée");
        var entry = new RandomLootEntry(100, swordTemplate, ItemRarity.Common);
        var random = new Random(12345);

        // Act - Generate multiple items to ensure consistency
        var items = new List<Item>();
        for (int i = 0; i < 10; i++)
        {
            items.Add(entry.GenerateItem(3, random));
        }

        // Assert - All items should be swords (based on the template)
        Assert.All(items, item =>
        {
            Assert.IsType<Weapon>(item);
            var weapon = (Weapon)item;
            Assert.Contains("Épée", weapon.Name); // Should contain "Épée" from the template
        });
    }

    [Fact]
    public void RandomLootEntry_WithDifferentTemplates_ShouldGenerateAppropriateItemTypes()
    {
        // Arrange
        var weaponTemplate = ItemTemplateRepository.GetAllWeaponTemplates().First();
        var armorTemplate = ItemTemplateRepository.GetAllArmorTemplates().First();
        var potionTemplate = ItemTemplateRepository.GetAllPotionTemplates().First();
        
        var weaponEntry = new RandomLootEntry(100, weaponTemplate, ItemRarity.Common);
        var armorEntry = new RandomLootEntry(100, armorTemplate, ItemRarity.Common);
        var potionEntry = new RandomLootEntry(100, potionTemplate, ItemRarity.Common);
        var random = new Random(12345);

        // Act
        var weaponItem = weaponEntry.GenerateItem(3, random);
        var armorItem = armorEntry.GenerateItem(3, random);
        var potionItem = potionEntry.GenerateItem(3, random);

        // Assert - Should generate correct types of items based on templates
        Assert.IsType<Weapon>(weaponItem);
        Assert.IsType<Armor>(armorItem);
        Assert.IsType<Potion>(potionItem);
    }
}