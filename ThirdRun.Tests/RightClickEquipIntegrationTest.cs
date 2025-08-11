using ThirdRun.Characters;
using ThirdRun.Items;
using MonogameRPG.Map;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests;

/// <summary>
/// Integration test that simulates the complete right-click to equip workflow
/// </summary>
public class RightClickEquipIntegrationTest
{
    [Fact] 
    public void RightClickWorkflow_EquipFromInventory_CompletesSuccessfully()
    {
        // Arrange - Simulate game setup
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Player", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
        
        // Add some items to inventory like monsters would drop
        var sword = new Weapon("Iron Sword", "A sturdy iron sword", 100, 5, 12);
        var armor = new Armor("Leather Armor", "Basic leather protection", 80, 3, 8);
        var potion = new Potion("Health Potion", "Restores health", 50, 25);
        
        character.Inventory.AddItem(sword);
        character.Inventory.AddItem(armor);
        character.Inventory.AddItem(potion);
        
        var initialAttackPower = character.AttackPower;
        
        // Act - Simulate right-clicking on the sword to equip it
        bool swordEquipped = character.Inventory.EquipItem(sword);
        character.Inventory.RemoveItem(sword); // UI would handle this removal
        
        // Act - Simulate right-clicking on the armor to equip it  
        bool armorEquipped = character.Inventory.EquipItem(armor);
        character.Inventory.RemoveItem(armor); // UI would handle this removal
        
        // Act - Try to right-click on potion (should fail)
        bool potionEquipped = character.Inventory.EquipItem(potion);
        
        // Assert - Verify the complete state
        Assert.True(swordEquipped);
        Assert.True(armorEquipped);
        Assert.False(potionEquipped); // Potions can't be equipped
        
        // Check equipped items
        Assert.Equal(sword, character.Weapon);
        Assert.Equal(armor, character.Armor);
        
        // Check stats updated correctly
        var expectedAttackPower = initialAttackPower + sword.BonusStats + armor.BonusStats;
        Assert.Equal(expectedAttackPower, character.AttackPower);
        
        // Check inventory state
        Assert.DoesNotContain(sword, character.Inventory.GetItems());
        Assert.DoesNotContain(armor, character.Inventory.GetItems());
        Assert.Contains(potion, character.Inventory.GetItems()); // Potion should still be there
        
        // Test equipment swapping
        var betterSword = new Weapon("Steel Sword", "A sharp steel sword", 200, 8, 18);
        character.Inventory.AddItem(betterSword);
        
        bool swapEquipped = character.Inventory.EquipItem(betterSword);
        character.Inventory.RemoveItem(betterSword); // UI would handle this removal
        
        Assert.True(swapEquipped);
        Assert.Equal(betterSword, character.Weapon);
        Assert.Contains(sword, character.Inventory.GetItems()); // Old sword back in inventory
        
        // Verify stats recalculated correctly after swap
        var finalExpectedAttackPower = initialAttackPower + betterSword.BonusStats + armor.BonusStats;
        Assert.Equal(finalExpectedAttackPower, character.AttackPower);
    }
}