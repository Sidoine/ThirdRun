using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using System.Collections.Generic;
using ThirdRun.UI.Components;

namespace ThirdRun.UI
{
    public class IconBar : Container
    {
        private readonly List<Button> _iconButtons = new List<Button>();
        private readonly Color _barColor = new Color(0, 0, 0, 180);
        private readonly BorderedTextureRenderer _barRenderer;
        private readonly DynamicSpriteFont _font;
        private const int ICON_SIZE = 32;
        private const int ICON_PADDING = 5;

        public IconBar(UiManager uiManager, Rectangle bounds)
            : base(uiManager, bounds)
        {
            _font = uiManager.FontSystem.GetFont(16);
            var tm = uiManager.ContentManager;
            var main = tm.Load<Texture2D>("UI/Bar/barre");
            var left = tm.Load<Texture2D>("UI/Bar/ornement-left");
            var right = tm.Load<Texture2D>("UI/Bar/ornement-right");
            _barRenderer = new BorderedTextureRenderer
            {
                Background = main,
                LeftBorder = left,
                LeftBorderMode = RepeatMode.Ratio,
                RightBorder = right,
                RightBorderMode = RepeatMode.Ratio,
            };
        }

        public void AddIconButton(string text, System.Action onClick, string? tooltipText = null)
        {
            int x = Bounds.Left + ICON_PADDING + (_iconButtons.Count * (ICON_SIZE + ICON_PADDING));
            var buttonRect = new Rectangle(x, Bounds.Top + ICON_PADDING, ICON_SIZE, ICON_SIZE);
            var button = new Button(UiManager, buttonRect, onClick, text, _font);
            button.SetColors(new Color(40, 40, 40, 200), new Color(60, 60, 60, 200));
            var tooltip = new ToolTip(UiManager, button, tooltipText ?? text, _font);
            _iconButtons.Add(button);
            AddChild(tooltip);
        }

        public override void Draw()
        {
            if (!Visible) return;

            // Dessiner le fond de la barre avec textures
            _barRenderer.Draw(UiManager.SpriteBatch, Bounds);

            base.Draw();
        }
    }
}