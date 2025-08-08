using MonogameRPG.Monsters;
using ThirdRun.Data;
using Xunit;

namespace ThirdRun.Tests
{
    public class MonsterCharacteristicTests
    {
        [Fact]
        public void MonsterType_HasCharacteristics_WhenCreated()
        {
            // Arrange & Act
            var monsterType = new MonsterType("Orc", 50, 12, "monsters/orc");

            // Assert
            Assert.NotNull(monsterType.Characteristics);
        }

        [Fact]
        public void MonsterType_Characteristics_CanBeSet()
        {
            // Arrange
            var monsterType = new MonsterType("Fire Elemental", 80, 15, "monsters/fire_elemental");

            // Act
            monsterType.Characteristics.SetValue(Characteristic.FireResistance, 25);
            monsterType.Characteristics.SetValue(Characteristic.IceResistance, -10); // Weakness
            monsterType.Characteristics.SetValue(Characteristic.MeleeAttackPower, 8);

            // Assert
            Assert.Equal(25, monsterType.Characteristics.GetValue(Characteristic.FireResistance));
            Assert.Equal(-10, monsterType.Characteristics.GetValue(Characteristic.IceResistance));
            Assert.Equal(8, monsterType.Characteristics.GetValue(Characteristic.MeleeAttackPower));
        }

        [Fact]
        public void Monster_InheritsCharacteristicsFromType()
        {
            // Arrange
            var monsterType = new MonsterType("Ice Mage", 60, 10, "monsters/ice_mage");
            monsterType.Characteristics.SetValue(Characteristic.SpellPower, 20);
            monsterType.Characteristics.SetValue(Characteristic.IceResistance, 30);
            monsterType.Characteristics.SetValue(Characteristic.FireResistance, -15);

            // Act
            var monster = new Monster(monsterType);

            // Assert
            Assert.Equal(20, monster.Characteristics.GetValue(Characteristic.SpellPower));
            Assert.Equal(30, monster.Characteristics.GetValue(Characteristic.IceResistance));
            Assert.Equal(-15, monster.Characteristics.GetValue(Characteristic.FireResistance));
            
            // Unset characteristics should be zero
            Assert.Equal(0, monster.Characteristics.GetValue(Characteristic.Armor));
            Assert.Equal(0, monster.Characteristics.GetValue(Characteristic.Haste));
        }

        [Fact]
        public void Monster_PreservesLegacyFunctionality()
        {
            // Arrange
            var monsterType = new MonsterType("Goblin", 30, 8, "monsters/goblin", 2);
            
            // Act
            var monster = new Monster(monsterType);

            // Assert - Legacy properties still work
            Assert.Equal("Goblin", monster.Type.Name);
            Assert.Equal(30, monster.CurrentHealth);
            Assert.Equal(30, monster.MaxHealth);
            Assert.Equal(8, monster.AttackPower);
            Assert.Equal(2, monster.Level);
            Assert.False(monster.IsDead);
        }

        [Fact]
        public void Monster_CharacteristicsAreIndependent()
        {
            // Arrange
            var monsterType = new MonsterType("Shadow Beast", 70, 14, "monsters/shadow");
            monsterType.Characteristics.SetValue(Characteristic.ShadowResistance, 40);
            
            var monster1 = new Monster(monsterType);
            var monster2 = new Monster(monsterType);

            // Act - Modify one monster's characteristics
            monster1.Characteristics.AddValue(Characteristic.ShadowResistance, 10);
            monster1.Characteristics.SetValue(Characteristic.Haste, 5);

            // Assert - monster2 is unaffected
            Assert.Equal(40, monster2.Characteristics.GetValue(Characteristic.ShadowResistance));
            Assert.Equal(0, monster2.Characteristics.GetValue(Characteristic.Haste));
            
            // monster1 has the modifications
            Assert.Equal(50, monster1.Characteristics.GetValue(Characteristic.ShadowResistance)); // 40 + 10
            Assert.Equal(5, monster1.Characteristics.GetValue(Characteristic.Haste));
            
            // Original type is unchanged
            Assert.Equal(40, monsterType.Characteristics.GetValue(Characteristic.ShadowResistance));
            Assert.Equal(0, monsterType.Characteristics.GetValue(Characteristic.Haste));
        }

        [Fact]
        public void Monster_AllMagicalResistances_CanBeSet()
        {
            // Arrange
            var monsterType = new MonsterType("Arcane Guardian", 100, 18, "monsters/arcane_guardian");
            monsterType.Characteristics.SetValue(Characteristic.FireResistance, 10);
            monsterType.Characteristics.SetValue(Characteristic.IceResistance, 15);
            monsterType.Characteristics.SetValue(Characteristic.ArcaneResistance, 25);
            monsterType.Characteristics.SetValue(Characteristic.ShadowResistance, 5);

            // Act
            var monster = new Monster(monsterType);

            // Assert - All resistances properly inherited
            Assert.Equal(10, monster.Characteristics.GetValue(Characteristic.FireResistance));
            Assert.Equal(15, monster.Characteristics.GetValue(Characteristic.IceResistance));
            Assert.Equal(25, monster.Characteristics.GetValue(Characteristic.ArcaneResistance));
            Assert.Equal(5, monster.Characteristics.GetValue(Characteristic.ShadowResistance));
        }
    }
}