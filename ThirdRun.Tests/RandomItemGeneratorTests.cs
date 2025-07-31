using ThirdRun.Items;
using System.Linq;

namespace ThirdRun.Tests;

public class RandomItemGeneratorTests
{
    [Fact]
    public void GenerateRandomItem_ShouldReturnValidItem()
    {
        // Arrange
        int monsterLevel = 1;
        
        // Act
        var item = RandomItemGenerator.GenerateRandomItem(monsterLevel);
        
        // Assert
        Assert.NotNull(item);
        Assert.False(string.IsNullOrEmpty(item.Name));
        Assert.False(string.IsNullOrEmpty(item.Description));
        Assert.True(item.Value > 0);
        Assert.True(item.ItemLevel >= 1);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void GenerateRandomItem_ItemLevelShouldBeBasedOnMonsterLevel(int monsterLevel)
    {
        // Arrange & Act
        var item = RandomItemGenerator.GenerateRandomItem(monsterLevel);
        
        // Assert
        // Item level should be monster level +/- 1, minimum 1
        Assert.True(item.ItemLevel >= 1);
        Assert.True(item.ItemLevel >= monsterLevel - 1);
        Assert.True(item.ItemLevel <= monsterLevel + 1);
    }
    
    [Fact]
    public void GenerateRandomItem_ShouldGenerateDifferentTypes()
    {
        // Arrange
        var generatedTypes = new HashSet<Type>();
        int monsterLevel = 5;
        
        // Act - Generate multiple items to check for variety
        for (int i = 0; i < 50; i++)
        {
            var item = RandomItemGenerator.GenerateRandomItem(monsterLevel);
            generatedTypes.Add(item.GetType());
        }
        
        // Assert - Should generate at least 2 different types (weapons, armor, or potions)
        Assert.True(generatedTypes.Count >= 2);
    }
    
    [Fact]
    public void GenerateRandomItem_WeaponShouldHaveCorrectProperties()
    {
        // Arrange & Act - Generate many items to find weapons
        var weapons = new List<Weapon>();
        for (int i = 0; i < 50; i++)
        {
            var item = RandomItemGenerator.GenerateRandomItem(3);
            if (item is Weapon weapon)
            {
                weapons.Add(weapon);
            }
        }
        
        // Assert
        Assert.True(weapons.Count > 0, "Should generate at least one weapon");
        
        foreach (var weapon in weapons)
        {
            Assert.True(weapon.Damage > 0);
            Assert.True(weapon.BonusStats >= 0);
            Assert.True(weapon.ItemLevel >= 1);
            // Just check that the name is not empty, since there are many weapon types
            Assert.False(string.IsNullOrEmpty(weapon.Name));
        }
    }
    
    [Fact]
    public void GenerateRandomItem_ArmorShouldHaveCorrectProperties()
    {
        // Arrange & Act - Generate many items to find armor
        var armors = new List<Armor>();
        for (int i = 0; i < 50; i++)
        {
            var item = RandomItemGenerator.GenerateRandomItem(3);
            if (item is Armor armor)
            {
                armors.Add(armor);
            }
        }
        
        // Assert
        Assert.True(armors.Count > 0, "Should generate at least one armor piece");
        
        foreach (var armor in armors)
        {
            Assert.True(armor.Defense > 0);
            Assert.True(armor.BonusStats >= 0);
            Assert.True(armor.ItemLevel >= 1);
        }
    }
    
    [Fact]
    public void GenerateRandomItem_PotionShouldHaveCorrectProperties()
    {
        // Arrange & Act - Generate many items to find potions
        var potions = new List<Potion>();
        for (int i = 0; i < 50; i++)
        {
            var item = RandomItemGenerator.GenerateRandomItem(3);
            if (item is Potion potion)
            {
                potions.Add(potion);
            }
        }
        
        // Assert
        Assert.True(potions.Count > 0, "Should generate at least one potion");
        
        foreach (var potion in potions)
        {
            Assert.True(potion.HealAmount > 0);
            Assert.True(potion.ItemLevel >= 1);
            // Check that the name contains level info or is a potion-related name
            Assert.True(potion.Name.Contains("Potion") || potion.Name.Contains("Élixir") || 
                       potion.Name.Contains("Philtre") || potion.Name.Contains("Breuvage") ||
                       potion.Name.Contains("Niv."));
        }
    }
    
    [Fact]
    public void GenerateRandomItem_HigherLevelShouldHaveBetterStats()
    {
        // Arrange
        var lowLevelItems = new List<Item>();
        var highLevelItems = new List<Item>();
        
        // Act - Generate items of different levels
        for (int i = 0; i < 20; i++)
        {
            lowLevelItems.Add(RandomItemGenerator.GenerateRandomItem(1));
            highLevelItems.Add(RandomItemGenerator.GenerateRandomItem(10));
        }
        
        // Assert - Higher level items should generally have better stats
        double avgLowValue = lowLevelItems.Average(item => item.Value);
        double avgHighValue = highLevelItems.Average(item => item.Value);
        
        Assert.True(avgHighValue > avgLowValue, 
            $"Higher level items should have better values on average. Low: {avgLowValue}, High: {avgHighValue}");
    }
}