using System.Collections.Generic;
using System.Linq;
using ThirdRun.Items;
using Microsoft.Xna.Framework;

namespace ThirdRun.Characters
{
    public class Inventory
    {
        private Dictionary<Point, Item> items;
        private const int GridWidth = 4;
        private const int GridHeight = 10; // Allow for larger inventory

        public int MaxGridHeight => GridHeight;

        public Inventory()
        {
            items = new Dictionary<Point, Item>();
        }

        public void AddItem(Item item)
        {
            // Find the first available slot
            Point? availableSlot = FindNextAvailableSlot();
            if (availableSlot.HasValue)
            {
                items[availableSlot.Value] = item;
            }
        }

        public void AddItem(Item item, Point coordinates)
        {
            // Add item at specific coordinates if the slot is empty
            if (!items.ContainsKey(coordinates))
            {
                items[coordinates] = item;
            }
        }

        public List<Item> GetItems()
        {
            return new List<Item>(items.Values);
        }

        public Dictionary<Point, Item> GetItemsWithCoordinates()
        {
            return new Dictionary<Point, Item>(items);
        }

        public Item? GetItemAt(Point coordinates)
        {
            return items.TryGetValue(coordinates, out Item? item) ? item : null;
        }

        public bool RemoveItem(Item item)
        {
            var kvp = items.FirstOrDefault(x => x.Value == item);
            if (!kvp.Equals(default(KeyValuePair<Point, Item>)))
            {
                items.Remove(kvp.Key);
                return true;
            }
            return false;
        }

        public bool RemoveItemAt(Point coordinates)
        {
            return items.Remove(coordinates);
        }

        public bool MoveItem(Point from, Point to)
        {
            if (items.TryGetValue(from, out Item? item))
            {
                if (!items.ContainsKey(to))
                {
                    // Move to empty slot
                    items.Remove(from);
                    items[to] = item;
                    return true;
                }
                else if (items.TryGetValue(to, out Item? targetItem))
                {
                    // Swap items
                    items[from] = targetItem;
                    items[to] = item;
                    return true;
                }
            }
            return false;
        }

        public bool IsSlotEmpty(Point coordinates)
        {
            return !items.ContainsKey(coordinates) && 
                   coordinates.X >= 0 && coordinates.X < GridWidth && 
                   coordinates.Y >= 0 && coordinates.Y < GridHeight;
        }

        private Point? FindNextAvailableSlot()
        {
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    Point slot = new Point(x, y);
                    if (!items.ContainsKey(slot))
                    {
                        return slot;
                    }
                }
            }
            return null; // Inventory is full
        }

        public bool EquipItem(Item item)
        {
            if (item is Equipment equipment)
            {
                // Utilise la méthode Equip du personnage
                return Owner.Equip(equipment);
            }
            return false;
        }

        public bool UsePotion(Potion potion)
        {
            if (items.Values.Contains(potion))
            {
                potion.Use(Owner);
                RemoveItem(potion);
                return true;
            }
            return false;
        }

        required public Character Owner { get; set; } // à initialiser lors de l'ajout à un personnage
    }
}