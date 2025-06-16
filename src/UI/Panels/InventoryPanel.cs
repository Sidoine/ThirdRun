using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;
using System.Collections.Generic;
using System;
using ThirdRun.Characters;
using ThirdRun.UI.Components;
using ThirdRun.UI;
using System.Linq;

namespace ThirdRun.UI.Panels
{
    public class InventoryPanel : Container
    {
        private const int PanelWidth = 320;
        private const int PanelPadding = 16;
        private const int ItemSize = 64;
        private const int ItemSpacing = 8;
        private const int ItemsPerRow = 4;
        private const int ButtonSize = 40;
        private const int ButtonMargin = 16;
        private int scrollOffset = 0;
        private int maxScroll = 0;

        public InventoryPanel(UiManager uiManager,  Rectangle bounds)
            : base(uiManager, bounds)
        {
        }

        public void Update(GameTime gameTime, int screenHeight, int screenWidth, Inventory inventory, MouseState mouse, MouseState previousMouse)
        {
            // Gestion du scroll à la molette (seulement si visible)
            if (Visible && Bounds.Contains(mouse.Position))
            {
                int wheel = mouse.ScrollWheelValue;
                if (wheel != 0)
                {
                    scrollOffset -= (wheel / 120) * (ItemSize + ItemSpacing);
                    scrollOffset = Math.Clamp(scrollOffset, 0, maxScroll);
                }
            }
        }

        public override bool HandleMouseClick(Point mousePosition)
        {
            return base.HandleMouseClick(mousePosition);
        }

        public override bool Visible { get => UiManager.CurrentState.IsInventoryVisible ; set => UiManager.CurrentState.IsInventoryVisible = value; }

        public override void Draw()
        {
            // Affichage du panneau d'inventaire si visible
            if (Visible)
            {
                UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), Bounds, Color.Black * 0.8f);
                // Affichage des objets
                var items = UiManager.GameState.Player.Characters.First().Inventory.GetItems();
                int x0 = Bounds.X + PanelPadding;
                int y0 = Bounds.Y + PanelPadding - scrollOffset;
                int col = 0, row = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    int x = x0 + col * (ItemSize + ItemSpacing);
                    int y = y0 + row * (ItemSize + ItemSpacing);
                    Rectangle itemRect = new Rectangle(x, y, ItemSize, ItemSize);
                    UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), itemRect, Color.Gray * 0.5f);
                    col++;
                    if (col >= ItemsPerRow)
                    {
                        col = 0;
                        row++;
                    }
                }
                // Calcul du scroll max
                int totalRows = (int)Math.Ceiling(items.Count / (float)ItemsPerRow);
                int contentHeight = totalRows * (ItemSize + ItemSpacing);
                maxScroll = Math.Max(0, contentHeight - (Bounds.Bottom - 2 * PanelPadding));
            }
        }
    }
}
