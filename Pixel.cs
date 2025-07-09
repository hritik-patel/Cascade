using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
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
    }

    public void ChangeType(PixelType newType, Pixel?[,] grid, int x, int y)
    {
        Type = newType;
        switch (newType)
        {
            case PixelType.Sand:
                Color = Color.Yellow;
                break;
            case PixelType.Water:
                Color = Color.Blue;
                break;
            case PixelType.WetSand:
                grid[x, y] = new WetSand();
                break;
        }
    }

    public PixelType GetType()
    {
        return Type;
    }

    public virtual void PixelUpdate(Pixel?[,] grid, int x, int y, int gridWidth, int gridHeight, Random random, float deltaTime)
    {
    }
}