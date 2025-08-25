using System;

namespace ThirdRun.Items
{
    /// <summary>
    /// Represents an entry in a monster's loot table with weight and generation logic
    /// </summary>
    public abstract class LootTableEntry
    {
        public int Weight { get; }

        protected LootTableEntry(int weight)
        {
            Weight = weight;
        }

        /// <summary>
        /// Generate an item based on this loot entry
        /// </summary>
        public abstract Item GenerateItem(int monsterLevel, Random random);
    }

    /// <summary>
    /// Loot entry that generates random items with specified rarity
    /// </summary>
    public class RandomLootEntry : LootTableEntry
    {
        public ItemTemplate Template { get; }
        public ItemRarity Rarity { get; }

        /// <summary>
        /// Creates a RandomLootEntry that uses a specific template
        /// </summary>
        public RandomLootEntry(int weight, ItemTemplate template, ItemRarity rarity) : base(weight)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            Rarity = rarity;
        }

        public override Item GenerateItem(int monsterLevel, Random random)
        {
            // Use specific template to create item
            int itemLevel = CalculateItemLevel(monsterLevel, random, Rarity);
            return Template.CreateItem(itemLevel, random);
        }

        private static int CalculateItemLevel(int monsterLevel, Random random, ItemRarity rarity)
        {
            // Base item level is monster level +/- 1, minimum 1
            int variation = random.Next(-1, 2); // -1, 0, or 1
            int baseLevel = Math.Max(1, monsterLevel + variation);
            
            // Apply rarity boost to item level
            int rarityBoost = rarity switch
            {
                ItemRarity.Common => 0,
                ItemRarity.Rare => random.Next(1, 3),    // +1 to +2 levels
                ItemRarity.Epic => random.Next(2, 5),    // +2 to +4 levels
                _ => 0
            };
            
            return baseLevel + rarityBoost;
        }
    }

    /// <summary>
    /// Loot entry that drops a specific unique item with fixed stats
    /// </summary>
    public class UniqueLootEntry : LootTableEntry
    {
        public UniqueItem UniqueItemTemplate { get; }

        public UniqueLootEntry(int weight, UniqueItem uniqueItemTemplate) : base(weight)
        {
            UniqueItemTemplate = uniqueItemTemplate;
        }

        public override Item GenerateItem(int monsterLevel, Random random)
        {
            return UniqueItemTemplate.CreateItem();
        }
    }
}