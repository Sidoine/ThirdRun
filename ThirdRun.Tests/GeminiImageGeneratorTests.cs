using Xunit;
using System.Threading.Tasks;
using System.IO;
using ThirdRun.Utils;

namespace ThirdRun.Tests
{
    public class GeminiImageGeneratorTests
    {
        [Fact]
        public void GeminiImageGenerator_CanBeInstantiated()
        {
            // Arrange & Act
            var generator = new GeminiImageGenerator("test-project", "us-central1", testMode: true);
            
            // Assert
            Assert.NotNull(generator);
        }

        [Fact]
        public void ValidateConfiguration_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var generator = new GeminiImageGenerator("valid-project", "us-central1", testMode: true);
            
            // Act
            bool isValid = generator.ValidateConfiguration();
            
            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateConfiguration_WithInvalidParameters_ReturnsFalse()
        {
            // Arrange
            var generator = new GeminiImageGenerator("", "", testMode: true);
            
            // Act
            bool isValid = generator.ValidateConfiguration();
            
            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task GenerateImageAsync_CreatesPlaceholderFile()
        {
            // Arrange
            var generator = new GeminiImageGenerator("test-project", "us-central1", testMode: true);
            string tempDir = Path.Combine(Path.GetTempPath(), "ThirdRunTests");
            string outputPath = Path.Combine(tempDir, "test_image.png");
            string metadataPath = Path.ChangeExtension(outputPath, ".txt");
            
            // Clean up any existing files
            if (File.Exists(metadataPath))
                File.Delete(metadataPath);
                
            // Act
            bool result = await generator.GenerateImageAsync("test prompt", outputPath);
            
            // Assert
            Assert.True(result);
            Assert.True(File.Exists(metadataPath));
            
            string content = await File.ReadAllTextAsync(metadataPath);
            Assert.Contains("test prompt", content);
            Assert.Contains("Generated image for prompt:", content);
            
            // Clean up
            if (File.Exists(metadataPath))
                File.Delete(metadataPath);
        }

        [Fact]
        public async Task GenerateImageAsync_WithGameCharacterPrompt_ReturnsSuccess()
        {
            // Arrange
            var generator = new GeminiImageGenerator(testMode: true);
            string tempDir = Path.Combine(Path.GetTempPath(), "ThirdRunTests");
            string outputPath = Path.Combine(tempDir, "warrior_sprite.png");
            
            // Act
            bool result = await generator.GenerateImageAsync("medieval warrior character sprite for 2D RPG game", outputPath);
            
            // Assert
            Assert.True(result);
            
            // Verify metadata file was created
            string metadataPath = Path.ChangeExtension(outputPath, ".txt");
            Assert.True(File.Exists(metadataPath));
            
            // Clean up
            if (File.Exists(metadataPath))
                File.Delete(metadataPath);
        }

        [Fact]
        public void Dispose_DoesNotThrowException()
        {
            // Arrange
            var generator = new GeminiImageGenerator(testMode: true);
            
            // Act & Assert
            var exception = Record.Exception(() => generator.Dispose());
            Assert.Null(exception);
        }
    }
}