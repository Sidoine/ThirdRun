using System.Collections.Generic;

namespace ThirdRun.Characters
{
    public class Inventory
    {
        private List<Item> items;

        public Inventory()
        {
            items = new List<Item>();
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public List<Item> GetItems()
        {
            return items;
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
            if (items.Contains(potion))
            {
                potion.Use(Owner);
                items.Remove(potion);
                return true;
            }
            return false;
        }

        required public Character Owner { get; set; } // à initialiser lors de l'ajout à un personnage
    }
}