using System;
using ThirdRun.Items;
using Microsoft.Xna.Framework.Content;

namespace ThirdRun.Tests;

public class ItemTests
{
    [Fact]
    public void Item_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        string name = "Test Item";
        string description = "A test item";
        int value = 100;
        
        // Act
        var item = new Item(name, description, value);
        
        // Assert
        Assert.Equal(name, item.Name);
        Assert.Equal(description, item.Description);
        Assert.Equal(value, item.Value);
    }
    
    [Fact]
    public void Weapon_Constructor_SetsAllProperties()
    {
        // Arrange
        string name = "Iron Sword";
        string description = "A sturdy iron sword";
        int value = 200;
        int bonusStats = 5;
        int damage = 15;
        
        // Act
        var weapon = new Weapon(name, description, value, bonusStats, damage);
        
        // Assert
        Assert.Equal(name, weapon.Name);
        Assert.Equal(description, weapon.Description);
        Assert.Equal(value, weapon.Value);
        Assert.Equal(bonusStats, weapon.BonusStats);
        Assert.Equal(damage, weapon.Damage);
    }
    
    [Fact]
    public void Armor_Constructor_SetsAllProperties()
    {
        // Arrange
        string name = "Chain Mail";
        string description = "Protective chain mail armor";
        int value = 150;
        int bonusStats = 3;
        int defense = 10;
        
        // Act
        var armor = new Armor(name, description, value, bonusStats, defense);
        
        // Assert
        Assert.Equal(name, armor.Name);
        Assert.Equal(description, armor.Description);
        Assert.Equal(value, armor.Value);
        Assert.Equal(bonusStats, armor.BonusStats);
        Assert.Equal(defense, armor.Defense);
    }
    
    [Fact]
    public void Potion_Constructor_SetsAllProperties()
    {
        // Arrange
        string name = "Health Potion";
        string description = "Restores health";
        int value = 50;
        int healAmount = 25;
        
        // Act
        var potion = new Potion(name, description, value, healAmount);
        
        // Assert
        Assert.Equal(name, potion.Name);
        Assert.Equal(description, potion.Description);
        Assert.Equal(value, potion.Value);
        Assert.Equal(healAmount, potion.HealAmount);
    }
}