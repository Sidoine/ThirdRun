using System.Linq;
using Xunit;
using ThirdRun.Utils;

namespace ThirdRun.Tests
{
    public class ItemImageMapperTests
    {
        [Fact]
        public void GetImagePath_WeaponWithPrefix_ReturnsCorrectPath()
        {
            // Test that prefixed weapons map to correct images
            Assert.Equal("Items/Weapons/epee", ItemImageMapper.GetImagePath("Rouillé Épée"));
            Assert.Equal("Items/Weapons/hache", ItemImageMapper.GetImagePath("Enchanté Hache"));
            Assert.Equal("Items/Weapons/arc", ItemImageMapper.GetImagePath("Magique Arc"));
        }

        [Fact]
        public void GetImagePath_ArmorWithPrefix_ReturnsCorrectPath()
        {
            // Test that prefixed armor maps to correct images
            Assert.Equal("Items/Armors/casque", ItemImageMapper.GetImagePath("Renforcé Casque"));
            Assert.Equal("Items/Armors/plastron", ItemImageMapper.GetImagePath("Légendaire Plastron"));
            Assert.Equal("Items/Armors/cotte_de_mailles", ItemImageMapper.GetImagePath("Béni Cotte de mailles"));
        }

        [Fact]
        public void GetImagePath_PotionWithLevel_ReturnsCorrectPath()
        {
            // Test that potions with level suffixes map to correct images
            Assert.Equal("Items/Potions/potion_soin", ItemImageMapper.GetImagePath("Potion de Soin (Niv. 2)"));
            Assert.Equal("Items/Potions/potion_force", ItemImageMapper.GetImagePath("Potion de Force (Niv. 5)"));
            Assert.Equal("Items/Potions/potion_agilite", ItemImageMapper.GetImagePath("Potion d'Agilité (Niv. 1)"));
        }

        [Fact]
        public void GetImagePath_BaseNamesWithoutPrefix_ReturnsCorrectPath()
        {
            // Test direct mapping without prefixes
            Assert.Equal("Items/Weapons/dague", ItemImageMapper.GetImagePath("Dague"));
            Assert.Equal("Items/Armors/gants", ItemImageMapper.GetImagePath("Gants"));
            Assert.Equal("Items/Potions/potion_magie", ItemImageMapper.GetImagePath("Potion de Magie"));
        }

        [Fact]
        public void GetImagePath_UnknownItem_ReturnsNull()
        {
            // Test that unknown items return null
            Assert.Null(ItemImageMapper.GetImagePath("Unknown Item"));
            Assert.Null(ItemImageMapper.GetImagePath("Mysterious Weapon"));
        }

        [Fact]
        public void GetAllImagePaths_ReturnsAllExpectedPaths()
        {
            // Test that all image paths are returned
            var allPaths = ItemImageMapper.GetAllImagePaths().ToList();
            
            // Verify we have all weapon images
            Assert.Contains("Items/Weapons/epee", allPaths);
            Assert.Contains("Items/Weapons/hache", allPaths);
            Assert.Contains("Items/Weapons/arc", allPaths);
            
            // Verify we have all armor images
            Assert.Contains("Items/Armors/casque", allPaths);
            Assert.Contains("Items/Armors/plastron", allPaths);
            
            // Verify we have all potion images
            Assert.Contains("Items/Potions/potion_soin", allPaths);
            Assert.Contains("Items/Potions/potion_force", allPaths);
            
            // Verify total count (8 weapons + 8 armors + 5 potions = 21)
            Assert.Equal(21, allPaths.Count);
        }
    }
}