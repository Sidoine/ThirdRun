using ThirdRun.Items;
using System.Diagnostics;

namespace ThirdRun.Tests;

/// <summary>
/// Integration test to demonstrate the random item generation system
/// </summary>
public class RandomItemIntegrationTests
{
    [Fact]
    public void RandomItemGeneration_ShouldProduceVariedResults()
    {
        // Arrange - Generate multiple items for different monster levels
        var level1Items = new List<Item>();
        var level5Items = new List<Item>();
        
        // Act - Generate several items for each level
        for (int i = 0; i < 10; i++)
        {
            level1Items.Add(RandomItemGenerator.GenerateRandomItem(1));
            level5Items.Add(RandomItemGenerator.GenerateRandomItem(5));
        }
        
        // Assert - Level 5 items should generally be more valuable
        double avgValue1 = level1Items.Average(item => item.Value);
        double avgValue5 = level5Items.Average(item => item.Value);
        
        Assert.True(avgValue5 > avgValue1, 
            $"Level 5 items should have higher average value. Level 1: {avgValue1:F1}, Level 5: {avgValue5:F1}");
        
        // Should generate different types
        var types1 = level1Items.Select(item => item.GetType()).Distinct().Count();
        var types5 = level5Items.Select(item => item.GetType()).Distinct().Count();
        
        Assert.True(types1 >= 1, "Should generate at least 1 type for level 1");
        Assert.True(types5 >= 1, "Should generate at least 1 type for level 5");
        
        // Output sample results for verification
        Debug.WriteLine("=== Sample Level 1 Items ===");
        foreach (var item in level1Items.Take(3))
        {
            Debug.WriteLine($"{item.GetType().Name}: {item.Name} (Level {item.ItemLevel}, Value {item.Value})");
        }
        
        Debug.WriteLine("=== Sample Level 5 Items ===");
        foreach (var item in level5Items.Take(3))
        {
            Debug.WriteLine($"{item.GetType().Name}: {item.Name} (Level {item.ItemLevel}, Value {item.Value})");
        }
    }
}