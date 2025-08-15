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
        public ItemTemplate? Template { get; }
        public ItemRarity Rarity { get; }

        /// <summary>
        /// Creates a RandomLootEntry that uses a specific template
        /// </summary>
        public RandomLootEntry(int weight, ItemTemplate template, ItemRarity rarity) : base(weight)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            Rarity = rarity;
        }

        /// <summary>
        /// Creates a RandomLootEntry that randomly selects from all available templates
        /// </summary>
        public RandomLootEntry(int weight, ItemRarity rarity) : base(weight)
        {
            Template = null; // Will randomly select template during generation
            Rarity = rarity;
        }

        public override Item GenerateItem(int monsterLevel, Random random)
        {
            if (Template != null)
            {
                // Use specific template
                int itemLevel = CalculateItemLevel(monsterLevel, random, Rarity);
                
                // Generate prefix based on template type
                string? prefix = Template switch
                {
                    WeaponTemplate => ItemTemplateRepository.GetRandomWeaponPrefix(random),
                    ArmorTemplate => ItemTemplateRepository.GetRandomArmorPrefix(random),
                    PotionTemplate => null, // Potions don't use prefixes
                    _ => null
                };
                
                return Template.CreateItem(itemLevel, random, prefix);
            }
            else
            {
                // Use RandomItemGenerator for backward compatibility
                return RandomItemGenerator.GenerateRandomItem(monsterLevel, random, Rarity);
            }
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