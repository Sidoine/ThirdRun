using Xunit;
using ThirdRun.Utils;
using System;

namespace ThirdRun.Tests
{
    public class PerlinNoiseTests
    {
        [Fact]
        public void PerlinNoise_WithSameSeed_ProducesSameValues()
        {
            // Arrange
            var noise1 = new PerlinNoise(123);
            var noise2 = new PerlinNoise(123);
            
            // Act
            double value1 = noise1.Noise(1.5, 2.5);
            double value2 = noise2.Noise(1.5, 2.5);
            
            // Assert
            Assert.Equal(value1, value2, 6); // 6 decimal places precision
        }
        
        [Fact]
        public void PerlinNoise_WithDifferentSeeds_ProducesDifferentValues()
        {
            // Arrange
            var noise1 = new PerlinNoise(123);
            var noise2 = new PerlinNoise(456);
            
            // Act
            double value1 = noise1.Noise(1.5, 2.5);
            double value2 = noise2.Noise(1.5, 2.5);
            
            // Assert
            Assert.NotEqual(value1, value2);
        }
        
        [Fact]
        public void PerlinNoise_ReturnsValueInExpectedRange()
        {
            // Arrange
            var noise = new PerlinNoise(0);
            
            // Act & Assert
            for (int i = 0; i < 100; i++)
            {
                double x = i * 0.1;
                double y = i * 0.15;
                double value = noise.Noise(x, y);
                
                Assert.True(value >= -1.0 && value <= 1.0, 
                    $"Noise value {value} at ({x}, {y}) is outside expected range [-1, 1]");
            }
        }
        
        [Fact]
        public void OctaveNoise_ReturnsValueInExpectedRange()
        {
            // Arrange
            var noise = new PerlinNoise(42);
            
            // Act & Assert
            for (int i = 0; i < 50; i++)
            {
                double x = i * 0.1;
                double y = i * 0.15;
                double value = noise.OctaveNoise(x, y, 4, 0.5);
                
                Assert.True(value >= -1.0 && value <= 1.0, 
                    $"Octave noise value {value} at ({x}, {y}) is outside expected range [-1, 1]");
            }
        }
        
        [Theory]
        [InlineData(1, 1.0, 2, 1.0)]
        [InlineData(2, 0.5, 3, 0.5)]
        [InlineData(4, 0.25, 5, 0.25)]
        public void OctaveNoise_WithDifferentOctaves_ProducesDifferentResults(int octaves1, double persistence1, int octaves2, double persistence2)
        {
            // Arrange
            var noise = new PerlinNoise(100);
            
            // Act - Use different coordinates to avoid potential zeros
            double value1 = noise.OctaveNoise(3.7, 4.3, octaves1, persistence1);
            double value2 = noise.OctaveNoise(3.7, 4.3, octaves2, persistence2);
            
            // Assert
            Assert.NotEqual(value1, value2);
        }
    }
}