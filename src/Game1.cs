using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;
using System.IO;
using ThirdRun.UI;
using ThirdRun.UI.Panels;
using ThirdRun.Data;
using MonogameRPG.Map;
using ThirdRun.Graphics.Map;

namespace MonogameRPG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private WorldMap worldMap;
        private FontSystem? _fontSystem;
        private DynamicSpriteFont _dynamicFont;
        private UiManager _uiManager;
        private Root _rootPanel;
        private GameState _gameState;
        private WorldMapView _worldMapView;

        private Dictionary<string, Texture2D> _itemIcons = new();
        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private bool _previousTownState = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            worldMap = null!;
            _spriteBatch = null!;
            _dynamicFont = null!;
            IsMouseVisible = true; // Affiche le curseur de la souris
            _uiManager = null!;
            _gameState = null!;
            _rootPanel = null!;
            _worldMapView = null!;
        }

        protected override void Initialize()
        {
            worldMap = new Map.WorldMap(new Random());
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            worldMap.Initialize();
            _worldMapView = new WorldMapView(Content);
            _gameState = new GameState
            {
                Player = new Player(worldMap, new Random()),
                WorldMap = worldMap,
            };
            worldMap.SetCharacters(_gameState.Player.Characters);
            // Chargement de la police Arial avec FontStashSharp
            _fontSystem = new FontSystem();
            using (var stream = File.OpenRead("Content/Arial.ttf"))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    _fontSystem.AddFont(ms.ToArray());
                }
            }
            _dynamicFont = _fontSystem.GetFont(24); // Taille 24px
            _uiManager = new UiManager(GraphicsDevice, _spriteBatch, _fontSystem, Content, _gameState);
            _rootPanel = new Root(_uiManager, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
            // Chargement des icônes d'objets (exemple, à adapter selon vos assets)
            // _itemIcons["Potion de soin"] = Content.Load<Texture2D>("Items/potion");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update world map logic (card transitions, cleanup, etc.)
            worldMap.Update();
            
            // Déplacement automatique des personnages vers le monstre le plus proche
            foreach (var character in worldMap.CurrentMap.Characters.ToArray())
            {
                character.UpdateGameTime((float)gameTime.TotalGameTime.TotalSeconds);
                if (!character.IsDead) // Only move if alive
                {
                    character.Move(worldMap.GetMonstersOnCurrentMap());
                }
            }
            
            // Update monsters with game time and AI behavior
            foreach (var monster in worldMap.GetMonstersOnCurrentMap())
            {
                monster.UpdateGameTime((float)gameTime.TotalGameTime.TotalSeconds);
                monster.Update(); // Add monster AI update
            }
            
            // Check for party wipe (all characters dead) and handle recovery
            CheckForPartyWipe();
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.I) && !_previousKeyboardState.IsKeyDown(Keys.I))
            {
                _uiManager.CurrentState.IsInventoryVisible = !_uiManager.CurrentState.IsInventoryVisible;
            }
            if (keyboard.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.P))
            {
                _uiManager.CurrentState.IsInTown = !_uiManager.CurrentState.IsInTown;
            }
            
            // Handle town state changes
            if (_uiManager.CurrentState.IsInTown != _previousTownState)
            {
                worldMap.ToggleTownMode();
                _previousTownState = _uiManager.CurrentState.IsInTown;
            }
            MouseState mouse = Mouse.GetState();
            _rootPanel.Update(gameTime);
            _rootPanel.UpdateHover(mouse.Position);
            
            // Handle mouse wheel scrolling
            if (mouse.ScrollWheelValue != _previousMouseState.ScrollWheelValue)
            {
                int scrollDelta = mouse.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
                _rootPanel.HandleScroll(mouse.Position, scrollDelta);
            }
            
            if (mouse.LeftButton != _previousMouseState.LeftButton)
            {
                if (mouse.LeftButton == ButtonState.Released)
                {
                    // Handle drag and drop first
                    if (_uiManager.DragAndDropManager.IsDragging)
                    {
                        bool dropHandled = _uiManager.DragAndDropManager.TryDrop(mouse.Position);
                        if (!dropHandled)
                        {
                            _rootPanel.HandleMouseUp(mouse.Position);
                        }
                    }
                    else
                    {
                        _rootPanel.HandleMouseClick(mouse.Position);
                        _rootPanel.HandleMouseUp(mouse.Position);
                    }
                }

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    _rootPanel.HandleMouseDown(mouse.Position);
                }
            }
            
            // Handle right-click for item equipping
            if (mouse.RightButton != _previousMouseState.RightButton)
            {
                if (mouse.RightButton == ButtonState.Released)
                {
                    _rootPanel.HandleMouseRightClick(mouse.Position);
                }
            }
            _previousMouseState = mouse;
            _previousKeyboardState = keyboard;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Calcul de la position moyenne des personnages
            Vector2 avg = Vector2.Zero;
            if (_gameState.Player.Characters.Count > 0)
            {
                foreach (var c in _gameState.Player.Characters)
                    avg += c.Position;
                avg /= _gameState.Player.Characters.Count;
            }
            // Calcul du décalage caméra (centrer sur la moyenne)
            var viewport = GraphicsDevice.Viewport;
            Vector2 screenCenter = new Vector2(viewport.Width / 2, viewport.Height / 2);
            Vector2 offset = screenCenter - avg;
            Matrix camera = Matrix.CreateTranslation(new Vector3(offset, 0f));
            _spriteBatch.Begin(transformMatrix: camera);
            _worldMapView.Render(_spriteBatch, worldMap, _dynamicFont); // On passe la police dynamique
            _spriteBatch.End();
            // Affichage du panneau d'inventaire (hors caméra)
            var rasterizerState = new RasterizerState() { ScissorTestEnable = true };
            _spriteBatch.Begin(rasterizerState: rasterizerState);
            _rootPanel.Draw();
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Checks if all characters are dead and handles party wipe recovery
        /// </summary>
        private void CheckForPartyWipe()
        {
            var allCharacters = _gameState.Player.Characters;
            if (allCharacters.Count > 0 && allCharacters.All(c => c.IsDead))
            {
                // All characters are dead - initiate party wipe recovery
                HandlePartyWipe();
            }
        }

        /// <summary>
        /// Handles party wipe recovery by teleporting all characters to town with full health
        /// </summary>
        private void HandlePartyWipe()
        {
            // Restore all characters to full health
            foreach (var character in _gameState.Player.Characters)
            {
                character.CurrentHealth = character.MaxHealth;
            }
            
            // Force toggle to town mode if not already there
            if (!_uiManager.CurrentState.IsInTown)
            {
                _uiManager.CurrentState.IsInTown = true;
                worldMap.ToggleTownMode();
                _previousTownState = true;
            }
        }
    }
}