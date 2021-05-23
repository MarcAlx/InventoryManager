using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    /// <summary>
    /// A point moving randomly inside Width and height
    /// </summary>
    public class RandomMovingPoint
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector2 Position { private set; get; } = new Vector2(0, 0);

        private double currentSpeed = 5;
        private int currentAngle = 45;

        private double minBoundWidth = 0;
        private double minBoundHeight = 0;
        private double maxBoundWidth = 0;
        private double maxBoundHeight = 0;

        private int textureWidth = 50;
        private int textureHeight = 50;

        private Random random = new Random();

        private Texture2D texture;

        public RandomMovingPoint()
        {
        }

        /// <summary>
        /// Update position of this point
        /// </summary>
        public void UpdatePosition(double speed, double minBoundWidth, double minBoundHeight, double maxBoundWidth, double maxBoundHeight)
        {
            this.minBoundHeight = minBoundHeight;
            this.maxBoundHeight = maxBoundHeight;
            this.minBoundWidth = minBoundWidth;
            this.maxBoundWidth = maxBoundWidth;

            if (this.currentSpeed >= 0)
            {
                this.currentSpeed = speed;
            }
            else
            {
                this.currentSpeed = -speed;
            }

            var futureX = this.Position.X + this.currentSpeed * Math.Cos(Toolkit.ConvertToRadians(this.currentAngle));
            var futureY = this.Position.Y + this.currentSpeed * Math.Sin(Toolkit.ConvertToRadians(this.currentAngle));

            if (futureX <= minBoundWidth
            ||  futureY <= minBoundHeight
            ||  futureX > this.maxBoundWidth
            ||  futureY > this.maxBoundHeight)
            {
                this.currentAngle = random.Next(90, 270);
                this.currentSpeed = -this.currentSpeed;
            }

            futureX = Math.Max(Math.Min(this.maxBoundWidth, futureX), this.minBoundWidth);
            futureY = Math.Max(Math.Min(this.maxBoundHeight, futureY), this.minBoundHeight);

            //Console.WriteLine(futureX + " - " + futureY);
            this.Position = new Vector2((float)futureX, (float)futureY);
        }

        /// <summary>
        /// Get texture for drawing
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public Texture2D GetTexture(GraphicsDevice device)
        {
            Texture2D rect = new Texture2D(device, textureWidth, textureHeight);

            Color[] data = new Color[textureWidth*textureHeight];
            for(var i = 0; i < data.Length; i++)
            {
                data[i] = Color.Black;
            }
            rect.SetData(data);

            return rect;
        }
    }
}
