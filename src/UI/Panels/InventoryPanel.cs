using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;
using System.Collections.Generic;
using System;
using ThirdRun.Characters;
using ThirdRun.Items;
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

        private Dictionary<string, Texture2D> itemIcons = new();
        private List<(Rectangle rect, Item item)> itemRectangles = new();

        public InventoryPanel(UiManager uiManager,  Rectangle bounds)
            : base(uiManager, bounds)
        {
            LoadItemIcons();
        }

        private void LoadItemIcons()
        {
            // Try to load common item icons, if they exist
            // This is optional - if icons don't exist, we'll show names instead
            
            // Try to load each icon individually, catching exceptions for each one
            try
            {
                itemIcons["Potion"] = UiManager.ContentManager.Load<Texture2D>("Items/potion");
            }
            catch
            {
                // Potion icon not available, will use text fallback
            }
            
            try
            {
                itemIcons["Sword"] = UiManager.ContentManager.Load<Texture2D>("Items/sword");
            }
            catch
            {
                // Sword icon not available, will use text fallback
            }
            
            try
            {
                itemIcons["Armor"] = UiManager.ContentManager.Load<Texture2D>("Items/armor");
            }
            catch
            {
                // Armor icon not available, will use text fallback
            }
        }

        public override bool HandleScroll(Point mousePosition, int scrollValue)
        {
            if (!Visible || !Bounds.Contains(mousePosition)) return false;

            scrollOffset -= scrollValue / 120 * (ItemSize + ItemSpacing);
            scrollOffset = Math.Clamp(scrollOffset, 0, maxScroll);
            return true;
        }

        public override bool HandleMouseClick(Point mousePosition)
        {
            return base.HandleMouseClick(mousePosition);
        }

        public override bool HandleMouseRightClick(Point mousePosition)
        {
            if (!Visible || !Bounds.Contains(mousePosition)) return false;

            // Check if right-click hit any item
            foreach (var (rect, item) in itemRectangles)
            {
                if (rect.Contains(mousePosition))
                {
                    // Try to equip the item if it's equipment
                    if (item is Equipment equipment)
                    {
                        var character = UiManager.GameState.Player.Characters.First();
                        bool equipped = character.Inventory.EquipItem(equipment);
                        if (equipped)
                        {
                            // Remove the item from inventory since it's now equipped
                            // The Character.Equip method handles swapping any existing equipment back to inventory
                            character.Inventory.GetItems().Remove(equipment);
                        }
                        return true;
                    }
                }
            }

            return base.HandleMouseRightClick(mousePosition);
        }

        public override bool Visible { get => UiManager.CurrentState.IsInventoryVisible ; set => UiManager.CurrentState.IsInventoryVisible = value; }

        public override void Draw()
        {
            // Affichage du panneau d'inventaire si visible
            if (Visible)
            {
                // Fond du panneau avec transparence
                UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), Bounds, Color.Black * 0.8f);
                
                // Bordure du panneau
                int borderThickness = 2;
                var borderColor = Color.Gray;
                // Top border
                UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                    new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, borderThickness), borderColor);
                // Bottom border
                UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                    new Rectangle(Bounds.X, Bounds.Bottom - borderThickness, Bounds.Width, borderThickness), borderColor);
                // Left border
                UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                    new Rectangle(Bounds.X, Bounds.Y, borderThickness, Bounds.Height), borderColor);
                // Right border
                UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                    new Rectangle(Bounds.Right - borderThickness, Bounds.Y, borderThickness, Bounds.Height), borderColor);

                // Titre du panneau
                var font = UiManager.FontSystem.GetFont(18);
                UiManager.SpriteBatch.DrawString(font, "Inventaire", 
                    new Vector2(Bounds.X + PanelPadding, Bounds.Y + 8), Color.White);

                // Affichage des objets
                var items = UiManager.GameState.Player.Characters.First().Inventory.GetItems();
                int x0 = Bounds.X + PanelPadding;
                int y0 = Bounds.Y + PanelPadding + 30 - scrollOffset; // +30 pour l'espace du titre
                int col = 0, row = 0;
                
                // Clear item rectangles for this frame
                itemRectangles.Clear();
                
                // Zone de clipping pour ne pas dessiner en dehors du panneau
                var originalScissorRect = UiManager.GraphicsDevice.ScissorRectangle;
                UiManager.GraphicsDevice.ScissorRectangle = new Rectangle(
                    Bounds.X + borderThickness, 
                    Bounds.Y + 30 + borderThickness, 
                    Bounds.Width - 2 * borderThickness, 
                    Bounds.Height - 30 - 2 * borderThickness);

                for (int i = 0; i < items.Count; i++)
                {
                    int x = x0 + col * (ItemSize + ItemSpacing);
                    int y = y0 + row * (ItemSize + ItemSpacing);
                    Rectangle itemRect = new Rectangle(x, y, ItemSize, ItemSize);
                    
                    // Ne dessiner que si visible dans la zone du panneau
                    if (itemRect.Bottom >= Bounds.Y + 30 && itemRect.Top < Bounds.Bottom)
                    {
                        var item = items[i];
                        
                        // Store item rectangle for click detection
                        itemRectangles.Add((itemRect, item));
                        
                        // Fond de l'item
                        UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                            itemRect, Color.DarkGray * 0.7f);
                        
                        // Bordure de l'item
                        var itemBorderColor = Color.LightGray;
                        // Top
                        UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                            new Rectangle(itemRect.X, itemRect.Y, itemRect.Width, 1), itemBorderColor);
                        // Bottom
                        UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                            new Rectangle(itemRect.X, itemRect.Bottom - 1, itemRect.Width, 1), itemBorderColor);
                        // Left
                        UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                            new Rectangle(itemRect.X, itemRect.Y, 1, itemRect.Height), itemBorderColor);
                        // Right
                        UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                            new Rectangle(itemRect.Right - 1, itemRect.Y, 1, itemRect.Height), itemBorderColor);

                        // Essayer d'afficher l'icône de l'item
                        bool iconDrawn = false;
                        if (itemIcons.ContainsKey(item.Name))
                        {
                            var iconTexture = itemIcons[item.Name];
                            var iconRect = new Rectangle(x + 4, y + 4, ItemSize - 8, ItemSize - 8);
                            UiManager.SpriteBatch.Draw(iconTexture, iconRect, Color.White);
                            iconDrawn = true;
                        }
                        
                        // Si pas d'icône, afficher le nom de l'item
                        if (!iconDrawn)
                        {
                            var itemFont = UiManager.FontSystem.GetFont(10);
                            var textSize = itemFont.MeasureString(item.Name);
                            var textRect = new Rectangle(x + 2, y + 2, ItemSize - 4, ItemSize - 4);
                            
                            // Dessiner le texte centré dans le rectangle
                            var textPos = new Vector2(
                                textRect.X + (textRect.Width - textSize.X) / 2,
                                textRect.Y + (textRect.Height - textSize.Y) / 2);
                            
                            UiManager.SpriteBatch.DrawString(itemFont, item.Name, textPos, Color.White);
                        }
                    }
                    
                    col++;
                    if (col >= ItemsPerRow)
                    {
                        col = 0;
                        row++;
                    }
                }
                
                // Restaurer le rectangle de clipping
                UiManager.GraphicsDevice.ScissorRectangle = originalScissorRect;
                
                // Calcul du scroll max
                int totalRows = (int)Math.Ceiling(items.Count / (float)ItemsPerRow);
                int contentHeight = totalRows * (ItemSize + ItemSpacing);
                int availableHeight = Bounds.Height - 30 - 2 * PanelPadding; // -30 pour le titre
                maxScroll = Math.Max(0, contentHeight - availableHeight);
                
                // Afficher la barre de scroll si nécessaire
                if (maxScroll > 0)
                {
                    DrawScrollbar();
                }
            }
        }

        private void DrawScrollbar()
        {
            int scrollbarWidth = 8;
            int scrollbarX = Bounds.Right - scrollbarWidth - 4;
            int scrollbarY = Bounds.Y + 30 + 4;
            int scrollbarHeight = Bounds.Height - 30 - 8;
            
            // Fond de la scrollbar
            Rectangle scrollbarBg = new Rectangle(scrollbarX, scrollbarY, scrollbarWidth, scrollbarHeight);
            UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                scrollbarBg, Color.DarkGray * 0.5f);
            
            // Thumb de la scrollbar
            if (maxScroll > 0)
            {
                float scrollRatio = (float)scrollOffset / maxScroll;
                int thumbHeight = Math.Max(20, scrollbarHeight / 4);
                int thumbY = scrollbarY + (int)(scrollRatio * (scrollbarHeight - thumbHeight));
                
                Rectangle scrollbarThumb = new Rectangle(scrollbarX, thumbY, scrollbarWidth, thumbHeight);
                UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                    scrollbarThumb, Color.LightGray * 0.8f);
            }
        }
    }
}
