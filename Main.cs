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
    int cellSize = 4;
    private int gridWidth = 255;
    private int gridHeight = 200;
    private Pixel?[,] grid;
    private Random random = new Random();
    

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

        int panelWidth = 400;
        int panelHeight = 400;

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
        pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        pixelTexture.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // TODO: Add your update logic here
        var mouse = Mouse.GetState();
        var kstate = Keyboard.GetState();
        Point mouseGridPos = new Point(mouse.X / cellSize, mouse.Y / cellSize);

        if (mouse.LeftButton == ButtonState.Pressed && gameArea.Contains(mouse.Position))
        {
            var gridX = mouse.X / cellSize;
            var gridY = mouse.Y / cellSize;

            if (gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight && grid[gridX, gridY] == null)
            {
                grid[gridX, gridY] = new Pixel(PixelType.Sand, Color.Yellow);
            }
        }

        if (kstate.IsKeyDown(Keys.W) && gameArea.Contains(mouse.Position))
        {
            var gridX = mouse.X / cellSize;
            var gridY = mouse.Y / cellSize;

            if (gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight && grid[gridX, gridY] == null)
            {
                grid[gridX, gridY] = new Pixel(PixelType.Water, Color.Blue);
            }
        }

        // Update all pixels
        for (int y = gridHeight - 2; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null && grid[x, y + 1] == null)
                {
                    grid[x, y + 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y + 1]!.Position.Y += 1;
                }
            }
        }
        UpdateGrid();
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

    private void UpdateGrid()
    {
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

                if (pixel.Type == PixelType.Sand)
                {
                    int belowX = x;
                    int belowY = y + 1;

                    int leftX = x - 1;
                    int rightX = x + 1;

                    bool belowEmpty = IsCellEmpty(belowX, belowY);
                    bool leftBelowEmpty = IsCellEmpty(leftX, belowY);
                    bool rightBelowEmpty = IsCellEmpty(rightX, belowY);

                    if (belowEmpty)
                    {
                        MovePixel(x, y, belowX, belowY);
                    }
                    else if (leftBelowEmpty && !rightBelowEmpty)
                    {
                        MovePixel(x, y, leftX, belowY);
                    }
                    else if (!leftBelowEmpty && rightBelowEmpty)
                    {
                        MovePixel(x, y, rightX, belowY);
                    }
                    else if (leftBelowEmpty && rightBelowEmpty)
                    {
                        int randomInt = random.Next(0, 2);
                        if (randomInt == 0)
                            MovePixel(x, y, leftX, belowY);
                        else
                            MovePixel(x, y, rightX, belowY);
                    }
                }

                if (pixel.Type == PixelType.Water)
                {
                    // Water movement logic
                    // Water can move down, down-left, or down-right
                    // If it can't move down, it tries to move sideways

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

                    if (belowEmpty)
                    {
                        // Move straight down
                        MovePixel(x, y, belowX, belowY);
                    }
                    else if (rightBelowEmpty && leftBelowEmpty)
                    {
                        if (pixel.LastDirection == 1)
                            MovePixel(x, y, rightX, belowY);
                        else if (pixel.LastDirection == -1)
                            MovePixel(x, y, leftX, belowY);
                        else
                        {
                            // No direction set yet, choose randomly
                            if (random.Next(2) == 0)
                            {
                                MovePixel(x, y, rightX, belowY);
                                pixel.LastDirection = 1;
                            }
                            else
                            {
                                MovePixel(x, y, leftX, belowY);
                                pixel.LastDirection = -1;
                            }
                        }
                    }

                    else if (rightBelowEmpty)
                    {
                        // Only down-right free
                        MovePixel(x, y, rightX, belowY);
                    }
                    else if (leftBelowEmpty)
                    {
                        // Only down-left free
                        MovePixel(x, y, leftX, belowY);
                    }
                    else
                    {
                        // Can't move down or diagonally down
                        // Try to move sideways
                        if (leftEmpty && rightEmpty)
                        {
                            // Both sides free, pick randomly
                            if (random.Next(2) == 0)
                                MovePixel(x, y, leftX, y);
                            else
                                MovePixel(x, y, rightX, y);
                        }
                        else if (leftEmpty)
                        {
                            MovePixel(x, y, leftX, y);
                        }
                        else if (rightEmpty)
                        {
                            MovePixel(x, y, rightX, y);
                        }
                        else
                        {
                            // No move possible
                        }
                    }
                }

            }
        }
    }

    private bool IsCellEmpty(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return false;
        }
        return grid[x, y] == null;
    }

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
    }   
}

