using System;
using Microsoft.Xna.Framework;

public class WetSand : Sand
{
    public WetSand()
        : base(PixelType.WetSand, new Color(216, 160, 28, 200)) { }

    // Wet sand behaves like sand, but takes longer to fall and form the pyramid sand shape
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
        this.FallDelay -= deltaTime;
        if (this.FallDelay > 0f)
        {
            if (!soaked)
            {
                this.FallDelay = 0;
            }
            // Skip this pixel if it hasn't reached its fall delay
            return;
        }

        int belowX = x;
        int belowY = y + 1;

        int leftX = x - 1;
        int rightX = x + 1;

        // Since wet sand is heavier, add a slight fall delay in water
        if (GridMethods.IsCellWater(belowX, belowY, gridWidth, gridHeight, grid))
        {
            this.FallDelay = 0.125f;
        }
        else
        {
            this.FallDelay = 0;
        }

        // Checks if the cells are empty to allow movement
        bool belowEmpty = GridMethods.IsCellEmpty(belowX, belowY, gridWidth, gridHeight, grid);
        bool leftBelowEmpty = GridMethods.IsCellEmpty(leftX, belowY, gridWidth, gridHeight, grid);
        bool rightBelowEmpty = GridMethods.IsCellEmpty(rightX, belowY, gridWidth, gridHeight, grid);
        bool leftEmpty = GridMethods.IsCellEmpty(leftX, y, gridWidth, gridHeight, grid);
        bool rightEmpty = GridMethods.IsCellEmpty(rightX, y, gridWidth, gridHeight, grid);

        if (belowEmpty)
        {
            GridMethods.MovePixel(x, y, belowX, belowY, grid);
        }
        else if (GridMethods.IsCellWater(belowX, belowY, gridWidth, gridHeight, grid))
        {
            GridMethods.SwapPixel(x, y, belowX, belowY, grid);
            // Set new fall delay after falling through water
            this.FallDelay = 0.0625f;
        }
        else if (leftBelowEmpty && !rightBelowEmpty && leftEmpty)
        {
            GridMethods.MovePixel(x, y, leftX, y, grid);
            GridMethods.MovePixel(leftX, y, leftX, belowY, grid);
        }
        else if (!leftBelowEmpty && rightBelowEmpty && rightEmpty)
        {
            GridMethods.MovePixel(x, y, rightX, y, grid);
            GridMethods.MovePixel(rightX, y, rightX, belowY, grid);
        }
        else if (leftBelowEmpty && rightBelowEmpty && leftEmpty && rightEmpty)
        {
            int randomInt = random.Next(0, 2);
            if (randomInt == 0)
            {
                GridMethods.MovePixel(x, y, leftX, y, grid);
                GridMethods.MovePixel(leftX, y, leftX, belowY, grid);
            }
            else
            {
                GridMethods.MovePixel(x, y, rightX, y, grid);
                GridMethods.MovePixel(rightX, y, rightX, belowY, grid);
            }
        }
    }
}
