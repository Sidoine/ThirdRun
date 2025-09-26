using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonogameRPG.Map;
using System.Collections.Generic;

namespace ThirdRun.Graphics.Map
{
    public class TileTypeView
    {
        private readonly Dictionary<TileTypeEnum, Texture2D> _textures = new();
        private readonly ContentManager _contentManager;
        private bool _texturesLoaded = false;

        public TileTypeView(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public void Render(SpriteBatch spriteBatch, TileType tileType, int x, int y)
        {
            if (!_texturesLoaded)
            {
                LoadTextures();
                _texturesLoaded = true;
            }

            Texture2D texture;
            if (_textures.ContainsKey(tileType.Type))
            {
                texture = _textures[tileType.Type];
            }
            else
            {
                // Fallback to creating a solid color texture if texture file is missing
                texture = CreateFallbackTexture(spriteBatch.GraphicsDevice, tileType);
            }
            
            spriteBatch.Draw(texture, new Rectangle(x, y, tileType.Width, tileType.Height), Color.White);
        }

        private void LoadTextures()
        {
            // Load all tile textures from the Map folder
            try
            {
                _textures[TileTypeEnum.Grass] = _contentManager.Load<Texture2D>("Map/grass");
                _textures[TileTypeEnum.Water] = _contentManager.Load<Texture2D>("Map/water");
                _textures[TileTypeEnum.Rock] = _contentManager.Load<Texture2D>("Map/rock");
                _textures[TileTypeEnum.Tree] = _contentManager.Load<Texture2D>("Map/tree");
                _textures[TileTypeEnum.Building] = _contentManager.Load<Texture2D>("Map/building");
                _textures[TileTypeEnum.Door] = _contentManager.Load<Texture2D>("Map/door");
                _textures[TileTypeEnum.River] = _contentManager.Load<Texture2D>("Map/river");
                _textures[TileTypeEnum.Road] = _contentManager.Load<Texture2D>("Map/road");
                _textures[TileTypeEnum.Bridge] = _contentManager.Load<Texture2D>("Map/bridge");
                _textures[TileTypeEnum.Hill] = _contentManager.Load<Texture2D>("Map/hill");
            }
            catch (ContentLoadException ex)
            {
                System.Console.WriteLine($"Warning: Could not load some tile textures: {ex.Message}");
            }
        }

        private Texture2D CreateFallbackTexture(GraphicsDevice graphicsDevice, TileType tileType)
        {
            var texture = new Texture2D(graphicsDevice, tileType.Width, tileType.Height);
            Color[] data = new Color[tileType.Width * tileType.Height];
            for (int i = 0; i < data.Length; i++) data[i] = tileType.Color;
            texture.SetData(data);
            return texture;
        }
    }
}