using System;
using ThirdRun.Data;
using ThirdRun.Items;
using MonogameRPG.Monsters;
using MonogameRPG.Map;
using Xunit;

namespace ThirdRun.Tests
{
    /// <summary>
    /// Integration test demonstrating how to use the characteristic system
    /// </summary>
    public class CharacteristicSystemIntegrationTest
    {
        [Fact]
        public void CharacteristicSystem_Integration_WorksCorrectly()
        {
            // Setup world map for testing
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            // Create a powerful weapon with multiple characteristics
            var flamingSword = new Weapon("Flaming Sword", "A sword wreathed in fire", 500, 15, 25);
            flamingSword.Characteristics.AddValue(Characteristic.MeleeAttackPower, 20); // Add 20 more to the 15 from legacy
            flamingSword.Characteristics.SetValue(Characteristic.CriticalChance, 8);
            flamingSword.Characteristics.SetValue(Characteristic.FireResistance, 10);
            
            // Verify weapon characteristics are set correctly
            Assert.Equal(35, flamingSword.Characteristics.GetValue(Characteristic.MeleeAttackPower)); // 15 (legacy) + 20 (additional)
            Assert.Equal(8, flamingSword.Characteristics.GetValue(Characteristic.CriticalChance));
            Assert.Equal(10, flamingSword.Characteristics.GetValue(Characteristic.FireResistance));
            
            // Create magical armor with resistances
            var arcaneRobes = new Armor("Arcane Robes", "Robes of magical protection", 800, 0, 12);
            arcaneRobes.Characteristics.SetValue(Characteristic.SpellPower, 25);
            arcaneRobes.Characteristics.SetValue(Characteristic.ArcaneResistance, 30);
            arcaneRobes.Characteristics.SetValue(Characteristic.ShadowResistance, 15);
            
            // Verify armor characteristics
            Assert.Equal(25, arcaneRobes.Characteristics.GetValue(Characteristic.SpellPower));
            Assert.Equal(30, arcaneRobes.Characteristics.GetValue(Characteristic.ArcaneResistance));
            Assert.Equal(15, arcaneRobes.Characteristics.GetValue(Characteristic.ShadowResistance));
            
            // Create a monster with characteristics
            var fireDragon = new MonsterType("Fire Dragon", 200, 30, "monsters/fire_dragon", 10);
            fireDragon.Characteristics.AddValue(Characteristic.MeleeAttackPower, 35); // Add 35 to the base 30
            fireDragon.Characteristics.SetValue(Characteristic.FireResistance, 50);
            fireDragon.Characteristics.SetValue(Characteristic.IceResistance, -25); // Weakness
            fireDragon.Characteristics.SetValue(Characteristic.Haste, 5);
            
            var dragon = new Monster(fireDragon, worldMap.CurrentMap, worldMap);
            
            // Verify monster characteristics are copied correctly
            Assert.Equal(200, dragon.MaxHealth);
            Assert.Equal(65, dragon.AttackPower); // 30 (base) + 35 (additional)
            Assert.Equal(50, dragon.Characteristics.GetValue(Characteristic.FireResistance));
            Assert.Equal(-25, dragon.Characteristics.GetValue(Characteristic.IceResistance)); // Weakness
            Assert.Equal(5, dragon.Characteristics.GetValue(Characteristic.Haste));
            
            // Test equipment application on character
            var warrior = new Character("Brave Warrior", CharacterClass.Guerrier, 120, 20, worldMap.CurrentMap, worldMap);
            
            // Check warrior's initial state
            Assert.Equal(20, warrior.AttackPower);
            Assert.Equal(0, warrior.Characteristics.GetValue(Characteristic.CriticalChance));
            Assert.Equal(0, warrior.Characteristics.GetValue(Characteristic.FireResistance));
            
            // Equip the flaming sword
            warrior.Equip(flamingSword);
            
            // Verify equipment bonuses are applied
            Assert.Equal(55, warrior.AttackPower); // 20 (base) + 35 (weapon characteristics)
            Assert.Equal(8, warrior.Characteristics.GetValue(Characteristic.CriticalChance));
            Assert.Equal(10, warrior.Characteristics.GetValue(Characteristic.FireResistance));
            
            // Unequip and verify bonuses are removed
            flamingSword.Unequip(warrior);
            
            Assert.Equal(20, warrior.AttackPower); // Back to base
            Assert.Equal(0, warrior.Characteristics.GetValue(Characteristic.CriticalChance));
            Assert.Equal(0, warrior.Characteristics.GetValue(Characteristic.FireResistance));
        }
        
        [Fact]
        public void CharacteristicSystem_HealthCharacteristic_WorksCorrectly()
        {
            // Test the new Health characteristic
            var worldMap = new WorldMap();
            worldMap.Initialize();
            var character = new Character("Test Character", CharacterClass.Guerrier, 100, 15, worldMap.CurrentMap, worldMap);
            
            // Verify initial health
            Assert.Equal(100, character.MaxHealth);
            Assert.Equal(100, character.Characteristics.GetValue(Characteristic.Health));
            
            // Modify health through characteristics
            character.Characteristics.AddValue(Characteristic.Health, 50);
            Assert.Equal(150, character.MaxHealth);
            
            // Test monster health characteristic
            var toughMonster = new MonsterType("Tough Monster", 300, 25, "monsters/tough", 5);
            Assert.Equal(300, toughMonster.BaseHealth);
            Assert.Equal(300, toughMonster.Characteristics.GetValue(Characteristic.Health));
            
            var monster = new Monster(toughMonster, worldMap.CurrentMap, worldMap);
            Assert.Equal(300, monster.MaxHealth);
            Assert.Equal(300, monster.CurrentHealth);
        }
    }
}