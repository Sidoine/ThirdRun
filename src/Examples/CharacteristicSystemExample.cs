using System;
using ThirdRun.Data;
using ThirdRun.Items;
using MonogameRPG.Monsters;
using MonogameRPG.Map;

namespace ThirdRun.Examples
{
    /// <summary>
    /// Example demonstrating how to use the new characteristic system
    /// </summary>
    public class CharacteristicSystemExample
    {
        public static void DemonstrateSystem()
        {
            Console.WriteLine("=== Characteristic System Demo ===");
            
            // Create a powerful weapon with multiple characteristics
            var flamingSword = new Weapon("Flaming Sword", "A sword wreathed in fire", 500, 15, 25);
            flamingSword.Characteristics.SetValue(Characteristic.MeleeAttackPower, 20);
            flamingSword.Characteristics.SetValue(Characteristic.CriticalChance, 8);
            flamingSword.Characteristics.SetValue(Characteristic.FireResistance, 10);
            
            Console.WriteLine($"Created {flamingSword.Name} with characteristics:");
            Console.WriteLine($"  Melee Attack Power: {flamingSword.Characteristics.GetValue(Characteristic.MeleeAttackPower)}");
            Console.WriteLine($"  Critical Chance: {flamingSword.Characteristics.GetValue(Characteristic.CriticalChance)}");
            Console.WriteLine($"  Fire Resistance: {flamingSword.Characteristics.GetValue(Characteristic.FireResistance)}");
            
            // Create a magical armor with resistances
            var arcaneRobes = new Armor("Arcane Robes", "Robes of magical protection", 800, 8, 12);
            arcaneRobes.Characteristics.SetValue(Characteristic.SpellPower, 25);
            arcaneRobes.Characteristics.SetValue(Characteristic.ArcaneResistance, 30);
            arcaneRobes.Characteristics.SetValue(Characteristic.ShadowResistance, 15);
            
            Console.WriteLine($"\nCreated {arcaneRobes.Name} with characteristics:");
            Console.WriteLine($"  Spell Power: {arcaneRobes.Characteristics.GetValue(Characteristic.SpellPower)}");
            Console.WriteLine($"  Arcane Resistance: {arcaneRobes.Characteristics.GetValue(Characteristic.ArcaneResistance)}");
            Console.WriteLine($"  Shadow Resistance: {arcaneRobes.Characteristics.GetValue(Characteristic.ShadowResistance)}");
            
            // Create a monster with characteristics
            var fireDragon = new MonsterType("Fire Dragon", 200, 30, "monsters/fire_dragon", 10);
            fireDragon.Characteristics.SetValue(Characteristic.MeleeAttackPower, 35);
            fireDragon.Characteristics.SetValue(Characteristic.FireResistance, 50);
            fireDragon.Characteristics.SetValue(Characteristic.IceResistance, -25); // Weakness
            fireDragon.Characteristics.SetValue(Characteristic.Haste, 5);
            
            var dragon = new Monster(fireDragon);
            
            Console.WriteLine($"\nCreated {dragon.Type.Name} with characteristics:");
            Console.WriteLine($"  Melee Attack Power: {dragon.Characteristics.GetValue(Characteristic.MeleeAttackPower)}");
            Console.WriteLine($"  Fire Resistance: {dragon.Characteristics.GetValue(Characteristic.FireResistance)}");
            Console.WriteLine($"  Ice Resistance: {dragon.Characteristics.GetValue(Characteristic.IceResistance)} (weakness)");
            Console.WriteLine($"  Haste: {dragon.Characteristics.GetValue(Characteristic.Haste)}");
            
            // Demonstrate equipment application
            var worldMap = new WorldMap();
            worldMap.Initialize();
            var warrior = new Character("Brave Warrior", CharacterClass.Guerrier, 120, 20, worldMap);
            
            Console.WriteLine($"\n=== Equipment Demo ===");
            Console.WriteLine($"Warrior before equipping:");
            Console.WriteLine($"  Attack Power: {warrior.AttackPower}");
            Console.WriteLine($"  Melee Attack Power: {warrior.Characteristics.GetValue(Characteristic.MeleeAttackPower)}");
            Console.WriteLine($"  Critical Chance: {warrior.Characteristics.GetValue(Characteristic.CriticalChance)}");
            
            // Equip the flaming sword
            warrior.Equip(flamingSword);
            
            Console.WriteLine($"\nWarrior after equipping {flamingSword.Name}:");
            Console.WriteLine($"  Attack Power: {warrior.AttackPower} (legacy bonus: +{flamingSword.BonusStats})");
            Console.WriteLine($"  Melee Attack Power: {warrior.Characteristics.GetValue(Characteristic.MeleeAttackPower)} (characteristic bonus)");
            Console.WriteLine($"  Critical Chance: {warrior.Characteristics.GetValue(Characteristic.CriticalChance)} (characteristic bonus)");
            Console.WriteLine($"  Fire Resistance: {warrior.Characteristics.GetValue(Characteristic.FireResistance)} (characteristic bonus)");
            
            Console.WriteLine("\n=== Demo Complete ===");
        }
    }
}