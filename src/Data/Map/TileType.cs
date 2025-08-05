using Microsoft.Xna.Framework;

namespace MonogameRPG.Map
{
    public class TileType
    {
        // Suppression des instances statiques Herbe/Eau/Roche
        public string Name { get; }
        public Color Color { get; }
        public int Width { get; }
        public int Height { get; }

        public bool IsWalkable { get; private set; }

        public TileType(string name, Color color, int width, int height, bool isWalkable)
        {
            Name = name;
            Color = color;
            Width = width;
            Height = height;
            IsWalkable = isWalkable;            
        }
    }
}
