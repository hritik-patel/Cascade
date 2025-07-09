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
                grid[gridX, gridY] = new Sand(PixelType.Sand, Color.Yellow);
            }
        }

        // If the right button is pressed and the mouse is within the game area, create a new water pixel
        if (kstate.IsKeyDown(Keys.W) && gameArea.Contains(mouse.Position))
        {
            var gridX = mouse.X / cellSize;
            var gridY = mouse.Y / cellSize;

            if (gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight && grid[gridX, gridY] == null)
            {
                grid[gridX, gridY] = new Water();
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
        _spriteBatch.Draw(pixelTexture, gameArea, Color.Black);
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
                pixel.PixelUpdate(grid, x, y, gridWidth, gridHeight, random, deltaTime);
            }
        }
    }
}

