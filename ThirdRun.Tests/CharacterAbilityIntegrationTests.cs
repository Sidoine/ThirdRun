using System;
using MonogameRPG;
using ThirdRun.Data.Abilities;
using MonogameRPG.Map;
using System.Linq;

namespace ThirdRun.Tests
{
    public class CharacterAbilityIntegrationTests
    {
        [Fact]
        public void Character_Guerrier_ShouldHaveMeleeAndSelfHeal()
        {
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            var character = new Character("Test Warrior", CharacterClass.Guerrier, 100, 20, worldMap.CurrentMap, worldMap);
            
            Assert.Equal(2, character.Abilities.Count);
            Assert.True(character.Abilities.Any(a => a.Name == "Melee Attack"));
            Assert.True(character.Abilities.Any(a => a.Name == "Self Heal"));
        }
        
        [Fact]
        public void Character_Chasseur_ShouldHaveMeleeAndRanged()
        {
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            var character = new Character("Test Hunter", CharacterClass.Chasseur, 100, 15, worldMap.CurrentMap, worldMap);
            
            Assert.Equal(3, character.Abilities.Count); // Melee Attack + Ranged Attack + Regeneration buff
            Assert.True(character.Abilities.Any(a => a.Name == "Melee Attack"));
            Assert.True(character.Abilities.Any(a => a.Name == "Ranged Attack"));
            Assert.True(character.Abilities.Any(a => a.Name == "Regeneration"));
        }
        
        [Fact]
        public void Character_Pretre_ShouldHaveMeleeAndHealing()
        {
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            var character = new Character("Test Priest", CharacterClass.Prêtre, 80, 10, worldMap.CurrentMap, worldMap);
            
            Assert.Equal(4, character.Abilities.Count); // Melee Attack + Heal + Attack Power buff + Self Heal
            Assert.True(character.Abilities.Any(a => a.Name == "Melee Attack"));
            Assert.True(character.Abilities.Any(a => a.Name == "Heal"));
            Assert.True(character.Abilities.Any(a => a.Name == "Blessing of Strength"));
            Assert.True(character.Abilities.Any(a => a.Name == "Self Heal"));
        }
        
        [Fact]
        public void Character_Mage_ShouldHaveMeleeAndRanged()
        {
            var worldMap = new WorldMap(new Random(12345));
            worldMap.Initialize();
            var character = new Character("Test Mage", CharacterClass.Mage, 70, 12, worldMap.CurrentMap, worldMap);
            
            Assert.Equal(3, character.Abilities.Count); // Melee Attack + Ranged Attack + Weakness debuff
            Assert.True(character.Abilities.Any(a => a.Name == "Melee Attack"));
            Assert.True(character.Abilities.Any(a => a.Name == "Ranged Attack"));
            Assert.True(character.Abilities.Any(a => a.Name == "Curse of Weakness"));
        }
    }
}