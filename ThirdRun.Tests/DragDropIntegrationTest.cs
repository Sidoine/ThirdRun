using System;
using ThirdRun.Characters;
using ThirdRun.Items;
using MonogameRPG.Map;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests;

/// <summary>
/// Integration test to validate the complete drag and drop workflow
/// </summary>
public class DragDropIntegrationTest
{
    [Fact]
    public void DragDropWorkflow_MoveItemInInventory_CompletesSuccessfully()
    {
        // Arrange - Simulate game setup with items
        var worldMap = new WorldMap(new Random(12345));
        worldMap.Initialize();
        var character = new Character("Test Player", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
        
        // Add items at specific coordinates like they would appear in the UI
        var sword = new Weapon("Iron Sword", "A sturdy iron sword", 100, 5, 12);
        var armor = new Armor("Leather Armor", "Basic leather protection", 80, 3, 8);
        var potion = new Potion("Health Potion", "Restores health", 50, 25);
        
        character.Inventory.AddItem(sword, new Point(0, 0));   // Top-left slot
        character.Inventory.AddItem(armor, new Point(1, 0));   // Next to sword
        character.Inventory.AddItem(potion, new Point(0, 1));  // Below sword
        
        // Verify initial layout
        Assert.Equal(sword, character.Inventory.GetItemAt(new Point(0, 0)));
        Assert.Equal(armor, character.Inventory.GetItemAt(new Point(1, 0)));
        Assert.Equal(potion, character.Inventory.GetItemAt(new Point(0, 1)));
        Assert.True(character.Inventory.IsSlotEmpty(new Point(2, 0)));
        
        // Act 1 - Move sword from (0,0) to (2,0) (simulating drag and drop)
        bool swordMoved = character.Inventory.MoveItem(new Point(0, 0), new Point(2, 0));
        
        // Assert 1 - Verify sword moved correctly
        Assert.True(swordMoved);
        Assert.Null(character.Inventory.GetItemAt(new Point(0, 0))); // Original slot empty
        Assert.Equal(sword, character.Inventory.GetItemAt(new Point(2, 0))); // Sword in new slot
        Assert.Equal(armor, character.Inventory.GetItemAt(new Point(1, 0))); // Other items unchanged
        Assert.Equal(potion, character.Inventory.GetItemAt(new Point(0, 1)));
        
        // Act 2 - Try to move armor to occupied slot (should swap items)
        bool armorMoveToOccupied = character.Inventory.MoveItem(new Point(1, 0), new Point(2, 0));
        
        // Assert 2 - Verify items were swapped
        Assert.True(armorMoveToOccupied);
        Assert.Equal(sword, character.Inventory.GetItemAt(new Point(1, 0))); // Sword swapped here
        Assert.Equal(armor, character.Inventory.GetItemAt(new Point(2, 0))); // Armor moved here
        
        // Act 3 - Move potion to empty slot
        bool potionMoved = character.Inventory.MoveItem(new Point(0, 1), new Point(3, 1));
        
        // Assert 3 - Verify potion moved
        Assert.True(potionMoved);
        Assert.Null(character.Inventory.GetItemAt(new Point(0, 1))); // Original slot empty
        Assert.Equal(potion, character.Inventory.GetItemAt(new Point(3, 1))); // Potion in new slot
    }
    
    [Fact]
    public void DragDropWorkflow_EquipItemByDrop_CompletesSuccessfully()
    {
        // Arrange - Character with items in inventory
        var worldMap = new WorldMap(new Random(12345));
        worldMap.Initialize();
        var character = new Character("Test Player", CharacterClass.Guerrier, 100, 10, worldMap.CurrentMap, worldMap);
        
        var sword = new Weapon("Iron Sword", "A sturdy iron sword", 100, 5, 12);
        var armor = new Armor("Leather Armor", "Basic leather protection", 80, 3, 8);
        
        character.Inventory.AddItem(sword, new Point(0, 0));
        character.Inventory.AddItem(armor, new Point(1, 0));
        
        var initialAttackPower = character.AttackPower;
        
        // Act 1 - "Drop" sword on character to equip it (simulating drag from inventory to character portrait)
        bool swordEquipped = character.Inventory.EquipItem(sword);
        character.Inventory.RemoveItemAt(new Point(0, 0)); // UI would handle this removal
        
        // Assert 1 - Verify sword equipped and removed from inventory
        Assert.True(swordEquipped);
        Assert.Equal(sword, character.Weapon);
        Assert.Equal(initialAttackPower + sword.BonusStats, character.AttackPower);
        Assert.Null(character.Inventory.GetItemAt(new Point(0, 0))); // Removed from inventory
        
        // Act 2 - "Drop" armor on character to equip it
        bool armorEquipped = character.Inventory.EquipItem(armor);
        character.Inventory.RemoveItemAt(new Point(1, 0)); // UI would handle this removal
        
        // Assert 2 - Verify armor equipped and stats updated
        Assert.True(armorEquipped);
        Assert.Equal(armor, character.Armor);
        Assert.Equal(initialAttackPower + sword.BonusStats + armor.BonusStats, character.AttackPower);
        Assert.Null(character.Inventory.GetItemAt(new Point(1, 0))); // Removed from inventory
        
        // Act 3 - Equip better sword (should put old sword back in inventory)
        var betterSword = new Weapon("Steel Sword", "A sharp steel sword", 200, 8, 18);
        character.Inventory.AddItem(betterSword, new Point(0, 0));
        
        bool betterSwordEquipped = character.Inventory.EquipItem(betterSword);
        character.Inventory.RemoveItemAt(new Point(0, 0)); // UI would handle this removal
        
        // Assert 3 - Verify weapon swap worked correctly
        Assert.True(betterSwordEquipped);
        Assert.Equal(betterSword, character.Weapon);
        Assert.Contains(sword, character.Inventory.GetItems()); // Old sword returned to inventory
        
        // Verify final stats
        var expectedFinalAttackPower = initialAttackPower + betterSword.BonusStats + armor.BonusStats;
        Assert.Equal(expectedFinalAttackPower, character.AttackPower);
    }
}