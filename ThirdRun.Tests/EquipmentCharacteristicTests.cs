using Microsoft.Xna.Framework;
using ThirdRun.Items;
using ThirdRun.Data;
using MonogameRPG.Map;
using Xunit;

namespace ThirdRun.Tests
{
    public class EquipmentCharacteristicTests
    {
        [Fact]
        public void Equipment_HasCharacteristics_WhenCreated()
        {
            // Arrange & Act
            var equipment = new Equipment("Test Equipment", "A test item", 100, 5);

            // Assert
            Assert.NotNull(equipment.Characteristics);
        }

        [Fact]
        public void Equipment_Characteristics_CanBeSet()
        {
            // Arrange
            var equipment = new Equipment("Magic Sword", "A powerful sword", 200, 10);

            // Act
            equipment.Characteristics.SetValue(Characteristic.MeleeAttackPower, 15);
            equipment.Characteristics.SetValue(Characteristic.CriticalChance, 5);

            // Assert
            Assert.Equal(15, equipment.Characteristics.GetValue(Characteristic.MeleeAttackPower));
            Assert.Equal(5, equipment.Characteristics.GetValue(Characteristic.CriticalChance));
        }

        [Fact]
        public void Equipment_Equip_AppliesBothLegacyAndCharacteristicBonuses()
        {
            // Arrange
            var worldMap = new WorldMap(); // Use parameterless constructor
            worldMap.Initialize();
            var character = new Character("TestHero", CharacterClass.Guerrier, 100, 10, worldMap);
            var equipment = new Equipment("Enhanced Armor", "Magical armor", 300, 8);
            
            equipment.Characteristics.SetValue(Characteristic.Armor, 12);
            equipment.Characteristics.SetValue(Characteristic.FireResistance, 7);

            // Act
            equipment.Equip(character);

            // Assert
            // Legacy behavior - AttackPower increases by BonusStats
            Assert.Equal(18, character.AttackPower); // 10 + 8
            
            // New behavior - Characteristics are applied
            Assert.Equal(12, character.Characteristics.GetValue(Characteristic.Armor));
            Assert.Equal(7, character.Characteristics.GetValue(Characteristic.FireResistance));
        }

        [Fact]
        public void Equipment_Unequip_RemovesBothLegacyAndCharacteristicBonuses()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            var character = new Character("TestHero", CharacterClass.Guerrier, 100, 10, worldMap);
            var equipment = new Equipment("Enhanced Weapon", "Magical weapon", 250, 6);
            
            equipment.Characteristics.SetValue(Characteristic.SpellPower, 20);
            equipment.Characteristics.SetValue(Characteristic.Haste, 3);

            // Equip first
            equipment.Equip(character);

            // Act - Unequip
            equipment.Unequip(character);

            // Assert
            // Legacy behavior - AttackPower restored
            Assert.Equal(10, character.AttackPower); // Back to original
            
            // New behavior - Characteristics removed
            Assert.Equal(0, character.Characteristics.GetValue(Characteristic.SpellPower));
            Assert.Equal(0, character.Characteristics.GetValue(Characteristic.Haste));
        }

        [Fact]
        public void Equipment_MultipleCharacteristics_AllAppliedCorrectly()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            var character = new Character("TestHero", CharacterClass.Mage, 80, 8, worldMap);
            var equipment = new Equipment("Arcane Robes", "Robes of the arcane", 400, 0); // No legacy bonus stats
            
            // Set multiple characteristics
            equipment.Characteristics.SetValue(Characteristic.SpellPower, 25);
            equipment.Characteristics.SetValue(Characteristic.ArcaneResistance, 15);
            equipment.Characteristics.SetValue(Characteristic.ShadowResistance, 10);
            equipment.Characteristics.SetValue(Characteristic.CriticalChance, 8);

            // Act
            equipment.Equip(character);

            // Assert - All characteristics applied
            Assert.Equal(25, character.Characteristics.GetValue(Characteristic.SpellPower));
            Assert.Equal(15, character.Characteristics.GetValue(Characteristic.ArcaneResistance));
            Assert.Equal(10, character.Characteristics.GetValue(Characteristic.ShadowResistance));
            Assert.Equal(8, character.Characteristics.GetValue(Characteristic.CriticalChance));
            
            // And unaffected characteristics remain at their base values
            Assert.Equal(8, character.Characteristics.GetValue(Characteristic.MeleeAttackPower)); // Character's base attack
            Assert.Equal(0, character.Characteristics.GetValue(Characteristic.FireResistance));
        }

        [Fact]
        public void Equipment_PreservesLegacyBehavior()
        {
            // Arrange
            var worldMap = new WorldMap();
            worldMap.Initialize();
            var character = new Character("TestHero", CharacterClass.Guerrier, 100, 12, worldMap);
            var equipment = new Equipment("Basic Sword", "A simple sword", 150, 7);

            // Act - Don't set any characteristics, just use legacy BonusStats
            equipment.Equip(character);

            // Assert - Legacy behavior still works
            Assert.Equal(19, character.AttackPower); // 12 + 7
            Assert.Equal("Basic Sword", equipment.Name);
            Assert.Equal(7, equipment.BonusStats);
        }
    }
}