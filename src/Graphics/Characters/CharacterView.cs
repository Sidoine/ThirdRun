using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ThirdRun.Graphics.Characters
{
    public class CharacterView
    {
        private readonly Dictionary<CharacterClass, Texture2D> _textures = new();
        private readonly ContentManager _contentManager;
        private static readonly int DefaultSize = 40;

        public CharacterView(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        private static string GetTexturePathForClass(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Guerrier => "Characters/warrior",
                CharacterClass.Mage => "Characters/mage",
                CharacterClass.Prêtre => "Characters/priest",
                CharacterClass.Chasseur => "Characters/hunter",
                _ => "Characters/warrior"
            };
        }

        private Texture2D GetTexture(CharacterClass characterClass)
        {
            if (!_textures.ContainsKey(characterClass))
            {
                string path = GetTexturePathForClass(characterClass);
                _textures[characterClass] = _contentManager.Load<Texture2D>(path);
            }
            return _textures[characterClass];
        }

        public void Render(SpriteBatch spriteBatch, Character character)
        {
            RenderAtPosition(spriteBatch, character, character.Position);
        }

        public void RenderAtPosition(SpriteBatch spriteBatch, Character character, Vector2 renderPosition)
        {
            var texture = GetTexture(character.Class);
            int size = DefaultSize;
            spriteBatch.Draw(texture, new Rectangle((int)(renderPosition.X - size / 2), (int)(renderPosition.Y - size / 2), size, size), Color.White);
            
            // Temporarily set position for health bar drawing
            Vector2 originalPos = character.Position;
            character.Position = renderPosition;
            UnitView.DrawHealthBar(spriteBatch, character, size, 6);
            character.Position = originalPos;
        }
    }
}