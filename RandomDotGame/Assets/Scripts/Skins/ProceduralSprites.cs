using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates anti-aliased white sprites from signed distance functions, so skins
/// can be tinted with any color and need no image assets.
/// Coordinates are normalized to [-1, 1] with y pointing up.
/// </summary>
public static class ProceduralSprites
{
    const int Size = 256;

    static readonly Dictionary<SkinShape, Sprite> cache = new Dictionary<SkinShape, Sprite>();
    static Sprite roundedRect;
    static Sprite circle;

    public static Sprite Get(SkinShape shape)
    {
        if (shape == SkinShape.Original) return null;
        if (cache.TryGetValue(shape, out Sprite s) && s != null) return s;
        s = Build(SdfFor(shape), Size, Vector4.zero);
        cache[shape] = s;
        return s;
    }

    /// <summary>White rounded rectangle, 9-sliced, for UI panels and buttons.</summary>
    public static Sprite RoundedRect()
    {
        if (roundedRect != null) return roundedRect;
        const int size = 96;
        const float radiusPx = 32f;
        float half = size / 2f;
        roundedRect = Build((x, y) =>
        {
            // work in pixels, then convert back to normalized units
            float px = x * half, py = y * half;
            float qx = Mathf.Abs(px) - (half - radiusPx);
            float qy = Mathf.Abs(py) - (half - radiusPx);
            float d = new Vector2(Mathf.Max(qx, 0), Mathf.Max(qy, 0)).magnitude + Mathf.Min(Mathf.Max(qx, qy), 0) - (radiusPx - 1f);
            return d / half;
        }, size, new Vector4(radiusPx, radiusPx, radiusPx, radiusPx));
        return roundedRect;
    }

    public static Sprite Circle()
    {
        if (circle != null) return circle;
        circle = Build((x, y) => new Vector2(x, y).magnitude - 0.96f, 128, Vector4.zero);
        return circle;
    }

    static Sprite Build(Func<float, float, float> sdf, int size, Vector4 border)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.hideFlags = HideFlags.DontUnloadUnusedAsset;

        var pixels = new Color32[size * size];
        float pixel = 2f / size; // one pixel in normalized units
        for (int y = 0; y < size; ++y)
        {
            float ny = (y + 0.5f) / size * 2f - 1f;
            for (int x = 0; x < size; ++x)
            {
                float nx = (x + 0.5f) / size * 2f - 1f;
                float d = sdf(nx, ny);
                float a = Mathf.Clamp01(0.5f - d / pixel);
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(a * 255f));
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply(false, true);

        var sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
        sprite.hideFlags = HideFlags.DontUnloadUnusedAsset;
        return sprite;
    }

    // ---------------------------------------------------------------- shapes

    static Func<float, float, float> SdfFor(SkinShape shape)
    {
        switch (shape)
        {
            case SkinShape.Circle: return (x, y) => Len(x, y) - 0.94f;
            case SkinShape.Ring: return (x, y) => Mathf.Abs(Len(x, y) - 0.88f) - 0.06f;
            case SkinShape.DashedRing: return (x, y) => DashedRing(x, y, 0.88f, 0.06f, 12, 0.6f);
            case SkinShape.DottedRing: return (x, y) => DottedRing(x, y, 0.88f, 0.08f, 14);
            case SkinShape.DoubleRing:
                return (x, y) => Mathf.Min(Mathf.Abs(Len(x, y) - 0.91f) - 0.035f,
                                           DashedRing(x, y, 0.81f, 0.025f, 8, 0.7f));
            case SkinShape.RoundedSquare: return (x, y) => RoundBox(x, y, 0.84f, 0.22f);
            case SkinShape.SquareRing: return (x, y) => Mathf.Abs(RoundBox(x, y, 0.86f, 0.24f) + 0.06f) - 0.06f;
            case SkinShape.Star: return (x, y) => Star5(x, y + 0.06f, 0.98f, 0.5f);
            case SkinShape.Hexagon: return (x, y) => Hexagon(x, y, 0.72f) - 0.08f;
            case SkinShape.HexagonRing: return (x, y) => Mathf.Abs(Hexagon(x, y, 0.74f) - 0.06f) - 0.05f;
            case SkinShape.Heart: return Heart;
            case SkinShape.Flower: return Flower;
            default: return (x, y) => Len(x, y) - 0.94f;
        }
    }

    static float Len(float x, float y) => Mathf.Sqrt(x * x + y * y);

    static float Repeat(float v, float period) => v - period * Mathf.Floor(v / period);

    static float DashedRing(float x, float y, float radius, float thickness, int dashes, float duty)
    {
        float r = Len(x, y);
        float ringD = Mathf.Abs(r - radius) - thickness;
        float seg = Mathf.PI * 2f / dashes;
        float ang = Repeat(Mathf.Atan2(y, x), seg) - seg * 0.5f;      // angle from dash center
        float arcD = (Mathf.Abs(ang) - seg * duty * 0.5f) * radius;    // distance along the arc
        return Mathf.Max(ringD, arcD);
    }

    static float DottedRing(float x, float y, float radius, float dotRadius, int count)
    {
        float seg = Mathf.PI * 2f / count;
        float r = Len(x, y);
        float ang = Repeat(Mathf.Atan2(y, x) + seg * 0.5f, seg) - seg * 0.5f;
        float qx = r * Mathf.Cos(ang) - radius;
        float qy = r * Mathf.Sin(ang);
        return Len(qx, qy) - dotRadius;
    }

    static float RoundBox(float x, float y, float halfSize, float radius)
    {
        float qx = Mathf.Abs(x) - (halfSize - radius);
        float qy = Mathf.Abs(y) - (halfSize - radius);
        return Len(Mathf.Max(qx, 0), Mathf.Max(qy, 0)) + Mathf.Min(Mathf.Max(qx, qy), 0) - radius;
    }

    // Inigo Quilez' 2D SDFs (https://iquilezles.org/articles/distfunctions2d/)
    static float Star5(float x, float y, float r, float rf)
    {
        var k1 = new Vector2(0.809016994375f, -0.587785252292f);
        var k2 = new Vector2(-k1.x, k1.y);
        var p = new Vector2(Mathf.Abs(x), y);
        p -= 2f * Mathf.Max(Vector2.Dot(k1, p), 0f) * k1;
        p -= 2f * Mathf.Max(Vector2.Dot(k2, p), 0f) * k2;
        p.x = Mathf.Abs(p.x);
        p.y -= r;
        var ba = rf * new Vector2(-k1.y, k1.x) - new Vector2(0, 1);
        float h = Mathf.Clamp(Vector2.Dot(p, ba) / Vector2.Dot(ba, ba), 0f, r);
        return (p - ba * h).magnitude * Mathf.Sign(p.y * ba.x - p.x * ba.y);
    }

    static float Hexagon(float x, float y, float r)
    {
        const float kx = -0.866025404f, ky = 0.5f, kz = 0.577350269f;
        var p = new Vector2(Mathf.Abs(x), Mathf.Abs(y));
        float d = Mathf.Min(kx * p.x + ky * p.y, 0f);
        p -= 2f * d * new Vector2(kx, ky);
        p -= new Vector2(Mathf.Clamp(p.x, -kz * r, kz * r), r);
        return p.magnitude * Mathf.Sign(p.y);
    }

    static float Heart(float x, float y)
    {
        const float scale = 0.66f;         // heart units per normalized unit
        const float offsetY = 0.55f;       // vertically center the heart
        var p = new Vector2(Mathf.Abs(x) * scale, y * scale + offsetY);
        float d;
        if (p.y + p.x > 1f)
        {
            d = (p - new Vector2(0.25f, 0.75f)).magnitude - Mathf.Sqrt(2f) / 4f;
        }
        else
        {
            float a = (p - new Vector2(0f, 1f)).sqrMagnitude;
            float b = (p - 0.5f * Mathf.Max(p.x + p.y, 0f) * Vector2.one).sqrMagnitude;
            d = Mathf.Sqrt(Mathf.Min(a, b)) * Mathf.Sign(p.x - p.y);
        }
        return d / scale;
    }

    static float Flower(float x, float y)
    {
        float r = Len(x, y);
        float a = Mathf.Atan2(y, x);
        float petals = 0.62f + 0.32f * Mathf.Abs(Mathf.Cos(a * 2.5f));
        // not an exact distance, scaled down so the anti-aliasing stays smooth
        return (r - petals) * 0.8f;
    }
}
