using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Cascade
{
    public class Pixel
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public bool IsActive;

        private Texture2D _texture;
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 1, 1);


        public Pixel(Texture2D texture, Vector2 startPosition, Color color)
        {
            _texture = texture;
            Position = startPosition;
            Color = color;
            Velocity = Vector2.Zero;
        }

        public void Update(float deltaTime, Rectangle bounds, List<Pixel> allPixels)
        {   
            // Apply velocity
            Position += Velocity * deltaTime;

            // Checking if the next position is within bounds or is not colliding with other pixels
            Vector2 nextPosition = Position + Velocity * deltaTime;
            Rectangle nextBounds = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, 1, 1);

            if (!bounds.Contains(nextBounds))
            {
                Velocity = Vector2.Zero;
                Position = new Vector2(
                MathHelper.Clamp(nextPosition.X, bounds.Left, bounds.Right - 1),
                MathHelper.Clamp(nextPosition.Y, bounds.Top, bounds.Bottom - 1)
                );
            return;
            }

            foreach (var pixel in allPixels)
            {
                // Check for collision with other pixels
                if (pixel == this) continue;

                    if (pixel.Bounds.Intersects(nextBounds))
                    {
                    // Stop movement if colliding with another pixel (freeze in place)
                        Velocity = Vector2.Zero;
                        return;
                    }
            }

            // Update position if no collisions
            Position = nextPosition;

            // Clamp pixel to floor boundary
            float floorY = bounds.Bottom - 1;

            if (Position.Y >= floorY)
            {
                Position.Y = floorY;
                Velocity = Vector2.Zero;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color);
        }
    }
}
