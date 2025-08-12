using System;
using MonogameRPG.Monsters;
using Xunit;
using System.Linq;

namespace ThirdRun.Tests
{
    public class MonsterTemplateRepositoryTests
    {
        [Fact]
        public void GetAllMonsterTemplates_ReturnsMultipleTemplates()
        {
            // Act
            var templates = MonsterTemplateRepository.GetAllMonsterTemplates();

            // Assert
            Assert.NotEmpty(templates);
            Assert.True(templates.Count > 5, "Should have more than 5 monster types");
        }

        [Fact]
        public void GetRandomTemplate_ReturnsValidTemplate()
        {
            // Act
            var template = MonsterTemplateRepository.GetRandomTemplate(new Random(12345));

            // Assert
            Assert.NotNull(template);
            Assert.False(string.IsNullOrEmpty(template.Name));
            Assert.False(string.IsNullOrEmpty(template.ImagePath));
            Assert.True(template.BaseHealth > 0);
            Assert.True(template.BaseAttack > 0);
            Assert.True(template.Level > 0);
        }

        [Fact]
        public void GetRandomTemplateForLevel_ReturnsAppropriateLevel()
        {
            // Act
            var random = new Random(12345);
            var template1 = MonsterTemplateRepository.GetRandomTemplateForLevel(1, 2, random);
            var template5 = MonsterTemplateRepository.GetRandomTemplateForLevel(4, 5, random);

            // Assert
            Assert.InRange(template1.Level, 1, 2);
            Assert.InRange(template5.Level, 4, 5);
        }

        [Fact]
        public void GetCategoryTemplates_ReturnsDifferentTypes()
        {
            // Act
            var humanoid = MonsterTemplateRepository.GetHumanoidTemplates();
            var undead = MonsterTemplateRepository.GetUndeadTemplates();
            var beasts = MonsterTemplateRepository.GetBeastTemplates();
            var magical = MonsterTemplateRepository.GetMagicalTemplates();
            var creatures = MonsterTemplateRepository.GetCreatureTemplates();

            // Assert
            Assert.NotEmpty(humanoid);
            Assert.NotEmpty(undead);
            Assert.NotEmpty(beasts);
            Assert.NotEmpty(magical);
            Assert.NotEmpty(creatures);
        }

        [Fact]
        public void GetAllImagePaths_ReturnsUniqueImagePaths()
        {
            // Act
            var imagePaths = MonsterTemplateRepository.GetAllImagePaths().ToList();

            // Assert
            Assert.NotEmpty(imagePaths);
            Assert.All(imagePaths, path => Assert.StartsWith("Monsters/", path));
            
            // Check for some expected paths
            Assert.Contains("Monsters/orc", imagePaths);
            Assert.Contains("Monsters/goblin", imagePaths);
            Assert.Contains("Monsters/skeleton", imagePaths);
            Assert.Contains("Monsters/wolf", imagePaths);
        }

        [Fact]
        public void MonsterTemplate_ToMonsterType_CreatesValidMonsterType()
        {
            // Arrange
            var template = MonsterTemplateRepository.GetRandomTemplate(new Random(12345));

            // Act
            var monsterType = template.ToMonsterType();

            // Assert
            Assert.Equal(template.Name, monsterType.Name);
            Assert.Equal(template.BaseHealth, monsterType.BaseHealth);
            Assert.Equal(template.BaseAttack, monsterType.BaseAttack);
            Assert.Equal(template.ImagePath, monsterType.TexturePath);
            Assert.Equal(template.Level, monsterType.Level);
        }

        [Fact]
        public void CreateRandomMonsterType_ReturnsValidMonsterType()
        {
            // Act
            var random = new Random(12345);
            var monsterType = MonsterTemplateRepository.CreateRandomMonsterType(random);

            // Assert
            Assert.NotNull(monsterType);
            Assert.False(string.IsNullOrEmpty(monsterType.Name));
            Assert.True(monsterType.BaseHealth > 0);
            Assert.True(monsterType.BaseAttack > 0);
            Assert.True(monsterType.Level > 0);
        }

        [Fact]
        public void CreateRandomMonsterTypeForLevel_ReturnsAppropriateLevel()
        {
            // Act
            var random = new Random(12345);
            var monsterType = MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(2, 4, random);

            // Assert
            Assert.InRange(monsterType.Level, 2, 4);
        }

        [Fact]
        public void MonsterTemplates_HaveDiverseStats()
        {
            // Act
            var templates = MonsterTemplateRepository.GetAllMonsterTemplates();

            // Assert - Check that we have variety in stats
            var healthValues = templates.Select(t => t.BaseHealth).Distinct().Count();
            var attackValues = templates.Select(t => t.BaseAttack).Distinct().Count();
            var levels = templates.Select(t => t.Level).Distinct().Count();

            Assert.True(healthValues > 3, "Should have diverse health values");
            Assert.True(attackValues > 3, "Should have diverse attack values");
            Assert.True(levels > 1, "Should have different level monsters");
        }

        [Fact]
        public void MonsterTemplates_ImagePaths_AreWellFormed()
        {
            // Act
            var templates = MonsterTemplateRepository.GetAllMonsterTemplates();

            // Assert
            foreach (var template in templates)
            {
                Assert.StartsWith("Monsters/", template.ImagePath);
                Assert.DoesNotContain(" ", template.ImagePath); // No spaces in paths
                Assert.DoesNotContain("\\", template.ImagePath); // Forward slashes only
            }
        }
    }
}