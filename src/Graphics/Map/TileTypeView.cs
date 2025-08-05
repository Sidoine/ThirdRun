using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameRPG.Map;
using System.Collections.Generic;

namespace ThirdRun.Graphics.Map
{
    public class TileTypeView
    {
        private readonly Dictionary<TileType, Texture2D> _textures = new();

        public void Render(SpriteBatch spriteBatch, TileType tileType, int x, int y)
        {
            if (!_textures.ContainsKey(tileType))
            {
                // Crée une texture unie si elle n'est pas déjà chargée
                CreateTexture(spriteBatch.GraphicsDevice, tileType);
            }
            var texture = _textures[tileType];
            spriteBatch.Draw(texture, new Rectangle(x, y, tileType.Width, tileType.Height), Color.White);
        }

        private void CreateTexture(GraphicsDevice graphicsDevice, TileType tileType)
        {
            if (!_textures.ContainsKey(tileType))
            {
                var texture = new Texture2D(graphicsDevice, tileType.Width, tileType.Height);
                Color[] data = new Color[tileType.Width * tileType.Height];
                for (int i = 0; i < data.Length; i++) data[i] = tileType.Color;
                texture.SetData(data);
                _textures[tileType] = texture;
            }
        }
    }
}