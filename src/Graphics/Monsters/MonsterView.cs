using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonogameRPG.Monsters;
using FontStashSharp;
using System.Collections.Generic;

namespace ThirdRun.Graphics.Monsters
{
    public class MonsterView
    {
        private readonly Dictionary<string, Texture2D> _textures = new();
        private readonly ContentManager _contentManager;
        private static readonly int DefaultSize = 40;

        public MonsterView(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        private Texture2D GetTexture(MonsterType type)
        {
            if (!_textures.ContainsKey(type.TexturePath))
            {
                _textures[type.TexturePath] = _contentManager.Load<Texture2D>(type.TexturePath);
            }
            return _textures[type.TexturePath];
        }

        public void RenderAtPosition(SpriteBatch spriteBatch, Monster monster, DynamicSpriteFont dynamicFont, Vector2 renderPosition)
        {
            var texture = GetTexture(monster.Type);
            
            // N'affiche rien si le monstre est mort
            if (monster.IsDead) return;

            spriteBatch.Draw(texture, new Rectangle((int)(renderPosition.X - DefaultSize / 2), (int)(renderPosition.Y - DefaultSize / 2), DefaultSize, DefaultSize), Color.White);
            
            // Temporarily set position for health bar drawing
            Vector2 originalPos = monster.Position;
            monster.Position = renderPosition;
            UnitView.DrawHealthBar(spriteBatch, monster, DefaultSize, 6);
            monster.Position = originalPos;
            
            // Display the monster's name above its position
            spriteBatch.DrawString(dynamicFont, monster.Type.Name, new Vector2(renderPosition.X - DefaultSize / 2, renderPosition.Y - DefaultSize), Color.White); 
        }
    }
}