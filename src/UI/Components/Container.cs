using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Classe de base pour les containers d'interface utilisateur qui peuvent contenir des éléments interactifs
    /// </summary>
    public class Container : UIElement
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
    }
}