using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Pixel
{
    public Vector2 Position;
    public Color Color;
    public bool IsActive;

    public Pixel(Color color)
    {
        Color = color;
        IsActive = true;
    }
}