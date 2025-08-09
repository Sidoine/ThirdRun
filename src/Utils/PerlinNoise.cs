using System;

namespace ThirdRun.Utils
{
    /// <summary>
    /// Simple Perlin noise implementation for terrain generation
    /// </summary>
    public class PerlinNoise
    {
        private readonly int[] permutation;
        private readonly Random random;

        public PerlinNoise(int seed = 0)
        {
            random = new Random(seed);
            permutation = new int[512];
            
            // Initialize permutation table
            int[] p = new int[256];
            for (int i = 0; i < 256; i++)
                p[i] = i;

            // Shuffle
            for (int i = 0; i < 256; i++)
            {
                int j = random.Next(256);
                (p[i], p[j]) = (p[j], p[i]);
            }

            // Duplicate for seamless wrapping
            for (int i = 0; i < 256; i++)
            {
                permutation[i] = permutation[i + 256] = p[i];
            }
        }

        /// <summary>
        /// Generate 2D Perlin noise value at given coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Noise value between -1.0 and 1.0</returns>
        public double Noise(double x, double y)
        {
            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;
            
            double xf = x - Math.Floor(x);
            double yf = y - Math.Floor(y);
            
            double u = Fade(xf);
            double v = Fade(yf);
            
            int aa = permutation[permutation[xi] + yi];
            int ab = permutation[permutation[xi] + yi + 1];
            int ba = permutation[permutation[xi + 1] + yi];
            int bb = permutation[permutation[xi + 1] + yi + 1];
            
            double lerp1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
            double lerp2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);
            
            return Lerp(lerp1, lerp2, v);
        }

        /// <summary>
        /// Generate octaved noise for more natural terrain
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="octaves">Number of octaves</param>
        /// <param name="persistence">Persistence value</param>
        /// <returns>Noise value between -1.0 and 1.0</returns>
        public double OctaveNoise(double x, double y, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;
            
            for (int i = 0; i < octaves; i++)
            {
                total += Noise(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }
            
            return total / maxValue;
        }

        private static double Fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static double Lerp(double a, double b, double t)
        {
            return a + t * (b - a);
        }

        private static double Grad(int hash, double x, double y)
        {
            int h = hash & 15;
            double u = h < 8 ? x : y;
            double v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
    }
}