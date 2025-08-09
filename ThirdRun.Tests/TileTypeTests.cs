using Xunit;
using Microsoft.Xna.Framework;
using MonogameRPG.Map;

namespace ThirdRun.Tests
{
    public class TileTypeTests
    {
        [Fact]
        public void TileType_Constructor_SetsAllProperties()
        {
            // Arrange
            string name = "Test Tile";
            Color color = Color.Red;
            int width = 48;
            int height = 48;
            bool isWalkable = true;
            var type = TileTypeEnum.Grass;
            
            // Act
            var tileType = new TileType(name, color, width, height, isWalkable, type);
            
            // Assert
            Assert.Equal(name, tileType.Name);
            Assert.Equal(color, tileType.Color);
            Assert.Equal(width, tileType.Width);
            Assert.Equal(height, tileType.Height);
            Assert.Equal(isWalkable, tileType.IsWalkable);
            Assert.Equal(type, tileType.Type);
        }
        
        [Theory]
        [InlineData(TileTypeEnum.Grass, "Herbe", true)]
        [InlineData(TileTypeEnum.Water, "Eau", false)]
        [InlineData(TileTypeEnum.Rock, "Roche", false)]
        [InlineData(TileTypeEnum.Tree, "Arbre", false)]
        [InlineData(TileTypeEnum.Building, "Bâtiment", false)]
        [InlineData(TileTypeEnum.Door, "Porte", true)]
        [InlineData(TileTypeEnum.River, "Rivière", false)]
        [InlineData(TileTypeEnum.Road, "Route", true)]
        [InlineData(TileTypeEnum.Bridge, "Pont", true)]
        [InlineData(TileTypeEnum.Hill, "Colline", true)]
        public void TileType_FactoryMethods_CreateCorrectTileTypes(TileTypeEnum expectedType, string expectedName, bool expectedWalkable)
        {
            // Arrange
            int width = 48;
            int height = 48;
            
            // Act
            TileType tileType = expectedType switch
            {
                TileTypeEnum.Grass => TileType.CreateGrass(width, height),
                TileTypeEnum.Water => TileType.CreateWater(width, height),
                TileTypeEnum.Rock => TileType.CreateRock(width, height),
                TileTypeEnum.Tree => TileType.CreateTree(width, height),
                TileTypeEnum.Building => TileType.CreateBuilding(width, height),
                TileTypeEnum.Door => TileType.CreateDoor(width, height),
                TileTypeEnum.River => TileType.CreateRiver(width, height),
                TileTypeEnum.Road => TileType.CreateRoad(width, height),
                TileTypeEnum.Bridge => TileType.CreateBridge(width, height),
                TileTypeEnum.Hill => TileType.CreateHill(width, height),
                _ => throw new ArgumentException($"Unsupported tile type: {expectedType}")
            };
            
            // Assert
            Assert.Equal(expectedName, tileType.Name);
            Assert.Equal(expectedType, tileType.Type);
            Assert.Equal(expectedWalkable, tileType.IsWalkable);
            Assert.Equal(width, tileType.Width);
            Assert.Equal(height, tileType.Height);
        }
        
        [Fact]
        public void TileType_CreateGrass_HasCorrectColor()
        {
            // Act
            var grassTile = TileType.CreateGrass(48, 48);
            
            // Assert
            Assert.Equal(Color.ForestGreen, grassTile.Color);
        }
        
        [Fact]
        public void TileType_CreateWater_HasCorrectColor()
        {
            // Act
            var waterTile = TileType.CreateWater(48, 48);
            
            // Assert
            Assert.Equal(Color.Blue, waterTile.Color);
        }
        
        [Fact]
        public void TileType_CreateRoad_HasCorrectColor()
        {
            // Act
            var roadTile = TileType.CreateRoad(48, 48);
            
            // Assert
            Assert.Equal(Color.DarkGray, roadTile.Color);
        }
        
        [Fact]
        public void TileType_CreateBridge_HasCorrectColor()
        {
            // Act
            var bridgeTile = TileType.CreateBridge(48, 48);
            
            // Assert
            Assert.Equal(Color.Brown, bridgeTile.Color);
        }
        
        [Fact]
        public void TileType_CreateTree_HasCorrectColor()
        {
            // Act
            var treeTile = TileType.CreateTree(48, 48);
            
            // Assert
            Assert.Equal(Color.DarkGreen, treeTile.Color);
        }
        
        [Fact]
        public void TileType_CreateBuilding_HasCorrectColor()
        {
            // Act
            var buildingTile = TileType.CreateBuilding(48, 48);
            
            // Assert
            Assert.Equal(Color.SaddleBrown, buildingTile.Color);
        }
        
        [Fact]
        public void TileType_CreateDoor_HasCorrectColor()
        {
            // Act
            var doorTile = TileType.CreateDoor(48, 48);
            
            // Assert
            Assert.Equal(Color.Goldenrod, doorTile.Color);
        }
        
        [Theory]
        [InlineData(32, 32)]
        [InlineData(48, 48)]
        [InlineData(64, 64)]
        public void TileType_FactoryMethods_RespectDimensions(int width, int height)
        {
            // Act
            var grassTile = TileType.CreateGrass(width, height);
            var waterTile = TileType.CreateWater(width, height);
            var roadTile = TileType.CreateRoad(width, height);
            
            // Assert
            Assert.Equal(width, grassTile.Width);
            Assert.Equal(height, grassTile.Height);
            Assert.Equal(width, waterTile.Width);
            Assert.Equal(height, waterTile.Height);
            Assert.Equal(width, roadTile.Width);
            Assert.Equal(height, roadTile.Height);
        }
        
        [Fact]
        public void TileTypeEnum_HasAllExpectedValues()
        {
            // Arrange
            var expectedTypes = new[]
            {
                TileTypeEnum.Grass,
                TileTypeEnum.Water,
                TileTypeEnum.Rock,
                TileTypeEnum.Tree,
                TileTypeEnum.Building,
                TileTypeEnum.Door,
                TileTypeEnum.River,
                TileTypeEnum.Road,
                TileTypeEnum.Bridge,
                TileTypeEnum.Hill
            };
            
            // Act
            var allValues = Enum.GetValues<TileTypeEnum>();
            
            // Assert
            Assert.Equal(expectedTypes.Length, allValues.Length);
            foreach (var expectedType in expectedTypes)
            {
                Assert.Contains(expectedType, allValues);
            }
        }
    }
}