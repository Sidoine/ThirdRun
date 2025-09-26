using System;
using ThirdRun.Characters;
using ThirdRun.Items;

namespace ThirdRun.Tests;

public class InventoryTests
{
    [Fact]
    public void Inventory_GetItems_InitializesEmptyItemsList()
    {
        // Arrange
        var inventory = new Inventory() { Owner = null! }; // For testing, we'll allow null owner
        
        // Act & Assert
        Assert.NotNull(inventory.GetItems());
        Assert.Empty(inventory.GetItems());
    }
    
    [Fact]
    public void AddItem_AddsItemToInventory()
    {
        // Arrange
        var inventory = new Inventory() { Owner = null! }; // For testing, we'll allow null owner
        var item = new Item("Test Item", "A test item", 100);
        
        // Act
        inventory.AddItem(item);
        
        // Assert
        var items = inventory.GetItems();
        Assert.Single(items);
        Assert.Contains(item, items);
    }
    
    [Fact]
    public void AddItem_MultipleItems_AddsAllItems()
    {
        // Arrange
        var inventory = new Inventory() { Owner = null! }; // For testing, we'll allow null owner
        var item1 = new Item("Item 1", "First item", 50);
        var item2 = new Item("Item 2", "Second item", 75);
        
        // Act
        inventory.AddItem(item1);
        inventory.AddItem(item2);
        
        // Assert
        var items = inventory.GetItems();
        Assert.Equal(2, items.Count);
        Assert.Contains(item1, items);
        Assert.Contains(item2, items);
    }
}