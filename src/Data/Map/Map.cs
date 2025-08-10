using System.Collections.Generic;
using MonogameRPG.Monsters;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using ThirdRun.Data.Map;
using ThirdRun.Data.NPCs;

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
        private List<NPC> npcs = new List<NPC>();
        public List<Character> Characters => characters;
        public List<NPC> NPCs => npcs;
        public bool IsTownZone { get; set; } = false;
        private Color characterColor = Color.CornflowerBlue;
        private readonly AdvancedMapGenerator mapGenerator;

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
            
            // Create map generator with seed based on world position for consistency
            int seed = worldPosition.X * 1000 + worldPosition.Y;
            mapGenerator = new AdvancedMapGenerator(seed);
        }

        public void GenerateRandomMap(int spawnCount = 2)
        {
            // Use advanced map generator instead of simple random generation
            tiles = mapGenerator.GenerateMap(GridWidth, GridHeight, WorldPosition);
            
            // Find suitable spawn points for monsters on walkable terrain
            monsterSpawnPoints.Clear();
            int placed = 0;
            var rand = new System.Random(WorldPosition.X * 1000 + WorldPosition.Y + 100);
            
            while (placed < spawnCount)
            {
                int x = rand.Next(GridWidth);
                int y = rand.Next(GridHeight);
                
                // Spawn on walkable tiles (grass, road, hill, door)
                if (tiles[x, y].IsWalkable)
                {
                    monsterSpawnPoints.Add(new Vector2(x, y));
                    placed++;
                }
            }
        }

        public void SpawnMonsters()
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
                var monster = new Monster(type);
                monster.Position = new Vector2(
                    spawn.X * TileWidth + TileWidth / 2 - monsterSize / 2,
                    spawn.Y * TileHeight + TileHeight / 2 - monsterSize / 2) + Position;
                monsters.Add(monster);
            }
        }

        public void SpawnNPCs()
        {
            npcs.Clear();
            var rand = new System.Random();
            
            // Define NPC names and types for the town
            var npcDefinitions = new (string name, NPCType type)[]
            {
                ("Marcus", NPCType.Merchant),
                ("Eleanor", NPCType.Innkeeper),
                ("Gareth", NPCType.Guard),
                ("Thorin", NPCType.Blacksmith)
            };
            
            // Place NPCs at random walkable positions
            var walkablePositions = GetWalkablePositions(4); // Get 4 random walkable positions
            for (int i = 0; i < Math.Min(npcDefinitions.Length, walkablePositions.Count); i++)
            {
                var def = npcDefinitions[i];
                var pos = walkablePositions[i];
                var npc = new NPC(def.name, def.type, new Vector2(
                    pos.X * TileWidth + TileWidth / 2,
                    pos.Y * TileHeight + TileHeight / 2) + Position);
                npcs.Add(npc);
            }
        }
        
        private List<Vector2> GetWalkablePositions(int count)
        {
            var positions = new List<Vector2>();
            var rand = new System.Random();
            int attempts = 0;
            
            while (positions.Count < count && attempts < 100)
            {
                int x = rand.Next(GridWidth);
                int y = rand.Next(GridHeight);
                
                if (tiles[x, y].IsWalkable)
                {
                    positions.Add(new Vector2(x, y));
                }
                attempts++;
            }
            
            return positions;
        }



        public void SetCharacters(List<Character> chars)
        {
            characters.Clear();
            characters.AddRange(chars);
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