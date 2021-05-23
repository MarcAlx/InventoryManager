using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    /// <summary>
    /// Contains general purpose methods
    /// </summary>
    public class Toolkit
    {
        public static void DrawFPSAt(Vector2 location, SpriteFont font, SpriteBatch spriteBatch, float fps)
        {
            spriteBatch.DrawString(font, String.Format("{0:00.00} fps",fps), location, Color.Black);
        }

        /// <summary>
        /// Create a color from HSL
        /// @see https://github.com/craftworkgames/MonoGame.Extended/blob/develop/src/dotnet/MonoGame.Extended/ColorHelper.cs
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="lightness"></param>
        /// <returns></returns>
        public static Color FromHsl(float hue, float saturation, float lightness)
        {
            var hsl = new Vector4(hue, saturation, lightness, 1);
            var color = new Vector4(0, 0, 0, hsl.W);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (hsl.Y == 0.0f)
                color.X = color.Y = color.Z = hsl.Z;
            else
            {
                var q = hsl.Z < 0.5f ? hsl.Z * (1.0f + hsl.Y) : hsl.Z + hsl.Y - hsl.Z * hsl.Y;
                var p = 2.0f * hsl.Z - q;

                color.X = HueToRgb(p, q, hsl.X + 1.0f / 3.0f);
                color.Y = HueToRgb(p, q, hsl.X);
                color.Z = HueToRgb(p, q, hsl.X - 1.0f / 3.0f);
            }

            return new Color(color);
        }

        /// <summary>
        /// HUE to RGB conversion
        /// @see https://github.com/craftworkgames/MonoGame.Extended/blob/develop/src/dotnet/MonoGame.Extended/ColorHelper.cs
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0.0f) t += 1.0f;
            if (t > 1.0f) t -= 1.0f;
            if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
            return p;
        }

        /// <summary>
        /// Convert a degree angle to radians
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        /// <summary>
        /// Generate a color ramp of nb color between from and to
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static List<Color> GenerateRampBetween(Color from, Color to, int nb)
        {
            int rMin = from.R;
            int gMin = from.G;
            int bMin = from.B;

            int rMax = to.R;
            int gMax = to.G;
            int bMax = to.B;
            var size = nb;
            var res = new List<Color>(nb);
            for (int i = 0; i < size; i++)
            {
                var rAverage = rMin + (int)((rMax - rMin) * i / size);
                var gAverage = gMin + (int)((gMax - gMin) * i / size);
                var bAverage = bMin + (int)((bMax - bMin) * i / size);
                res.Add(new Color(rAverage, gAverage, bAverage));
            }
            return res;
        }
    }
}
