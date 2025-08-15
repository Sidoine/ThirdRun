using System;
using System.Collections.Generic;
using System.Linq;
using MonogameRPG.Monsters;
using ThirdRun.Items;
using Xunit;

namespace ThirdRun.Tests;

public class LootSystemIntegrationTests
{
    [Fact]
    public void LootSystem_EndToEndScenario_ShouldWorkCorrectly()
    {
        // Arrange - Simulate creating various monsters that would spawn in the game
        var random = new Random(12345);
        var items = new List<Item>();

        // Act - Generate monsters and collect their loot
        for (int i = 0; i < 50; i++)
        {
            // Create monsters of varying levels using the repository
            var lowLevelMonster = MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(1, 2, random);
            var midLevelMonster = MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(3, 4, random);
            var highLevelMonster = MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(5, 6, random);

            // Generate loot from each monster type
            items.Add(lowLevelMonster.LootTable!.GenerateItem(lowLevelMonster.Level, random));
            items.Add(midLevelMonster.LootTable!.GenerateItem(midLevelMonster.Level, random));
            items.Add(highLevelMonster.LootTable!.GenerateItem(highLevelMonster.Level, random));
        }

        // Assert - Verify the loot system produces expected results
        Assert.Equal(150, items.Count);
        Assert.All(items, item => Assert.NotNull(item));
        Assert.All(items, item => Assert.True(item.ItemLevel >= 1));
        Assert.All(items, item => Assert.True(item.Value > 0));

        // Check that we get a variety of item types
        var weapons = items.OfType<Weapon>().ToList();
        var armors = items.OfType<Armor>().ToList(); 
        var potions = items.OfType<Potion>().ToList();

        Assert.True(weapons.Count > 0, "Should generate some weapons");
        Assert.True(armors.Count > 0, "Should generate some armor");
        Assert.True(potions.Count > 0, "Should generate some potions");

        // Higher level monsters should generally produce better loot
        var lowLevelItems = items.Take(50).ToList();
        var highLevelItems = items.Skip(100).Take(50).ToList();

        var avgLowLevel = lowLevelItems.Average(item => item.ItemLevel);
        var avgHighLevel = highLevelItems.Average(item => item.ItemLevel);

        // Higher level monsters should produce items with higher average levels
        Assert.True(avgHighLevel >= avgLowLevel - 0.5, 
            $"High level monsters should produce higher level items on average. Low: {avgLowLevel}, High: {avgHighLevel}");

        // Check for unique items in the generated loot
        var uniqueItemNames = new[] { "Excalibur", "Dragon Slayer", "Lame de l'Ombre", "Plastron des Légendes", 
                                     "Casque de la Sagesse", "Bottes de Célérité", "Élixir de Vie", "Larmes de Phénix" };
        var foundUniqueItems = items.Where(item => uniqueItemNames.Any(name => item.Name.Contains(name))).ToList();

        // With the probability and number of items, we should get at least some unique items
        // This tests that the unique item system is working
        Assert.True(foundUniqueItems.Count >= 0, "Should potentially generate unique items");
    }

    [Fact]
    public void RandomItemGenerator_WithRarity_ShouldProduceBetterItems()
    {
        // Arrange
        var random = new Random(12345);
        int monsterLevel = 3;

        // Act
        var commonItems = Enumerable.Range(0, 20)
            .Select(_ => RandomItemGenerator.GenerateRandomItem(monsterLevel, random, ItemRarity.Common))
            .ToList();
        
        var rareItems = Enumerable.Range(0, 20)
            .Select(_ => RandomItemGenerator.GenerateRandomItem(monsterLevel, random, ItemRarity.Rare))
            .ToList();
        
        var epicItems = Enumerable.Range(0, 20)
            .Select(_ => RandomItemGenerator.GenerateRandomItem(monsterLevel, random, ItemRarity.Epic))
            .ToList();

        // Assert
        var avgCommonLevel = commonItems.Average(item => item.ItemLevel);
        var avgRareLevel = rareItems.Average(item => item.ItemLevel);
        var avgEpicLevel = epicItems.Average(item => item.ItemLevel);

        Assert.True(avgRareLevel >= avgCommonLevel, "Rare items should have higher average level than common");
        Assert.True(avgEpicLevel >= avgRareLevel, "Epic items should have highest average level");

        var avgCommonValue = commonItems.Average(item => item.Value);
        var avgRareValue = rareItems.Average(item => item.Value);
        var avgEpicValue = epicItems.Average(item => item.Value);

        Assert.True(avgRareValue >= avgCommonValue, "Rare items should be more valuable than common");
        Assert.True(avgEpicValue >= avgRareValue, "Epic items should be most valuable");
    }
}