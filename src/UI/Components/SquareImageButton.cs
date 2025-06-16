using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;

namespace ThirdRun.UI.Components
{
    public class SquareImageButton : UIElement
    {
        private Texture2D? _icon;
        private Action _onClick;
        private Color _defaultColor = new Color(40, 40, 40, 60);
        private Color _hoverColor = new Color(60, 100, 60, 80);
        private bool _isHovered;
        private readonly Texture2D _pixelTexture;

        public SquareImageButton(UiManager uiManager, Rectangle bounds, Texture2D? icon, Action onClick)
            : base(uiManager, bounds)
        {
            _icon = icon;
            _onClick = onClick;
            _pixelTexture = new Texture2D(uiManager.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public void SetColors(Color defaultColor, Color hoverColor)
        {
            _defaultColor = defaultColor;
            _hoverColor = hoverColor;
        }

        public override void UpdateHover(Point mousePosition)
        {
            if (!Visible) return;
            _isHovered = Bounds.Contains(mousePosition);
        }

        public override bool HandleMouseClick(Point mousePosition)
        {
            if (!Visible || !_isHovered) return false;
            _onClick?.Invoke();
            return true;
        }

        public override void Draw()
        {
            if (!Visible) return;
            UiManager.SpriteBatch.Draw(_pixelTexture, Bounds, _isHovered ? _hoverColor : _defaultColor);
            if (_icon != null)
            {
                // Centrer l'icône dans le bouton
                int iconSize = Math.Min(Bounds.Width, Bounds.Height) - 8;
                Rectangle iconRect = new Rectangle(
                    Bounds.X + (Bounds.Width - iconSize) / 2,
                    Bounds.Y + (Bounds.Height - iconSize) / 2,
                    iconSize, iconSize);
                UiManager.SpriteBatch.Draw(_icon, iconRect, Color.White);
            }
        }

        public SquareImageButton WithBounds(Rectangle newBounds)
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
