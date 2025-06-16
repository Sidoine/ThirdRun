using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;
using System.Collections.Generic;
using System;
using ThirdRun.Characters;

namespace ThirdRun.Characters
{
    public class InventoryPanel
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
        private bool isScrolling = false;
        private int lastMouseY = 0;

        public bool Visible { get; set; } = true;
        public Rectangle PanelBounds { get; private set; }
        public Rectangle ButtonBounds { get; private set; }

        public InventoryPanel(int screenHeight, int screenWidth)
        {
            PanelBounds = new Rectangle(0, 0, PanelWidth, screenHeight);
            ButtonBounds = new Rectangle(screenWidth - ButtonSize - ButtonMargin, screenHeight - ButtonSize - ButtonMargin, ButtonSize, ButtonSize);
        }

        public void Update(GameTime gameTime, int screenHeight, int screenWidth, Inventory inventory, MouseState mouse, MouseState previousMouse)
        {
            // Gestion du scroll à la molette (seulement si visible)
            if (Visible && PanelBounds.Contains(mouse.Position))
            {
                int wheel = mouse.ScrollWheelValue;
                if (wheel != 0)
                {
                    scrollOffset -= (wheel / 120) * (ItemSize + ItemSpacing);
                    scrollOffset = Math.Clamp(scrollOffset, 0, maxScroll);
                }
            }
            // Gestion du bouton (toujours visible, clic au relâchement)
            if (previousMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released && ButtonBounds.Contains(mouse.Position))
            {
                Visible = !Visible;
            }
        }

        public void Toggle()
        {
            Visible = !Visible;
        }

        public void Draw(SpriteBatch spriteBatch, DynamicSpriteFont font, Inventory inventory, Dictionary<string, Texture2D> itemIcons, int screenHeight)
        {
            // Affichage du panneau d'inventaire si visible
            if (Visible)
            {
                spriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(spriteBatch.GraphicsDevice), PanelBounds, Color.Black * 0.8f);
                // Affichage des objets
                var items = inventory.GetItems();
                int x0 = PanelBounds.X + PanelPadding;
                int y0 = PanelBounds.Y + PanelPadding - scrollOffset;
                int col = 0, row = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    int x = x0 + col * (ItemSize + ItemSpacing);
                    int y = y0 + row * (ItemSize + ItemSpacing);
                    Rectangle itemRect = new Rectangle(x, y, ItemSize, ItemSize);
                    // Icône ou nom
                    if (itemIcons != null && itemIcons.TryGetValue(items[i].Name, out var icon))
                    {
                        spriteBatch.Draw(icon, itemRect, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(spriteBatch.GraphicsDevice), itemRect, Color.Gray * 0.5f);
                        spriteBatch.DrawString(font, items[i].Name, new Vector2(x + 4, y + 4), Color.White);
                    }
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
                maxScroll = Math.Max(0, contentHeight - (screenHeight - 2 * PanelPadding));
            }
            // Bouton d'ouverture/fermeture (toujours visible)
            spriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(spriteBatch.GraphicsDevice), ButtonBounds, Color.DarkRed);
            string label = Visible ? "X" : ">";
            spriteBatch.DrawString(font, label, new Vector2(ButtonBounds.X + 12, ButtonBounds.Y + 6), Color.White);
        }
    }
}
