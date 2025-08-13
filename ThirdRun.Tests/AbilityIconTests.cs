using System;
using System.IO;
using Xunit;
using ThirdRun.Data.Abilities;

namespace ThirdRun.Tests
{
    public class AbilityIconTests
    {
        [Fact]
        public void MeleeAttackAbility_ShouldHaveIconPath()
        {
            // Arrange & Act
            var ability = new MeleeAttackAbility();
            
            // Assert
            Assert.NotNull(ability.IconPath);
            Assert.NotEmpty(ability.IconPath);
            Assert.Equal("Abilities/melee_attack", ability.IconPath);
        }

        [Fact]
        public void HealAbility_ShouldHaveIconPath()
        {
            // Arrange & Act
            var ability = new HealAbility();
            
            // Assert
            Assert.NotNull(ability.IconPath);
            Assert.NotEmpty(ability.IconPath);
            Assert.Equal("Abilities/heal", ability.IconPath);
        }

        [Fact]
        public void RangedAttackAbility_ShouldHaveIconPath()
        {
            // Arrange & Act
            var ability = new RangedAttackAbility();
            
            // Assert
            Assert.NotNull(ability.IconPath);
            Assert.NotEmpty(ability.IconPath);
            Assert.Equal("Abilities/ranged_attack", ability.IconPath);
        }

        [Fact]
        public void SelfHealAbility_ShouldHaveIconPath()
        {
            // Arrange & Act
            var ability = new SelfHealAbility();
            
            // Assert
            Assert.NotNull(ability.IconPath);
            Assert.NotEmpty(ability.IconPath);
            Assert.Equal("Abilities/self_heal", ability.IconPath);
        }

        [Fact]
        public void AbilityIcons_ShouldExistInContentFolder()
        {
            // Arrange
            string contentPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "Content", "Abilities");
            
            // Act & Assert
            Assert.True(Directory.Exists(contentPath), $"Abilities content directory should exist at {contentPath}");
            
            string meleeIcon = Path.Combine(contentPath, "melee_attack.png");
            string healIcon = Path.Combine(contentPath, "heal.png");
            string rangedIcon = Path.Combine(contentPath, "ranged_attack.png");
            string selfHealIcon = Path.Combine(contentPath, "self_heal.png");
            
            Assert.True(File.Exists(meleeIcon), $"Melee attack icon should exist at {meleeIcon}");
            Assert.True(File.Exists(healIcon), $"Heal icon should exist at {healIcon}");
            Assert.True(File.Exists(rangedIcon), $"Ranged attack icon should exist at {rangedIcon}");
            Assert.True(File.Exists(selfHealIcon), $"Self heal icon should exist at {selfHealIcon}");
        }
    }
}