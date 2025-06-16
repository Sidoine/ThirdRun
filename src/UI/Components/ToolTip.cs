using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;

namespace ThirdRun.UI.Components
{
    public class ToolTip : Container
    {
        private readonly UIElement _child;
        private readonly string _text;
        private readonly SpriteFontBase _font;
        private bool _isHovered;
        private readonly Texture2D _pixelTexture;
        private readonly Color _bgColor = new Color(30, 30, 30, 220);
        private readonly Color _borderColor = new Color(80, 80, 80, 255);

        public ToolTip(UiManager uiManager, UIElement child, string text, SpriteFontBase font)
            : base(uiManager, child.Bounds)
        {
            _child = child;
            AddChild(_child);
            _text = text;
            _font = font;
            _pixelTexture = new Texture2D(uiManager.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public override void UpdateHover(Point mousePosition)
        {
            _isHovered = _child.ContainsPoint(mousePosition);
            _child.UpdateHover(mousePosition);
        }

        public override void Draw()
        {
            _child.Draw();
            if (_isHovered && !string.IsNullOrEmpty(_text))
            {
                var textSize = _font.MeasureString(_text);
                // Afficher le tooltip à droite et légèrement au-dessus de l'élément enfant
                var pos = new Vector2(_child.Bounds.Right + 12, _child.Bounds.Top - (int)textSize.Y - 8);
                // Si le tooltip serait trop haut, l'afficher en dessous
                if (pos.Y < 0) pos.Y = _child.Bounds.Bottom + 8;
                var rect = new Rectangle((int)pos.X, (int)pos.Y, (int)textSize.X + 12, (int)textSize.Y + 8);
                UiManager.SpriteBatch.Draw(_pixelTexture, rect, _bgColor);
                UiManager.SpriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y, rect.Width, 2), _borderColor);
                UiManager.SpriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), _borderColor);
                UiManager.SpriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y, 2, rect.Height), _borderColor);
                UiManager.SpriteBatch.Draw(_pixelTexture, new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), _borderColor);
                _font.DrawText(UiManager.SpriteBatch, _text, pos + new Vector2(6, 4), Color.White);
            }
        }
    }
}
