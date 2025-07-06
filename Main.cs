using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        pixelTexture.SetData(new[] {Color.White});
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // TODO: Add your update logic here
        var mouse = Mouse.GetState();
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (mouse.LeftButton == ButtonState.Pressed && gameArea.Contains(mouse.Position))
        {
        // Create a new falling pixel with initial downward velocity (98f is a gravity like value)
        var newPixel = new Pixel(pixelTexture, mouse.Position.ToVector2(), Color.Yellow);
        newPixel.Velocity = new Vector2(0, 98f);
        pixels.Add(newPixel);
        }

        // Update all pixels
        foreach (var pixel in pixels)
        {
            pixel.Update(dt, gameArea);
        }

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

        foreach (var pixel in pixels)
        {
            pixel.Draw(_spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
