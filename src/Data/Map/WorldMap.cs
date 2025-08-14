using System.Collections.Generic;
using System.Linq;
using MonogameRPG.Monsters;
using System;
using Microsoft.Xna.Framework;
using ThirdRun.Data.NPCs;
using ThirdRun.Data.Dungeons;

namespace MonogameRPG.Map
{
    public class WorldMap(Random random)
    {
        private readonly Dictionary<Point, Map> maps = new Dictionary<Point, Map>();
        private Point currentMapPosition = Point.Zero;
        private Point lastHostileMapPosition = Point.Zero;
        private List<Character> characters = [];
        private Map? townMap = null; // Dedicated town map
        private bool isInTownMode = false; // Track if we're currently in town mode
        
        // Dungeon system
        private List<Map>? dungeonMaps = null; // Current dungeon maps
        private int currentDungeonMapIndex = 0;
        private bool isInDungeonMode = false; // Track if we're currently in dungeon mode
        private Dungeon? currentDungeon = null;
        
        private readonly Random random = random;

        public Map CurrentMap => 
            isInDungeonMode && dungeonMaps != null && currentDungeonMapIndex < dungeonMaps.Count ? dungeonMaps[currentDungeonMapIndex] :
            isInTownMode && townMap != null ? townMap : 
            (maps.TryGetValue(currentMapPosition, out Map? value) ? value : throw new Exception("Current map not found at position: " + currentMapPosition));
        public Point CurrentMapPosition => currentMapPosition;
        public bool IsInTown => isInTownMode;
        public bool IsInDungeon => isInDungeonMode;
        public Dungeon? CurrentDungeon => currentDungeon;

        public void Initialize()
        {
            // Create the initial card at (0,0)
            var initialMap = new Map(Point.Zero, random);
            initialMap.GenerateRandomMap();
            initialMap.SpawnMonsters(this);
            maps[Point.Zero] = initialMap;
            currentMapPosition = Point.Zero;

            // Create dedicated town map at a special position
            townMap = new Map(new Point(-999, -999), random); // Special position for town
            townMap.GenerateRandomMap();
            townMap.IsTownZone = true;
            townMap.SpawnNPCs(this);
            maps[townMap.WorldPosition] = townMap;
        }

        public void SetCharacters(List<Character> chars)
        {
            characters = chars;
            // Set characters on the current card
            if (CurrentMap != null)
            {
                CurrentMap.SetCharacters(chars);
            }
        }

        public void Update()
        {
            // Handle dungeon progression
            if (isInDungeonMode && CanProgressInDungeon())
            {
                // Check if we should automatically progress (e.g., after a short delay)
                // For now, progression will be manual or triggered by game logic
            }
        }

        private Map GenerateNewAdjacentMap()
        {
            // Check if we need to generate a new adjacent card
            var availableDirections = GetAvailableDirections();
            Direction direction;
            if (availableDirections.Count > 0)
            {
                // Generate a new card in a random available direction
                direction = availableDirections[random.Next(availableDirections.Count)];
            }
            else
            {
                // If there are no available directions, generate one anyway by choosing a random direction
                // This ensures the game always continues
                var directions = new Direction[] { Direction.North, Direction.South, Direction.East, Direction.West };
                direction = directions[random.Next(directions.Length)];
            }
            return GenerateAdjacentMap(direction);
        }


        private List<Direction> GetAvailableDirections()
        {
            var available = new List<Direction>();
            var directions = new Direction[] { Direction.North, Direction.South, Direction.East, Direction.West };

            foreach (var dir in directions)
            {
                var adjacentPos = GetAdjacentPosition(currentMapPosition, dir);
                if (!maps.ContainsKey(adjacentPos))
                {
                    available.Add(dir);
                }
            }

            return available;
        }

        private Point GetAdjacentPosition(Point cardPos, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(cardPos.X, cardPos.Y - 1);
                case Direction.South:
                    return new Point(cardPos.X, cardPos.Y + 1);
                case Direction.East:
                    return new Point(cardPos.X + 1, cardPos.Y);
                case Direction.West:
                    return new Point(cardPos.X - 1, cardPos.Y);
                default:
                    return cardPos;
            }
        }

        private Map GenerateAdjacentMap(Direction direction)
        {
            var newCardPos = GetAdjacentPosition(currentMapPosition, direction);

            if (!maps.ContainsKey(newCardPos))
            {
                var newCard = new Map(newCardPos, random);
                newCard.GenerateRandomMap();
                newCard.SpawnMonsters(this);
                maps[newCardPos] = newCard;
            }
            return maps[newCardPos];
        }

        public Map GetAdjacentCardWithMonsters()
        {
            foreach (var map in GetAdjacentMaps())
            {
                if (map.HasLivingMonsters())
                {
                    return map;
                }
            }
            return GenerateNewAdjacentMap();
        }

        public Map? GetMapAtPosition(Vector2 worldPosition)
        {
            // Convert world position to card coordinates
            foreach (var kvp in maps)
            {
                var card = kvp.Value;
                var cardWorldPos = card.WorldPosition;

                // Calculate the bounds of this card in world coordinates
                float cardLeft = cardWorldPos.X * Map.GridWidth * Map.TileWidth;
                float cardTop = cardWorldPos.Y * Map.GridHeight * Map.TileHeight;
                float cardRight = cardLeft + Map.GridWidth * Map.TileWidth;
                float cardBottom = cardTop + Map.GridHeight * Map.TileHeight;

                if (worldPosition.X >= cardLeft && worldPosition.X < cardRight &&
                    worldPosition.Y >= cardTop && worldPosition.Y < cardBottom)
                {
                    return card;
                }
            }
            return null;
        }

        public void UpdateCurrentMap()
        {
            if (characters.All(x => x.Map != CurrentMap))
            {
                currentMapPosition = characters.First().Map.WorldPosition;
                CleanupEmptyCards();
            }
        }

        private IEnumerable<Map> GetAdjacentMaps()
        {
            // Get all maps that are adjacent to the current map
            var directions = new Direction[] { Direction.North, Direction.South, Direction.East, Direction.West };
            foreach (var dir in directions)
            {
                var adjacentPos = GetAdjacentPosition(currentMapPosition, dir);
                if (maps.TryGetValue(adjacentPos, out Map? map))
                {
                    yield return map;
                }
            }
        }

        private void CleanupEmptyCards()
        {
            var cardsToRemove = new List<Point>();

            foreach (var kvp in maps)
            {
                var mapPosition = kvp.Key;
                var card = kvp.Value;

                // Don't remove current card
                if (mapPosition == currentMapPosition) continue;

                // Don't remove adjacent cards (keep them for potential re-entry)
                if (IsAdjacentToCurrentMap(mapPosition)) continue;

                // Remove card if it has no characters and no living monsters
                if (!card.Characters.Any() && !card.HasLivingMonsters())
                {
                    cardsToRemove.Add(mapPosition);
                }
            }

            foreach (var cardPos in cardsToRemove)
            {
                maps.Remove(cardPos);
            }
        }

        private bool IsAdjacentToCurrentMap(Point cardPos)
        {
            var diff = cardPos - currentMapPosition;
            var distance = Math.Abs(diff.X) + Math.Abs(diff.Y);
            return distance <= 1; // Adjacent cards have distance 1
        }



        public List<Monster> GetMonstersOnCurrentMap()
        {
            return CurrentMap.GetMonsters();
        }

        public IEnumerable<Map> GetAllMaps()
        {
            if (isInTownMode && townMap != null) return [townMap];
            return maps.Values;
        }

        /// <summary>
        /// Converts absolute tile coordinates to relative coordinates within a specific map.
        /// </summary>
        /// <param name="absoluteX">Absolute X tile coordinate</param>
        /// <param name="absoluteY">Absolute Y tile coordinate</param>
        /// <returns>Tuple containing the Map (or null if not found) and relative coordinates</returns>
        public (Map? map, int relativeX, int relativeY) GetRelativeTileCoordinate(int absoluteX, int absoluteY)
        {
            // Calculate which map this absolute coordinate belongs to
            int mapX = absoluteX / Map.GridWidth;
            int mapY = absoluteY / Map.GridHeight;
            
            // Handle negative coordinates correctly
            if (absoluteX < 0 && absoluteX % Map.GridWidth != 0)
            {
                mapX--;
            }
            if (absoluteY < 0 && absoluteY % Map.GridHeight != 0)
            {
                mapY--;
            }
            
            // Calculate relative coordinates within the map
            int relativeX = absoluteX - mapX * Map.GridWidth;
            int relativeY = absoluteY - mapY * Map.GridHeight;
            
            // Try to get the map
            maps.TryGetValue(new Point(mapX, mapY), out Map? map);
            
            return (map, relativeX, relativeY);
        }

        // Retourne la liste des cases accessibles (Herbe) autour d'une case
        public List<(Point point, int cost)> GetNeighbors(Point cell, Point? targetCell = null)
        {
            var neighbors = new List<(Point point, int cost)>();
            int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };
            for (int i = 0; i < 4; i++)
            {
                int nx = cell.X + directions[i, 0];
                int ny = cell.Y + directions[i, 1];
                
                // Use the new method to get map and relative coordinates
                var (map, rx, ry) = GetRelativeTileCoordinate(nx, ny);
                
                if (map != null)
                {
                    // Verify that relative coordinates are within bounds
                    if (rx < 0 || rx >= Map.GridWidth || ry < 0 || ry >= Map.GridHeight)
                    {
                        // This should not happen with correct coordinate conversion
                        throw new InvalidOperationException(
                            $"GetRelativeTileCoordinate returned out-of-bounds coordinates: " +
                            $"absolute=({nx},{ny}), relative=({rx},{ry}), bounds=({Map.GridWidth},{Map.GridHeight})");
                    }
                    
                    Point neighborPoint = new Point(nx, ny);
                    
                    if (map.Tiles[rx, ry].IsWalkable)
                    {
                        // Check for unit collision
                        var tile = map.Tiles[rx, ry];
                        bool isOccupied = tile.IsOccupied;
                        bool isTarget = targetCell.HasValue && neighborPoint == targetCell.Value;
                        
                        if (!isOccupied || isTarget)
                        {
                            // Allow movement to unoccupied tiles or to the target tile (for combat)
                            neighbors.Add((neighborPoint, 1));
                        }
                        // If occupied and not target, don't add as neighbor (blocked)
                    }
                    else if (map != CurrentMap)
                        neighbors.Add((neighborPoint, 10));
                }
            }
            return neighbors;
        }

        public List<Vector2> FindPathAStar(Vector2 start, Vector2 end)
        {
            // Conversion en coordonnées de grille
            Point startCell = new Point((int)Math.Floor(start.X / Map.TileWidth), (int)Math.Floor(start.Y / Map.TileHeight));
            Point endCell = new Point((int)Math.Floor(end.X / Map.TileWidth), (int)Math.Floor(end.Y / Map.TileHeight));
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
                foreach (var (neighbor, cost) in GetNeighbors(current, endCell))
                {
                    float tentativeG = gScore[current] + cost;
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
            var path = new List<Vector2> { new Vector2(current.X * Map.TileWidth + Map.TileWidth / 2, current.Y * Map.TileHeight + Map.TileHeight / 2) };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, new Vector2(current.X * Map.TileWidth + Map.TileWidth / 2, current.Y * Map.TileHeight + Map.TileHeight / 2));
            }
            return path;
        }

        public void ToggleTownMode()
        {
            if (isInTownMode)
            {
                // Switch back to hostile zone
                isInTownMode = false;
                
                // Teleport characters back to the hostile map
                if (maps.ContainsKey(lastHostileMapPosition))
                {
                    currentMapPosition = lastHostileMapPosition;
                    CurrentMap.TeleportCharacters(characters);
                }
            }
            else
            {
                // Remember current hostile position before going to town
                lastHostileMapPosition = currentMapPosition;
                
                // Switch to town mode
                isInTownMode = true;
                
                // Teleport characters to the town map
                if (townMap != null)
                {
                    townMap.TeleportCharacters(characters);
                }
            }
        }

        public void EnterDungeon()
        {
            if (isInDungeonMode || isInTownMode) return; // Can't enter dungeon from town or another dungeon
            
            // Calculate mean character level
            int meanLevel = CalculateMeanCharacterLevel();
            
            // Find appropriate dungeon
            var dungeon = DungeonRepository.GetDungeonForLevel(meanLevel);
            if (dungeon == null) return; // No appropriate dungeon found
            
            // Remember current position before entering dungeon
            lastHostileMapPosition = currentMapPosition;
            
            // Enter dungeon mode
            isInDungeonMode = true;
            currentDungeon = dungeon;
            currentDungeonMapIndex = 0;
            
            // Generate dungeon maps
            dungeonMaps = GenerateDungeonMaps(dungeon);
            
            // Teleport characters to first dungeon map
            if (dungeonMaps.Count > 0)
            {
                dungeonMaps[0].TeleportCharacters(characters);
            }
        }

        public void ExitDungeon()
        {
            if (!isInDungeonMode) return;
            
            // Switch back to hostile zone
            isInDungeonMode = false;
            currentDungeon = null;
            dungeonMaps = null;
            currentDungeonMapIndex = 0;
            
            // Teleport characters back to the hostile map
            if (maps.ContainsKey(lastHostileMapPosition))
            {
                currentMapPosition = lastHostileMapPosition;
                CurrentMap.TeleportCharacters(characters);
            }
        }

        public bool CanProgressInDungeon()
        {
            if (!isInDungeonMode || dungeonMaps == null) return false;
            
            // Can progress if current map has no living monsters
            return !CurrentMap.HasLivingMonsters();
        }

        public bool ProgressDungeon()
        {
            if (!CanProgressInDungeon() || dungeonMaps == null) return false;
            
            currentDungeonMapIndex++;
            
            // Check if dungeon is completed
            if (currentDungeonMapIndex >= dungeonMaps.Count)
            {
                ExitDungeon(); // Auto-exit when dungeon is complete
                return true;
            }
            
            // Teleport to next map
            dungeonMaps[currentDungeonMapIndex].TeleportCharacters(characters);
            return true;
        }

        private int CalculateMeanCharacterLevel()
        {
            if (characters.Count == 0) return 1;
            
            int totalLevel = characters.Sum(character => character.Level);
            return Math.Max(1, totalLevel / characters.Count);
        }

        private List<Map> GenerateDungeonMaps(Dungeon dungeon)
        {
            var dungeonMaps = new List<Map>();
            
            for (int i = 0; i < dungeon.Maps.Count; i++)
            {
                var mapDef = dungeon.Maps[i];
                var map = new Map(new Point(-1000 - i, -1000), random); // Special positions for dungeon maps
                map.GenerateRandomMap(mapDef.MonsterCount);
                
                // Spawn monsters based on map definition
                SpawnDungeonMonsters(map, mapDef);
                
                dungeonMaps.Add(map);
            }
            
            return dungeonMaps;
        }

        private void SpawnDungeonMonsters(Map map, DungeonMapDefinition mapDef)
        {
            var spawnPoints = map.GetMonsterSpawnPoints();
            int monstersToSpawn = Math.Min(mapDef.MonsterCount, spawnPoints.Count);
            
            for (int i = 0; i < monstersToSpawn; i++)
            {
                var spawnPoint = spawnPoints[i];
                var monsterLevel = random.Next(mapDef.MinMonsterLevel, mapDef.MaxMonsterLevel + 1);
                var monsterType = MonsterTemplateRepository.CreateRandomMonsterTypeForLevel(monsterLevel, monsterLevel, random);
                
                // Make boss monsters stronger if this map has a boss and it's the last monster
                if (mapDef.HasBoss && i == monstersToSpawn - 1)
                {
                    // Boost boss stats
                    monsterType.BaseHealth = (int)(monsterType.BaseHealth * 1.5f);
                    monsterType.BaseAttack = (int)(monsterType.BaseAttack * 1.3f);
                }
                
                var monster = new Monster(monsterType, map, this, random);
                monster.Position = spawnPoint;
                map.AddUnit(monster);
            }
        }

        public List<NPC> GetNPCsOnCurrentMap()
        {
            return CurrentMap.NPCs.ToList();
        }
    }
}