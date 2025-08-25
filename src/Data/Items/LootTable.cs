using System;
using System.Collections.Generic;
using System.Linq;

namespace ThirdRun.Items
{
    /// <summary>
    /// Manages a weighted collection of loot entries for monsters
    /// </summary>
    public class LootTable
    {
        private readonly List<LootTableEntry> entries;
        private readonly int totalWeight;

        public LootTable(params LootTableEntry[] entries) : this(entries.AsEnumerable())
        {
        }

        public LootTable(IEnumerable<LootTableEntry> entries)
        {
            this.entries = new List<LootTableEntry>(entries);
            this.totalWeight = this.entries.Sum(entry => entry.Weight);
            
            if (this.totalWeight == 0)
            {
                throw new ArgumentException("Loot table must have at least one entry with positive weight");
            }
        }

        /// <summary>
        /// Generate a random item from this loot table based on weights
        /// </summary>
        public Item GenerateItem(int monsterLevel, Random random)
        {
            if (entries.Count == 0)
            {
                throw new InvalidOperationException("Cannot generate item from empty loot table");
            }

            int randomValue = random.Next(0, totalWeight);
            int currentWeight = 0;

            foreach (var entry in entries)
            {
                currentWeight += entry.Weight;
                if (randomValue < currentWeight)
                {
                    return entry.GenerateItem(monsterLevel, random);
                }
            }

            // This should never happen, but return the last entry as fallback
            return entries.Last().GenerateItem(monsterLevel, random);
        }

        /// <summary>
        /// Get all entries in this loot table (for testing purposes)
        /// </summary>
        public IReadOnlyList<LootTableEntry> GetEntries()
        {
            return entries.AsReadOnly();
        }

        /// <summary>
        /// Get the total weight of all entries
        /// </summary>
        public int GetTotalWeight()
        {
            return totalWeight;
        }
    }
}