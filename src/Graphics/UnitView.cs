using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameRPG;
using ThirdRun.Utils;

namespace ThirdRun.Graphics
{
    public static class UnitView
    {
        private const int HealthBarOffset = 25;

        public static void DrawHealthBar(SpriteBatch spriteBatch, Unit unit, int width = 40, int height = 6)
        {
            // Position de la barre de vie juste au-dessus de l'unité
            var barPos = new Vector2(unit.Position.X, unit.Position.Y - HealthBarOffset);
            Rectangle bgRect = new Rectangle((int)barPos.X - width / 2, (int)barPos.Y, width, height);
            Rectangle fgRect = bgRect;
            float percent = MathHelper.Clamp(unit.CurrentHealth / (float)unit.MaxHealth, 0f, 1f);
            fgRect.Width = (int)(width * percent);
            // Fond (rouge foncé)
            spriteBatch.Draw(Helpers.GetPixel(spriteBatch.GraphicsDevice), bgRect, Color.DarkRed);
            // Barre de vie (vert)
            spriteBatch.Draw(Helpers.GetPixel(spriteBatch.GraphicsDevice), fgRect, Color.LimeGreen);
        }
    }
}