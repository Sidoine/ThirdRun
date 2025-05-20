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
        private Texture2D texture;

        public TileType(string name, Color color, int width, int height, GraphicsDevice graphicsDevice)
        {
            Name = name;
            Color = color;
            Width = width;
            Height = height;
            texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; i++) data[i] = Color;
            texture.SetData(data);
        }

        public void Render(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(texture, new Rectangle(x, y, Width, Height), Color.White);
        }
    }
}
