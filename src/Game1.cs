using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameRPG.Map;
using ThirdRun.Characters;
using FontStashSharp;
using System.IO;
using MonoGameGum;
using System;
using System.Linq;
using GumRuntime;
using MonoGameGum.Forms.Controls;
using Gum.Wireframe;

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

        private GumService Gum => GumService.Default;

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
            Gum.Initialize(this);

            var mainMenuPanel = new Panel();
            mainMenuPanel.Dock(Dock.Fill);
            mainMenuPanel.AddToRoot();
            var charactersButton = new Button();
            charactersButton.Anchor(Anchor.TopLeft);
            charactersButton.Text = "Characters";
            mainMenuPanel.AddChild(charactersButton);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            map.GenerateRandomMap(GraphicsDevice);
            map.SpawnMonsters(Content);
            // Création d'un personnage de test au centre de la carte
            var hero = new Character("Héros", CharacterClass.Guerrier, 30, 5, GraphicsDevice, Content)
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
        }

        protected override void Update(GameTime gameTime)
        {
            Gum.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Déplacement automatique des personnages vers le monstre le plus proche
            foreach (var character in characters)
            {
                character.Move(map.GetMonsters(), map);
            }

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

            Gum.Draw();
            base.Draw(gameTime);
        }
    }
}