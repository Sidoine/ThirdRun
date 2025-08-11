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
        private Tile[,] tiles;
        public Tile[,] Tiles => tiles;
        public const int TileWidth = 48;
        public const int TileHeight = 48;
        public const int GridWidth = 25;
        public const int GridHeight = 18;
        public Point WorldPosition { get; set; }
        public Vector2 Position => new Vector2(WorldPosition.X * GridWidth * TileWidth, WorldPosition.Y * GridHeight * TileHeight);
        private List<Vector2> monsterSpawnPoints;
        private int monsterSize = 20;
        
        // Central list containing all units on this map
        private List<Unit> units = new List<Unit>();
        
        // Filtered views of the central units list
        public IEnumerable<Character> Characters => units.OfType<Character>();
        public IEnumerable<Monster> Monsters => units.OfType<Monster>();  
        public IEnumerable<NPC> NPCs => units.OfType<NPC>();
        public bool IsTownZone { get; set; } = false;
        private Color characterColor = Color.CornflowerBlue;
        private readonly AdvancedMapGenerator mapGenerator;



        public Map(Point worldPosition)
        {
            WorldPosition = worldPosition;
            monsterSpawnPoints = new List<Vector2>();
            tiles = new Tile[0, 0];
            
            // Create map generator with seed based on world position for consistency
            int seed = worldPosition.X * 1000 + worldPosition.Y;
            mapGenerator = new AdvancedMapGenerator(seed);
        }

        public void GenerateRandomMap(int spawnCount = 2)
        {
            // Use advanced map generator instead of simple random generation
            var tileTypes = mapGenerator.GenerateMap(GridWidth, GridHeight, WorldPosition);
            // Initialize tiles array with Tile objects
            tiles = new Tile[GridWidth, GridHeight];
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    tiles[x, y] = new Tile(tileTypes[x, y]);
                }
            }
            
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
            // Clear existing monsters from units list
            var existingMonsters = Monsters;
            foreach (var monster in existingMonsters)
            {
                RemoveUnit(monster);
            }
            
            var rand = new System.Random();
            
            // Calculate area difficulty based on distance from origin (0,0)
            int distanceFromOrigin = Math.Abs(WorldPosition.X) + Math.Abs(WorldPosition.Y);
            int areaLevel = Math.Min(distanceFromOrigin + 1, 5); // Level 1-5 based on distance
            
            foreach (var spawn in monsterSpawnPoints)
            {
                // Use MonsterTemplateRepository to get appropriate monster for area level
                var type = MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(1, areaLevel + 1);
                var monster = new Monster(type);
                monster.Position = new Vector2(
                    spawn.X * TileWidth + TileWidth / 2 - monsterSize / 2,
                    spawn.Y * TileHeight + TileHeight / 2 - monsterSize / 2) + Position;
                
                AddUnit(monster);
            }
        }

        public void SpawnNPCs()
        {
            // Clear existing NPCs from units list
            var existingNPCs = NPCs;
            foreach (var npc in existingNPCs)
            {
                RemoveUnit(npc);
            }
            
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
                AddUnit(npc);
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
                
                if (tiles[x, y].IsWalkable && !tiles[x, y].IsOccupied)
                {
                    positions.Add(new Vector2(x, y));
                }
                attempts++;
            }
            
            return positions;
        }



        public void SetCharacters(List<Character> chars)
        {
            // Clear existing characters from units list
            var existingCharacters = Characters;
            foreach (var character in existingCharacters)
            {
                RemoveUnit(character);
            }
            
            // Add new characters to units list
            foreach (var character in chars)
            {
                AddUnit(character);
            }
        }

        public void TeleportCharacters(List<Character> chars)
        {
            // Clear existing characters from units list
            var existingCharacters = Characters;
            foreach (var character in existingCharacters)
            {
                RemoveUnit(character);
            }
            
            // Get valid spawn positions for the characters
            var validPositions = GetWalkablePositions(chars.Count);
            
            // Position each character at a valid location on this map and add to units list
            for (int i = 0; i < chars.Count && i < validPositions.Count; i++)
            {
                var pos = validPositions[i];
                chars[i].Position = new Vector2(
                    pos.X * TileWidth + TileWidth / 2,
                    pos.Y * TileHeight + TileHeight / 2) + Position;
                
                AddUnit(chars[i]);
            }
        }



        public List<Vector2> GetMonsterSpawnPoints()
        {
            return monsterSpawnPoints;
        }

        public List<Monster> GetMonsters()
        {
            return Monsters.ToList();
        }

        public bool HasLivingMonsters()
        {
            foreach (var monster in Monsters)
            {
                if (!monster.IsDead)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Adds a unit to this map
        /// </summary>
        /// <param name="unit">The unit to add</param>
        public void AddUnit(Unit unit)
        {
            if (!units.Contains(unit))
            {
                units.Add(unit);
                UpdateUnitPosition(unit);
            }
        }
        
        /// <summary>
        /// Removes a unit from this map
        /// </summary>
        /// <param name="unit">The unit to remove</param>
        public void RemoveUnit(Unit unit)
        {
            if (units.Contains(unit))
            {
                RemoveUnitFromGrid(unit);
                units.Remove(unit);
            }
        }
        
        /// <summary>
        /// Converts world position to tile coordinates on this map
        /// </summary>
        /// <param name="worldPosition">World position in pixels</param>
        /// <returns>Tile coordinates, or null if position is outside this map</returns>
        public Point? WorldPositionToTileCoordinates(Vector2 worldPosition)
        {
            // Convert world position to relative position on this map
            Vector2 relativePos = worldPosition - Position;
            
            // Convert to tile coordinates
            int tileX = (int)(relativePos.X / TileWidth);
            int tileY = (int)(relativePos.Y / TileHeight);
            
            // Check bounds
            if (tileX >= 0 && tileX < GridWidth && tileY >= 0 && tileY < GridHeight)
            {
                return new Point(tileX, tileY);
            }
            
            return null;
        }
        
        /// <summary>
        /// Gets the unit at the specified tile coordinates
        /// </summary>
        /// <param name="tileX">X coordinate of the tile</param>
        /// <param name="tileY">Y coordinate of the tile</param>
        /// <returns>Unit at the tile, or null if no unit or out of bounds</returns>
        public Unit? GetUnitAtTile(int tileX, int tileY)
        {
            if (tileX >= 0 && tileX < GridWidth && tileY >= 0 && tileY < GridHeight)
            {
                return tiles[tileX, tileY].GetFirstUnit();
            }
            return null;
        }
        
        /// <summary>
        /// Gets the unit at the specified world position
        /// </summary>
        /// <param name="worldPosition">World position in pixels</param>
        /// <returns>Unit at the position, or null if no unit or out of bounds</returns>
        public Unit? GetUnitAtWorldPosition(Vector2 worldPosition)
        {
            var tileCoords = WorldPositionToTileCoordinates(worldPosition);
            if (tileCoords.HasValue)
            {
                return GetUnitAtTile(tileCoords.Value.X, tileCoords.Value.Y);
            }
            return null;
        }
        
        /// <summary>
        /// Updates the unit's position in the tile grid
        /// </summary>
        /// <param name="unit">The unit to update</param>
        /// <param name="oldPosition">The unit's previous position (to clear the old tile location)</param>
        public void UpdateUnitPosition(Unit unit, Vector2? oldPosition = null)
        {
            // Clear old position if provided
            if (oldPosition.HasValue)
            {
                var oldTileCoords = WorldPositionToTileCoordinates(oldPosition.Value);
                if (oldTileCoords.HasValue)
                {
                    var oldX = oldTileCoords.Value.X;
                    var oldY = oldTileCoords.Value.Y;
                    tiles[oldX, oldY].RemoveUnit(unit);
                }
            }
            
            // Set new position
            var newTileCoords = WorldPositionToTileCoordinates(unit.Position);
            if (newTileCoords.HasValue)
            {
                var newX = newTileCoords.Value.X;
                var newY = newTileCoords.Value.Y;
                tiles[newX, newY].AddUnit(unit);
            }
        }
        
        /// <summary>
        /// Removes a unit from the tile grid
        /// </summary>
        /// <param name="unit">The unit to remove</param>
        public void RemoveUnitFromGrid(Unit unit)
        {
            var tileCoords = WorldPositionToTileCoordinates(unit.Position);
            if (tileCoords.HasValue)
            {
                var x = tileCoords.Value.X;
                var y = tileCoords.Value.Y;
                tiles[x, y].RemoveUnit(unit);
            }
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