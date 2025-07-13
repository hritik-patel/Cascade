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
        Color = variate(color);
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

    public Color variate(Color color)
    {
        // Randomly vary the color slightly, with a 10% chance
        if (random.Next(0, 100) < 50)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            // Finding the largest value in the color either r, g, or b
            // and then randomly varying that value by 10% of its original value
            if (r >= g && r >= b)
            {
                int delta = (int)(r * 0.1);
                r = (byte)Math.Clamp(r + random.Next(-delta/4, delta + 1), 0, 255);
            }
            else if (g >= r && g >= b)
            {
                int delta = (int)(g * 0.1);
                g = (byte)Math.Clamp(g + random.Next(-delta/4, delta + 1), 0, 255);
            }
            else
            {
                int delta = (int)(b * 0.1);
                b = (byte)Math.Clamp(b + random.Next(-delta/4, delta + 1), 0, 255);
            }

            return new Color(r, g, b, color.A);
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
