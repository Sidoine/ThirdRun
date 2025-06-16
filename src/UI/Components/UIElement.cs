using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using FontStashSharp;

namespace ThirdRun.UI.Components
{
    public abstract class UIElement : IDisposable
    {
        public Rectangle Bounds;
        public virtual bool Visible { get; set; } = true;
        protected UiManager UiManager { get; }
        private bool _disposed;

        public UIElement(UiManager uiManager, Rectangle bounds)
        {
            this.UiManager = uiManager;
            Bounds = bounds;
        }

        public virtual bool ContainsPoint(Point point)
        {
            return Visible && Bounds.Contains(point);
        }

        public virtual void UpdateHover(Point mousePosition)
        {
            // À implémenter dans les classes dérivées si nécessaire
        }

        public virtual bool HandleMouseDown(Point mousePosition)
        {
            return false; // À implémenter dans les classes dérivées si nécessaire
        }

        public virtual bool HandleMouseUp(Point mousePosition)
        {
            return false; // À implémenter dans les classes dérivées si nécessaire
        }

        public virtual bool HandleMouseClick(Point mousePosition)
        {
            return false; // À implémenter dans les classes dérivées si nécessaire
        }

        public virtual bool HandleMouseRightClick(Point mousePosition)
        {
            return false; // À implémenter dans les classes dérivées si nécessaire
        }

        public virtual bool HandleKeyPress(Keys key)
        {
            return false; // À implémenter dans les classes dérivées si nécessaire
        }

        public virtual bool HandleScroll(Point mousePosition, int scrollValue)
        {
            return false; // À implémenter dans les classes dérivées si nécessaire
        }

        public virtual void Draw()
        {
            // À implémenter dans les classes dérivées
        }

        public virtual void Update(GameTime gameTime)
        {
            // À implémenter dans les classes dérivées si nécessaire
        }

        public virtual void UpdateBounds(Rectangle newBounds)
        {
            Bounds = newBounds;
        }

        public bool IsVisible
        {
            get => Visible;
            set => Visible = value;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Hide()
        {
            Visible = false;
        }

        public virtual void Show()
        {
            Visible = true;
        }

        public virtual void Scroll(int scrollValue)
        {
            Bounds.Y += scrollValue;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    UiManager.SpriteBatch.Dispose();
                }

                _disposed = true;
            }
        }

        ~UIElement()
        {
            Dispose(false);
        }
    }
}