using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;

namespace ThirdRun.UI.Components
{
    public class TextInput : Container
    {
        private readonly Texture2D _pixelTexture;
        private readonly Color _backgroundColor = new Color(30, 30, 30, 200);
        private readonly Color _borderColor = new Color(100, 100, 100, 200);
        private readonly Color _textColor = Color.White;
        private readonly Color _placeholderColor = new Color(150, 150, 150, 200);
        private readonly SpriteFontBase font;

        public string Text { get; set; } = "";
        public string Placeholder { get; set; } = "";
        private bool _isFocused = false;
        private float _cursorBlinkTimer;

        public TextInput(UiManager uiManager, Rectangle bounds, string initialText, SpriteFontBase font)
            : base(uiManager, bounds)
        {
            Text = initialText;
            this.font = font;
            _pixelTexture = new Texture2D(uiManager.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public override bool HandleMouseClick(Point mousePosition)
        {
            if (Bounds.Contains(mousePosition))
            {
                _isFocused = true;
                return true;
            }
            else
            {
                _isFocused = false;
            }
            return false;
        }

        public override bool HandleKeyPress(Keys key)
        {
            if (!_isFocused) return false;

            switch (key)
            {
                case Keys.Back:
                    if (Text.Length > 0)
                        Text = Text.Substring(0, Text.Length - 1);
                    break;
                case Keys.Enter:
                    _isFocused = false;
                    break;
                default:
                    var keyChar = GetCharFromKey(key);
                    if (keyChar != null)
                        Text += keyChar;
                    break;
            }
            return true;
        }

        private char? GetCharFromKey(Keys key)
        {
            // Gérer les touches de lettres
            if (key >= Keys.A && key <= Keys.Z)
            {
                return (char)('A' + (key - Keys.A));
            }
            // Gérer les touches numériques
            if (key >= Keys.D0 && key <= Keys.D9)
            {
                return (char)('0' + (key - Keys.D0));
            }
            // Gérer l'espace
            if (key == Keys.Space)
            {
                return ' ';
            }
            return null;
        }

        public override void Update(GameTime gameTime)
        {
            if (_isFocused)
            {
                _cursorBlinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_cursorBlinkTimer > 1f)
                    _cursorBlinkTimer = 0f;
            }
            base.Update(gameTime);
        }

        public override void Draw()
        {
            if (!Visible) return;

            // Fond
            UiManager.SpriteBatch.Draw(_pixelTexture, Bounds, _backgroundColor);

            // Bordure
            var borderRect = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1);
            UiManager.SpriteBatch.Draw(_pixelTexture, borderRect, _borderColor);
            borderRect.Y = Bounds.Bottom - 1;
            UiManager.SpriteBatch.Draw(_pixelTexture, borderRect, _borderColor);
            borderRect = new Rectangle(Bounds.X, Bounds.Y, 1, Bounds.Height);
            UiManager.SpriteBatch.Draw(_pixelTexture, borderRect, _borderColor);
            borderRect.X = Bounds.Right - 1;
            UiManager.SpriteBatch.Draw(_pixelTexture, borderRect, _borderColor);

            // Texte
            var textToDraw = Text;
            var color = _textColor;
            if (string.IsNullOrEmpty(textToDraw))
            {
                textToDraw = Placeholder;
                color = _placeholderColor;
            }

            var textPos = new Vector2(Bounds.X + 5, Bounds.Center.Y - font.MeasureString(textToDraw).Y / 2);
            font.DrawText(UiManager.SpriteBatch, textToDraw, textPos, color);

            // Curseur
            if (_isFocused && _cursorBlinkTimer < 0.5f)
            {
                var cursorX = (int)(Bounds.X + 5 + font.MeasureString(Text).X);
                var cursorRect = new Rectangle(cursorX, Bounds.Y + 5, 1, Bounds.Height - 10);
                UiManager.SpriteBatch.Draw(_pixelTexture, cursorRect, _textColor);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pixelTexture?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}