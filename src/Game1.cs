using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;
using System.IO;
using ThirdRun.Characters;
using System.Linq;

namespace MonogameRPG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Map.Map map;
        private List<Character> characters = new List<Character>();
        private FontSystem? _fontSystem;
        private DynamicSpriteFont _dynamicFont;
        private InventoryPanel? _inventoryPanel;
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
            // Création d'un personnage de test au centre de la carte
            var hero = new Character("Héros", CharacterClass.Guerrier, 30, 1, GraphicsDevice, Content)
            {
                Position = new Vector2(
                    map.GridWidth / 2 * map.TileWidth + map.TileWidth / 2,
                    map.GridHeight / 2 * map.TileHeight + map.TileHeight / 2)
            };
            characters.Clear();
            characters.Add(hero);
            map.SetCharacters(characters);
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
            _inventoryPanel = new InventoryPanel(_graphics.PreferredBackBufferHeight, _graphics.PreferredBackBufferWidth);
            // Chargement des icônes d'objets (exemple, à adapter selon vos assets)
            // _itemIcons["Potion de soin"] = Content.Load<Texture2D>("Items/potion");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Déplacement automatique des personnages vers le monstre le plus proche
            foreach (var character in characters)
            {
                character.Move(map.GetMonsters(), map);
            }
            KeyboardState keyboard = Keyboard.GetState();
            if (_inventoryPanel != null && keyboard.IsKeyDown(Keys.I))
            {
                _inventoryPanel.Toggle();
            }
            MouseState mouse = Mouse.GetState();
            if (_inventoryPanel != null)
                _inventoryPanel.Update(gameTime, _graphics.PreferredBackBufferHeight, _graphics.PreferredBackBufferWidth, characters.First().Inventory, mouse, _previousMouseState);
            _previousMouseState = mouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Calcul de la position moyenne des personnages
            Vector2 avg = Vector2.Zero;
            if (characters.Count > 0)
            {
                foreach (var c in characters)
                    avg += c.Position;
                avg /= characters.Count;
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
            if (_inventoryPanel != null)
                _inventoryPanel.Draw(_spriteBatch, _dynamicFont, characters.First().Inventory, _itemIcons, _graphics.PreferredBackBufferHeight);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}