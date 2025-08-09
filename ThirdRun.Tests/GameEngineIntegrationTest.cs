using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonogameRPG.Map;
using ThirdRun.Data;
using ThirdRun.Characters;
using Xunit;

namespace ThirdRun.Tests
{
    /// <summary>
    /// Integration test for the game engine that runs the core game loop for 1000 frames
    /// and verifies no exceptions are thrown during normal gameplay simulation.
    /// </summary>
    public class GameEngineIntegrationTest
    {
        [Fact]
        public void GameEngine_RunFor1000Frames_ShouldNotThrowExceptions()
        {
            // Arrange - Set up the game state similar to Game1.Initialize() and LoadContent()
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var gameState = new GameState
            {
                Player = new Player(worldMap),
                WorldMap = worldMap,
            };
            
            worldMap.SetCharacters(gameState.Player.Characters);
            
            var startTime = TimeSpan.Zero;
            var frameTimeIncrement = TimeSpan.FromMilliseconds(16.67); // ~60 FPS
            var exceptions = new List<Exception>();
            
            // Act - Simulate 1000 frames of game execution
            for (int frame = 0; frame < 1000; frame++)
            {
                try
                {
                    var gameTime = new GameTime(startTime, frameTimeIncrement);
                    startTime += frameTimeIncrement;
                    
                    // Simulate the core game logic from Game1.Update()
                    SimulateGameFrame(worldMap, gameState, gameTime);
                }
                catch (Exception ex)
                {
                    exceptions.Add(new Exception($"Exception on frame {frame}: {ex.Message}", ex));
                }
            }
            
            // Assert - Verify no exceptions occurred during the 1000 frame simulation
            if (exceptions.Count > 0)
            {
                var aggregateException = new AggregateException(
                    $"Game engine threw {exceptions.Count} exception(s) during 1000 frame test",
                    exceptions
                );
                throw aggregateException;
            }
            
            // Additional verifications to ensure the game state is still valid
            Assert.NotNull(gameState.Player);
            Assert.NotNull(gameState.WorldMap);
            Assert.NotEmpty(gameState.Player.Characters);
            Assert.True(gameState.Player.Characters.TrueForAll(c => c.CurrentHealth > 0), 
                "All characters should still be alive after 1000 frames");
        }
        
        /// <summary>
        /// Simulates one frame of the game engine, including all the core logic from Game1.Update()
        /// but without any graphics-dependent operations.
        /// </summary>
        private static void SimulateGameFrame(WorldMap worldMap, GameState gameState, GameTime gameTime)
        {
            // Update world map logic (card transitions, cleanup, etc.)
            worldMap.Update();
            
            // Update characters and simulate movement toward monsters
            foreach (var character in worldMap.CurrentMap.Characters.ToArray())
            {
                character.UpdateGameTime((float)gameTime.TotalGameTime.TotalSeconds);
                
                // Simulate character movement toward closest monster
                var monsters = worldMap.GetMonstersOnCurrentMap();
                if (monsters.Count > 0)
                {
                    character.Move(monsters);
                }
            }
            
            // Update monsters with game time
            foreach (var monster in worldMap.GetMonstersOnCurrentMap())
            {
                monster.UpdateGameTime((float)gameTime.TotalGameTime.TotalSeconds);
            }
            
            // Simulate world map updates (character transitions between maps)
            worldMap.UpdateCurrentMap();
        }
        
        [Fact]
        public void GameEngine_RunFor100FramesWithCombat_ShouldHandleCombatCorrectly()
        {
            // Arrange - Set up a game state with characters positioned near monsters
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var gameState = new GameState
            {
                Player = new Player(worldMap),
                WorldMap = worldMap,
            };
            
            worldMap.SetCharacters(gameState.Player.Characters);
            
            // Position characters near monsters to force combat situations
            var monsters = worldMap.GetMonstersOnCurrentMap();
            if (monsters.Count > 0)
            {
                var firstMonster = monsters[0];
                foreach (var character in gameState.Player.Characters)
                {
                    // Position character close to monster to trigger combat
                    character.Position = firstMonster.Position + new Vector2(50, 0);
                }
            }
            
            var startTime = TimeSpan.Zero;
            var frameTimeIncrement = TimeSpan.FromMilliseconds(16.67);
            var exceptions = new List<Exception>();
            
            // Act - Run shorter test focused on combat scenarios
            for (int frame = 0; frame < 100; frame++)
            {
                try
                {
                    var gameTime = new GameTime(startTime, frameTimeIncrement);
                    startTime += frameTimeIncrement;
                    
                    SimulateGameFrame(worldMap, gameState, gameTime);
                }
                catch (Exception ex)
                {
                    exceptions.Add(new Exception($"Combat frame {frame} exception: {ex.Message}", ex));
                }
            }
            
            // Assert - Verify no exceptions during combat simulation
            if (exceptions.Count > 0)
            {
                var aggregateException = new AggregateException(
                    $"Game engine threw {exceptions.Count} exception(s) during combat test",
                    exceptions
                );
                throw aggregateException;
            }
            
            // Verify game state is still consistent after combat
            Assert.NotNull(gameState.WorldMap.CurrentMap);
            Assert.NotEmpty(gameState.Player.Characters);
        }
        
        [Fact]
        public void GameEngine_RunFor50FramesWithMapTransitions_ShouldHandleTransitionsCorrectly()
        {
            // Arrange - Set up game state and force map transitions
            var worldMap = new WorldMap();
            worldMap.Initialize();
            
            var gameState = new GameState
            {
                Player = new Player(worldMap),
                WorldMap = worldMap,
            };
            
            worldMap.SetCharacters(gameState.Player.Characters);
            
            var startTime = TimeSpan.Zero;
            var frameTimeIncrement = TimeSpan.FromMilliseconds(16.67);
            var exceptions = new List<Exception>();
            var initialMapPosition = worldMap.CurrentMapPosition;
            
            // Act - Run test that may trigger map transitions
            for (int frame = 0; frame < 50; frame++)
            {
                try
                {
                    var gameTime = new GameTime(startTime, frameTimeIncrement);
                    startTime += frameTimeIncrement;
                    
                    SimulateGameFrame(worldMap, gameState, gameTime);
                    
                    // Occasionally trigger map transition logic
                    if (frame % 10 == 0)
                    {
                        worldMap.UpdateCurrentMap();
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(new Exception($"Map transition frame {frame} exception: {ex.Message}", ex));
                }
            }
            
            // Assert - Verify no exceptions during map transition simulation
            if (exceptions.Count > 0)
            {
                var aggregateException = new AggregateException(
                    $"Game engine threw {exceptions.Count} exception(s) during map transition test",
                    exceptions
                );
                throw aggregateException;
            }
            
            // Verify the world map is still in a valid state
            Assert.NotNull(worldMap.CurrentMap);
            Assert.NotEmpty(gameState.Player.Characters);
        }
    }
}