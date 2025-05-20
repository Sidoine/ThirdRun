using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdRun.Utils;

namespace MonogameRPG
{
    public abstract class Unit
    {
        public Vector2 Position { get; set; }
        public int Health { get; set; }
        public int AttackPower { get; set; }
        public bool IsDead => Health <= 0;

        public void DrawHealthBar(SpriteBatch spriteBatch, int width = 40, int height = 6)
        {
            // Position de la barre de vie juste au-dessus de l'unité
            Vector2 barPos = new Vector2(Position.X, Position.Y - 12);
            Rectangle bgRect = new Rectangle((int)barPos.X - width / 2, (int)barPos.Y, width, height);
            Rectangle fgRect = bgRect;
            float percent = MathHelper.Clamp(Health / 100f, 0f, 1f);
            fgRect.Width = (int)(width * percent);
            // Fond (rouge foncé)
            spriteBatch.Draw(Helpers.GetPixel(spriteBatch.GraphicsDevice), bgRect, Color.DarkRed);
            // Barre de vie (vert)
            spriteBatch.Draw(Helpers.GetPixel(spriteBatch.GraphicsDevice), fgRect, Color.LimeGreen);
        }
    }
}
