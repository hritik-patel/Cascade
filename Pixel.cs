using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cascade
{
    public class Pixel
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public bool IsActive;

        private Texture2D _texture;

        public Pixel(Texture2D texture, Vector2 startPosition, Color color)
        {
            _texture = texture;
            Position = startPosition;
            Color = color;
            Velocity = Vector2.Zero;
        }

        public void Update(float deltaTime, Rectangle bounds)
        {   
            // Apply velocity
            Position += Velocity * deltaTime;

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
