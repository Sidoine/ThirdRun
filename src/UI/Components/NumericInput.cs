using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;

namespace ThirdRun.UI.Components
{
    public class NumericInput : Container
    {
        private readonly Texture2D _pixelTexture;
        private readonly Color _backgroundColor = new Color(30, 30, 30, 200);
        private readonly Color _borderColor = new Color(100, 100, 100, 200);
        private readonly Color _textColor = Color.White;
        
        public int Value { get; private set; }
        private string _text = "0";
        private bool _isFocused = false;
        private float _cursorBlinkTimer;
        private readonly int _minValue;
        private readonly int _maxValue;

        public SpriteFontBase Font { get; }

        public NumericInput(UiManager uiManager, Rectangle bounds, int minValue, int maxValue, SpriteFontBase font)
            : base(uiManager, bounds)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            Font = font;
            Value = minValue;
            _text = minValue.ToString();

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
                    if (_text.Length > 0)
                    {
                        _text = _text.Substring(0, _text.Length - 1);
                        if (string.IsNullOrEmpty(_text))
                        {
                            _text = "0";
                        }
                        UpdateValue();
                    }
                    break;
                case Keys.Enter:
                    _isFocused = false;
                    break;
                default:
                    var keyChar = GetCharFromKey(key);
                    if (keyChar.HasValue && char.IsDigit(keyChar.Value))
                    {
                        if (_text == "0")
                            _text = keyChar.Value.ToString();
                        else
                            _text += keyChar.Value;
                        UpdateValue();
                    }
                    break;
            }
            return true;
        }

        private void UpdateValue()
        {
            if (int.TryParse(_text, out int newValue))
            {
                Value = MathHelper.Clamp(newValue, _minValue, _maxValue);
                _text = Value.ToString();
            }
        }

        private char? GetCharFromKey(Keys key)
        {
            // Gérer les touches numériques
            if (key >= Keys.D0 && key <= Keys.D9)
            {
                return (char)('0' + (key - Keys.D0));
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
            var textPos = new Vector2(Bounds.Right - 5 - Font.MeasureString(_text).X, 
                Bounds.Center.Y - Font.MeasureString(_text).Y / 2);
            Font.DrawText(UiManager.SpriteBatch, _text, textPos, _textColor);

            // Curseur
            if (_isFocused && _cursorBlinkTimer < 0.5f)
            {
                var cursorX = textPos.X + Font.MeasureString(_text).X + 2;
                var cursorRect = new Rectangle((int)cursorX, Bounds.Y + 5, 1, Bounds.Height - 10);
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