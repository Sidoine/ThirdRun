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
        public ItemRarity Rarity { get; }

        public RandomLootEntry(int weight, ItemRarity rarity) : base(weight)
        {
            Rarity = rarity;
        }

        public override Item GenerateItem(int monsterLevel, Random random)
        {
            return RandomItemGenerator.GenerateRandomItem(monsterLevel, random, Rarity);
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