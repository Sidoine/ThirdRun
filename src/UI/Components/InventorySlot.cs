using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using ThirdRun.Items;
using ThirdRun.Characters;
using System.Collections.Generic;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Represents an individual inventory slot that can accept dropped items
    /// </summary>
    public class InventorySlot : UIElement, IDropTarget
    {
        private const int ItemSize = 64;
        private Dictionary<string, Texture2D> itemIcons;
        private Character character;
        private Point slotCoordinates;

        public Point SlotCoordinates => slotCoordinates;
        public Item? Item => character.Inventory.GetItemAt(slotCoordinates);
        public bool IsEmpty => character.Inventory.IsSlotEmpty(slotCoordinates);

        public InventorySlot(UiManager uiManager, Rectangle bounds, Character character, Point coordinates, Dictionary<string, Texture2D> itemIcons)
            : base(uiManager, bounds)
        {
            this.character = character;
            this.slotCoordinates = coordinates;
            this.itemIcons = itemIcons;
        }

        public override bool HandleMouseDown(Point mousePosition)
        {
            if (!Visible || !Bounds.Contains(mousePosition)) return false;

            var item = Item;
            if (item != null)
            {
                // Start dragging the item from this slot
                var dragManager = UiManager.DragAndDropManager;
                return dragManager.StartDrag(item, slotCoordinates, mousePosition, Bounds.Location, this);
            }

            return false;
        }

        public override bool HandleMouseRightClick(Point mousePosition)
        {
            if (!Visible || !Bounds.Contains(mousePosition)) return false;

            var item = Item;
            if (item is Equipment equipment)
            {
                bool equipped = character.Inventory.EquipItem(equipment);
                if (equipped)
                {
                    // Remove the item from inventory since it's now equipped
                    character.Inventory.RemoveItem(equipment);
                }
                return true;
            }

            return false;
        }

        #region IDropTarget Implementation

        public bool CanAcceptDrop(Item item, Point position)
        {
            return Visible && Bounds.Contains(position);
        }

        public bool HandleDrop(Item item, Point position, Point sourceCoordinates)
        {
            if (!CanAcceptDrop(item, position)) return false;

            // Move or swap item in inventory
            return character.Inventory.MoveItem(sourceCoordinates, slotCoordinates);
        }

        public Rectangle GetDropBounds()
        {
            return Bounds;
        }

        #endregion

        public override void Draw()
        {
            if (!Visible) return;

            var item = Item;
            var dragManager = UiManager.DragAndDropManager;
            
            // Skip drawing if this item is being dragged
            if (item != null && !dragManager.ShouldShowDraggedItemAtPosition(item, slotCoordinates))
                return;

            DrawSlotBackground();

            if (item != null)
            {
                DrawItemAt(item, Bounds);
            }
        }

        private void DrawSlotBackground()
        {
            // Fond de la slot
            UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                Bounds, Color.DarkGray * 0.7f);
            
            // Bordure de la slot
            var itemBorderColor = Color.LightGray;
            // Top
            UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1), itemBorderColor);
            // Bottom
            UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                new Rectangle(Bounds.X, Bounds.Bottom - 1, Bounds.Width, 1), itemBorderColor);
            // Left
            UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                new Rectangle(Bounds.X, Bounds.Y, 1, Bounds.Height), itemBorderColor);
            // Right
            UiManager.SpriteBatch.Draw(ThirdRun.Utils.Helpers.GetPixel(UiManager.GraphicsDevice), 
                new Rectangle(Bounds.Right - 1, Bounds.Y, 1, Bounds.Height), itemBorderColor);
        }

        private void DrawItemAt(Item item, Rectangle itemRect, float alpha = 1.0f)
        {
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