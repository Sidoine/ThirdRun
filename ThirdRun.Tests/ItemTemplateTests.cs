using System;
using System.Linq;
using Xunit;
using ThirdRun.Items;

namespace ThirdRun.Tests
{
    public class ItemTemplateTests
    {
        [Fact]
        public void WeaponTemplate_CreatesWeaponWithCorrectProperties()
        {
            // Arrange
            var template = new WeaponTemplate("Épée", "Une épée classique", "Items/Weapons/epee", 2);
            
            // Act
            var weapon = (Weapon)template.CreateItem(3, "Magique");
            
            // Assert
            Assert.Equal("Magique Épée", weapon.Name);
            Assert.Equal("Items/Weapons/epee", weapon.ImagePath);
            Assert.Equal(3, weapon.ItemLevel);
            Assert.True(weapon.Damage > 0);
            Assert.True(weapon.Value > 0);
        }

        [Fact]
        public void ArmorTemplate_CreatesArmorWithCorrectProperties()
        {
            // Arrange
            var template = new ArmorTemplate("Casque", "Un casque de protection", "Items/Armors/casque", 2);
            
            // Act
            var armor = (Armor)template.CreateItem(2, "Renforcé");
            
            // Assert
            Assert.Equal("Renforcé Casque", armor.Name);
            Assert.Equal("Items/Armors/casque", armor.ImagePath);
            Assert.Equal(2, armor.ItemLevel);
            Assert.True(armor.Defense > 0);
            Assert.True(armor.Value > 0);
        }

        [Fact]
        public void PotionTemplate_CreatesPotionWithCorrectProperties()
        {
            // Arrange
            var template = new PotionTemplate("Potion de Soin", "Restaure la santé", "Items/Potions/potion_soin", 3);
            
            // Act
            var potion = (Potion)template.CreateItem(4);
            
            // Assert
            Assert.Equal("Potion de Soin (Niv. 4)", potion.Name);
            Assert.Equal("Items/Potions/potion_soin", potion.ImagePath);
            Assert.Equal(4, potion.ItemLevel);
            Assert.True(potion.HealAmount > 0);
            Assert.True(potion.Value > 0);
        }
    }

    public class ItemTemplateRepositoryTests
    {
        [Fact]
        public void GetRandomWeaponTemplate_ReturnsValidTemplate()
        {
            // Act
            var template = ItemTemplateRepository.GetRandomWeaponTemplate();
            
            // Assert
            Assert.NotNull(template);
            Assert.NotNull(template.BaseName);
            Assert.NotNull(template.ImagePath);
            Assert.True(template.ImagePath.StartsWith("Items/Weapons/"));
        }

        [Fact]
        public void GetRandomArmorTemplate_ReturnsValidTemplate()
        {
            // Act
            var template = ItemTemplateRepository.GetRandomArmorTemplate();
            
            // Assert
            Assert.NotNull(template);
            Assert.NotNull(template.BaseName);
            Assert.NotNull(template.ImagePath);
            Assert.True(template.ImagePath.StartsWith("Items/Armors/"));
        }

        [Fact]
        public void GetRandomPotionTemplate_ReturnsValidTemplate()
        {
            // Act
            var template = ItemTemplateRepository.GetRandomPotionTemplate();
            
            // Assert
            Assert.NotNull(template);
            Assert.NotNull(template.BaseName);
            Assert.NotNull(template.ImagePath);
            Assert.True(template.ImagePath.StartsWith("Items/Potions/"));
        }

        [Fact]
        public void GetAllImagePaths_ReturnsAllAvailableImages()
        {
            // Act
            var imagePaths = ItemTemplateRepository.GetAllImagePaths().ToList();
            
            // Assert
            Assert.True(imagePaths.Count > 0);
            
            // Should contain weapon images
            Assert.Contains("Items/Weapons/epee", imagePaths);
            Assert.Contains("Items/Weapons/hache", imagePaths);
            
            // Should contain armor images
            Assert.Contains("Items/Armors/casque", imagePaths);
            Assert.Contains("Items/Armors/plastron", imagePaths);
            
            // Should contain potion images
            Assert.Contains("Items/Potions/potion_soin", imagePaths);
            Assert.Contains("Items/Potions/potion_magie", imagePaths);
        }

        [Fact]
        public void AllTemplates_HaveValidImagePaths()
        {
            // Act
            var allWeapons = ItemTemplateRepository.GetAllWeaponTemplates();
            var allArmors = ItemTemplateRepository.GetAllArmorTemplates();
            var allPotions = ItemTemplateRepository.GetAllPotionTemplates();
            
            // Assert - All weapon templates should have valid image paths
            foreach (var weapon in allWeapons)
            {
                Assert.NotNull(weapon.ImagePath);
                Assert.True(weapon.ImagePath.StartsWith("Items/Weapons/"));
            }
            
            // Assert - All armor templates should have valid image paths
            foreach (var armor in allArmors)
            {
                Assert.NotNull(armor.ImagePath);
                Assert.True(armor.ImagePath.StartsWith("Items/Armors/"));
            }
            
            // Assert - All potion templates should have valid image paths
            foreach (var potion in allPotions)
            {
                Assert.NotNull(potion.ImagePath);
                Assert.True(potion.ImagePath.StartsWith("Items/Potions/"));
            }
        }

        [Fact]
        public void GetRandomWeaponPrefix_ReturnsValidPrefix()
        {
            // Act
            var prefix = ItemTemplateRepository.GetRandomWeaponPrefix();
            
            // Assert
            Assert.NotNull(prefix);
            Assert.NotEmpty(prefix);
        }

        [Fact]
        public void GetRandomArmorPrefix_ReturnsValidPrefix()
        {
            // Act
            var prefix = ItemTemplateRepository.GetRandomArmorPrefix();
            
            // Assert
            Assert.NotNull(prefix);
            Assert.NotEmpty(prefix);
        }
    }

    public class RandomItemGeneratorWithTemplatesTests
    {
        [Fact]
        public void GenerateRandomItem_CreatesItemWithImagePath()
        {
            // Act
            var item = RandomItemGenerator.GenerateRandomItem(1);
            
            // Assert
            Assert.NotNull(item);
            Assert.NotNull(item.ImagePath);
            Assert.NotEmpty(item.ImagePath);
        }

        [Fact]
        public void GenerateRandomWeapon_HasCorrectImagePath()
        {
            // Act - Generate many items to test different weapon types
            for (int i = 0; i < 50; i++)
            {
                var item = RandomItemGenerator.GenerateRandomItem(1);
                if (item is Weapon weapon)
                {
                    // Assert
                    Assert.NotNull(weapon.ImagePath);
                    Assert.True(weapon.ImagePath.StartsWith("Items/Weapons/"));
                }
            }
        }

        [Fact]
        public void GenerateRandomArmor_HasCorrectImagePath()
        {
            // Act - Generate many items to test different armor types
            for (int i = 0; i < 50; i++)
            {
                var item = RandomItemGenerator.GenerateRandomItem(1);
                if (item is Armor armor)
                {
                    // Assert
                    Assert.NotNull(armor.ImagePath);
                    Assert.True(armor.ImagePath.StartsWith("Items/Armors/"));
                }
            }
        }

        [Fact]
        public void GenerateRandomPotion_HasCorrectImagePath()
        {
            // Act - Generate many items to test different potion types
            for (int i = 0; i < 50; i++)
            {
                var item = RandomItemGenerator.GenerateRandomItem(1);
                if (item is Potion potion)
                {
                    // Assert
                    Assert.NotNull(potion.ImagePath);
                    Assert.True(potion.ImagePath.StartsWith("Items/Potions/"));
                }
            }
        }
    }
}