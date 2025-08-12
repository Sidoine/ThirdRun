using System;
using MonogameRPG;
using ThirdRun.Data.Abilities;
using MonogameRPG.Map;
using MonogameRPG.Monsters;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests
{
    public class CombatScenarioTests
    {
        [Fact]
        public void CombatScenario_HunterVsMonster_ShouldUseRangedAttack()
        {
            // Arrange - Create a realistic combat scenario
            var random = new Random(12345);
            var worldMap = new WorldMap(random);
            worldMap.Initialize();
            var hunter = new Character("Archer", CharacterClass.Chasseur, 100, 15, worldMap.CurrentMap, worldMap);
            var monster = new Monster(new MonsterType("Orc", 50, 12, "Monsters/orc", 1), worldMap.CurrentMap, worldMap, random);
            
            // Position them at range (hunter can use ranged attack)
            hunter.Position = new Vector2(0, 0);
            monster.Position = new Vector2(100, 0); // Within ranged attack range (128px)
            
            // Set game time and get ranged attack ability
            hunter.UpdateGameTime(0f);
            var rangedAttack = hunter.Abilities.First(a => a.Name == "Ranged Attack");
            
            // Act - Hunter attacks monster at range
            hunter.UseAbility(rangedAttack, monster);
            
            // Assert - Monster should take damage from ranged attack
            Assert.Equal(35, monster.CurrentHealth); // 50 - 15 = 35
        }
        
        [Fact]
        public void CombatScenario_PriestHealing_ShouldHealDamagedAlly()
        {
            // Arrange - Create healing scenario
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            var priest = new Character("Healer", CharacterClass.Prêtre, 80, 10, worldMap.CurrentMap, worldMap);
            var warrior = new Character("Tank", CharacterClass.Guerrier, 100, 20, worldMap.CurrentMap, worldMap);
            
            // Damage the warrior
            warrior.CurrentHealth = 30;
            
            // Position them within heal range
            priest.Position = new Vector2(0, 0);
            warrior.Position = new Vector2(50, 0); // Within heal range (64px)
            
            // Set game time and get heal ability
            priest.UpdateGameTime(0f);
            var healAbility = priest.Abilities.First(a => a.Name == "Heal");
            
            // Act - Priest heals the warrior
            priest.UseAbility(healAbility, warrior);
            
            // Assert - Warrior should be healed
            Assert.Equal(35, warrior.CurrentHealth); // 30 + 5 (priest's heal power) = 35
        }
        
        [Fact]
        public void CombatScenario_AbilityCooldowns_ShouldPreventSpamming()
        {
            // Arrange - Test cooldown system using the proven ability system
            var random = new Random(12345);
            var worldMap = new WorldMap(random);
            worldMap.Initialize();
            var warrior = new Character("Fighter", CharacterClass.Guerrier, 100, 20, worldMap.CurrentMap, worldMap);
            var monster = new Monster(new MonsterType("Goblin", 100, 8, "Monsters/goblin", 1), worldMap.CurrentMap, worldMap, random);
            
            warrior.Position = new Vector2(0, 0);
            monster.Position = new Vector2(20, 0); // Within melee range
            
            // Get the default melee attack ability
            var meleeAttack = warrior.DefaultAbility;
            
            // Act & Assert - First attack should work
            warrior.UpdateGameTime(1f);
            warrior.UseAbility(meleeAttack, monster);
            Assert.Equal(80, monster.CurrentHealth); // 100 - 20 = 80
            
            // Immediate second attack should be blocked by cooldown
            var initialHealth = monster.CurrentHealth;
            warrior.UseAbility(meleeAttack, monster);
            Assert.Equal(initialHealth, monster.CurrentHealth); // Should be unchanged due to cooldown
            
            // After cooldown expires, attack should work again
            warrior.UpdateGameTime(3f); // Wait for cooldown to expire
            warrior.UseAbility(meleeAttack, monster);
            Assert.Equal(60, monster.CurrentHealth); // 80 - 20 = 60
        }
    }
}