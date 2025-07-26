using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Graph
{
    private Texture2D baseCircleTexture;
    private Texture2D sliceTexture;
    private SpriteBatch spriteBatch;

    public Graph(Texture2D baseCircleTexture, Texture2D sliceTexture, SpriteBatch spriteBatch)
    {
        this.baseCircleTexture = baseCircleTexture;
        this.sliceTexture = sliceTexture;
        this.spriteBatch = spriteBatch;
    }

    public void DrawGraph(
        Vector2 center,
        int totalCells,
        int sandCount,
        int waterCount,
        int wetSandCount,
        int fireCount,
        int steamCount,
        int glassCount
    )
    {
        if (totalCells <= 0)
            return;

        float sandPercentage = (float)sandCount / totalCells;
        float waterPercentage = (float)waterCount / totalCells;
        float wetSandPercentage = (float)wetSandCount / totalCells;
        float firePercentage = (float)fireCount / totalCells;
        float steamPercentage = (float)steamCount / totalCells;
        float glassPercentage = (float)glassCount / totalCells;

        var percentages = new Dictionary<PixelType, float>
        {
            { PixelType.Sand, sandPercentage },
            { PixelType.Water, waterPercentage },
            { PixelType.WetSand, wetSandPercentage },
            { PixelType.Fire, firePercentage },
            { PixelType.Steam, steamPercentage },
            { PixelType.Glass, glassPercentage },
        };

        // Draw base white circle
        spriteBatch.Draw(
            baseCircleTexture,
            center,
            null,
            Color.White,
            0f,
            new Vector2(baseCircleTexture.Width / 2, baseCircleTexture.Height / 2),
            1.05f,
            SpriteEffects.None,
            0f
        );

        float startRotation = -MathHelper.PiOver2;

        foreach (var kvp in percentages)
        {
            float sweepAngle = kvp.Value * MathHelper.TwoPi;
            float sliceStep = MathHelper.TwoPi / 360f;
            int slices = (int)(sweepAngle / sliceStep);

            for (int i = 0; i < slices; i++)
            {
                spriteBatch.Draw(
                    sliceTexture,
                    center,
                    null,
                    GetColorForPixelType(kvp.Key),
                    startRotation + +(i * sliceStep),
                    new Vector2(sliceTexture.Width / 2f, sliceTexture.Height),
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }
            startRotation += sweepAngle;
        }
    }

    private Color GetColorForPixelType(PixelType type)
    {
        return type switch
        {
            PixelType.Sand => new Color(225, 191, 146, 200),
            PixelType.Water => new Color(100, 149, 237, 200),
            PixelType.WetSand => new Color(216, 160, 28, 200),
            PixelType.Fire => new Color(255, 100, 0, 200),
            PixelType.Steam => new Color(255, 255, 255, 200),
            PixelType.Glass => new Color(175, 225, 225, 200),
            _ => Color.Gray,
        };
    }
}
