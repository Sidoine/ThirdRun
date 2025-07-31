using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonogameRPG.Monsters;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using FontStashSharp;

namespace MonogameRPG.Map
{
    public class Map
    {
        private TileType[,] tiles;
        public TileType[,] Tiles => tiles;
        public const int TileWidth = 48;
        public const int TileHeight = 48;
        public const int GridWidth = 25;
        public const int GridHeight = 18;
        public Point WorldPosition { get; set; }
        public Vector2 Position => new Vector2(WorldPosition.X * GridWidth * TileWidth, WorldPosition.Y * GridHeight * TileHeight);
        private List<Vector2> monsterSpawnPoints;
        private int monsterSize = 20;
        private List<Monster> monsters = new List<Monster>();
        private List<Character> characters = new List<Character>();
        public List<Character> Characters => characters;
        private Color characterColor = Color.CornflowerBlue;
        private TileType herbeTile = null!;
        private TileType eauTile = null!;
        private TileType rocheTile = null!;

        private static readonly MonsterType[] MonsterTypes = new MonsterType[]
        {
            new MonsterType("Orc Faible", 15, 3, "Monsters/orc", 1),
            new MonsterType("Orc", 20, 4, "Monsters/orc", 2),
            new MonsterType("Orc Guerrier", 25, 6, "Monsters/orc", 3),
            new MonsterType("Orc Chef", 35, 8, "Monsters/orc", 4),
            new MonsterType("Orc Élite", 50, 12, "Monsters/orc", 5),
        };

        public Map(Point worldPosition)
        {
            WorldPosition = worldPosition;
            monsterSpawnPoints = new List<Vector2>();
            tiles = new TileType[0, 0];
        }

        public void GenerateRandomMap(GraphicsDevice graphicsDevice, int spawnCount = 2)
        {
            herbeTile = new TileType("Herbe", Color.ForestGreen, TileWidth, TileHeight, graphicsDevice, true);
            eauTile = new TileType("Eau", Color.Blue, TileWidth, TileHeight, graphicsDevice, false);
            rocheTile = new TileType("Roche", Color.Gray, TileWidth, TileHeight, graphicsDevice, false);
            tiles = new TileType[GridWidth, GridHeight];
            var rand = new System.Random();
            for (int x = 0; x < GridWidth; x++)
                for (int y = 0; y < GridHeight; y++)
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
                int x = rand.Next(GridWidth);
                int y = rand.Next(GridHeight);
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
            
            // Calculate area difficulty based on distance from origin (0,0)
            int distanceFromOrigin = Math.Abs(WorldPosition.X) + Math.Abs(WorldPosition.Y);
            int areaLevel = Math.Min(distanceFromOrigin + 1, 5); // Level 1-5 based on distance
            
            foreach (var spawn in monsterSpawnPoints)
            {
                // Choose monster type appropriate for area level
                var availableTypes = MonsterTypes.Where(mt => mt.Level <= areaLevel + 1).ToArray();
                if (availableTypes.Length == 0) availableTypes = MonsterTypes; // Fallback
                
                var type = availableTypes[rand.Next(availableTypes.Length)];
                var monster = new Monster(type, contentManager);
                monster.Position = new Vector2(
                    spawn.X * TileWidth + TileWidth / 2 - monsterSize / 2,
                    spawn.Y * TileHeight + TileHeight / 2 - monsterSize / 2) + Position;
                monsters.Add(monster);
            }
        }

        public void SetCharacters(List<Character> chars)
        {
            characters.Clear();
            characters.AddRange(chars);
        }

        public void Render(SpriteBatch spriteBatch, DynamicSpriteFont dynamicFont, Vector2 offset = default)
        {
            if (tiles == null) return;
            
            Vector2 renderOffset = offset + Position;
            
            for (int x = 0; x < GridWidth; x++)
                for (int y = 0; y < GridHeight; y++)
                {
                    Vector2 tilePos = renderOffset + new Vector2(x * TileWidth, y * TileHeight);
                    tiles[x, y].Render(spriteBatch, (int)tilePos.X, (int)tilePos.Y);
                }
            // Affichage des monstres
            foreach (var monster in monsters)
            {
                if (!monster.IsDead && monster.Position != Vector2.Zero)
                {
                    Vector2 renderPos = monster.Position;
                    monster.RenderAtPosition(spriteBatch, dynamicFont, renderPos);
                }
            }
            // Affichage des personnages
            foreach (var character in characters)
            {
                if (character.Position != Vector2.Zero)
                {
                    Vector2 renderPos = character.Position;
                    character.RenderAtPosition(spriteBatch, renderPos);
                }
            }
        }

        public List<Vector2> GetMonsterSpawnPoints()
        {
            return monsterSpawnPoints;
        }

        public List<Monster> GetMonsters()
        {
            return monsters;
        }

        public bool HasLivingMonsters()
        {
            foreach (var monster in monsters)
            {
                if (!monster.IsDead)
                    return true;
            }
            return false;
        }
    }

    public enum Direction
    {
        North,
        South,
        East,
        West
    }
}