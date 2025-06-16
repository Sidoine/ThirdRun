using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThirdRun.UI.Components
{
    public enum RepeatMode
    {
        Strech,
        Ratio,
        Repeat
    }

    /// <summary>
    /// Permet de dessiner une zone avec une texture répétée et des bordures optionnelles.
    /// </summary>
    public class BorderedTextureRenderer
    {
        public Texture2D? Background { get; set; }
        public Texture2D? LeftBorder { get; set; }
        public RepeatMode LeftBorderMode { get; set; } = RepeatMode.Strech;
        public Texture2D? RightBorder { get; set; }
        public RepeatMode RightBorderMode { get; set; } = RepeatMode.Strech;
        public Texture2D? TopBorder { get; set; }
        public RepeatMode TopBorderMode { get; set; } = RepeatMode.Strech;
        public Texture2D? BottomBorder { get; set; }
        public RepeatMode BottomBorderMode { get; set; } = RepeatMode.Strech;
        public Texture2D? TopLeftCorner { get; set; }
        public Texture2D? TopRightCorner { get; set; }
        public Texture2D? BottomLeftCorner { get; set; }
        public Texture2D? BottomRightCorner { get; set; }

        /// <summary>
        /// Dessine la zone texturée avec bordures.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Rectangle bounds)
        {
            var color = Color.White;
            int x = bounds.X;
            int y = bounds.Y;
            int width = bounds.Width;
            int height = bounds.Height;

            if (Background != null && width > 0)
            {
                spriteBatch.Draw(Background, bounds, color);
            }

            if (LeftBorder != null)
            {
                var leftRect = new Rectangle(x, y, LeftBorderMode == RepeatMode.Ratio ? LeftBorder.Width * height / LeftBorder.Height : LeftBorder.Width, height);
                spriteBatch.Draw(LeftBorder, leftRect, color);
            }

            if (RightBorder != null)
            {
                var borderWidth = LeftBorderMode == RepeatMode.Ratio ? RightBorder.Width * height / RightBorder.Height : RightBorder.Width;
                var rightRect = new Rectangle(bounds.Right - borderWidth, y, borderWidth, height);
                spriteBatch.Draw(RightBorder, rightRect, color);
            }

            if (TopBorder != null)
            {
                var topHeight = TopBorderMode == RepeatMode.Ratio ? TopBorder.Height * width / TopBorder.Width : TopBorder.Height;
                var topRect = new Rectangle(x, y, width, topHeight);
                spriteBatch.Draw(TopBorder, topRect, color);
            }

            if (BottomBorder != null)
            {
                var borderHeight = BottomBorderMode == RepeatMode.Ratio ? BottomBorder.Height * width / BottomBorder.Width : BottomBorder.Height;
                var bottomRect = new Rectangle(x, bounds.Bottom - borderHeight, width, borderHeight);
                spriteBatch.Draw(BottomBorder, bottomRect, color);
            }

            if (TopLeftCorner != null)
            {
                var cornerRect = new Rectangle(x, y, TopLeftCorner.Width, TopLeftCorner.Height);
                spriteBatch.Draw(TopLeftCorner, cornerRect, color);
            }

            if (TopRightCorner != null)
            {
                var cornerRect = new Rectangle(bounds.Right - TopRightCorner.Width, y, TopRightCorner.Width, TopRightCorner.Height);
                spriteBatch.Draw(TopRightCorner, cornerRect, color);
            }

            if (BottomLeftCorner != null)
            {
                var cornerRect = new Rectangle(x, bounds.Bottom - BottomLeftCorner.Height, BottomLeftCorner.Width, BottomLeftCorner.Height);
                spriteBatch.Draw(BottomLeftCorner, cornerRect, color);
            }

            if (BottomRightCorner != null)
            {
                var cornerRect = new Rectangle(bounds.Right - BottomRightCorner.Width, bounds.Bottom - BottomRightCorner.Height, BottomRightCorner.Width, BottomRightCorner.Height);
                spriteBatch.Draw(BottomRightCorner, cornerRect, color);
            }
        }
    }
}
