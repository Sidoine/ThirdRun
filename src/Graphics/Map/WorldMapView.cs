using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonogameRPG.Map;
using FontStashSharp;
using System.Collections.Generic;

namespace ThirdRun.Graphics.Map
{
    public class WorldMapView
    {
        private readonly MapView _mapView;

        public WorldMapView(ContentManager contentManager)
        {
            _mapView = new MapView(contentManager);
        }

        public void Render(SpriteBatch spriteBatch, WorldMap worldMap, DynamicSpriteFont dynamicFont)
        {
            // Render all maps relative to their world positions
            foreach (var map in worldMap.GetAllMaps())
            {
                _mapView.Render(spriteBatch, map, dynamicFont);
            }
        }
    }
}