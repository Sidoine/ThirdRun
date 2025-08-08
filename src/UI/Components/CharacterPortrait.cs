using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdRun.Items;
using ThirdRun.Characters;
using System.Linq;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Represents an individual character portrait that can accept equipment drops
    /// </summary>
    public class CharacterPortrait : SquareImageButton, IDropTarget
    {
        private Character character;

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
    }
}