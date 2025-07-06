using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public enum PixelType
{
    Sand,
    Water,
}

public class Pixel
{
    public Vector2 Position;
    public Color Color;
    public PixelType Type;
    public bool IsActive;
    
    // -1 = left, 1 = right, 0 = no direction yet
    public int LastDirection = 0;

    public Pixel(PixelType type, Color color)
    {
        Color = color;
        Type = type;
        IsActive = true;
    }
}