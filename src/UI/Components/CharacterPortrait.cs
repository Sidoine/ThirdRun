using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdRun.Items;
using ThirdRun.Characters;
using ThirdRun.Data;
using ThirdRun.Utils;
using System.Linq;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Represents an individual character portrait that can accept equipment drops
    /// </summary>
    public class CharacterPortrait : SquareImageButton, IDropTarget
    {
        private Character character;
        
        // Resource bar display constants
        private const int BarHeight = 4;
        private const int BarWidth = 50;
        private const int BarSpacing = 2;
        private const int BarOffset = 8;
        
        // Resource colors - using colors defined in ResourceType
        private static readonly Color HealthBarColor = Color.LimeGreen;
        private static readonly Color HealthBarBackground = Color.DarkRed;

        public Character Character => character;

        public CharacterPortrait(UiManager uiManager, Rectangle bounds, Character character, Texture2D? texture, System.Action onClick)
            : base(uiManager, bounds, texture, onClick)
        {
            this.character = character;
            
            // Set colors for better visibility
            SetColors(
                new Color(40, 40, 40, 200), // Default: semi-transparent dark
                new Color(80, 120, 80, 220)  // Hover: semi-transparent green
            );
        }

        #region IDropTarget Implementation

        public bool CanAcceptDrop(Item item, Point position)
        {
            if (!Visible || !Bounds.Contains(position)) return false;
            
            // Only accept equipment items
            return item is Equipment;
        }

        public bool HandleDrop(Item item, Point position, Point sourceCoordinates)
        {
            if (!CanAcceptDrop(item, position)) return false;

            if (item is Equipment equipment)
            {
                bool equipped = character.Inventory.EquipItem(equipment);
                if (equipped)
                {
                    // Get the source character (usually the first character for now)
                    var sourceCharacter = UiManager.GameState.Player.Characters.First();
                    sourceCharacter.Inventory.RemoveItemAt(sourceCoordinates);
                    return true;
                }
            }

            return false;
        }

        public Rectangle GetDropBounds()
        {
            return Bounds;
        }

        #endregion

        public override void Draw()
        {
            // Draw the base portrait (button and icon)
            base.Draw();
            
            // Draw health and resource bars
            DrawResourceBars();
        }

        private void DrawResourceBars()
        {
            if (!Visible) return;
            
            // Calculate bar positions (below the portrait)
            int barStartY = Bounds.Bottom - BarOffset;
            int barX = Bounds.X + (Bounds.Width - BarWidth) / 2; // Center horizontally
            
            int barIndex = 0;
            
            // Draw health bar first
            DrawBar(barX, barStartY - ((barIndex + 1) * (BarHeight + BarSpacing)), 
                   character.CurrentHealth, character.GetEffectiveMaxHealth(), 
                   HealthBarColor, HealthBarBackground);
            barIndex++;
            
            // Draw bars for all resources
            foreach (var resourceType in character.Resources.GetAllResourceTypes())
            {
                var resource = character.Resources.GetResource(resourceType);
                if (resource != null)
                {
                    DrawBar(barX, barStartY - ((barIndex + 1) * (BarHeight + BarSpacing)), 
                           resource.CurrentValue, resource.MaxValue, 
                           resourceType.Color, Color.DarkGray);
                    barIndex++;
                }
            }
        }

        private void DrawBar(int x, int y, float currentValue, float maxValue, Color foregroundColor, Color backgroundColor)
        {
            if (maxValue <= 0) return;
            
            var pixel = Helpers.GetPixel(UiManager.GraphicsDevice);
            
            // Background rectangle (full bar)
            Rectangle bgRect = new Rectangle(x, y, BarWidth, BarHeight);
            
            // Foreground rectangle (filled portion)
            float percent = MathHelper.Clamp(currentValue / maxValue, 0f, 1f);
            Rectangle fgRect = new Rectangle(x, y, (int)(BarWidth * percent), BarHeight);
            
            // Draw background first, then foreground
            UiManager.SpriteBatch.Draw(pixel, bgRect, backgroundColor);
            if (percent > 0)
            {
                UiManager.SpriteBatch.Draw(pixel, fgRect, foregroundColor);
            }
        }
    }
}