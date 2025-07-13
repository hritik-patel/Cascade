using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Button
{
    private Rectangle rect;
    public bool IsHovered { get; private set; }
    private SpriteFont font;
    public String name;
    private Texture2D texture;
    private Texture2D selectedTexture;
    public bool isActive = false;
    private Color baseColor;
    private Color hoverColor;
    private Color activeColor;
    private Color currentColor;

    public Button(
        Rectangle rectangle,
        String name,
        SpriteFont font,
        Texture2D texture,
        Texture2D selectedTexture,
        Color color
    )
    {
        rect = rectangle;
        this.name = name;
        this.font = font;
        this.texture = texture;
        this.selectedTexture = selectedTexture;
        baseColor = color;
        hoverColor = TintColor(baseColor, 20);
        activeColor = TintColor(baseColor, -30);
    }

    // Checks if the button is clicked, if clicked change the colour to gray, else keep the original color
    public bool IsClicked()
    {
        var mouse = Mouse.GetState();
        Point mousePos = mouse.Position;
        IsHovered = rect.Contains(mousePos);

        if (IsHovered && !isActive)
            currentColor = hoverColor;
        else if (!isActive)
            currentColor = baseColor;

        return IsHovered && mouse.LeftButton == ButtonState.Pressed;
    }

    public void SetActive(bool active)
    {
        isActive = active;
        currentColor = active ? activeColor : baseColor;
    }

    // Tint the button by a value to make it lighter or darker
    private Color TintColor(Color c, int delta)
    {
        return new Color(
            Math.Clamp(c.R + delta, 0, 255),
            Math.Clamp(c.G + delta, 0, 255),
            Math.Clamp(c.B + delta, 0, 255),
            c.A
        );
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (isActive)
            spriteBatch.Draw(selectedTexture, rect, currentColor);
        else
        {
            spriteBatch.Draw(texture, rect, currentColor);
        }

        // Center the text in the button
        Vector2 textSize = font.MeasureString(name);
        Vector2 textPosition = new Vector2(
            rect.X + (rect.Width - textSize.X) / 2f,
            rect.Y + (rect.Height - textSize.Y) / 2f
        );
        spriteBatch.DrawString(font, name, textPosition, Color.Black);
    }
}
