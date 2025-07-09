using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
public class Sand : Pixel
{
    public bool soaked = false;
    public Sand(PixelType type, Color color) : base(type, color)
    {
    }

    private bool checkSoaked()
    {
        // Check if the sand pixel is soaked
        if (soaked)
        {
            return true;
        }
        return false;
    }

    // If the pixel is a sand pixel, check if it can fall
    // Sand falls down if the cell below is empty or contains water
    // If it falls through water, it changes color and sets a fall delay to simulate a slower fall
    public override void PixelUpdate(Pixel?[,] grid, int x, int y, int gridWidth, int gridHeight, Random random, float deltaTime)
    {
        this.FallDelay -= deltaTime;
        if (this.FallDelay > 0f)
        {
            // Skip this pixel if it hasn't reached its fall delay
            return;
        }
        int belowX = x;
        int belowY = y + 1;

        int leftX = x - 1;
        int rightX = x + 1;

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
            // Set new fall delay after falling through water and change the colour to a darker shade
            this.FallDelay = 0.0625f;
            this.ChangeType(PixelType.WetSand, grid, x, y);
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