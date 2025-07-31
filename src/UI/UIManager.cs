using FontStashSharp;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ThirdRun.Data;

namespace ThirdRun.UI;

public class UiManager
{
    public SpriteBatch SpriteBatch { get; }
    public FontSystem FontSystem { get; }
    public Texture2D Pixel { get; }

    public ContentManager ContentManager { get; }
    public GraphicsDevice GraphicsDevice { get; }


    public State CurrentState { get; } = new State();
    public GameState GameState { get; }

    public UiManager(
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        FontSystem fontSystem,
        ContentManager contentManager,
        GameState gameState)
    {
        GraphicsDevice = graphicsDevice;
        SpriteBatch = spriteBatch;
        FontSystem = fontSystem;
        ContentManager = contentManager;
        GameState = gameState;
        // Initialisation de la texture 1x1 blanche
        Pixel = new Texture2D(graphicsDevice, 1, 1);
        Pixel.SetData(new[] { Microsoft.Xna.Framework.Color.White });
    }

    public class State
    {
        public bool IsInventoryVisible { get; set; } = false;
        public bool IsCharacterDetailsVisible { get; set; } = false;
        public Character? SelectedCharacter { get; set; } = null;
    }
}
