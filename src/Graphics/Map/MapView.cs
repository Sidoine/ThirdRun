using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonogameRPG.Map;
using FontStashSharp;
using ThirdRun.Graphics.Characters;
using ThirdRun.Graphics.Monsters;

namespace ThirdRun.Graphics.Map
{
    public class MapView
    {
        private readonly TileTypeView _tileTypeView;
        private readonly CharacterView _characterView;
        private readonly MonsterView _monsterView;

        public MapView(ContentManager contentManager)
        {
            _tileTypeView = new TileTypeView(contentManager);
            _characterView = new CharacterView(contentManager);
            _monsterView = new MonsterView(contentManager);
        }

        public void Render(SpriteBatch spriteBatch, MonogameRPG.Map.Map map, DynamicSpriteFont dynamicFont, Vector2 offset = default)
        {
            if (map.Tiles == null) return;
            
            Vector2 renderOffset = offset + map.Position;
            
            for (int x = 0; x < MonogameRPG.Map.Map.GridWidth; x++)
                for (int y = 0; y < MonogameRPG.Map.Map.GridHeight; y++)
                {
                    Vector2 tilePos = renderOffset + new Vector2(x * MonogameRPG.Map.Map.TileWidth, y * MonogameRPG.Map.Map.TileHeight);
                    _tileTypeView.Render(spriteBatch, map.Tiles[x, y], (int)tilePos.X, (int)tilePos.Y);
                }
            
            // Affichage des monstres
            foreach (var monster in map.GetMonsters())
            {
                if (!monster.IsDead && monster.Position != Vector2.Zero)
                {
                    Vector2 renderPos = monster.Position;
                    _monsterView.RenderAtPosition(spriteBatch, monster, dynamicFont, renderPos);
                }
            }
            
            // Affichage des personnages
            foreach (var character in map.Characters)
            {
                if (character.Position != Vector2.Zero)
                {
                    Vector2 renderPos = character.Position;
                    _characterView.RenderAtPosition(spriteBatch, character, renderPos);
                }
            }
        }
    }
}