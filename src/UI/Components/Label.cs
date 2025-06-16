using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using System;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Composant d'interface utilisateur pour afficher du texte
    /// </summary>
    public class Label : UIElement
    {
        private string _text;
        private Func<string>? _textProvider;
        private Color _textColor = Color.White;
        private SpriteFontBase _font;

        public string Text
        {
            get => _textProvider != null ? _textProvider() : _text;
            set
            {
                _text = value;
                _textProvider = null;
            }
        }

        public Color TextColor
        {
            get => _textColor;
            set => _textColor = value;
        }

        public Label(UiManager uiManager, Rectangle bounds, string text, SpriteFontBase font) 
            : base(uiManager, bounds)
        {
            _text = text;
            _font = font;
        }

        public Label(UiManager uiManager, Rectangle bounds, Func<string> textProvider, SpriteFontBase font)
            : base(uiManager, bounds)
        {
            _textProvider = textProvider;
            _text = string.Empty;
            _font = font;
        }

        public override void Draw()
        {
            if (!Visible)
                return;

            string displayText = Text;
            Vector2 textSize = _font.MeasureString(displayText);
            Vector2 position = new Vector2(Bounds.X, Bounds.Y);

            // Ajustement de la position du texte pour qu'il soit centré verticalement
            position.Y += (Bounds.Height - textSize.Y) / 2;

            _font.DrawText(UiManager.SpriteBatch, displayText, position, _textColor);
        }
    }
}