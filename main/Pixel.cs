using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public enum PixelType
{
    Sand,
    Water,
    WetSand,
}

public class Pixel
{
    public Vector2 Position;
    public Color Color;
    public PixelType Type;
    public float FallDelay;
    public bool HasUpdated;
    public Random random = new Random();

    // -1 = left, 1 = right, 0 = no direction yet
    public int LastDirection;

    public Pixel(PixelType type, Color color)
    {
        Color = color;
        Type = type;
        HasUpdated = false;
        FallDelay = 0;
        LastDirection = 0;
        if (type == PixelType.Water)
        {
            Color = Variate(color);
        }
    }

    public void ChangeType(PixelType newType, Pixel?[,] grid, int x, int y)
    {
        Type = newType;
        switch (newType)
        {
            case PixelType.Sand:
                Color = new Color(225, 191, 146, 200);
                break;
            case PixelType.Water:
                Color = new Color(100, 149, 237, 200);
                break;
            case PixelType.WetSand:
                // Remove the old pixel and create a new WetSand pixel
                grid[x, y] = null;
                grid[x, y - 1] = null;
                grid[x, y] = new WetSand();
                break;
        }
    }

    public PixelType GetType()
    {
        return Type;
    }

    public Color Variate(Color color)
    {
        if (random.Next(0, 100) < 50)
        {
            HSLColour hsl = HSLColour.FromRGB(color.R, color.G, color.B);

            // Shift hue by +-10 degrees
            float offset = random.Next(-10, 10) / 360f;
            hsl.Hue = (hsl.Hue + offset + 1f) % 1f;

            Color varied = hsl.ToRGB();
            return new Color(varied.R, varied.G, varied.B, color.A);
        }

        return color;
    }

    public virtual void PixelUpdate(
        Pixel?[,] grid,
        int x,
        int y,
        int gridWidth,
        int gridHeight,
        Random random,
        float deltaTime
    ) { }
}
