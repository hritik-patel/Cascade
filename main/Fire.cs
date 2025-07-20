using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

public class Fire : Pixel
{
    // HSL makes it easier to darken
    public HSLColour hsl = new HSLColour(24f, 1f, 0.5f);

    public Fire()
        : base(PixelType.Fire, new Color(255, 100, 0, 200)) { }

    public override void PixelUpdate(
        Pixel?[,] grid,
        int x,
        int y,
        int gridWidth,
        int gridHeight,
        Random random,
        float deltaTime
    )
    {
        TryMoveUp(x, y, grid, gridWidth, gridHeight, random);

        // Darken the pixel over time to black
        hsl.Luminosity -= 0.01f;
        if (hsl.Luminosity <= 0f)
        {
            grid[(int)Position.X, (int)Position.Y] = null;
            return;
        }
        Color = hsl.ToRGB();
    }

    // Move the fire pixel up or to the side randomly
    public void TryMoveUp(
        int x,
        int y,
        Pixel?[,] grid,
        int gridWidth,
        int gridHeight,
        Random random
    )
    {
        // Store the possible cells it can move to
        List<(int, int)> targets = new() { (x, y - 1), (x - 1, y - 1), (x + 1, y - 1) };

        // Randomise which cell it moves to
        foreach (var (tx, ty) in targets.OrderBy(pos => random.Next()))
        {
            if (GridMethods.IsInBounds(tx, ty, gridWidth, gridHeight) && grid[tx, ty] == null)
            {
                GridMethods.SwapPixel(x, y, tx, ty, grid);
                break;
            }
        }
    }
}
