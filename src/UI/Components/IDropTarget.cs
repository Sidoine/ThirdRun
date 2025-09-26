using Microsoft.Xna.Framework;
using ThirdRun.Items;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Interface for UI components that can receive dropped items
    /// </summary>
    public interface IDropTarget
    {
        /// <summary>
        /// Checks if the drop target can accept the given item at the specified position
        /// </summary>
        /// <param name="item">The item being dropped</param>
        /// <param name="position">The mouse position where the drop occurs</param>
        /// <returns>True if the item can be dropped, false otherwise</returns>
        bool CanAcceptDrop(Item item, Point position);

        /// <summary>
        /// Handles the drop of an item at the specified position
        /// </summary>
        /// <param name="item">The item being dropped</param>
        /// <param name="position">The mouse position where the drop occurs</param>
        /// <param name="sourceCoordinates">The original coordinates of the item (for swapping)</param>
        /// <returns>True if the drop was handled successfully, false otherwise</returns>
        bool HandleDrop(Item item, Point position, Point sourceCoordinates);

        /// <summary>
        /// Gets the bounds of this drop target for hit testing
        /// </summary>
        Rectangle GetDropBounds();
    }
}