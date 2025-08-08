using Microsoft.Xna.Framework;
using MonogameRPG;
using ThirdRun.Data;
using Xunit;

namespace ThirdRun.Tests
{
    public class UnitCharacteristicTests
    {
        [Fact]
        public void Unit_HasCharacteristics_WhenCreated()
        {
            // Arrange & Act
            var unit = new TestUnit();

            // Assert
            Assert.NotNull(unit.Characteristics);
        }

        [Fact]
        public void Unit_Characteristics_AreInitializedToZero()
        {
            // Arrange & Act
            var unit = new TestUnit();

            // Assert
            Assert.Equal(0, unit.Characteristics.GetValue(Characteristic.MeleeAttackPower));
            Assert.Equal(0, unit.Characteristics.GetValue(Characteristic.Armor));
            Assert.Equal(0, unit.Characteristics.GetValue(Characteristic.FireResistance));
        }

        [Fact]
        public void Unit_Characteristics_CanBeModified()
        {
            // Arrange
            var unit = new TestUnit();

            // Act
            unit.Characteristics.SetValue(Characteristic.MeleeAttackPower, 15);
            unit.Characteristics.SetValue(Characteristic.IceResistance, 10);

            // Assert
            Assert.Equal(15, unit.Characteristics.GetValue(Characteristic.MeleeAttackPower));
            Assert.Equal(10, unit.Characteristics.GetValue(Characteristic.IceResistance));
        }

        [Fact]
        public void Unit_Characteristics_PreserveExistingFunctionality()
        {
            // Arrange & Act
            var unit = new TestUnit
            {
                Position = new Vector2(10, 20),
                CurrentHealth = 80,
                MaxHealth = 100,
                AttackPower = 15
            };

            // Assert - Existing properties still work
            Assert.Equal(new Vector2(10, 20), unit.Position);
            Assert.Equal(80, unit.CurrentHealth);
            Assert.Equal(100, unit.MaxHealth);
            Assert.Equal(15, unit.AttackPower);
            Assert.False(unit.IsDead);
        }

        // Test implementation of abstract Unit class
        private class TestUnit : Unit
        {
            public TestUnit() : base()
            {
                // Constructor calls base() which initializes Characteristics
            }
        }
    }
}