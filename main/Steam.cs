using System;
using Microsoft.Xna.Framework;

public class Steam : Fire
{
    public HSLColour hsl = new HSLColour(0.5f, 0.0f, 1.0f);

    public Steam()
        : base(PixelType.Steam, new Color(255, 255, 255, 200)) { }

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
        // Darken the pixel over time to black
        hsl.Luminosity -= 0.0025f;
        if (hsl.Luminosity <= 0f)
        {
            grid[(int)position.X, (int)position.Y] = null;
            return;
        }
        color = hsl.ToRGB();

        this.fallDelay -= deltaTime;
        if (this.fallDelay > 0f)
        {
            return;
        }
        TryMoveUp(x, y, grid, gridWidth, gridHeight, random);
    }
}
