using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FontStashSharp;
using System;
using System.Collections.Generic;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Container permettant de faire défiler son contenu
    /// </summary>
    public class ScrollableContainer : Container
    {
        private const int ScrollSpeed = 1;
        private int _scrollOffset = 0;
        private int _contentHeight = 0;
        private Texture2D _scrollbarTexture;
        private int? _step;

        public ScrollableContainer(UiManager uiManager, Rectangle bounds, int? step = null)
            : base(uiManager, bounds)
        {
            _step = step;
            _scrollbarTexture = CreatePixelTexture(UiManager.GraphicsDevice);
        }

        private Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
        {
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }

        /// <summary>
        /// Efface tous les éléments du container
        /// </summary>
        public void Clear()
        {
            Children.Clear();
            _contentHeight = 0;
            _scrollOffset = 0;
        }

        /// <summary>
        /// Calcule la hauteur totale du contenu
        /// </summary>
        private void UpdateContentHeight()
        {
            _contentHeight = 0;
            foreach (var child in Children)
            {
                int childBottom = child.Bounds.Y + child.Bounds.Height;
                _contentHeight = Math.Max(_contentHeight, childBottom);
            }
        }

        public override void AddChild(UIElement child)
        {
            base.AddChild(child);
            UpdateContentHeight();
        }

        public override void RemoveChild(UIElement child)
        {
            base.RemoveChild(child);
            UpdateContentHeight();
        }

        public override void Update(GameTime gameTime)
        {
            // Mise à jour des enfants avec les positions ajustées
            foreach (var child in Children)
            {   // Ne mettre à jour que les enfants visibles
                if (Bounds.Intersects(child.Bounds))
                {
                    child.Update(gameTime);
                }
            }
        }

        public override bool HandleScroll(Point mousePosition, int scrollValue)
        {
            if (!Visible || _contentHeight <= Bounds.Height)
                return false;

            int scrollChange = -scrollValue / ScrollSpeed;
         
            int maxScroll = Math.Max(0, _contentHeight - Bounds.Height);
            if (_step != null)
            {
                if (scrollChange < 0)
                {
                    scrollChange = _scrollOffset > 0 ? -_step.Value : 0;
                }
                else
                {
                    scrollChange = _scrollOffset < maxScroll ? _step.Value : 0;
                }
            }
            else 
            {
                scrollChange = Math.Clamp(scrollChange, -_scrollOffset, maxScroll - _scrollOffset);
            }
            
            _scrollOffset += scrollChange;
            foreach (var child in Children)
            {
                child.Scroll(-scrollChange);
            }
            return true;
        }

        public override void Draw()
        {
            if (!Visible)
                return;

            // Dessiner uniquement les enfants visibles
            foreach (var child in Children)
            {
                // Ne dessiner que si l'enfant est visible
                if (Bounds.Intersects(child.Bounds))
                {
                    // Dessiner l'enfant
                    child.Draw();
                }
            }

            // Dessiner la scrollbar si nécessaire
            if (_contentHeight > Bounds.Height)
            {
                int scrollbarWidth = 8;
                int scrollbarHeight = (int)(Bounds.Height * Bounds.Height / (float)_contentHeight);
                int scrollbarY = (int)(Bounds.Y + (_scrollOffset / (float)_contentHeight) * Bounds.Height);
                
                Rectangle scrollbarRect = new Rectangle(
                    Bounds.Right - scrollbarWidth - 2,
                    scrollbarY,
                    scrollbarWidth,
                    scrollbarHeight);
                
                UiManager.SpriteBatch.Draw(_scrollbarTexture, scrollbarRect, new Color(150, 150, 150, 150));
            }
        }
    }
}