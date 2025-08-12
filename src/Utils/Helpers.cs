using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThirdRun.Utils
{
    public static class Helpers
    {
        private static Texture2D? _pixel;
        private static Random? _sharedRandom;

        /// <summary>
        /// Sets the shared Random instance with a specific seed. Used for testing reproducibility.
        /// </summary>
        public static void SetRandomSeed(int seed)
        {
            _sharedRandom = new Random(seed);
        }

        /// <summary>
        /// Resets the shared Random instance to use a new unseeded Random.
        /// </summary>
        public static void ResetRandom()
        {
            _sharedRandom = null;
        }

        public static int RandomNumber(int min, int max)
        {
            Random random = _sharedRandom ?? new Random();
            return random.Next(min, max);
        }

        public static float CalculateDistance(Vector2 pointA, Vector2 pointB)
        {
            return Vector2.Distance(pointA, pointB);
        }

        public static Texture2D GetPixel(GraphicsDevice device)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(device, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
            return _pixel;
        }
    }
}