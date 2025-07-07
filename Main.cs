using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Cascade;

public class Main : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Rectangle gameArea;
    private Rectangle selectionPanel;
    private Rectangle debugPanel;
    private List<Pixel> pixels = new List<Pixel>();
    private Texture2D pixelTexture;
    private int cellSize = 4;
    private int gridWidth = 255;
    private int gridHeight = 200;
    private Pixel?[,] grid;
    private Random random = new Random();
    private float deltaTime;


    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        // Setting the window size
        _graphics.PreferredBackBufferWidth = 1420;
        _graphics.PreferredBackBufferHeight = 800;
        _graphics.ApplyChanges();

        // Setting the game area and side screens
        int gameWidth = 1020;
        int gameHeight = 800;

        // Side panel sizes
        int panelWidth = 400;
        int panelHeight = 400;

        // Create rectangles for game area and side panels
        gameArea = new Rectangle(0, 0, gameWidth, gameHeight);
        selectionPanel = new Rectangle(gameWidth, 0, panelWidth, panelHeight);
        debugPanel = new Rectangle(gameWidth, panelHeight, panelWidth, panelHeight);
        grid = new Pixel?[gridWidth, gridHeight];

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        // Create a 1x1 white texture for drawing pixels
        pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        pixelTexture.SetData(new[] {Color.White});
    }

    protected override void Update(GameTime gameTime)
    {   
        // The game quits when the backspace/delete button is pressed or escape key is pressed
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // TODO: Add your update logic here
        // Get the current mouse state and keyboard state for inputs (for now using both)
        var mouse = Mouse.GetState();
        var kstate = Keyboard.GetState();
        // Calculate the mouse position in cell coordinates
        Point mouseGridPos = new Point(mouse.X / cellSize, mouse.Y / cellSize);

        // If the left button is pressed and the mouse is within the game area, create a new sand pixel
        if (mouse.LeftButton == ButtonState.Pressed && gameArea.Contains(mouse.Position))
        {
            var gridX = mouse.X / cellSize;
            var gridY = mouse.Y / cellSize;

            if (gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight && grid[gridX, gridY] == null)
            {
                grid[gridX, gridY] = new Pixel(PixelType.Sand, Color.Yellow);
            }
        }

        // If the right button is pressed and the mouse is within the game area, create a new water pixel
        if (kstate.IsKeyDown(Keys.W) && gameArea.Contains(mouse.Position))
        {
            var gridX = mouse.X / cellSize;
            var gridY = mouse.Y / cellSize;

            if (gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight && grid[gridX, gridY] == null)
            {
                grid[gridX, gridY] = new Pixel(PixelType.Water, Color.Blue);
            }
        }

        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        // Update the grid for specific pixel types
        UpdateGrid(deltaTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        // Draw the main screen and side screens using colours to differentiate for now
        _spriteBatch.Draw(pixelTexture, gameArea, Color.DarkSlateBlue);
        _spriteBatch.Draw(pixelTexture, selectionPanel, Color.Gray);
        _spriteBatch.Draw(pixelTexture, debugPanel, Color.DarkRed);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Pixel? pixel = grid[x, y];
                if (pixel != null)
                {
                    _spriteBatch.Draw(pixelTexture, new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize), pixel.Color);
                }
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void UpdateGrid(float deltaTime)
    {
        // Reset all HasUpdated flags
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                    grid[x, y]!.HasUpdated = false;
            }
        }

        // Loop bottom to top to prevent pixels from moving into themselves
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                var pixel = grid[x, y];
                if (pixel == null)
                {
                    continue;
                }

                // If the pixel is a sand pixel, check if it can fall
                // Sand falls down if the cell below is empty or contains water
                // If it falls through water, it changes color and sets a fall delay to simulate a slower fall
                if (pixel.Type == PixelType.Sand)
                {
                    pixel.FallDelay -= deltaTime;
                    if (pixel.FallDelay > 0f)
                    {
                        // Skip this pixel if it hasn't reached its fall delay
                        continue;
                    }
                    int belowX = x;
                    int belowY = y + 1;

                    int leftX = x - 1;
                    int rightX = x + 1;

                    // Checks if the cells are empty to allow movement
                    bool belowEmpty = IsCellEmpty(belowX, belowY);
                    bool leftBelowEmpty = IsCellEmpty(leftX, belowY);
                    bool rightBelowEmpty = IsCellEmpty(rightX, belowY);
                    bool leftEmpty = IsCellEmpty(leftX, y);
                    bool rightEmpty = IsCellEmpty(rightX, y);

                    if (belowEmpty)
                    {
                        MovePixel(x, y, belowX, belowY);
                    }
                    else if (IsCellWater(belowX, belowY))
                    {
                        SwapPixel(x, y, belowX, belowY);
                        // Set new fall delay after falling through water and change the colour to a darker shade
                        pixel.FallDelay = 0.0625f;
                        pixel.ChangeColor(new Color(216, 160, 28, 200));
                    }
                    else if (leftBelowEmpty && !rightBelowEmpty && leftEmpty)
                    {
                        MovePixel(x, y, leftX, y);
                        MovePixel(leftX, y, leftX, belowY);
                    }
                    else if (!leftBelowEmpty && rightBelowEmpty && rightEmpty)
                    {
                        MovePixel(x, y, rightX, y);
                        MovePixel(rightX, y, rightX, belowY);
                    }
                    else if (leftBelowEmpty && rightBelowEmpty && leftEmpty && rightEmpty)
                    {
                        int randomInt = random.Next(0, 2);
                        if (randomInt == 0)
                        {
                            MovePixel(x, y, leftX, y);
                            MovePixel(leftX, y, leftX, belowY);
                        }
                        else
                        {
                            MovePixel(x, y, rightX, y);
                            MovePixel(rightX, y, rightX, belowY);
                        }
                    }
                }
                // If the pixel is a water pixel, check if it can fall or move sideways/diagonally
                // Priority is given to falling down, then diagonally down, then sideways
                // NOTE : Redo the diagonal movement to be more fluid, moving sideways and then down as 2 seperate movements
                if (pixel.Type == PixelType.Water)
                {
                    // If the pixel has already been updated, skip it
                    if (pixel.HasUpdated)
                    {
                        continue;
                    }

                    // Check below and diagonals
                    int belowX = x;
                    int belowY = y + 1;

                    int leftX = x - 1;
                    int rightX = x + 1;

                    bool belowEmpty = IsCellEmpty(belowX, belowY);
                    bool rightBelowEmpty = IsCellEmpty(rightX, belowY);
                    bool leftBelowEmpty = IsCellEmpty(leftX, belowY);

                    bool leftEmpty = IsCellEmpty(leftX, y);
                    bool rightEmpty = IsCellEmpty(rightX, y);

                    // Move straight down if possible
                    if (belowEmpty)
                    {
                        MovePixel(x, y, belowX, belowY);
                    }
                    // If both diagonal cells below are empty, move based on the last direction it moved
                    else if (rightBelowEmpty && leftBelowEmpty && leftEmpty && rightEmpty)
                    {
                        if (pixel.LastDirection == 1)
                        {
                            MovePixel(x, y, rightX, y);
                            MovePixel(rightX, y, rightX, belowY);
                        }
                        else if (pixel.LastDirection == -1)
                        {
                            MovePixel(x, y, leftX, y);
                            MovePixel(leftX, y, leftX, belowY);
                        }
                        else
                        {
                            // No direction set yet, choose randomly, and set LastDirection
                            if (random.Next(2) == 0)
                            {
                                MovePixel(x, y, rightX, y);
                                MovePixel(rightX, y, rightX, belowY);
                                pixel.LastDirection = 1;
                            }
                            else
                            {
                                MovePixel(x, y, leftX, y);
                                MovePixel(leftX, y, leftX, belowY);
                                pixel.LastDirection = -1;
                            }
                        }
                    }

                    // Only down-right free
                    else if (rightBelowEmpty && rightEmpty)
                    {
                        MovePixel(x, y, rightX, y);
                        MovePixel(rightX, y, rightX, belowY);
                    }
                    // Only down-left free
                    else if (leftBelowEmpty && leftEmpty)
                    {
                        MovePixel(x, y, leftX, y);
                        MovePixel(leftX, y, leftX, belowY);
                    }
                    // If both left and right sides are empty, keep moving sideways based on the last direction
                    else if (leftEmpty && rightEmpty && IsInBounds(leftX, y) && IsInBounds(rightX, y))
                    {
                        if (pixel.LastDirection == 1)
                            MovePixel(x, y, rightX, y);
                        else if (pixel.LastDirection == -1)
                            MovePixel(x, y, leftX, y);
                        else
                        {
                            // No direction set yet, choose randomly, and set LastDirection
                            if (random.Next(2) == 0)
                            {
                                MovePixel(x, y, rightX, y);
                                pixel.LastDirection = 1;
                            }
                            else
                            {
                                MovePixel(x, y, leftX, y);
                                pixel.LastDirection = -1;
                            }
                        }
                    }
                    // If only one side is empty, move to that side, and set the last direction
                    else if (leftEmpty && !rightEmpty && IsInBounds(leftX, y))
                    {
                        if (grid[leftX, y] == null)
                        {
                            MovePixel(x, y, leftX, y);
                            pixel.LastDirection = -1;
                        }
                        else
                        {
                            MovePixel(x, y, x, y);
                            pixel.LastDirection = 0;
                        }
                    }
                    else if (!leftEmpty && rightEmpty && IsInBounds(rightX, y))
                    {
                        if (grid[rightX, y] == null)
                        {
                            MovePixel(x, y, rightX, y);
                            pixel.LastDirection = 1; // Set last direction to right
                        }
                        else
                        {
                            pixel.LastDirection = 0;
                        }
                    }
                    else
                    {
                        // No move possible, do nothing
                        // Reset last direction if no move is made
                        pixel.LastDirection = 0;
                    }
                }
            }
        }
    }

    // Check if the coordinates (x, y) are within the grid bounds
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    // Check if the cell at (x, y) is empty
    private bool IsCellEmpty(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return false;
        }
        return grid[x, y] == null;
    }

    // Move a pixel from one cell to another
    private void MovePixel(int fromX, int fromY, int toX, int toY)
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
    private bool IsCellWater(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return false;
        }
        return grid[x, y]?.Type == PixelType.Water;
    }

    // Swap two pixels in the grid
    private void SwapPixel(int fromX, int fromY, int toX, int toY)
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

