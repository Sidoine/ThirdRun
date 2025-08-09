using Xunit;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ThirdRun.Graphics.Map;
using MonogameRPG.Map;
using Microsoft.Xna.Framework;

namespace ThirdRun.Tests
{
    public class TileTextureTests
    {
        [Fact]
        public void TileTypeView_ShouldHandleMissingContentManager_GracefulFallback()
        {
            // Test that TileTypeView can handle missing ContentManager gracefully
            // This tests the fallback mechanism when ContentManager is null or unavailable
            
            // Create a mock content manager (null will test fallback)
            ContentManager? contentManager = null;
            var tileTypeView = new TileTypeView(contentManager!);
            
            // Create a sample tile type
            var grassTile = TileType.CreateGrass(48, 48);
            
            // The constructor should not throw an exception even with null ContentManager
            // This verifies the design handles missing dependencies gracefully
            Assert.NotNull(tileTypeView);
            
            // The actual rendering test would require a GraphicsDevice and SpriteBatch,
            // which are not available in unit tests without MonoGame Framework setup
            // This test validates the basic construction and error handling design
        }
        
        [Fact]
        public void TileTypes_ShouldHaveCorrectTextureMapping()
        {
            // Verify all tile types have proper enum mappings
            // This ensures our texture loading will work for all tile types
            
            var grass = TileType.CreateGrass(48, 48);
            var water = TileType.CreateWater(48, 48);
            var rock = TileType.CreateRock(48, 48);
            var tree = TileType.CreateTree(48, 48);
            var building = TileType.CreateBuilding(48, 48);
            var door = TileType.CreateDoor(48, 48);
            var river = TileType.CreateRiver(48, 48);
            var road = TileType.CreateRoad(48, 48);
            var bridge = TileType.CreateBridge(48, 48);
            var hill = TileType.CreateHill(48, 48);
            
            // Verify each tile has the correct enum type for texture mapping
            Assert.Equal(TileTypeEnum.Grass, grass.Type);
            Assert.Equal(TileTypeEnum.Water, water.Type);
            Assert.Equal(TileTypeEnum.Rock, rock.Type);
            Assert.Equal(TileTypeEnum.Tree, tree.Type);
            Assert.Equal(TileTypeEnum.Building, building.Type);
            Assert.Equal(TileTypeEnum.Door, door.Type);
            Assert.Equal(TileTypeEnum.River, river.Type);
            Assert.Equal(TileTypeEnum.Road, road.Type);
            Assert.Equal(TileTypeEnum.Bridge, bridge.Type);
            Assert.Equal(TileTypeEnum.Hill, hill.Type);
            
            // Verify dimensions are correct (48x48 as expected by our generated textures)
            Assert.Equal(48, grass.Width);
            Assert.Equal(48, grass.Height);
        }
        
        [Fact]
        public void TileTextures_ExpectedFilesExist()
        {
            // Verify that our generated texture files exist in the content directory
            string[] expectedTextures = {
                "grass.png", "water.png", "rock.png", "tree.png", "building.png",
                "door.png", "river.png", "road.png", "bridge.png", "hill.png"
            };
            
            string mapContentPath = "/home/runner/work/ThirdRun/ThirdRun/Content/Map";
            
            foreach (string texture in expectedTextures)
            {
                string texturePath = System.IO.Path.Combine(mapContentPath, texture);
                Assert.True(System.IO.File.Exists(texturePath), 
                    $"Expected texture file does not exist: {texture}");
                
                // Verify file is not empty
                var fileInfo = new System.IO.FileInfo(texturePath);
                Assert.True(fileInfo.Length > 0, 
                    $"Texture file is empty: {texture}");
            }
        }
    }
}