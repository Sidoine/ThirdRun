using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ThirdRun.Data.NPCs;
using FontStashSharp;
using System.Collections.Generic;

namespace ThirdRun.Graphics.NPCs
{
    public class NPCView
    {
        private readonly Dictionary<NPCType, Texture2D> _textures = new();
        private readonly ContentManager _contentManager;
        private static readonly int DefaultSize = 40;

        public NPCView(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        private Texture2D GetTexture(NPCType type)
        {
            if (!_textures.ContainsKey(type))
            {
                // Use character textures for NPCs for now - could add specific NPC textures later
                string texturePath = type switch
                {
                    NPCType.Merchant => "Characters/mage", // Use mage texture for merchant
                    NPCType.Guard => "Characters/warrior", // Use warrior texture for guard
                    NPCType.Innkeeper => "Characters/priest", // Use priest texture for innkeeper
                    NPCType.Blacksmith => "Characters/hunter", // Use hunter texture for blacksmith
                    _ => "Characters/warrior"
                };
                
                _textures[type] = _contentManager.Load<Texture2D>(texturePath);
            }
            return _textures[type];
        }

        public void RenderAtPosition(SpriteBatch spriteBatch, NPC npc, DynamicSpriteFont dynamicFont, Vector2 renderPosition)
        {
            Texture2D texture = GetTexture(npc.Type);
            var rect = new Rectangle((int)renderPosition.X, (int)renderPosition.Y, DefaultSize, DefaultSize);
            
            // Render NPC with a green tint to distinguish from characters/monsters
            spriteBatch.Draw(texture, rect, Color.LightGreen);

            // Render NPC name above them
            spriteBatch.DrawString(dynamicFont, npc.Name, new Vector2(renderPosition.X - DefaultSize / 2, renderPosition.Y - DefaultSize), Color.White);
        }
    }
}