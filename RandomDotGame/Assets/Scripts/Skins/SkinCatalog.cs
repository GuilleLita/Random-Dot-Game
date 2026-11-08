using System;
using UnityEngine;

/// <summary>Shapes that can be drawn procedurally (no art needed).</summary>
public enum SkinShape
{
    Original,       // Sprite already assigned in the Dot prefab
    Circle,
    Ring,
    DashedRing,
    DottedRing,
    DoubleRing,
    RoundedSquare,
    SquareRing,
    Star,
    Hexagon,
    HexagonRing,
    Heart,
    Flower,
}

[Serializable]
public class SkinDefinition
{
    public string id;
    public string displayName;
    public SkinShape inner;
    public SkinShape outer;
    public Color[] palette;
    public float rotationSpeed = 110f;
    public bool free;

    /// <summary>
    /// Product id used in Google Play Console / App Store Connect.
    /// Create one NON-CONSUMABLE product per paid skin with exactly this id.
    /// </summary>
    public string ProductId => "skin_" + id;
}

/// <summary>
/// List of every skin in the game.
/// To use your own art instead of the procedural shapes put sprites in
/// Assets/Resources/Skins/ named "{id}_inner" and "{id}_outer".
/// </summary>
public static class SkinCatalog
{
    public const int AdsToUnlock = 3;

    static Color C(float r, float g, float b) => new Color(r, g, b, 1f);

    public static readonly SkinDefinition[] All = new SkinDefinition[]
    {
        new SkinDefinition
        {
            id = "clasico", displayName = "Clásico", free = true,
            inner = SkinShape.Original, outer = SkinShape.Original, rotationSpeed = 110f,
            palette = new[]
            {
                C(0.0f, 0.4f, 1.0f), C(0.2f, 1.0f, 0.4f), C(1.0f, 0.2f, 0.2f), C(1.0f, 0.8f, 0.0f),
                C(0.0f, 1.0f, 1.0f), C(1.0f, 0.0f, 1.0f), C(1.0f, 0.5f, 0.0f), C(0.6f, 0.3f, 1.0f),
                C(0.6f, 1.0f, 0.0f), C(1.0f, 0.4f, 0.7f), C(0.2f, 1.0f, 0.8f), C(1.0f, 1.0f, 1.0f),
            }
        },
        new SkinDefinition
        {
            id = "neon", displayName = "Neón",
            inner = SkinShape.Circle, outer = SkinShape.DoubleRing, rotationSpeed = 220f,
            palette = new[]
            {
                C(0.0f, 1.0f, 0.9f), C(1.0f, 0.0f, 0.8f), C(0.7f, 1.0f, 0.0f), C(1.0f, 0.9f, 0.0f),
                C(0.3f, 0.5f, 1.0f), C(1.0f, 0.3f, 0.0f), C(0.6f, 0.0f, 1.0f), C(0.0f, 1.0f, 0.3f),
            }
        },
        new SkinDefinition
        {
            id = "pastel", displayName = "Pastel",
            inner = SkinShape.Circle, outer = SkinShape.DashedRing, rotationSpeed = 70f,
            palette = new[]
            {
                C(1.0f, 0.70f, 0.75f), C(0.70f, 0.85f, 1.0f), C(0.75f, 1.0f, 0.80f), C(1.0f, 0.95f, 0.65f),
                C(0.85f, 0.75f, 1.0f), C(1.0f, 0.80f, 0.60f), C(0.65f, 1.0f, 0.95f), C(0.95f, 0.75f, 0.95f),
            }
        },
        new SkinDefinition
        {
            id = "pixel", displayName = "Cuadrado",
            inner = SkinShape.RoundedSquare, outer = SkinShape.SquareRing, rotationSpeed = 60f,
            palette = new[]
            {
                C(1.0f, 0.25f, 0.25f), C(0.25f, 0.6f, 1.0f), C(1.0f, 0.85f, 0.1f), C(0.3f, 0.9f, 0.4f),
                C(1.0f, 0.5f, 0.1f), C(0.7f, 0.35f, 1.0f), C(0.1f, 0.9f, 0.9f), C(1.0f, 0.4f, 0.75f),
            }
        },
        new SkinDefinition
        {
            id = "estrella", displayName = "Estrella",
            inner = SkinShape.Star, outer = SkinShape.DottedRing, rotationSpeed = 90f,
            palette = new[]
            {
                C(1.0f, 0.85f, 0.2f), C(1.0f, 0.6f, 0.1f), C(1.0f, 0.95f, 0.6f), C(1.0f, 0.4f, 0.3f),
                C(0.9f, 0.7f, 1.0f), C(0.6f, 0.9f, 1.0f), C(1.0f, 1.0f, 1.0f), C(0.7f, 1.0f, 0.5f),
            }
        },
        new SkinDefinition
        {
            id = "panal", displayName = "Panal",
            inner = SkinShape.Hexagon, outer = SkinShape.HexagonRing, rotationSpeed = 45f,
            palette = new[]
            {
                C(1.0f, 0.75f, 0.0f), C(0.2f, 0.8f, 1.0f), C(0.3f, 1.0f, 0.6f), C(0.5f, 0.5f, 1.0f),
                C(1.0f, 0.45f, 0.2f), C(0.0f, 0.9f, 0.8f), C(0.9f, 0.4f, 1.0f), C(0.8f, 1.0f, 0.3f),
            }
        },
        new SkinDefinition
        {
            id = "corazon", displayName = "Corazón",
            inner = SkinShape.Heart, outer = SkinShape.DottedRing, rotationSpeed = 80f,
            palette = new[]
            {
                C(1.0f, 0.15f, 0.3f), C(1.0f, 0.45f, 0.65f), C(0.9f, 0.1f, 0.5f), C(1.0f, 0.6f, 0.5f),
                C(0.75f, 0.3f, 1.0f), C(1.0f, 0.3f, 0.1f), C(1.0f, 0.75f, 0.85f), C(0.6f, 0.1f, 0.3f),
            }
        },
        new SkinDefinition
        {
            id = "flor", displayName = "Flor",
            inner = SkinShape.Flower, outer = SkinShape.DashedRing, rotationSpeed = 50f,
            palette = new[]
            {
                C(1.0f, 0.55f, 0.75f), C(1.0f, 0.85f, 0.25f), C(0.6f, 0.45f, 1.0f), C(0.45f, 0.9f, 0.45f),
                C(1.0f, 0.45f, 0.3f), C(0.4f, 0.75f, 1.0f), C(1.0f, 1.0f, 1.0f), C(0.95f, 0.6f, 1.0f),
            }
        },
    };

    public static SkinDefinition Default => All[0];

    public static SkinDefinition Get(string id)
    {
        foreach (var s in All) if (s.id == id) return s;
        return null;
    }

    public static SkinDefinition GetByProductId(string productId)
    {
        foreach (var s in All) if (s.ProductId == productId) return s;
        return null;
    }
}
