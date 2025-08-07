using ThirdRun.Characters;
using ThirdRun.Items;
using MonogameRPG.Map;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests;

public class DragDropTests
{
    [Fact]
    public void Inventory_WithCoordinates_AllowsItemMovement()
    {
        // Arrange
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Character", CharacterClass.Guerrier, 100, 10, worldMap);
        var sword = new Weapon("Test Sword", "A test sword", 100, 5, 10);
        var armor = new Armor("Test Armor", "A test armor", 100, 3, 15);
        
        // Add items to specific coordinates
        character.Inventory.AddItem(sword, new Point(0, 0));
        character.Inventory.AddItem(armor, new Point(1, 0));
        
        // Act & Assert - Verify items are at expected coordinates
        Assert.Equal(sword, character.Inventory.GetItemAt(new Point(0, 0)));
        Assert.Equal(armor, character.Inventory.GetItemAt(new Point(1, 0)));
        Assert.Null(character.Inventory.GetItemAt(new Point(2, 0)));
        
        // Act - Move sword from (0,0) to (2,0)
        bool moved = character.Inventory.MoveItem(new Point(0, 0), new Point(2, 0));
        
        // Assert - Verify movement was successful
        Assert.True(moved);
        Assert.Null(character.Inventory.GetItemAt(new Point(0, 0)));
        Assert.Equal(armor, character.Inventory.GetItemAt(new Point(1, 0))); // unchanged
        Assert.Equal(sword, character.Inventory.GetItemAt(new Point(2, 0))); // moved here
    }
    
    [Fact]
    public void Inventory_MoveToOccupiedSlot_SwapsItems()
    {
        // Arrange
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Character", CharacterClass.Guerrier, 100, 10, worldMap);
        var sword = new Weapon("Test Sword", "A test sword", 100, 5, 10);
        var armor = new Armor("Test Armor", "A test armor", 100, 3, 15);
        
        character.Inventory.AddItem(sword, new Point(0, 0));
        character.Inventory.AddItem(armor, new Point(1, 0));
        
        // Act - Try to move sword to occupied slot - should swap items
        bool moved = character.Inventory.MoveItem(new Point(0, 0), new Point(1, 0));
        
        // Assert - Movement should succeed and items should be swapped
        Assert.True(moved);
        Assert.Equal(armor, character.Inventory.GetItemAt(new Point(0, 0))); // armor moved here
        Assert.Equal(sword, character.Inventory.GetItemAt(new Point(1, 0))); // sword moved here
    }
    
    [Fact]
    public void Inventory_IsSlotEmpty_ReturnsCorrectState()
    {
        // Arrange
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Character", CharacterClass.Guerrier, 100, 10, worldMap);
        var sword = new Weapon("Test Sword", "A test sword", 100, 5, 10);
        
        character.Inventory.AddItem(sword, new Point(1, 1));
        
        // Act & Assert
        Assert.True(character.Inventory.IsSlotEmpty(new Point(0, 0))); // empty slot
        Assert.False(character.Inventory.IsSlotEmpty(new Point(1, 1))); // occupied slot
        Assert.True(character.Inventory.IsSlotEmpty(new Point(2, 2))); // empty slot
        
        // Test boundary conditions
        Assert.False(character.Inventory.IsSlotEmpty(new Point(-1, 0))); // out of bounds
        Assert.False(character.Inventory.IsSlotEmpty(new Point(0, -1))); // out of bounds
        Assert.False(character.Inventory.IsSlotEmpty(new Point(4, 0))); // out of bounds (grid width = 4)
        Assert.False(character.Inventory.IsSlotEmpty(new Point(0, 10))); // out of bounds (grid height = 10)
    }
    
    [Fact]
    public void Inventory_RemoveItemAt_RemovesItemFromCoordinates()
    {
        // Arrange
        var worldMap = new WorldMap();
        worldMap.Initialize();
        var character = new Character("Test Character", CharacterClass.Guerrier, 100, 10, worldMap);
        var sword = new Weapon("Test Sword", "A test sword", 100, 5, 10);
        
        character.Inventory.AddItem(sword, new Point(1, 1));
        
        // Verify item is there
        Assert.Equal(sword, character.Inventory.GetItemAt(new Point(1, 1)));
        
        // Act - Remove item at coordinates
        bool removed = character.Inventory.RemoveItemAt(new Point(1, 1));
        
        // Assert - Item should be gone
        Assert.True(removed);
        Assert.Null(character.Inventory.GetItemAt(new Point(1, 1)));
        Assert.True(character.Inventory.IsSlotEmpty(new Point(1, 1)));
    }
}