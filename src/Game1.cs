using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;
using System.IO;
using ThirdRun.UI;
using ThirdRun.UI.Panels;
using ThirdRun.Data;
using MonogameRPG.Map;

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

        private Dictionary<string, Texture2D> _itemIcons = new();
        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;

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
        }

        protected override void Initialize()
        {
            worldMap = new Map.WorldMap(Content, GraphicsDevice);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            worldMap.Initialize();
            _gameState = new GameState
            {
                Player = new Player(worldMap, Content),
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
                character.Move(worldMap.GetMonstersOnCurrentMap());
            }
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.I) && !_previousKeyboardState.IsKeyDown(Keys.I))
            {
                _uiManager.CurrentState.IsInventoryVisible = !_uiManager.CurrentState.IsInventoryVisible;
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
                    _rootPanel.HandleMouseClick(mouse.Position);
                }

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    _rootPanel.HandleMouseDown(mouse.Position);
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
            worldMap.Render(_spriteBatch, _dynamicFont); // On passe la police dynamique
            _spriteBatch.End();
            // Affichage du panneau d'inventaire (hors caméra)
            var rasterizerState = new RasterizerState() { ScissorTestEnable = true };
            _spriteBatch.Begin(rasterizerState: rasterizerState);
            _rootPanel.Draw();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}