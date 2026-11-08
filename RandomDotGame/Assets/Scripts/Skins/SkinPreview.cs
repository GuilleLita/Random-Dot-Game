using UnityEngine;
using UnityEngine.UI;

/// <summary>Animated preview of a skin (rotating outer ring, cycling palette colors).</summary>
public class SkinPreview : MonoBehaviour
{
    Image inner, outer;
    SkinDefinition skin;
    float colorTimer;
    int colorIndex;

    public static SkinPreview Create(Transform parent, float size)
    {
        var go = new GameObject("SkinPreview", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.sizeDelta = new Vector2(size, size);

        var preview = go.AddComponent<SkinPreview>();
        // Same proportions as the Dot prefab (inner 302 / outer 396)
        preview.inner = NewImage("Inner", rt, size * 302f / 396f);
        preview.outer = NewImage("Outer", rt, size);
        return preview;
    }

    static Image NewImage(string name, Transform parent, float size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        ((RectTransform)go.transform).sizeDelta = new Vector2(size, size);
        var img = go.GetComponent<Image>();
        img.raycastTarget = false;
        img.preserveAspect = true;
        return img;
    }

    public void SetSkin(SkinDefinition s)
    {
        if (skin == s) return;
        skin = s;
        inner.sprite = SkinVisuals.Inner(s);
        outer.sprite = SkinVisuals.Outer(s);
        colorIndex = 0;
        ApplyColor();
    }

    void ApplyColor()
    {
        if (skin == null || skin.palette == null || skin.palette.Length == 0) return;
        Color c = skin.palette[colorIndex % skin.palette.Length];
        inner.color = c;
        outer.color = c;
    }

    void Update()
    {
        if (skin == null) return;
        outer.rectTransform.Rotate(0f, 0f, skin.rotationSpeed * Time.unscaledDeltaTime);

        colorTimer += Time.unscaledDeltaTime;
        if (colorTimer > 0.9f)
        {
            colorTimer = 0f;
            colorIndex++;
            ApplyColor();
        }
    }
}
