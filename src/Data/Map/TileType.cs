using Microsoft.Xna.Framework;

namespace MonogameRPG.Map
{
    public enum TileTypeEnum
    {
        Grass,
        Water,
        Rock,
        Tree,
        Building,
        Door,
        River,
        Road,
        Bridge,
        Hill
    }

    public class TileType
    {
        public string Name { get; }
        public Color Color { get; }
        public int Width { get; }
        public int Height { get; }
        public bool IsWalkable { get; private set; }
        public TileTypeEnum Type { get; }

        public TileType(string name, Color color, int width, int height, bool isWalkable, TileTypeEnum type)
        {
            Name = name;
            Color = color;
            Width = width;
            Height = height;
            IsWalkable = isWalkable;
            Type = type;
        }

        // Static factory methods for easy creation of common tile types
        public static TileType CreateGrass(int width, int height) => 
            new TileType("Herbe", Color.ForestGreen, width, height, true, TileTypeEnum.Grass);

        public static TileType CreateWater(int width, int height) => 
            new TileType("Eau", Color.Blue, width, height, false, TileTypeEnum.Water);

        public static TileType CreateRock(int width, int height) => 
            new TileType("Roche", Color.Gray, width, height, false, TileTypeEnum.Rock);

        public static TileType CreateTree(int width, int height) => 
            new TileType("Arbre", Color.DarkGreen, width, height, false, TileTypeEnum.Tree);

        public static TileType CreateBuilding(int width, int height) => 
            new TileType("Bâtiment", Color.SaddleBrown, width, height, false, TileTypeEnum.Building);

        public static TileType CreateDoor(int width, int height) => 
            new TileType("Porte", Color.Goldenrod, width, height, true, TileTypeEnum.Door);

        public static TileType CreateRiver(int width, int height) => 
            new TileType("Rivière", Color.DarkBlue, width, height, false, TileTypeEnum.River);

        public static TileType CreateRoad(int width, int height) => 
            new TileType("Route", Color.DarkGray, width, height, true, TileTypeEnum.Road);

        public static TileType CreateBridge(int width, int height) => 
            new TileType("Pont", Color.Brown, width, height, true, TileTypeEnum.Bridge);

        public static TileType CreateHill(int width, int height) => 
            new TileType("Colline", Color.YellowGreen, width, height, true, TileTypeEnum.Hill);
    }
}
