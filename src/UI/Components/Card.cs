using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Composant d'interface utilisateur pour afficher un panel avec un fond coloré et une bordure optionnelle
    /// </summary>
    public class Card : UIElement
    {
        private Texture2D _backgroundTexture;
        private Texture2D? _borderTexture;  // Marqué comme nullable
        private Color _backgroundColor;
        private Color _borderColor;
        private bool _showBorder;

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

        public Color BorderColor
        {
            get => _borderColor;
            set => _borderColor = value;
        }

        public bool ShowBorder
        {
            get => _showBorder;
            set => _showBorder = value;
        }

        public Card(UiManager uiManager, Rectangle bounds, Color backgroundColor)
            : base(uiManager, bounds)
        {
            _backgroundTexture = uiManager.Pixel;
            _backgroundColor = backgroundColor;
            _borderTexture = null;  // Initialisation explicite à null
            _borderColor = Color.White;
            _showBorder = false;
        }

        public Card(UiManager uiManager, Rectangle bounds, Texture2D backgroundTexture, Color backgroundColor)
            : base(uiManager, bounds)
        {
            _backgroundTexture = backgroundTexture;
            _backgroundColor = backgroundColor;
            _borderTexture = null;  // Initialisation explicite à null
            _borderColor = Color.White;
            _showBorder = false;
        }

        public Card(UiManager uiManager, Rectangle bounds, Texture2D backgroundTexture, 
                    Color backgroundColor, Texture2D borderTexture, Color borderColor)
            : base(uiManager, bounds)
        {
            _backgroundTexture = backgroundTexture;
            _backgroundColor = backgroundColor;
            _borderTexture = borderTexture;
            _borderColor = borderColor;
            _showBorder = true;
        }

        public override void Draw()
        {
            if (!Visible)
                return;

            // Dessiner le fond du panneau
            UiManager.SpriteBatch.Draw(_backgroundTexture, Bounds, _backgroundColor);

            // Dessiner la bordure si activée
            if (_showBorder && _borderTexture != null)
            {
                UiManager.SpriteBatch.Draw(_borderTexture, Bounds, _borderColor);
            }
        }
    }
}