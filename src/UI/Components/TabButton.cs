using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;

namespace ThirdRun.UI.Components
{
    public class TabButton : Button
    {
        private bool _isSelected;
        private Color _selectedColor = new Color(60, 60, 60, 200);
        private Texture2D _pixelTexture;

        public bool IsSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }

        public TabButton(UiManager uiManager, Rectangle bounds, string text, System.Action onClick, SpriteFontBase font)
            : base(uiManager, bounds, onClick, text, font)
        {
            SetColors(new Color(0, 0, 0, 200), new Color(40, 40, 40, 200));
            _pixelTexture = new Texture2D(uiManager.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public override void Draw()
        {
            if (IsSelected)
            {
                UiManager.SpriteBatch.Draw(_pixelTexture, Bounds, _selectedColor);
            }
            base.Draw();
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