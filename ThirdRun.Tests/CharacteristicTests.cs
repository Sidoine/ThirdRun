using ThirdRun.Data;
using Xunit;

namespace ThirdRun.Tests
{
    public class CharacteristicTests
    {
        [Fact]
        public void CharacteristicValues_DefaultValue_IsZero()
        {
            // Arrange
            var characteristics = new CharacteristicValues();

            // Act & Assert
            Assert.Equal(0, characteristics.GetValue(Characteristic.MeleeAttackPower));
            Assert.Equal(0, characteristics.GetValue(Characteristic.SpellPower));
            Assert.Equal(0, characteristics.GetValue(Characteristic.Armor));
        }

        [Fact]
        public void CharacteristicValues_SetValue_UpdatesCorrectly()
        {
            // Arrange
            var characteristics = new CharacteristicValues();

            // Act
            characteristics.SetValue(Characteristic.MeleeAttackPower, 10);
            characteristics.SetValue(Characteristic.CriticalChance, 5);

            // Assert
            Assert.Equal(10, characteristics.GetValue(Characteristic.MeleeAttackPower));
            Assert.Equal(5, characteristics.GetValue(Characteristic.CriticalChance));
            Assert.Equal(0, characteristics.GetValue(Characteristic.SpellPower)); // Unchanged
        }

        [Fact]
        public void CharacteristicValues_AddValue_AccumulatesCorrectly()
        {
            // Arrange
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.Armor, 5);

            // Act
            characteristics.AddValue(Characteristic.Armor, 3);
            characteristics.AddValue(Characteristic.Armor, 2);

            // Assert
            Assert.Equal(10, characteristics.GetValue(Characteristic.Armor));
        }

        [Fact]
        public void CharacteristicValues_RemoveValue_SubtractsCorrectly()
        {
            // Arrange
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.HealingPower, 15);

            // Act
            characteristics.RemoveValue(Characteristic.HealingPower, 5);

            // Assert
            Assert.Equal(10, characteristics.GetValue(Characteristic.HealingPower));
        }

        [Fact]
        public void CharacteristicValues_GetAllValues_ReturnsCorrectDictionary()
        {
            // Arrange
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.MeleeAttackPower, 10);
            characteristics.SetValue(Characteristic.FireResistance, 20);

            // Act
            var allValues = characteristics.GetAllValues();

            // Assert
            Assert.Equal(2, allValues.Count);
            Assert.Equal(10, allValues[Characteristic.MeleeAttackPower]);
            Assert.Equal(20, allValues[Characteristic.FireResistance]);
        }

        [Fact]
        public void CharacteristicValues_Clear_RemovesAllValues()
        {
            // Arrange
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.MeleeAttackPower, 10);
            characteristics.SetValue(Characteristic.Haste, 5);

            // Act
            characteristics.Clear();

            // Assert
            Assert.Equal(0, characteristics.GetValue(Characteristic.MeleeAttackPower));
            Assert.Equal(0, characteristics.GetValue(Characteristic.Haste));
            Assert.Empty(characteristics.GetAllValues());
        }

        [Theory]
        [InlineData(Characteristic.MeleeAttackPower)]
        [InlineData(Characteristic.RangedAttackPower)]
        [InlineData(Characteristic.SpellPower)]
        [InlineData(Characteristic.HealingPower)]
        [InlineData(Characteristic.CriticalChance)]
        [InlineData(Characteristic.Haste)]
        [InlineData(Characteristic.Armor)]
        [InlineData(Characteristic.FireResistance)]
        [InlineData(Characteristic.IceResistance)]
        [InlineData(Characteristic.ArcaneResistance)]
        [InlineData(Characteristic.ShadowResistance)]
        public void Characteristic_Enum_HasAllRequiredValues(Characteristic characteristic)
        {
            // Arrange & Act
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(characteristic, 1);

            // Assert - Should not throw exception and should set the value
            Assert.Equal(1, characteristics.GetValue(characteristic));
        }
    }
}