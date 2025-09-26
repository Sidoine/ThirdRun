using Microsoft.Xna.Framework;
using ThirdRun.Items;
using System.Collections.Generic;
using System.Linq;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Central manager for drag and drop operations in the UI system
    /// </summary>
    public class DragAndDropManager
    {
        private bool isDragging = false;
        private Item? draggedItem = null;
        private Point draggedItemOriginalCoords;
        private Point dragOffset;
        private Point currentMousePosition;
        private IDropTarget? sourceDropTarget = null;
        private List<IDropTarget> dropTargets = new();

        public bool IsDragging => isDragging;
        public Item? DraggedItem => draggedItem;
        public Point CurrentMousePosition => currentMousePosition;
        public Point DragOffset => dragOffset;

        /// <summary>
        /// Register a drop target with the manager
        /// </summary>
        public void RegisterDropTarget(IDropTarget dropTarget)
        {
            if (!dropTargets.Contains(dropTarget))
            {
                dropTargets.Add(dropTarget);
            }
        }

        /// <summary>
        /// Unregister a drop target from the manager
        /// </summary>
        public void UnregisterDropTarget(IDropTarget dropTarget)
        {
            dropTargets.Remove(dropTarget);
        }

        /// <summary>
        /// Start dragging an item
        /// </summary>
        public bool StartDrag(Item item, Point itemOriginalCoords, Point mousePosition, Point itemBounds, IDropTarget source)
        {
            if (isDragging) return false;

            isDragging = true;
            draggedItem = item;
            draggedItemOriginalCoords = itemOriginalCoords;
            dragOffset = new Point(mousePosition.X - itemBounds.X, mousePosition.Y - itemBounds.Y);
            currentMousePosition = mousePosition;
            sourceDropTarget = source;
            return true;
        }

        /// <summary>
        /// Update the current mouse position during drag
        /// </summary>
        public void UpdateMousePosition(Point mousePosition)
        {
            currentMousePosition = mousePosition;
        }

        /// <summary>
        /// Attempt to drop the currently dragged item
        /// </summary>
        public bool TryDrop(Point mousePosition)
        {
            if (!isDragging || draggedItem == null) return false;

            // Find the best drop target at the mouse position
            IDropTarget? targetDropTarget = FindDropTargetAtPosition(mousePosition);
            
            bool dropHandled = false;
            if (targetDropTarget != null && targetDropTarget.CanAcceptDrop(draggedItem, mousePosition))
            {
                dropHandled = targetDropTarget.HandleDrop(draggedItem, mousePosition, draggedItemOriginalCoords);
            }

            // End the drag operation
            EndDrag();
            return dropHandled;
        }

        /// <summary>
        /// Cancel the current drag operation
        /// </summary>
        public void CancelDrag()
        {
            EndDrag();
        }

        /// <summary>
        /// End the drag operation
        /// </summary>
        private void EndDrag()
        {
            isDragging = false;
            draggedItem = null;
            sourceDropTarget = null;
        }

        /// <summary>
        /// Find the most appropriate drop target at the given position
        /// </summary>
        private IDropTarget? FindDropTargetAtPosition(Point position)
        {
            // Sort drop targets by area (smallest first) to get most specific target
            var candidates = dropTargets
                .Where(dt => dt.GetDropBounds().Contains(position))
                .OrderBy(dt => dt.GetDropBounds().Width * dt.GetDropBounds().Height);

            return candidates.FirstOrDefault();
        }

        /// <summary>
        /// Check if the dragged item should be shown at a specific position
        /// </summary>
        public bool ShouldShowDraggedItemAtPosition(Item item, Point coordinates)
        {
            return !(isDragging && item == draggedItem && coordinates == draggedItemOriginalCoords);
        }
    }
}