using ThirdRun.Characters;
using ThirdRun.Items;
using MonogameRPG.Map;

namespace ThirdRun.Tests;

public class InventoryEquipmentTests
{
    [Fact]
    public void EquipItem_WithWeapon_EquipsWeaponAndUpdatesStats()
    {
        // Arrange
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Warrior", CharacterClass.Guerrier, 100, 10, worldMap);
        var weapon = new Weapon("Test Sword", "A test sword", 100, 5, 10);
        character.Inventory.AddItem(weapon);
        
        var initialAttackPower = character.AttackPower;
        
        // Act
        bool equipped = character.Inventory.EquipItem(weapon);
        
        // Assert
        Assert.True(equipped);
        Assert.Equal(weapon, character.Weapon);
        Assert.Equal(initialAttackPower + weapon.BonusStats, character.AttackPower);
        Assert.Contains(weapon, character.Inventory.GetItems()); // Item should still be in inventory
    }
    
    [Fact]
    public void EquipItem_WithArmor_EquipsArmorAndUpdatesStats()
    {
        // Arrange
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Warrior", CharacterClass.Guerrier, 100, 10, worldMap);
        var armor = new Armor("Test Armor", "A test armor", 100, 3, 15);
        character.Inventory.AddItem(armor);
        
        var initialAttackPower = character.AttackPower;
        
        // Act
        bool equipped = character.Inventory.EquipItem(armor);
        
        // Assert
        Assert.True(equipped);
        Assert.Equal(armor, character.Armor);
        Assert.Equal(initialAttackPower + armor.BonusStats, character.AttackPower);
        Assert.Contains(armor, character.Inventory.GetItems()); // Item should still be in inventory
    }
    
    [Fact]
    public void EquipItem_WithWeaponWhenAlreadyEquipped_SwapsWeaponsCorrectly()
    {
        // Arrange
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Warrior", CharacterClass.Guerrier, 100, 10, worldMap);
        var oldWeapon = new Weapon("Old Sword", "An old sword", 50, 3, 8);
        var newWeapon = new Weapon("New Sword", "A new sword", 100, 7, 12);
        
        // Equip first weapon
        character.Inventory.AddItem(oldWeapon);
        character.Inventory.EquipItem(oldWeapon);
        character.Inventory.AddItem(newWeapon);
        
        var expectedAttackPower = 10 + newWeapon.BonusStats; // base + new weapon bonus (old weapon already removed)
        
        // Act
        bool equipped = character.Inventory.EquipItem(newWeapon);
        
        // Assert
        Assert.True(equipped);
        Assert.Equal(newWeapon, character.Weapon);
        Assert.Equal(expectedAttackPower, character.AttackPower);
        Assert.Contains(newWeapon, character.Inventory.GetItems()); // New weapon should still be in inventory (UI handles removal)
        Assert.Contains(oldWeapon, character.Inventory.GetItems());
    }
    
    [Fact]
    public void EquipItem_WithArmorWhenAlreadyEquipped_SwapsArmorCorrectly()
    {
        // Arrange
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Warrior", CharacterClass.Guerrier, 100, 10, worldMap);
        var oldArmor = new Armor("Old Armor", "An old armor", 50, 2, 10);
        var newArmor = new Armor("New Armor", "A new armor", 100, 5, 20);
        
        // Equip first armor
        character.Inventory.AddItem(oldArmor);
        character.Inventory.EquipItem(oldArmor);
        character.Inventory.AddItem(newArmor);
        
        var expectedAttackPower = 10 + newArmor.BonusStats; // base + new armor bonus (old armor already removed)
        
        // Act
        bool equipped = character.Inventory.EquipItem(newArmor);
        
        // Assert
        Assert.True(equipped);
        Assert.Equal(newArmor, character.Armor);
        Assert.Equal(expectedAttackPower, character.AttackPower);
        Assert.Contains(newArmor, character.Inventory.GetItems()); // New armor should still be in inventory (UI handles removal)  
        Assert.Contains(oldArmor, character.Inventory.GetItems());
    }
    
    [Fact]
    public void EquipItem_WithNonEquipment_ReturnsFalse()
    {
        // Arrange
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Warrior", CharacterClass.Guerrier, 100, 10, worldMap);
        var potion = new Potion("Health Potion", "Restores health", 50, 25);
        character.Inventory.AddItem(potion);
        
        // Act
        bool equipped = character.Inventory.EquipItem(potion);
        
        // Assert
        Assert.False(equipped);
        Assert.Contains(potion, character.Inventory.GetItems());
    }
    
    [Fact]
    public void EquipItem_WithClassRestriction_ReturnsFalse()
    {
        // Arrange
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Mage", CharacterClass.Mage, 100, 10, worldMap);
        var sword = new Weapon("Épée", "A sword that mages can't use", 100, 5, 10);
        character.Inventory.AddItem(sword);
        
        // Act
        bool equipped = character.Inventory.EquipItem(sword);
        
        // Assert
        Assert.False(equipped);
        Assert.Null(character.Weapon);
        Assert.Contains(sword, character.Inventory.GetItems());
    }
}