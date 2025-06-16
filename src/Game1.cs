using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;
using System.IO;
using ThirdRun.UI;
using ThirdRun.UI.Panels;
using ThirdRun.Data;

namespace MonogameRPG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Map.Map map;
        private FontSystem? _fontSystem;
        private DynamicSpriteFont _dynamicFont;
        private UiManager _uiManager;
        private Root _rootPanel;
        private GameState _gameState;

        private Dictionary<string, Texture2D> _itemIcons = new();
        private MouseState _previousMouseState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            map = null!;
            _spriteBatch = null!;
            _dynamicFont = null!;
            IsMouseVisible = true; // Affiche le curseur de la souris
            _uiManager = null!;
            _gameState = null!;
            _rootPanel = null!;
        }

        protected override void Initialize()
        {
            map = new();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            map.GenerateRandomMap(GraphicsDevice);
            map.SpawnMonsters(Content);
            _gameState = new GameState
            {
                Player = new Player(map, Content),
                Map = map,
            };
            map.SetCharacters(_gameState.Player.Characters);
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

            // Déplacement automatique des personnages vers le monstre le plus proche
            foreach (var character in _gameState.Player.Characters)
            {
                character.Move(map.GetMonsters(), map);
            }
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.I))
            {
                _uiManager.CurrentState.IsInventoryVisible = !_uiManager.CurrentState.IsInventoryVisible;
            }
            MouseState mouse = Mouse.GetState();
            _rootPanel.Update(gameTime);
            _rootPanel.UpdateHover(mouse.Position);
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
            map.Render(_spriteBatch, _dynamicFont); // On passe la police dynamique
            _spriteBatch.End();
            // Affichage du panneau d'inventaire (hors caméra)
            _spriteBatch.Begin();
            _rootPanel.Draw();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}