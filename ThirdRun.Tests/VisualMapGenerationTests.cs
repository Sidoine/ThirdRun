using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonogameRPG.Map;
using Xunit;

namespace ThirdRun.Tests
{
    /// <summary>
    /// Visual demonstration test of the new advanced map generation
    /// </summary>
    public class VisualMapGenerationTests
    {
        [Fact]
        public void DemonstrateAdvancedMapGeneration()
        {
            Console.WriteLine("=== ThirdRun Advanced Map Generation Demo ===\n");
            
            // Generate a few different maps to show variety
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Map {i + 1} - World Position ({i}, 0):");
                GenerateAndDisplayMap(new Point(i, 0));
                Console.WriteLine();
            }
            
            Console.WriteLine("Legend:");
            Console.WriteLine("  G = Grass (walkable)     W = Water");
            Console.WriteLine("  H = Hill (walkable)      R = Rock");
            Console.WriteLine("  T = Tree                 B = Building");
            Console.WriteLine("  D = Door (walkable)      V = River");
            Console.WriteLine("  O = Road (walkable)      + = Bridge (walkable)");
            Console.WriteLine("  * = Monster spawn point");
        }
        
        private void GenerateAndDisplayMap(Point worldPosition)
        {
            var map = new MonogameRPG.Map.Map(worldPosition);
            map.GenerateRandomMap(3);
            
            var spawnPoints = map.GetMonsterSpawnPoints();
            var spawnPositions = new HashSet<(int x, int y)>();
            foreach (var spawn in spawnPoints)
            {
                spawnPositions.Add(((int)spawn.X, (int)spawn.Y));
            }
            
            for (int y = 0; y < MonogameRPG.Map.Map.GridHeight; y++)
            {
                for (int x = 0; x < MonogameRPG.Map.Map.GridWidth; x++)
                {
                    // Check if there's a monster spawn point here
                    if (spawnPositions.Contains((x, y)))
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        char symbol = map.Tiles[x, y].Type switch
                        {
                            TileTypeEnum.Grass => 'G',
                            TileTypeEnum.Water => 'W',
                            TileTypeEnum.Rock => 'R',
                            TileTypeEnum.Tree => 'T',
                            TileTypeEnum.Building => 'B',
                            TileTypeEnum.Door => 'D',
                            TileTypeEnum.River => 'V',
                            TileTypeEnum.Road => 'O',
                            TileTypeEnum.Bridge => '+',
                            TileTypeEnum.Hill => 'H',
                            _ => '?'
                        };
                        Console.Write(symbol);
                    }
                }
                Console.WriteLine();
            }
            
            // Display statistics
            var tileStats = new Dictionary<TileTypeEnum, int>();
            for (int x = 0; x < MonogameRPG.Map.Map.GridWidth; x++)
            {
                for (int y = 0; y < MonogameRPG.Map.Map.GridHeight; y++)
                {
                    var type = map.Tiles[x, y].Type;
                    tileStats.TryGetValue(type, out int count);
                    tileStats[type] = count + 1;
                }
            }
            
            Console.WriteLine($"Tile Statistics (Total: {MonogameRPG.Map.Map.GridWidth * MonogameRPG.Map.Map.GridHeight}):");
            foreach (var kvp in tileStats)
            {
                double percentage = (kvp.Value / (double)(MonogameRPG.Map.Map.GridWidth * MonogameRPG.Map.Map.GridHeight)) * 100;
                Console.WriteLine($"  {kvp.Key}: {kvp.Value} tiles ({percentage:F1}%)");
            }
            Console.WriteLine($"Monster Spawn Points: {spawnPoints.Count}");
        }
    }
}