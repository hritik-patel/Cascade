using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public enum PixelType
{
    Sand,
    Water,
    WetSand,
    Fire,
    Steam,
    Glass,
}

public class Pixel
{
    public Vector2 position;
    public Color color;
    public PixelType type;
    public float fallDelay;
    public bool hasUpdated;
    public int temp;
    public Random random = new Random();

    // -1 = left, 1 = right, 0 = no direction yet
    public int lastDirection;

    public Pixel(PixelType type, Color color)
    {
        this.color = color;
        this.type = type;
        hasUpdated = false;
        fallDelay = 0;
        lastDirection = 0;
        temp = 20;
        if (type == PixelType.Water || type == PixelType.Fire)
        {
            color = Variate(color);
        }
    }

    public void ChangeType(PixelType newType, Pixel?[,] grid, int x, int y)
    {
        type = newType;
        switch (newType)
        {
            case PixelType.Sand:
                color = new Color(225, 191, 146, 200);
                break;
            case PixelType.Water:
                color = new Color(100, 149, 237, 200);
                break;
            case PixelType.WetSand:
                // Remove the old pixel and create a new WetSand pixel
                grid[x, y] = null;
                grid[x, y - 1] = null;
                grid[x, y] = new WetSand();
                break;
            case PixelType.Steam:
                grid[x, y] = null;
                grid[x, y - 1] = null;
                grid[x, y] = new Steam();
                break;
            case PixelType.Glass:
                grid[x, y] = null;
                grid[x, y] = new Glass();
                break;
        }
    }

    public PixelType GetType()
    {
        return type;
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

    public void Heat(int x)
    {
        temp += x;
    }

    public void Cool(int x)
    {
        int y = temp - x;
        if (y >= 20)
            temp = y;
    }
}
