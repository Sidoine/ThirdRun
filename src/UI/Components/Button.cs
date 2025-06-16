using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;

namespace ThirdRun.UI.Components
{
    public class Button : Container
    {
        private string? _text;

        // TODO not used yet
        private Color _defaultColor = new Color(40, 40, 40, 60);
        private Color _hoverColor = new Color(60, 60, 60, 80);
        protected bool IsHovered { get; private set; }

        protected bool IsPressed { get; private set; }

        private Action _onClick;
        private readonly Texture2D _pixelTexture;

        private BorderedTextureRenderer _clickedBorder;

        private BorderedTextureRenderer _border;

        public SpriteFontBase? Font { get; }

        public Button(UiManager uiManager, Rectangle bounds, Action onClick, string? text = null, SpriteFontBase? font = null)
            : base(uiManager, bounds)
        {
            _text = text;
            _onClick = onClick;
            Font = text != null ? (font ?? uiManager.FontSystem.GetFont(16)) : null;

            // Créer la texture de pixel blanc pour le fond
            _pixelTexture = new Texture2D(uiManager.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            _clickedBorder = new BorderedTextureRenderer
            {
                BottomBorder = uiManager.ContentManager.Load<Texture2D>("UI/Button/bottom-on"),
                TopBorder = uiManager.ContentManager.Load<Texture2D>("UI/Button/top-on"),
                LeftBorder = uiManager.ContentManager.Load<Texture2D>("UI/Button/left-on"),
                RightBorder = uiManager.ContentManager.Load<Texture2D>("UI/Button/right-on"),
                TopLeftCorner = uiManager.ContentManager.Load<Texture2D>("UI/Button/top-left-on"),
                TopRightCorner = uiManager.ContentManager.Load<Texture2D>("UI/Button/top-right-on"),
                BottomLeftCorner = uiManager.ContentManager.Load<Texture2D>("UI/Button/bottom-left-on"),
                BottomRightCorner = uiManager.ContentManager.Load<Texture2D>("UI/Button/bottom-right-on"),
            };

            _border = new BorderedTextureRenderer
            {
                BottomBorder = uiManager.ContentManager.Load<Texture2D>("UI/Button/bottom"),
                TopBorder = uiManager.ContentManager.Load<Texture2D>("UI/Button/top"),
                LeftBorder = uiManager.ContentManager.Load<Texture2D>("UI/Button/left"),
                RightBorder = uiManager.ContentManager.Load<Texture2D>("UI/Button/right"),
                TopLeftCorner = uiManager.ContentManager.Load<Texture2D>("UI/Button/top-left"),
                TopRightCorner = uiManager.ContentManager.Load<Texture2D>("UI/Button/top-right"),
                BottomLeftCorner = uiManager.ContentManager.Load<Texture2D>("UI/Button/bottom-left"),
                BottomRightCorner = uiManager.ContentManager.Load<Texture2D>("UI/Button/bottom-right"),
            };
        }

        public void SetColors(Color defaultColor, Color hoverColor)
        {
            _defaultColor = defaultColor;
            _hoverColor = hoverColor;
        }

        public void SetText(string text)
        {
            _text = text;
        }

        public override void UpdateHover(Point mousePosition)
        {
            if (!Visible) return;
            IsHovered = Bounds.Contains(mousePosition);
        }

        public override bool HandleMouseClick(Point mousePosition)
        {
            if (!Visible || !IsHovered) return false;
            IsPressed = false;
            _onClick?.Invoke();
            return true;
        }

        public override bool HandleMouseDown(Point mousePosition)
        {
            if (!Visible) return false;
            IsPressed = true;
            return true;
        }

        public override void Draw()
        {
            if (!Visible) return;
            base.Draw();

            // UiManager.SpriteBatch.Draw(_pixelTexture, Bounds, _isHovered ? _hoverColor : _defaultColor);
            if (IsHovered)
                _clickedBorder.Draw(UiManager.SpriteBatch, Bounds);
            else
                _border.Draw(UiManager.SpriteBatch, Bounds);

            if (_text != null && Font != null)
            {
                Vector2 textSize = Font.MeasureString(_text);
                Vector2 textPosition = new Vector2(
                    Bounds.Center.X - textSize.X / 2,
                    Bounds.Center.Y - textSize.Y / 2);

                Font.DrawText(UiManager.SpriteBatch, _text, textPosition, Color.White);
            }
        }

        public Button WithBounds(Rectangle newBounds)
        {
            Bounds = newBounds;
            return this;
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