using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Cascade;

public class Main : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _debugFont;
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
    private int cells = 0;
    private int sandCount = 0;
    private int waterCount = 0;
    private int wetSandCount = 0;
    private int fireCount = 0;
    private int steamCount = 0;
    private int glassCount = 0;
    private Graph debugGraph;
    private Texture2D circleTexture;
    private Texture2D sliceTexture;
    private Button sandButton;
    private Button waterButton;
    private Button wetSandButton;
    private Button fireButton;
    private Button steamButton;
    private Button glassButton;
    private Texture2D buttonTexture;
    private List<Button> buttons = new List<Button>();
    private Pixel pixel;

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
        pixelTexture.SetData(new[] { Color.White });
        _debugFont = Content.Load<SpriteFont>("DebugFont");
        Texture2D circleTexture = Content.Load<Texture2D>("circle");
        Texture2D sliceTexture = Content.Load<Texture2D>("slice");
        Texture2D buttonTexture = Content.Load<Texture2D>("button");
        Texture2D selectedButtonTexture = Content.Load<Texture2D>("button_selected");
        debugGraph = new Graph(circleTexture, sliceTexture, _spriteBatch);
        sandButton = new Button(
            new Rectangle(1030, 10, 100, 40),
            "Sand",
            _debugFont,
            buttonTexture,
            selectedButtonTexture,
            new Color(225, 191, 146, 200)
        );
        waterButton = new Button(
            new Rectangle(1160, 10, 100, 40),
            "Water",
            _debugFont,
            buttonTexture,
            selectedButtonTexture,
            new Color(100, 149, 237, 200)
        );
        wetSandButton = new Button(
            new Rectangle(1290, 10, 100, 40),
            "Wet Sand",
            _debugFont,
            buttonTexture,
            selectedButtonTexture,
            new Color(216, 160, 28, 200)
        );
        fireButton = new Button(
            new Rectangle(1030, 65, 100, 40),
            "Fire",
            _debugFont,
            buttonTexture,
            selectedButtonTexture,
            new Color(255, 100, 0, 200)
        );
        steamButton = new Button(
            new Rectangle(1160, 65, 100, 40),
            "Steam",
            _debugFont,
            buttonTexture,
            selectedButtonTexture,
            new Color(255, 255, 255, 200)
        );
        glassButton = new Button(
            new Rectangle(1290, 65, 100, 40),
            "Glass",
            _debugFont,
            buttonTexture,
            selectedButtonTexture,
            new Color(195, 225, 235, 200)
        );
        // Add buttons to the list for easy management
        buttons.Add(sandButton);
        buttons.Add(waterButton);
        buttons.Add(wetSandButton);
        buttons.Add(fireButton);
        buttons.Add(steamButton);
        buttons.Add(glassButton);
    }

    protected override void Update(GameTime gameTime)
    {
        // The game quits when the backspace/delete button is pressed or escape key is pressed
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
        {
            Exit();
        }

        // TODO: Add your update logic here
        // Get the current mouse state and keyboard state for inputs (for now using both)
        var mouse = Mouse.GetState();
        var kstate = Keyboard.GetState();
        // Calculate the mouse position in cell coordinates
        Point mouseGridPos = new Point(mouse.X / cellSize, mouse.Y / cellSize);

        // Check which button is clicked
        foreach (var button in buttons)
        {
            if (button.IsClicked())
            {
                foreach (var other in buttons)
                    other.SetActive(false);

                button.SetActive(true);
                break;
            }
        }

        Button activeButton = buttons.FirstOrDefault(b => b.isActive);
        if (activeButton != null)
        {
            // Set the pixel type based on the active button
            switch (activeButton.name)
            {
                case "Sand":
                    pixel = new Sand(PixelType.Sand, new Color(225, 191, 146, 200));
                    break;
                case "Water":
                    pixel = new Water();
                    break;
                case "Wet Sand":
                    pixel = new WetSand();
                    break;
                case "Fire":
                    pixel = new Fire(PixelType.Fire, new Color(255, 100, 0, 200));
                    break;
                case "Steam":
                    pixel = new Steam();
                    break;
                case "Glass":
                    pixel = new Glass();
                    break;
                default:
                    pixel = null;
                    break;
            }
        }

        // If the left button is pressed and the mouse is within the game area, create a new pixel
        if (mouse.LeftButton == ButtonState.Pressed && gameArea.Contains(mouse.Position))
        {
            var gridX = mouse.X / cellSize;
            var gridY = mouse.Y / cellSize;

            if (
                gridX >= 0
                && gridX < gridWidth
                && gridY >= 0
                && gridY < gridHeight
                && grid[gridX, gridY] == null
            )
            {
                grid[gridX, gridY] = pixel;
            }
        }

        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        sandButton.IsClicked();
        waterButton.IsClicked();
        wetSandButton.IsClicked();
        fireButton.IsClicked();
        steamButton.IsClicked();
        glassButton.IsClicked();
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
        _spriteBatch.Draw(pixelTexture, selectionPanel, Color.White);
        _spriteBatch.Draw(pixelTexture, debugPanel, Color.Gray);
        _spriteBatch.DrawString(
            _debugFont,
            "Pixel count: " + cells,
            new Vector2(1030, 410),
            Color.White
        );
        _spriteBatch.DrawString(
            _debugFont,
            "Sand count: " + sandCount,
            new Vector2(1030, 450),
            Color.White
        );
        _spriteBatch.DrawString(
            _debugFont,
            "Wet Sand count: " + wetSandCount,
            new Vector2(1030, 490),
            Color.White
        );
        _spriteBatch.DrawString(
            _debugFont,
            "Water count: " + waterCount,
            new Vector2(1030, 530),
            Color.White
        );
        _spriteBatch.DrawString(
            _debugFont,
            "Fire: " + fireCount,
            new Vector2(1030, 570),
            Color.White
        );
        _spriteBatch.DrawString(
            _debugFont,
            "Steam: " + steamCount,
            new Vector2(1030, 610),
            Color.White
        );
        _spriteBatch.DrawString(
            _debugFont,
            "Glass: " + glassCount,
            new Vector2(1030, 650),
            Color.White
        );

        Vector2 center = new Vector2(debugPanel.X + 325, debugPanel.Y + 75);
        debugGraph.DrawGraph(
            center,
            cells,
            sandCount,
            waterCount,
            wetSandCount,
            fireCount,
            steamCount,
            glassCount
        );

        // Draw the buttons in the selection panel
        // Starting at the top left corner of the selection panel, place buttons in a 2x2 grid
        sandButton.Draw(_spriteBatch);
        waterButton.Draw(_spriteBatch);
        wetSandButton.Draw(_spriteBatch);
        fireButton.Draw(_spriteBatch);
        steamButton.Draw(_spriteBatch);
        glassButton.Draw(_spriteBatch);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Pixel? pixel = grid[x, y];
                if (pixel != null)
                {
                    _spriteBatch.Draw(
                        pixelTexture,
                        new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize),
                        pixel.color
                    );
                }
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void UpdateGrid(float deltaTime)
    {
        cells = 0;
        sandCount = 0;
        waterCount = 0;
        wetSandCount = 0;
        fireCount = 0;
        steamCount = 0;
        glassCount = 0;
        // Reset all HasUpdated flags + count cell types for the graph
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    cells++;
                    grid[x, y]!.hasUpdated = false;
                    switch (grid[x, y]!.GetType())
                    {
                        case PixelType.Sand:
                            sandCount++;
                            break;
                        case PixelType.Water:
                            waterCount++;
                            break;
                        case PixelType.WetSand:
                            wetSandCount++;
                            break;
                        case PixelType.Fire:
                            fireCount++;
                            break;
                        case PixelType.Steam:
                            steamCount++;
                            break;
                        case PixelType.Glass:
                            glassCount++;
                            break;
                    }
                }
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
