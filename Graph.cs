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

    public void DrawGraph(Vector2 center, int totalCells, int sandCount, int waterCount, int wetSandCount)
    {
        if (totalCells <= 0) return;

        float sandPercentage = (float)sandCount / totalCells;
        float waterPercentage = (float)waterCount / totalCells;
        float wetSandPercentage = (float)wetSandCount / totalCells;

        var percentages = new Dictionary<PixelType, float>
        {
            { PixelType.Sand, sandPercentage },
            { PixelType.Water, waterPercentage },
            { PixelType.WetSand, wetSandPercentage }
        };

        // Draw base white circle
        spriteBatch.Draw(
            baseCircleTexture,
            center,
            null,
            Color.White,
            0f,
            new Vector2(baseCircleTexture.Width / 2, baseCircleTexture.Height / 2),
            1f,
            SpriteEffects.None,
            0f
        );

        float startRotation = -MathHelper.PiOver2;

        foreach (var kvp in percentages)
        {
            float sweepAngle = kvp.Value * MathHelper.TwoPi;

            spriteBatch.Draw(
                sliceTexture,
                center,
                null,
                GetColorForPixelType(kvp.Key),
                startRotation,
                new Vector2(sliceTexture.Width / 2f, sliceTexture.Height),
                1f,
                SpriteEffects.None,
                0f
            );

            startRotation += sweepAngle;
        }
    }

    private Color GetColorForPixelType(PixelType type)
    {
        return type switch
        {
            PixelType.Sand => Color.Yellow,
            PixelType.Water => Color.Blue,
            PixelType.WetSand => new Color(216, 160, 28),
            _ => Color.Gray,
        };
    }
}
