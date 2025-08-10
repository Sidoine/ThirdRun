using System.Collections.Generic;
using System.Linq;
using MonogameRPG.Monsters;
using System;
using Microsoft.Xna.Framework;
using ThirdRun.Data.NPCs;

namespace MonogameRPG.Map
{
    public class WorldMap()
    {
        private readonly Dictionary<Point, Map> maps = new Dictionary<Point, Map>();
        private Point currentMapPosition = Point.Zero;
        private Point lastHostileMapPosition = Point.Zero;
        private List<Character> characters = [];

        public Map CurrentMap => maps.TryGetValue(currentMapPosition, out Map? value) ? value : throw new Exception("Current map not found at position: " + currentMapPosition);
        public Point CurrentMapPosition => currentMapPosition;
        public bool IsInTown => CurrentMap.IsTownZone;

        public void Initialize()
        {
            // Create the initial card at (0,0)
            var initialMap = new Map(Point.Zero);
            initialMap.GenerateRandomMap();
            initialMap.SpawnMonsters();
            maps[Point.Zero] = initialMap;
            currentMapPosition = Point.Zero;
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
        }

        private Map GenerateNewAdjacentMap()
        {
            // Check if we need to generate a new adjacent card
            var availableDirections = GetAvailableDirections();
            Direction direction;
            if (availableDirections.Count > 0)
            {
                // Generate a new card in a random available direction
                var rand = new Random();
                direction = availableDirections[rand.Next(availableDirections.Count)];
            }
            else
            {
                // If there are no available directions, generate one anyway by choosing a random direction
                // This ensures the game always continues
                var rand = new Random();
                var directions = new Direction[] { Direction.North, Direction.South, Direction.East, Direction.West };
                direction = directions[rand.Next(directions.Length)];
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
                var newCard = new Map(newCardPos);
                newCard.GenerateRandomMap();
                newCard.SpawnMonsters();
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
            return maps.Values;
        }

        // Retourne la liste des cases accessibles (Herbe) autour d'une case
        public List<(Point point, int cost)> GetNeighbors(Point cell)
        {
            var neighbors = new List<(Point point, int cost)>();
            int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };
            for (int i = 0; i < 4; i++)
            {
                int nx = cell.X + directions[i, 0];
                int ny = cell.Y + directions[i, 1];
                int mapX = nx / Map.GridWidth;
                if (nx < 0)
                {
                    mapX--;
                }
                int mapY = ny / Map.GridHeight;
                if (ny < 0)
                {
                    mapY--;
                }
                if (maps.TryGetValue(new Point(mapX, mapY), out Map? map))
                {
                    var rx = nx - mapX * Map.GridWidth;
                    var ry = ny - mapY * Map.GridHeight;
                    if (map.Tiles[rx, ry].IsWalkable)
                        neighbors.Add((new Point(nx, ny), 1));
                    else if (map != CurrentMap)
                        neighbors.Add((new Point(nx, ny), 10));
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
                foreach (var (neighbor, cost) in GetNeighbors(current))
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
            if (IsInTown)
            {
                // Switch back to hostile zone
                CurrentMap.ConvertToHostile();
                
                // If we have a previous hostile position, go back to it
                if (lastHostileMapPosition != Point.Zero && maps.ContainsKey(lastHostileMapPosition))
                {
                    currentMapPosition = lastHostileMapPosition;
                    CurrentMap.SetCharacters(characters);
                }
            }
            else
            {
                // Remember current hostile position before going to town
                lastHostileMapPosition = currentMapPosition;
                
                // Convert current map to town
                CurrentMap.ConvertToTown();
            }
        }

        public List<NPC> GetNPCsOnCurrentMap()
        {
            return CurrentMap.NPCs;
        }
    }
}