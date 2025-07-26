using System;
using Microsoft.Xna.Framework;

public class Glass : Sand
{
    public Glass()
        : base(PixelType.Glass, new Color(195, 225, 235, 200)) { }

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
        int belowX = x;
        int belowY = y + 1;

        int leftX = x - 1;
        int rightX = x + 1;

        // Check if the cell is empty under it to allow movement
        bool belowEmpty = GridMethods.IsCellEmpty(belowX, belowY, gridWidth, gridHeight, grid);

        if (belowEmpty)
        {
            GridMethods.MovePixel(x, y, belowX, belowY, grid);
        }
        // If the cell under it water, use SwapPixel
        else if (GridMethods.IsCellX(belowX, belowY, gridWidth, gridHeight, grid, PixelType.Water))
        {
            GridMethods.SwapPixel(x, y, belowX, belowY, grid);
        }
        else
        {
            // Do nothing
        }
    }
}
