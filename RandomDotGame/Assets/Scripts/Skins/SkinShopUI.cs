using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Skin shop. The whole UI is built from code, so it needs no scene setup:
/// it opens with the "ButtonSkins" button of the settings menu (found by name).
/// </summary>
public class SkinShopUI : MonoBehaviour
{
    public static bool IsOpen { get; private set; }

    [Tooltip("Name of the existing scene button that opens the shop")]
    [SerializeField] string openButtonName = "ButtonSkins";
    [Tooltip("Optional font. If empty Unity's built-in font is used")]
    [SerializeField] Font font;

    static readonly Color BgColor = new Color(0.05f, 0.05f, 0.08f, 0.94f);
    static readonly Color CardColor = new Color(1f, 1f, 1f, 0.08f);
    static readonly Color AccentColor = new Color(0.2f, 0.85f, 0.5f, 1f);
    static readonly Color BuyColor = new Color(1f, 0.75f, 0.1f, 1f);
    static readonly Color AdColor = new Color(0.3f, 0.55f, 1f, 1f);
    static readonly Color NeutralColor = new Color(1f, 1f, 1f, 0.18f);

    class Card
    {
        public SkinDefinition skin;
        public Image background;
        public Button buy, ad, use;
        public Text buyLabel, adLabel, useLabel;
    }

    readonly List<Card> cards = new List<Card>();
    GameObject root;
    Text toast;
    Image toastBackground;
    float toastTime;
    Button restoreButton;
    Sprite rounded;

    void Start()
    {
        if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rounded = ProceduralSprites.RoundedRect();

        var dotsLogic = FindFirstObjectByType<DotsLogic>();
        if (dotsLogic != null) SkinVisuals.CaptureOriginals(dotsLogic.dot);

        EnsureEventSystem();
        BuildUI();
        root.SetActive(false);
        HookOpenButton();

        SkinManager.Changed += Refresh;
        if (IAPManager.Instance != null) IAPManager.Instance.StateChanged += Refresh;
        if (RewardedAdService.Instance != null) RewardedAdService.Instance.ReadyChanged += Refresh;
    }

    void OnDestroy()
    {
        SkinManager.Changed -= Refresh;
        if (IAPManager.Instance != null) IAPManager.Instance.StateChanged -= Refresh;
        if (RewardedAdService.Instance != null) RewardedAdService.Instance.ReadyChanged -= Refresh;
        IsOpen = false;
    }

    public void Open()
    {
        if (root == null) return;
        root.SetActive(true);
        IsOpen = true;
        Refresh();
    }

    public void Close()
    {
        if (root == null) return;
        root.SetActive(false);
        IsOpen = false;
    }

    void Update()
    {
        if (!IsOpen) return;

        // Android back button
        if (Input.GetKeyDown(KeyCode.Escape)) Close();

        if (toastBackground.gameObject.activeSelf)
        {
            toastTime -= Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(toastTime / 0.4f);
            toast.color = new Color(1f, 1f, 1f, alpha);
            toastBackground.color = new Color(0f, 0f, 0f, 0.85f * alpha);
            if (toastTime <= 0f) toastBackground.gameObject.SetActive(false);
        }
    }

    void HookOpenButton()
    {
        foreach (var b in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (b.name == openButtonName)
            {
                b.onClick.AddListener(Open);
                return;
            }
        }
        Debug.LogWarning($"SkinShopUI: no button named '{openButtonName}' found, call SkinShopUI.Open() from your own button.");
    }

    static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    // ------------------------------------------------------------------ actions

    void OnBuy(SkinDefinition skin)
    {
        if (IAPManager.Instance == null) { ShowToast("La tienda no está disponible"); return; }
        IAPManager.Instance.Buy(skin, (ok, message) =>
        {
            if (ok)
            {
                SkinManager.Select(skin);
                ShowToast($"¡{skin.displayName} desbloqueada!");
            }
            else if (!string.IsNullOrEmpty(message))
            {
                ShowToast(message);
            }
            Refresh();
        });
        Refresh();
    }

    void OnWatchAd(SkinDefinition skin)
    {
        var ads = RewardedAdService.Instance;
        if (ads == null || !ads.Show(completed => OnAdFinished(skin, completed)))
        {
            ShowToast("No hay anuncios disponibles ahora mismo. Prueba en un momento.");
        }
    }

    void OnAdFinished(SkinDefinition skin, bool completed)
    {
        if (!completed)
        {
            ShowToast("Tienes que ver el anuncio completo");
            return;
        }
        if (SkinManager.RegisterAdWatched(skin))
        {
            SkinManager.Select(skin);
            ShowToast($"¡{skin.displayName} desbloqueada!");
        }
        else
        {
            int left = SkinCatalog.AdsToUnlock - SkinManager.AdsWatched(skin);
            ShowToast(left == 1 ? "¡Te queda 1 anuncio!" : $"¡Te quedan {left} anuncios!");
        }
    }

    void OnRestore()
    {
        if (IAPManager.Instance == null) return;
        IAPManager.Instance.RestorePurchases((ok, message) => ShowToast(message));
    }

    void ShowToast(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        toast.text = message;
        toast.color = Color.white;
        toastBackground.color = new Color(0f, 0f, 0f, 0.85f);
        toastBackground.gameObject.SetActive(true);
        toastTime = 2.5f;
    }

    void Refresh()
    {
        if (root == null || !root.activeSelf) return;

        var iap = IAPManager.Instance;
        var ads = RewardedAdService.Instance;
        bool adReady = ads != null && ads.IsReady;

        foreach (var card in cards)
        {
            var skin = card.skin;
            bool owned = SkinManager.IsOwned(skin);
            bool selected = SkinManager.IsSelected(skin);

            card.background.color = selected ? new Color(AccentColor.r, AccentColor.g, AccentColor.b, 0.25f) : CardColor;

            card.use.gameObject.SetActive(owned);
            card.buy.gameObject.SetActive(!owned);
            card.ad.gameObject.SetActive(!owned);

            if (owned)
            {
                card.use.interactable = !selected;
                card.useLabel.text = selected ? "EN USO" : "USAR";
                continue;
            }

            string price = iap != null ? iap.GetPrice(skin) : null;
            card.buy.interactable = iap != null && iap.CanBuy(skin);
            card.buyLabel.text = price != null ? "Comprar " + price : "Comprar";

            int watched = SkinManager.AdsWatched(skin);
            card.ad.interactable = adReady;
            card.adLabel.text = $"Ver anuncio {watched}/{SkinCatalog.AdsToUnlock}";
        }

        if (restoreButton != null)
            restoreButton.gameObject.SetActive(iap != null && iap.NeedsRestoreButton);
    }

    // --------------------------------------------------------------- UI building

    void BuildUI()
    {
        var canvasGo = new GameObject("SkinShopCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0f; // always 1080 units wide

        // Dark background that blocks touches to the game
        var bg = NewRect("SkinShop", canvasGo.transform);
        Stretch(bg);
        AddImage(bg, BgColor, null);
        root = bg.gameObject;

        // Content inside the safe area (notches, rounded corners)
        var safe = NewRect("SafeArea", bg);
        safe.gameObject.AddComponent<SafeAreaFitter>();

        // Header
        var title = NewRect("Title", safe);
        title.anchorMin = new Vector2(0, 1); title.anchorMax = new Vector2(1, 1);
        title.pivot = new Vector2(0.5f, 1);
        title.anchoredPosition = new Vector2(0, -30);
        title.sizeDelta = new Vector2(-300, 140);
        var titleText = AddText(title, "SKINS", 80, TextAnchor.MiddleCenter, Color.white);
        titleText.fontStyle = FontStyle.Bold;

        var close = MakeButton(safe, "X", NeutralColor, 64, out _);
        var closeRt = (RectTransform)close.transform;
        closeRt.anchorMin = closeRt.anchorMax = new Vector2(1, 1);
        closeRt.pivot = new Vector2(1, 1);
        closeRt.anchoredPosition = new Vector2(-40, -40);
        closeRt.sizeDelta = new Vector2(120, 120);
        close.onClick.AddListener(Close);

        // Scrollable grid of skins
        var scrollRt = NewRect("Scroll", safe);
        Stretch(scrollRt, 0, 170, 0, 200);
        var scroll = scrollRt.gameObject.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Elastic;
        scroll.scrollSensitivity = 40f;

        var viewport = NewRect("Viewport", scrollRt);
        Stretch(viewport);
        viewport.gameObject.AddComponent<RectMask2D>();
        AddImage(viewport, new Color(0, 0, 0, 0), null); // receives drags

        var content = NewRect("Content", viewport);
        content.anchorMin = new Vector2(0, 1); content.anchorMax = new Vector2(1, 1);
        content.pivot = new Vector2(0.5f, 1);
        content.sizeDelta = Vector2.zero;
        var grid = content.gameObject.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(470, 640);
        grid.spacing = new Vector2(40, 40);
        grid.padding = new RectOffset(0, 0, 20, 40);
        grid.childAlignment = TextAnchor.UpperCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 2;
        content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewport;
        scroll.content = content;

        foreach (var skin in SkinCatalog.All) cards.Add(BuildCard(content, skin));

        // Footer: restore purchases (iOS) + toast
        restoreButton = MakeButton(safe, "Restaurar compras", NeutralColor, 40, out _);
        var restoreRt = (RectTransform)restoreButton.transform;
        restoreRt.anchorMin = restoreRt.anchorMax = new Vector2(0.5f, 0);
        restoreRt.pivot = new Vector2(0.5f, 0);
        restoreRt.anchoredPosition = new Vector2(0, 50);
        restoreRt.sizeDelta = new Vector2(500, 100);
        restoreButton.onClick.AddListener(OnRestore);

        var toastRt = NewRect("Toast", safe);
        toastRt.anchorMin = new Vector2(0, 0); toastRt.anchorMax = new Vector2(1, 0);
        toastRt.pivot = new Vector2(0.5f, 0);
        toastRt.anchoredPosition = new Vector2(0, 170);
        toastRt.sizeDelta = new Vector2(-80, 120);
        toastBackground = AddImage(toastRt, new Color(0, 0, 0, 0.85f), rounded);
        toastBackground.raycastTarget = false;
        var toastTextRt = NewRect("Text", toastRt);
        Stretch(toastTextRt, 30, 10, 30, 10);
        toast = AddText(toastTextRt, "", 44, TextAnchor.MiddleCenter, Color.white);
        toastBackground.gameObject.SetActive(false);
    }

    Card BuildCard(Transform parent, SkinDefinition skin)
    {
        var card = new Card { skin = skin };
        var rt = NewRect("Skin_" + skin.id, parent);
        card.background = AddImage(rt, CardColor, rounded);

        var preview = SkinPreview.Create(rt, 280);
        var prt = (RectTransform)preview.transform;
        prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 1);
        prt.pivot = new Vector2(0.5f, 1);
        prt.anchoredPosition = new Vector2(0, -30);
        preview.SetSkin(skin);

        var name = NewRect("Name", rt);
        name.anchorMin = new Vector2(0, 1); name.anchorMax = new Vector2(1, 1);
        name.pivot = new Vector2(0.5f, 1);
        name.anchoredPosition = new Vector2(0, -320);
        name.sizeDelta = new Vector2(-20, 70);
        var nameText = AddText(name, skin.displayName, 50, TextAnchor.MiddleCenter, Color.white);
        nameText.fontStyle = FontStyle.Bold;

        card.use = MakeCardButton(rt, AccentColor, 30, out card.useLabel);
        card.use.onClick.AddListener(() => SkinManager.Select(skin));

        card.buy = MakeCardButton(rt, BuyColor, 135, out card.buyLabel);
        card.buy.onClick.AddListener(() => OnBuy(skin));

        card.ad = MakeCardButton(rt, AdColor, 30, out card.adLabel);
        card.ad.onClick.AddListener(() => OnWatchAd(skin));

        return card;
    }

    Button MakeCardButton(Transform parent, Color color, float bottom, out Text label)
    {
        var b = MakeButton(parent, "", color, 40, out label);
        var brt = (RectTransform)b.transform;
        brt.anchorMin = new Vector2(0, 0); brt.anchorMax = new Vector2(1, 0);
        brt.pivot = new Vector2(0.5f, 0);
        brt.anchoredPosition = new Vector2(0, bottom);
        brt.sizeDelta = new Vector2(-50, 95);
        label.color = new Color(0.08f, 0.08f, 0.1f);
        label.fontStyle = FontStyle.Bold;
        return b;
    }

    Button MakeButton(Transform parent, string text, Color color, int fontSize, out Text label)
    {
        var rt = NewRect("Button", parent);
        var img = AddImage(rt, color, rounded);
        var button = rt.gameObject.AddComponent<Button>();
        button.targetGraphic = img;
        var colors = button.colors;
        colors.disabledColor = new Color(0.6f, 0.6f, 0.6f, 0.45f);
        button.colors = colors;

        var labelRt = NewRect("Label", rt);
        Stretch(labelRt, 10, 0, 10, 0);
        label = AddText(labelRt, text, fontSize, TextAnchor.MiddleCenter, Color.white);
        return button;
    }

    static RectTransform NewRect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    static void Stretch(RectTransform rt, float left = 0, float bottom = 0, float right = 0, float top = 0)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
    }

    static Image AddImage(RectTransform rt, Color color, Sprite sprite)
    {
        var img = rt.gameObject.AddComponent<Image>();
        img.color = color;
        if (sprite != null)
        {
            img.sprite = sprite;
            img.type = Image.Type.Sliced;
        }
        return img;
    }

    Text AddText(RectTransform rt, string text, int size, TextAnchor anchor, Color color)
    {
        var t = rt.gameObject.AddComponent<Text>();
        t.font = font;
        t.text = text;
        t.fontSize = size;
        t.alignment = anchor;
        t.color = color;
        t.raycastTarget = false;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        t.resizeTextForBestFit = true;
        t.resizeTextMinSize = 20;
        t.resizeTextMaxSize = size;
        return t;
    }
}
