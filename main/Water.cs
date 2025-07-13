using System;
using Microsoft.Xna.Framework;

public class Water : Pixel
{
    private int maxMovement = 100;
    private int movementCounter = 0;
    private bool waterLocked = false;
    private float lastAnimated;

    public Water()
        : base(PixelType.Water, new Color(100, 149, 237, 200)) { }

    // If the pixel is a water pixel, check if it can fall or move sideways/diagonally
    // Priority is given to falling down, then diagonally down, then sideways
    // NOTE : Redo the diagonal movement to be more fluid, moving sideways and then down as 2 seperate movements
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
        // If the pixel has already been updated, skip it
        if (this.HasUpdated)
        {
            return;
        }

        this.FallDelay -= deltaTime;
        if (this.FallDelay > 0f)
        {
            // Skip this pixel if it hasn't reached its fall delay
            return;
        }

        // Add a slight fall delay based on the movement counter, to simulate the water slowing down as it moves right/left
        this.FallDelay = movementCounter * 0.004f;

        // Check below and diagonals
        int belowX = x;
        int belowY = y + 1;

        int leftX = x - 1;
        int rightX = x + 1;

        bool belowEmpty = GridMethods.IsCellEmpty(belowX, belowY, gridWidth, gridHeight, grid);
        bool rightBelowEmpty = GridMethods.IsCellEmpty(rightX, belowY, gridWidth, gridHeight, grid);
        bool leftBelowEmpty = GridMethods.IsCellEmpty(leftX, belowY, gridWidth, gridHeight, grid);

        bool leftEmpty = GridMethods.IsCellEmpty(leftX, y, gridWidth, gridHeight, grid);
        bool rightEmpty = GridMethods.IsCellEmpty(rightX, y, gridWidth, gridHeight, grid);

        // Move straight down if possible
        if (belowEmpty)
        {
            GridMethods.MovePixel(x, y, belowX, belowY, grid);
            movementCounter = 0;
        }
        // If the cell below is sand, 'soak' into it and turn it into wet sand
        else if (GridMethods.IsCellSand(belowX, belowY, gridWidth, gridHeight, grid))
        {
            GridMethods.SwapPixel(x, y, belowX, belowY, grid);
            this.ChangeType(PixelType.WetSand, grid, belowX, belowY);
        }
        // If both diagonal cells below are empty, move based on the last direction it moved
        else if (rightBelowEmpty && leftBelowEmpty && leftEmpty && rightEmpty)
        {
            if (this.LastDirection == 1)
            {
                GridMethods.MovePixel(x, y, rightX, y, grid);
                GridMethods.MovePixel(rightX, y, rightX, belowY, grid);
                movementCounter = 0;
            }
            else if (this.LastDirection == -1)
            {
                GridMethods.MovePixel(x, y, leftX, y, grid);
                GridMethods.MovePixel(leftX, y, leftX, belowY, grid);
                movementCounter = 0;
            }
            else
            {
                // No direction set yet, choose randomly, and set LastDirection
                if (random.Next(2) == 0)
                {
                    GridMethods.MovePixel(x, y, rightX, y, grid);
                    GridMethods.MovePixel(rightX, y, rightX, belowY, grid);
                    movementCounter = 0;
                    this.LastDirection = 1;
                }
                else
                {
                    GridMethods.MovePixel(x, y, leftX, y, grid);
                    GridMethods.MovePixel(leftX, y, leftX, belowY, grid);
                    movementCounter = 0;
                    this.LastDirection = -1;
                }
            }
        }
        else if (movementCounter >= maxMovement)
        {
            // If the pixel has moved less than the max movement, stop movement, do nothing
        }
        // Only down-right free
        else if (rightBelowEmpty && rightEmpty)
        {
            GridMethods.MovePixel(x, y, rightX, y, grid);
            GridMethods.MovePixel(rightX, y, rightX, belowY, grid);
            // Reset counter as it moved downwards
            movementCounter = 0;
        }
        // Only down-left free
        else if (leftBelowEmpty && leftEmpty)
        {
            GridMethods.MovePixel(x, y, leftX, y, grid);
            GridMethods.MovePixel(leftX, y, leftX, belowY, grid);
            movementCounter = 0;
        }
        // If both left and right sides are empty, keep moving sideways based on the last direction
        else if (
            leftEmpty
            && rightEmpty
            && GridMethods.IsInBounds(leftX, y, gridWidth, gridHeight)
            && GridMethods.IsInBounds(rightX, y, gridWidth, gridHeight)
        )
        {
            if (this.LastDirection == 1)
            {
                GridMethods.MovePixel(x, y, rightX, y, grid);
                // Count the movement as it didn't go downwards
                movementCounter++;
            }
            else if (this.LastDirection == -1)
            {
                GridMethods.MovePixel(x, y, leftX, y, grid);
                movementCounter++;
            }
            else
            {
                // No direction set yet, choose randomly, and set LastDirection
                if (random.Next(2) == 0)
                {
                    GridMethods.MovePixel(x, y, rightX, y, grid);
                    movementCounter++;
                    this.LastDirection = 1;
                }
                else
                {
                    GridMethods.MovePixel(x, y, leftX, y, grid);
                    movementCounter++;
                    this.LastDirection = -1;
                }
            }
        }
        // If only one side is empty, move to that side, and set the last direction
        else if (
            leftEmpty
            && !rightEmpty
            && GridMethods.IsInBounds(leftX, y, gridWidth, gridHeight)
        )
        {
            if (grid[leftX, y] == null)
            {
                GridMethods.MovePixel(x, y, leftX, y, grid);
                movementCounter++;
                this.LastDirection = -1;
            }
            else
            {
                GridMethods.MovePixel(x, y, x, y, grid);
                movementCounter++;
                this.LastDirection = 0;
            }
        }
        else if (
            !leftEmpty
            && rightEmpty
            && GridMethods.IsInBounds(rightX, y, gridWidth, gridHeight)
        )
        {
            if (grid[rightX, y] == null)
            {
                GridMethods.MovePixel(x, y, rightX, y, grid);
                movementCounter++;
                this.LastDirection = 1;
            }
            else
            {
                this.LastDirection = 0;
            }
        }
        // If it is 'waterLocked' (surrouned by water), 'animate' it.
        else if (GridMethods.IsCellWaterLocked(x, y, gridWidth, gridHeight, grid))
        {
            if (lastAnimated > 0f)
            {
                lastAnimated -= deltaTime;
            }
            else
            {
                var pixel = grid[x, y];
                pixel.Color = Variate(new Color(100, 149, 237, 200));
                double nextVariation = 0.1 + random.NextDouble() * (0.5 - 0.1);
                lastAnimated += (float)nextVariation;
            }
        }
        else
        {
            // No move possible, do nothing
            // Reset last direction if no move is made
            this.LastDirection = 0;
        }
    }
}
