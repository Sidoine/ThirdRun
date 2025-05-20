using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonogameRPG.Monsters;
using System;
using Microsoft.Xna.Framework.Content;
using FontStashSharp;

namespace MonogameRPG.Map
{
    public class Map
    {
        private TileType[,] tiles;
        public int TileWidth { get; private set; } = 48;
        public int TileHeight { get; private set; } = 48;
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }
        private List<Vector2> monsterSpawnPoints;
        private Color monsterColor = Color.Red;
        private int monsterSize = 20;
        private List<Monster> monsters = new List<Monster>();
        private List<Character> characters = new List<Character>();
        private Color characterColor = Color.CornflowerBlue;
        private TileType herbeTile = null!;
        private TileType eauTile = null!;
        private TileType rocheTile = null!;

        private static readonly MonsterType[] MonsterTypes = new MonsterType[]
        {
            new MonsterType("Orc", 15, 3, "Monsters/orc"),
        };

        public Map()
        {
            monsterSpawnPoints = new List<Vector2>();
            tiles = new TileType[0, 0];
        }

        public void GenerateRandomMap(GraphicsDevice graphicsDevice, int gridWidth = 25, int gridHeight = 18, int spawnCount = 10)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            herbeTile = new TileType("Herbe", Color.ForestGreen, TileWidth, TileHeight, graphicsDevice);
            eauTile = new TileType("Eau", Color.Blue, TileWidth, TileHeight, graphicsDevice);
            rocheTile = new TileType("Roche", Color.Gray, TileWidth, TileHeight, graphicsDevice);
            tiles = new TileType[gridWidth, gridHeight];
            var rand = new System.Random();
            for (int x = 0; x < gridWidth; x++)
                for (int y = 0; y < gridHeight; y++)
                {
                    int r = rand.Next(100);
                    if (r < 80) tiles[x, y] = herbeTile;
                    else if (r < 90) tiles[x, y] = eauTile;
                    else tiles[x, y] = rocheTile;
                }
            // Points d'apparition de monstres sur des cases herbe
            monsterSpawnPoints.Clear();
            int placed = 0;
            while (placed < spawnCount)
            {
                int x = rand.Next(gridWidth);
                int y = rand.Next(gridHeight);
                if (tiles[x, y] == herbeTile)
                {
                    monsterSpawnPoints.Add(new Vector2(x, y));
                    placed++;
                }
            }
        }

        public void SpawnMonsters(ContentManager contentManager)
        {
            monsters.Clear();
            var rand = new System.Random();
            foreach (var spawn in monsterSpawnPoints)
            {
                var type = MonsterTypes[rand.Next(MonsterTypes.Length)];
                var monster = new Monster(type, contentManager);
                monster.Position = new Vector2(
                    spawn.X * TileWidth + TileWidth / 2 - monsterSize / 2,
                    spawn.Y * TileHeight + TileHeight / 2 - monsterSize / 2);
                monsters.Add(monster);
            }
        }

        public void SetCharacters(List<Character> chars)
        {
            characters = chars;
        }

        public void Render(SpriteBatch spriteBatch, DynamicSpriteFont dynamicFont)
        {
            if (tiles == null) return;
            for (int x = 0; x < GridWidth; x++)
                for (int y = 0; y < GridHeight; y++)
                {
                    tiles[x, y].Render(spriteBatch, x * TileWidth, y * TileHeight);
                }
            // Affichage des monstres
            foreach (var monster in monsters)
            {
                monster.Render(spriteBatch, dynamicFont);
            }
            // Affichage des personnages (losange bleu)
            foreach (var character in characters)
            {
                character.Render(spriteBatch);
            }
        }

        private Texture2D CreateTileTexture(GraphicsDevice graphicsDevice, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; i++) data[i] = color;
            texture.SetData(data);
            return texture;
        }

        public List<Vector2> GetMonsterSpawnPoints()
        {
            return monsterSpawnPoints;
        }

        public List<Monster> GetMonsters()
        {
            return monsters;
        }

        // Retourne la liste des cases accessibles (Herbe) autour d'une case
        public List<Point> GetNeighbors(Point cell)
        {
            var neighbors = new List<Point>();
            int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };
            for (int i = 0; i < 4; i++)
            {
                int nx = cell.X + directions[i, 0];
                int ny = cell.Y + directions[i, 1];
                if (nx >= 0 && nx < GridWidth && ny >= 0 && ny < GridHeight)
                {
                    if (tiles[nx, ny] == herbeTile)
                        neighbors.Add(new Point(nx, ny));
                }
            }
            return neighbors;
        }

        public TileType GetTileType(int x, int y)
        {
            return tiles[x, y];
        }

        public List<Vector2> FindPathAStar(Vector2 start, Vector2 end)
        {
            // Conversion en coordonnées de grille
            Point startCell = new Point((int)(start.X / TileWidth), (int)(start.Y / TileHeight));
            Point endCell = new Point((int)(end.X / TileWidth), (int)(end.Y / TileHeight));
            var openSet = new SortedSet<(float, Point)>(Comparer<(float, Point)>.Create((a, b) => a.Item1 != b.Item1 ? a.Item1.CompareTo(b.Item1) : a.Item2.GetHashCode().CompareTo(b.Item2.GetHashCode())));
            var cameFrom = new Dictionary<Point, Point>();
            var gScore = new Dictionary<Point, float>();
            var fScore = new Dictionary<Point, float>();
            openSet.Add((0, startCell));
            gScore[startCell] = 0;
            fScore[startCell] = Heuristic(startCell, endCell);
            while (openSet.Count > 0)
            {
                var current = openSet.Min.Item2;
                if (current == endCell)
                    return ReconstructPath(cameFrom, current);
                openSet.Remove(openSet.Min);
                foreach (var neighbor in GetNeighbors(current))
                {
                    float tentativeG = gScore[current] + 1;
                    if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + Heuristic(neighbor, endCell);
                        if (!openSet.Contains((fScore[neighbor], neighbor)))
                            openSet.Add((fScore[neighbor], neighbor));
                    }
                }
            }
            return new List<Vector2>(); // Pas de chemin trouvé
        }

        private float Heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private List<Vector2> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
        {
            var path = new List<Vector2> { new Vector2(current.X * TileWidth + TileWidth / 2, current.Y * TileHeight + TileHeight / 2) };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, new Vector2(current.X * TileWidth + TileWidth / 2, current.Y * TileHeight + TileHeight / 2));
            }
            return path;
        }
    }
}