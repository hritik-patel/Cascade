using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public static class GridMethods
{
    // Check if the coordinates (x, y) are within the grid bounds
    public static bool IsInBounds(int x, int y, int gridWidth, int gridHeight)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    // Check if the cell at (x, y) is empty
    public static bool IsCellEmpty(int x, int y, int gridWidth, int gridHeight, Pixel?[,] grid)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return false;
        }
        return grid[x, y] == null;
    }

    // Move a pixel from one cell to another
    public static void MovePixel(int fromX, int fromY, int toX, int toY, Pixel?[,] grid)
    {
        var pixel = grid[fromX, fromY];
        if (pixel == null)
        {
            return;
        }

        grid[toX, toY] = pixel;
        grid[fromX, fromY] = null;
        pixel.Position = new Vector2(toX, toY);
        pixel.HasUpdated = true;
    }

    // Check if the cell at (x, y) is water
    public static bool IsCellX(
        int x,
        int y,
        int gridWidth,
        int gridHeight,
        Pixel?[,] grid,
        PixelType type
    )
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return false;
        }
        return grid[x, y]?.Type == type;
    }

    public static bool IsCellWaterLocked(
        int x,
        int y,
        int gridWidth,
        int gridHeight,
        Pixel?[,] grid
    )
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return false;
        }
        // Loop through surrounding cells (3x3 block centered on x,y)
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // Ignore itself
                if (dx == 0 && dy == 0)
                    continue;

                int nx = x + dx;
                int ny = y + dy;

                // If anything isnt water around it, return false
                if (!IsCellX(nx, ny, gridWidth, gridHeight, grid, PixelType.Water))
                    return false;
            }
        }

        return true;
    }

    // Swap two pixels in the grid
    public static void SwapPixel(int fromX, int fromY, int toX, int toY, Pixel?[,] grid)
    {
        var fromPixel = grid[fromX, fromY];
        var toPixel = grid[toX, toY];

        if (fromPixel == null || toPixel == null)
        {
            return;
        }

        // Swap the pixels
        grid[fromX, fromY] = toPixel;
        grid[toX, toY] = fromPixel;

        // Update their positions
        fromPixel.Position = new Vector2(toX, toY);
        toPixel.Position = new Vector2(fromX, fromY);
    }
}
