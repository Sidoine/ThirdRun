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
        private List<InventorySlot> inventorySlots = new();

        public InventoryPanel(UiManager uiManager,  Rectangle bounds)
            : base(uiManager, bounds)
        {
            LoadItemIcons();
            CreateInventorySlots();
        }

        private void LoadItemIcons()
        {
            // Load all available item images using the ItemTemplateRepository
            foreach (string imagePath in ItemTemplateRepository.GetAllImagePaths())
            {
                try
                {
                    var texture = UiManager.ContentManager.Load<Texture2D>(imagePath);
                    // Store by the image path for lookup
                    itemIcons[imagePath] = texture;
                }
                catch
                {
                    // Image not available, will use text fallback
                }
            }
        }

        private void CreateInventorySlots()
        {
            // Create inventory slots for a 4x10 grid
            var character = UiManager.GameState.Player.Characters.First();
            
            for (int y = 0; y < character.Inventory.MaxGridHeight; y++)
            {
                for (int x = 0; x < ItemsPerRow; x++)
                {
                    var coordinates = new Point(x, y);
                    var slotBounds = GetSlotBounds(coordinates);
                    
                    var slot = new InventorySlot(UiManager, slotBounds, character, coordinates, itemIcons);
                    inventorySlots.Add(slot);
                    AddChild(slot);
                    
                    // Register with the drag and drop manager
                    UiManager.DragAndDropManager.RegisterDropTarget(slot);
                }
            }
        }

        private Rectangle GetSlotBounds(Point coordinates)
        {
            int x0 = Bounds.X + PanelPadding;
            int y0 = Bounds.Y + PanelPadding + 30; // +30 for title space
            
            int x = x0 + coordinates.X * (ItemSize + ItemSpacing);
            int y = y0 + coordinates.Y * (ItemSize + ItemSpacing) - scrollOffset;
            
            return new Rectangle(x, y, ItemSize, ItemSize);
        }

        private void UpdateSlotBounds()
        {
            foreach (var slot in inventorySlots)
            {
                var newBounds = GetSlotBounds(slot.SlotCoordinates);
                slot.UpdateBounds(newBounds);
            }
        }

        public override bool HandleScroll(Point mousePosition, int scrollValue)
        {
            if (!Visible || !Bounds.Contains(mousePosition)) return false;

            scrollOffset -= scrollValue / 120 * (ItemSize + ItemSpacing);
            scrollOffset = Math.Clamp(scrollOffset, 0, maxScroll);
            
            // Update slot bounds after scrolling
            UpdateSlotBounds();
            return true;
        }

        public override void UpdateHover(Point mousePosition)
        {
            // Update the drag and drop manager with mouse position
            UiManager.DragAndDropManager.UpdateMousePosition(mousePosition);
            base.UpdateHover(mousePosition);
        }

        public override bool HandleMouseUp(Point mousePosition)
        {
            if (!Visible) return false;

            return base.HandleMouseUp(mousePosition);
        }

        public override bool Visible { get => UiManager.CurrentState.IsInventoryVisible ; set => UiManager.CurrentState.IsInventoryVisible = value; }

        public override void Draw()
        {
            // Affichage du panneau d'inventaire si visible
            if (Visible)
            {
                // Update slot bounds in case of scrolling
                UpdateSlotBounds();
                
                // Fond du panneau avec transparence
                UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), Bounds, Color.Black * 0.8f);
                
                // Bordure du panneau
                DrawPanelBorder();

                // Titre du panneau
                var font = UiManager.FontSystem.GetFont(18);
                UiManager.SpriteBatch.DrawString(font, "Inventaire", 
                    new Vector2(Bounds.X + PanelPadding, Bounds.Y + 8), Color.White);

                // Zone de clipping pour ne pas dessiner en dehors du panneau
                var originalScissorRect = UiManager.GraphicsDevice.ScissorRectangle;
                UiManager.GraphicsDevice.ScissorRectangle = new Rectangle(
                    Bounds.X + 2, 
                    Bounds.Y + 30 + 2, 
                    Bounds.Width - 4, 
                    Bounds.Height - 30 - 4);

                // Draw child components (inventory slots)
                foreach (var child in Children)
                {
                    child.Draw();
                }
                
                // Restaurer le rectangle de clipping
                UiManager.GraphicsDevice.ScissorRectangle = originalScissorRect;
                
                // Draw dragged item following mouse cursor (outside clipping area)
                var dragManager = UiManager.DragAndDropManager;
                if (dragManager.IsDragging && dragManager.DraggedItem != null)
                {
                    Rectangle dragRect = new Rectangle(
                        dragManager.CurrentMousePosition.X - dragManager.DragOffset.X,
                        dragManager.CurrentMousePosition.Y - dragManager.DragOffset.Y,
                        ItemSize,
                        ItemSize);
                    
                    // Draw with semi-transparency to indicate dragging
                    DrawItemAt(dragManager.DraggedItem, dragRect, 0.7f);
                }
                
                // Update scroll limits and draw scrollbar
                UpdateScrollLimits();
                if (maxScroll > 0)
                {
                    DrawScrollbar();
                }
            }
        }

        private void DrawPanelBorder()
        {
            int borderThickness = 2;
            var borderColor = Color.Gray;
            var pixel = ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice);
            
            // Top border
            UiManager.SpriteBatch.Draw(pixel, 
                new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, borderThickness), borderColor);
            // Bottom border
            UiManager.SpriteBatch.Draw(pixel, 
                new Rectangle(Bounds.X, Bounds.Bottom - borderThickness, Bounds.Width, borderThickness), borderColor);
            // Left border
            UiManager.SpriteBatch.Draw(pixel, 
                new Rectangle(Bounds.X, Bounds.Y, borderThickness, Bounds.Height), borderColor);
            // Right border
            UiManager.SpriteBatch.Draw(pixel, 
                new Rectangle(Bounds.Right - borderThickness, Bounds.Y, borderThickness, Bounds.Height), borderColor);
        }

        private void UpdateScrollLimits()
        {
            // Calculate scroll limits based on inventory content
            var character = UiManager.GameState.Player.Characters.First();
            var itemsWithCoords = character.Inventory.GetItemsWithCoordinates();
            int maxY = itemsWithCoords.Keys.Count > 0 ? itemsWithCoords.Keys.Max(coord => coord.Y) : 0;
            int contentHeight = (maxY + 1) * (ItemSize + ItemSpacing);
            int availableHeight = Bounds.Height - 30 - 2 * PanelPadding; // -30 pour le titre
            maxScroll = Math.Max(0, contentHeight - availableHeight);
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

        private void DrawItemAt(Item item, Rectangle itemRect, float alpha = 1.0f)
        {
            // Fond de l'item
            UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                itemRect, Color.DarkGray * (0.7f * alpha));
            
            // Bordure de l'item
            var itemBorderColor = Color.LightGray * alpha;
            var pixel = ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice);
            // Top
            UiManager.SpriteBatch.Draw(pixel, 
                new Rectangle(itemRect.X, itemRect.Y, itemRect.Width, 1), itemBorderColor);
            // Bottom
            UiManager.SpriteBatch.Draw(pixel, 
                new Rectangle(itemRect.X, itemRect.Bottom - 1, itemRect.Width, 1), itemBorderColor);
            // Left
            UiManager.SpriteBatch.Draw(pixel, 
                new Rectangle(itemRect.X, itemRect.Y, 1, itemRect.Height), itemBorderColor);
            // Right
            UiManager.SpriteBatch.Draw(pixel, 
                new Rectangle(itemRect.Right - 1, itemRect.Y, 1, itemRect.Height), itemBorderColor);

            // Try to display the item icon using the item's ImagePath first
            bool iconDrawn = false;
            string? imagePath = item.ImagePath;
            
            // If item doesn't have an ImagePath, fall back to the mapper (for backward compatibility)
            if (imagePath == null)
            {
                imagePath = ThirdRun.Utils.ItemImageMapper.GetImagePath(item.Name);
            }
            
            if (imagePath != null && itemIcons.TryGetValue(imagePath, out Texture2D? iconTexture))
            {
                var iconRect = new Rectangle(itemRect.X + 4, itemRect.Y + 4, ItemSize - 8, ItemSize - 8);
                UiManager.SpriteBatch.Draw(iconTexture, iconRect, Color.White * alpha);
                iconDrawn = true;
            }
            
            // Si pas d'icône, afficher le nom de l'item
            if (!iconDrawn)
            {
                var itemFont = UiManager.FontSystem.GetFont(10);
                var textSize = itemFont.MeasureString(item.Name);
                var textRect = new Rectangle(itemRect.X + 2, itemRect.Y + 2, ItemSize - 4, ItemSize - 4);
                
                // Dessiner le texte centré dans le rectangle
                var textPos = new Vector2(
                    textRect.X + (textRect.Width - textSize.X) / 2,
                    textRect.Y + (textRect.Height - textSize.Y) / 2);
                
                UiManager.SpriteBatch.DrawString(itemFont, item.Name, textPos, Color.White * alpha);
            }
        }
    }
}
