using UnityEngine;

/// <summary>Fits a RectTransform to Screen.safeArea.</summary>
public class SafeAreaFitter : MonoBehaviour
{
    Rect lastSafeArea;
    Vector2Int lastScreen;

    void OnEnable() { Apply(); }
    void Update()
    {
        if (Screen.safeArea != lastSafeArea || Screen.width != lastScreen.x || Screen.height != lastScreen.y) Apply();
    }

    void Apply()
    {
        var rt = (RectTransform)transform;
        Rect safe = Screen.safeArea;
        lastSafeArea = safe;
        lastScreen = new Vector2Int(Screen.width, Screen.height);
        if (Screen.width <= 0 || Screen.height <= 0) return;

        rt.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
        rt.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
