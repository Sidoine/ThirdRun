using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonogameRPG.Map;
using ThirdRun.Utils;

namespace ThirdRun.Data.Map
{
    /// <summary>
    /// Advanced map generator using Perlin noise and structured algorithms
    /// for realistic terrain generation
    /// </summary>
    public class AdvancedMapGenerator
    {
        private readonly PerlinNoise terrainNoise;
        private readonly PerlinNoise moistureNoise;
        private readonly Random random;
        
        // Terrain generation parameters
        private const double TerrainScale = 0.1;
        private const double MoistureScale = 0.08;
        private const int TerrainOctaves = 4;
        private const double TerrainPersistence = 0.5;

        public AdvancedMapGenerator(int seed = 0)
        {
            random = new Random(seed);
            terrainNoise = new PerlinNoise(seed);
            moistureNoise = new PerlinNoise(seed + 1000);
        }

        /// <summary>
        /// Generate a map with realistic terrain features
        /// </summary>
        public TileType[,] GenerateMap(int width, int height, Point worldPosition)
        {
            var tiles = new TileType[width, height];
            
            // Step 1: Generate base terrain using Perlin noise
            GenerateBaseTerrain(tiles, width, height, worldPosition);
            
            // Step 2: Add water bodies and rivers
            AddWaterFeatures(tiles, width, height, worldPosition);
            
            // Step 3: Group trees into forests
            AddForests(tiles, width, height);
            
            // Step 4: Generate buildings with doors
            AddBuildings(tiles, width, height);
            
            // Step 5: Generate roads
            AddRoads(tiles, width, height);
            
            // Step 6: Add bridges where roads cross rivers
            AddBridges(tiles, width, height);
            
            return tiles;
        }

        private void GenerateBaseTerrain(TileType[,] tiles, int width, int height, Point worldPosition)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Calculate world coordinates for noise generation
                    double worldX = (worldPosition.X * width + x) * TerrainScale;
                    double worldY = (worldPosition.Y * height + y) * TerrainScale;
                    
                    // Generate terrain height and moisture
                    double terrainHeight = terrainNoise.OctaveNoise(worldX, worldY, TerrainOctaves, TerrainPersistence);
                    double moisture = moistureNoise.OctaveNoise(worldX * MoistureScale, worldY * MoistureScale, 3, 0.6);
                    
                    // Determine tile type based on terrain height and moisture
                    if (terrainHeight > 0.3)
                    {
                        tiles[x, y] = TileType.CreateHill(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                    }
                    else if (terrainHeight < -0.3 && moisture > 0.2)
                    {
                        tiles[x, y] = TileType.CreateWater(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                    }
                    else if (terrainHeight < -0.1 && moisture > 0.4)
                    {
                        tiles[x, y] = TileType.CreateRiver(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                    }
                    else if (moisture < -0.3)
                    {
                        tiles[x, y] = TileType.CreateRock(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                    }
                    else
                    {
                        tiles[x, y] = TileType.CreateGrass(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                    }
                }
            }
        }

        private void AddWaterFeatures(TileType[,] tiles, int width, int height, Point worldPosition)
        {
            // Add lakes using coherent noise
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double worldX = (worldPosition.X * width + x) * 0.05;
                    double worldY = (worldPosition.Y * height + y) * 0.05;
                    
                    double lakeNoise = terrainNoise.OctaveNoise(worldX, worldY, 2, 0.8);
                    
                    if (lakeNoise > 0.6 && tiles[x, y].Type == TileTypeEnum.Grass)
                    {
                        tiles[x, y] = TileType.CreateWater(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                    }
                }
            }
        }

        private void AddForests(TileType[,] tiles, int width, int height)
        {
            // Group trees in forest patches
            for (int attempt = 0; attempt < 5; attempt++)
            {
                int centerX = random.Next(2, width - 2);
                int centerY = random.Next(2, height - 2);
                
                if (tiles[centerX, centerY].Type != TileTypeEnum.Grass) continue;
                
                int forestSize = random.Next(3, 7);
                
                for (int dx = -forestSize/2; dx <= forestSize/2; dx++)
                {
                    for (int dy = -forestSize/2; dy <= forestSize/2; dy++)
                    {
                        int x = centerX + dx;
                        int y = centerY + dy;
                        
                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            double distance = Math.Sqrt(dx * dx + dy * dy);
                            double probability = Math.Max(0, 1.0 - (distance / (forestSize / 2.0)));
                            
                            if (random.NextDouble() < probability * 0.7 && 
                                tiles[x, y].Type == TileTypeEnum.Grass)
                            {
                                tiles[x, y] = TileType.CreateTree(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                            }
                        }
                    }
                }
            }
        }

        private void AddBuildings(TileType[,] tiles, int width, int height)
        {
            // Generate 1-3 buildings as rectangles with doors
            int buildingCount = random.Next(1, 4);
            
            for (int i = 0; i < buildingCount; i++)
            {
                // Find suitable location for building
                int attempts = 0;
                while (attempts < 20)
                {
                    int buildingWidth = random.Next(3, Math.Max(4, Math.Min(6, width - 2)));
                    int buildingHeight = random.Next(3, Math.Max(4, Math.Min(5, height - 2)));
                    int startX = random.Next(1, Math.Max(2, width - buildingWidth - 1));
                    int startY = random.Next(1, Math.Max(2, height - buildingHeight - 1));
                    
                    // Check if area is suitable (mostly grass)
                    bool suitable = true;
                    for (int x = startX; x < startX + buildingWidth && suitable && x < width; x++)
                    {
                        for (int y = startY; y < startY + buildingHeight && suitable && y < height; y++)
                        {
                            if (tiles[x, y].Type != TileTypeEnum.Grass)
                                suitable = false;
                        }
                    }
                    
                    if (suitable)
                    {
                        // Place building walls
                        for (int x = startX; x < startX + buildingWidth && x < width; x++)
                        {
                            for (int y = startY; y < startY + buildingHeight && y < height; y++)
                            {
                                // Walls on perimeter, empty inside
                                if (x == startX || x == startX + buildingWidth - 1 ||
                                    y == startY || y == startY + buildingHeight - 1)
                                {
                                    tiles[x, y] = TileType.CreateBuilding(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                                }
                            }
                        }
                        
                        // Add door (hole in wall)
                        AddDoorToBuilding(tiles, startX, startY, buildingWidth, buildingHeight, width, height);
                        break;
                    }
                    attempts++;
                }
            }
        }

        private void AddDoorToBuilding(TileType[,] tiles, int startX, int startY, int width, int height, int mapWidth, int mapHeight)
        {
            // Only add door if building is large enough
            if (width < 3 || height < 3) return;
            
            // Choose a wall side for the door
            int side = random.Next(4);
            int doorX, doorY;
            
            switch (side)
            {
                case 0: // Top wall
                    doorX = startX + random.Next(1, width - 1);
                    doorY = startY;
                    break;
                case 1: // Right wall
                    doorX = startX + width - 1;
                    doorY = startY + random.Next(1, height - 1);
                    break;
                case 2: // Bottom wall
                    doorX = startX + random.Next(1, width - 1);
                    doorY = startY + height - 1;
                    break;
                default: // Left wall
                    doorX = startX;
                    doorY = startY + random.Next(1, height - 1);
                    break;
            }
            
            // Ensure door is within map bounds
            if (doorX >= 0 && doorX < mapWidth && doorY >= 0 && doorY < mapHeight)
            {
                tiles[doorX, doorY] = TileType.CreateDoor(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
            }
        }

        private void AddRoads(TileType[,] tiles, int width, int height)
        {
            // Add horizontal road
            if (random.NextDouble() < 0.6)
            {
                int roadY = random.Next(height / 3, 2 * height / 3);
                for (int x = 0; x < width; x++)
                {
                    if (tiles[x, roadY].Type == TileTypeEnum.Grass || 
                        tiles[x, roadY].Type == TileTypeEnum.Hill)
                    {
                        tiles[x, roadY] = TileType.CreateRoad(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                    }
                }
            }
            
            // Add vertical road
            if (random.NextDouble() < 0.4)
            {
                int roadX = random.Next(width / 3, 2 * width / 3);
                for (int y = 0; y < height; y++)
                {
                    if (tiles[roadX, y].Type == TileTypeEnum.Grass || 
                        tiles[roadX, y].Type == TileTypeEnum.Hill)
                    {
                        tiles[roadX, y] = TileType.CreateRoad(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                    }
                }
            }
        }

        private void AddBridges(TileType[,] tiles, int width, int height)
        {
            // Convert roads crossing rivers/water to bridges
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tiles[x, y].Type == TileTypeEnum.River || tiles[x, y].Type == TileTypeEnum.Water)
                    {
                        // Check if there should be a road here (adjacent road tiles)
                        bool hasAdjacentRoad = false;
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (dx == 0 && dy == 0) continue;
                                int nx = x + dx, ny = y + dy;
                                if (nx >= 0 && nx < width && ny >= 0 && ny < height &&
                                    tiles[nx, ny].Type == TileTypeEnum.Road)
                                {
                                    hasAdjacentRoad = true;
                                    break;
                                }
                            }
                            if (hasAdjacentRoad) break;
                        }
                        
                        if (hasAdjacentRoad)
                        {
                            tiles[x, y] = TileType.CreateBridge(MonogameRPG.Map.Map.TileWidth, MonogameRPG.Map.Map.TileHeight);
                        }
                    }
                }
            }
        }
    }
}