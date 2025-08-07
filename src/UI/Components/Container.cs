using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;
using ThirdRun.Items;
using System.Linq;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Classe de base pour les containers d'interface utilisateur qui peuvent contenir des éléments interactifs
    /// </summary>
    public class Container : UIElement, IDropTarget
    {
        protected List<UIElement> Children = new List<UIElement>();

        public Container(UiManager uiManager, Rectangle bounds)
            : base(uiManager, bounds)
        {
        }

        /// <summary>
        /// Ajoute un élément enfant au container
        /// </summary>
        public virtual void AddChild(UIElement child)
        {
            Children.Add(child);
        }

        /// <summary>
        /// Retire un élément enfant du container
        /// </summary>
        public virtual void RemoveChild(UIElement child)
        {
            Children.Remove(child);
        }

        /// <summary>
        /// Vérifie si le point spécifié est dans le container
        /// </summary>
        public override bool ContainsPoint(Point point)
        {
            return Visible && Bounds.Contains(point);
        }

        /// <summary>
        /// Met à jour l'état de survol de tous les éléments enfants
        /// </summary>
        public override void UpdateHover(Point mousePosition)
        {
            if (!Visible) return;

            foreach (var child in Children)
            {
                child.UpdateHover(mousePosition);
            }
        }

        /// <summary>
        /// Gère le clic de souris sur les éléments enfants du container
        /// </summary>
        public override bool HandleMouseClick(Point mousePosition)
        {
            if (!Visible) return false;

            foreach (var child in Children)
            {
                if (child.ContainsPoint(mousePosition))
                {
                    if (child.HandleMouseClick(mousePosition))
                        return true;
                }
            }

            return false;
        }

        public override bool HandleMouseDown(Point mousePosition)
        {
            if (!Visible) return false;

            foreach (var child in Children)
            {
                if (child.ContainsPoint(mousePosition))
                {
                    if (child.HandleMouseDown(mousePosition))
                        return true;
                }
            }

            return false;
        }

        public override bool HandleMouseUp(Point mousePosition)
        {
            if (!Visible) return false;

            foreach (var child in Children)
            {
                if (child.ContainsPoint(mousePosition))
                {
                    if (child.HandleMouseUp(mousePosition))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gère le clic droit de souris sur les éléments enfants du container
        /// </summary>
        public override bool HandleMouseRightClick(Point mousePosition)
        {
            if (!Visible) return false;

            foreach (var child in Children)
            {
                if (child is Container container)
                {
                    if (container.HandleMouseRightClick(mousePosition))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gère les touches du clavier pour les éléments enfants du container
        /// </summary>
        public override bool HandleKeyPress(Keys key)
        {
            if (!Visible) return false;

            foreach (var child in Children)
            {
                if (child is Container container)
                {
                    if (container.HandleKeyPress(key))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gère le défilement de la souris pour les éléments enfants du container
        /// </summary>
        public override bool HandleScroll(Point mousePosition, int scrollValue)
        {
            if (!Visible) return false;

            foreach (var child in Children)
            {
                if (child.ContainsPoint(mousePosition) && child.HandleScroll(mousePosition, scrollValue))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Dessine le container et tous ses éléments enfants
        /// </summary>
        public override void Draw()
        {
            if (!Visible) return;

            foreach (var child in Children)
            {
                child.Draw();
            }
        }

        /// <summary>
        /// Met à jour le container et tous ses éléments enfants
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (!Visible) return;

            foreach (var child in Children)
            {
                child.Update(gameTime);
            }
        }

        public override void Scroll(int scrollValue)
        {
            base.Scroll(scrollValue);

            foreach (var child in Children)
            {
                child.Scroll(scrollValue);
            }
        }

        /// <summary>
        /// Change la position et la taille du container
        /// </summary>
        public override void UpdateBounds(Rectangle newBounds)
        {
            Bounds = newBounds;
        }

        #region IDropTarget Implementation

        /// <summary>
        /// Check if this container or any of its children can accept the dropped item
        /// </summary>
        public virtual bool CanAcceptDrop(Item item, Point position)
        {
            if (!Visible || !Bounds.Contains(position)) return false;

            // Check children first (most specific targets)
            foreach (var child in Children.OfType<IDropTarget>())
            {
                if (child.CanAcceptDrop(item, position))
                    return true;
            }

            // Default container behavior - don't accept drops unless overridden
            return false;
        }

        /// <summary>
        /// Handle the drop by delegating to the most appropriate child drop target
        /// </summary>
        public virtual bool HandleDrop(Item item, Point position, Point sourceCoordinates)
        {
            if (!Visible || !Bounds.Contains(position)) return false;

            // Delegate to children first (most specific targets)
            // Sort by area to get most specific child first
            var childDropTargets = Children.OfType<IDropTarget>()
                .Where(dt => dt.CanAcceptDrop(item, position))
                .OrderBy(dt => dt.GetDropBounds().Width * dt.GetDropBounds().Height);

            foreach (var childTarget in childDropTargets)
            {
                if (childTarget.HandleDrop(item, position, sourceCoordinates))
                    return true;
            }

            // If no child handled it, container doesn't handle drops by default
            return false;
        }

        /// <summary>
        /// Get the bounds for drop target hit testing
        /// </summary>
        public virtual Rectangle GetDropBounds()
        {
            return Bounds;
        }

        #endregion
    }
}