using System;
using Microsoft.Xna.Framework;

public class HSLColour
{
    public float Hue;
    public float Saturation;
    public float Luminosity;

    public HSLColour(float H, float S, float L)
    {
        Hue = H;
        Saturation = S;
        Luminosity = L;
    }

    public static HSLColour FromRGB(byte R, byte G, byte B)
    {
        float _R = (R / 255f);
        float _G = (G / 255f);
        float _B = (B / 255f);

        float min = Math.Min(Math.Min(_R, _G), _B);
        float max = Math.Max(Math.Max(_R, _G), _B);
        float delta = max - min;

        float H = 0;
        float S = 0;
        float L = (float)((max + min) / 2.0f);

        if (delta != 0)
        {
            if (L < 0.5f)
            {
                S = (float)(delta / (max + min));
            }
            else
            {
                S = (float)(delta / (2.0f - max - min));
            }

            if (_R == max)
            {
                H = (_G - _B) / delta;
            }
            else if (_G == max)
            {
                H = 2f + (_B - _R) / delta;
            }
            else if (_B == max)
            {
                H = 4f + (_R - _G) / delta;
            }
        }
        H = H / 6f;
        if (H < 0f)
            H += 1f;
        return new HSLColour(H, S, L);
    }

    private float Hue_2_RGB(float v1, float v2, float vH)
    {
        if (vH < 0)
            vH += 1;
        if (vH > 1)
            vH -= 1;
        if ((6 * vH) < 1)
            return (v1 + (v2 - v1) * 6 * vH);
        if ((2 * vH) < 1)
            return (v2);
        if ((3 * vH) < 2)
            return (v1 + (v2 - v1) * ((2 / 3) - vH) * 6);
        return (v1);
    }

    public Color ToRGB()
    {
        byte r,
            g,
            b;

        double h = Hue / 360.0;

        if (Saturation == 0)
        {
            r = (byte)Math.Round(Luminosity * 255d);
            g = (byte)Math.Round(Luminosity * 255d);
            b = (byte)Math.Round(Luminosity * 255d);
        }
        else
        {
            double t1,
                t2;
            double th = h;

            if (Luminosity < 0.5d)
            {
                t2 = Luminosity * (1d + Saturation);
            }
            else
            {
                t2 = Luminosity + Saturation - (Luminosity * Saturation);
            }
            t1 = 2d * Luminosity - t2;

            double tr,
                tg,
                tb;
            tr = th + (1.0d / 3.0d);
            tg = th;
            tb = th - (1.0d / 3.0d);

            tr = ColorCalc(tr, t1, t2);
            tg = ColorCalc(tg, t1, t2);
            tb = ColorCalc(tb, t1, t2);
            r = (byte)Math.Round(tr * 255d);
            g = (byte)Math.Round(tg * 255d);
            b = (byte)Math.Round(tb * 255d);
        }
        return new Color(r, g, b);
    }

    private static double ColorCalc(double c, double t1, double t2)
    {
        if (c < 0)
            c += 1d;
        if (c > 1)
            c -= 1d;
        if (6.0d * c < 1.0d)
            return t1 + (t2 - t1) * 6.0d * c;
        if (2.0d * c < 1.0d)
            return t2;
        if (3.0d * c < 2.0d)
            return t1 + (t2 - t1) * (2.0d / 3.0d - c) * 6.0d;
        return t1;
    }
}
