using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Button
{
    private Rectangle rect;
    private Color color;
    private Color original;
    public bool IsHovered { get; private set; }
    private SpriteFont font;
    private String name;
    private Texture2D texture;

    public Button(Rectangle rectangle, String name, SpriteFont font, Texture2D texture, Color color)
    {
        rect = rectangle;
        this.name = name;
        this.font = font;
        this.texture = texture;
        this.color = color;
        original = color;
    }

    // Checks if the button is clicked, if clicked change the colour to gray, else keep the original color
    public bool IsClicked(MouseState mouse)
    {
        Point mousePos = mouse.Position;
        IsHovered = rect.Contains(mousePos);

        if (IsHovered && mouse.LeftButton == ButtonState.Pressed)
        {
            color = Color.Gray;
            return true;
        }
        color = original;
        return false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, rect, IsHovered ? Color.Gray : color);
        // Center the text in the button
        Vector2 textSize = font.MeasureString(name);
        Vector2 textPosition = new Vector2(
            rect.X + (rect.Width - textSize.X) / 2f,
            rect.Y + (rect.Height - textSize.Y) / 2f
        );
        spriteBatch.DrawString(font, name, textPosition, Color.Black);
    }
}
