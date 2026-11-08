using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Resolves the sprites of a skin (custom art > procedural shape > original prefab sprite).</summary>
public static class SkinVisuals
{
    static Sprite originalInner, originalOuter;
    static readonly Dictionary<string, Sprite> customCache = new Dictionary<string, Sprite>();

    /// <summary>Remembers the sprites of the Dot prefab, used by skins with SkinShape.Original.</summary>
    public static void CaptureOriginals(Sprite inner, Sprite outer)
    {
        if (originalInner == null) originalInner = inner;
        if (originalOuter == null) originalOuter = outer;
    }

    /// <summary>Reads the original sprites from the Dot prefab (inner = child 0, outer = child 1).</summary>
    public static void CaptureOriginals(GameObject dotPrefab)
    {
        if (dotPrefab == null || dotPrefab.transform.childCount < 2) return;
        var inner = dotPrefab.transform.GetChild(0).GetComponent<Image>();
        var outer = dotPrefab.transform.GetChild(1).GetComponent<Image>();
        CaptureOriginals(inner != null ? inner.sprite : null, outer != null ? outer.sprite : null);
    }

    public static Sprite Inner(SkinDefinition skin) => Resolve(skin.id + "_inner", skin.inner, originalInner);
    public static Sprite Outer(SkinDefinition skin) => Resolve(skin.id + "_outer", skin.outer, originalOuter);

    static Sprite Resolve(string resourceName, SkinShape shape, Sprite original)
    {
        if (!customCache.TryGetValue(resourceName, out Sprite custom))
        {
            custom = Resources.Load<Sprite>("Skins/" + resourceName);
            customCache[resourceName] = custom;
        }
        if (custom != null) return custom;
        if (shape == SkinShape.Original) return original;
        return ProceduralSprites.Get(shape);
    }
}
