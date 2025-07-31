using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameRPG.Map
{
    public class TileType
    {
        // Suppression des instances statiques Herbe/Eau/Roche
        public string Name { get; }
        public Color Color { get; }
        public int Width { get; }
        public int Height { get; }
        private Texture2D? texture;

        public bool IsWalkable { get; private set; }

        public TileType(string name, Color color, int width, int height, GraphicsDevice graphicsDevice, bool isWalkable)
        {
            Name = name;
            Color = color;
            Width = width;
            Height = height;
            IsWalkable = isWalkable;            
        }

        public void Render(SpriteBatch spriteBatch, int x, int y)
        {
            if (texture == null)
            {
                // Crée une texture unie si elle n'est pas déjà chargée
                CreateTexture(spriteBatch.GraphicsDevice);
            }
            spriteBatch.Draw(texture, new Rectangle(x, y, Width, Height), Color.White);
        }

        private void CreateTexture(GraphicsDevice graphicsDevice)
        {
            if (texture == null)
            {
                texture = new Texture2D(graphicsDevice, Width, Height);
                Color[] data = new Color[Width * Height];
                for (int i = 0; i < data.Length; i++) data[i] = Color;
                texture.SetData(data);
            }
        }
    }
}
