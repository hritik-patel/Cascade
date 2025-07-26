using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

public class Fire : Pixel
{
    // HSL makes it easier to darken
    public HSLColour hsl = new HSLColour(0.0666f, 1f, 0.5f);

    public Fire(PixelType type, Color colour)
        : base(type, colour) { }

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
        // Darken the pixel over time to black
        hsl.Luminosity -= 0.01f;
        if (hsl.Luminosity <= 0f)
        {
            grid[(int)position.X, (int)position.Y] = null;
            return;
        }
        color = hsl.ToRGB();

        this.fallDelay -= deltaTime;
        if (this.fallDelay > 0f)
        {
            return;
        }
        TryHeat(x, y, grid, gridWidth, gridHeight);
        TryMoveUp(x, y, grid, gridWidth, gridHeight, random);
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
            if (GridMethods.IsInBounds(tx, ty, gridWidth, gridHeight) && (grid[tx, ty] == null))
            {
                GridMethods.MovePixel(x, y, tx, ty, grid);
                this.fallDelay += 0.125f;
                break;
            }
        }
    }

    public void TryHeat(int x, int y, Pixel?[,] grid, int gridWidth, int gridHeight)
    {
        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (!GridMethods.IsInBounds(nx, ny, gridWidth, gridHeight))
                    continue;

                var neighbor = grid[nx, ny];
                if (neighbor == null)
                    continue;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                // Ignore itself
                if (distance == 0)
                    continue;
                // Heat up the water pixel based on the distance from the fire pixel
                int heat = (int)(5 / distance);
                neighbor.Heat(heat);
            }
        }
    }
}
