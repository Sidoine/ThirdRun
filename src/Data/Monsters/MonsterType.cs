using System.Linq;
using Microsoft.Xna.Framework;
using ThirdRun.Data;
using ThirdRun.Items;

namespace MonogameRPG.Monsters
{
    public class MonsterType
    {
        public string Name { get; set; }
        public string TexturePath { get; set; }
        public int Level { get; set; }
        public CharacteristicValues Characteristics { get; private set; }
        public LootTable LootTable { get; set; }

        // Properties that delegate to characteristics for backward compatibility
        public int BaseHealth 
        { 
            get => Characteristics.GetValue(Characteristic.Health);
            set => Characteristics.SetValue(Characteristic.Health, value);
        }
        
        public int BaseAttack 
        { 
            get => Characteristics.GetValue(Characteristic.MeleeAttackPower);
            set => Characteristics.SetValue(Characteristic.MeleeAttackPower, value);
        }

        public MonsterType(string name, int baseHealth, int baseAttack, string texturePath, int level = 1)
        {
            Name = name;
            TexturePath = texturePath;
            Level = level;
            Characteristics = new CharacteristicValues();
            
            // Set the values using characteristics
            BaseHealth = baseHealth;
            BaseAttack = baseAttack;
            
            // Initialize with a default loot table that generates random common items
            // Use the first weapon template as a placeholder - this will be replaced by ConfigureLootTable if used through MonsterTemplateRepository
            var defaultTemplate = ItemTemplateRepository.GetAllWeaponTemplates().First();
            LootTable = new LootTable(new RandomLootEntry(100, defaultTemplate, ItemRarity.Common));
        }
    }
}
