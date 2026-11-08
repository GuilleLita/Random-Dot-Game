using System;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

/// <summary>
/// In-app purchases (Unity IAP) for the paid skins.
/// Every paid skin is a NON-CONSUMABLE product whose id is SkinDefinition.ProductId
/// ("skin_neon", "skin_pastel", ...). The products must exist with the same ids
/// in Google Play Console and App Store Connect.
/// </summary>
public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    public static IAPManager Instance { get; private set; }

    /// <summary>Raised when the store finishes initializing (prices become available).</summary>
    public event Action StateChanged;

    public bool IsInitialized => controller != null;
    public bool IsPurchasing => pendingCallback != null;

    /// <summary>Apple requires a "Restore purchases" button. Google restores automatically.</summary>
    public bool NeedsRestoreButton =>
        Application.platform == RuntimePlatform.IPhonePlayer ||
        Application.platform == RuntimePlatform.OSXPlayer;

    IStoreController controller;
    IExtensionProvider extensions;
    Action<bool, string> pendingCallback;
    string pendingProductId;
    bool initializing;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    async void Start()
    {
        try
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
                await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            // IAP still works without Unity Gaming Services, it only logs a warning
            Debug.LogWarning("Unity Services could not be initialized: " + e.Message);
        }
        InitializePurchasing();
    }

    void InitializePurchasing()
    {
        if (IsInitialized || initializing) return;
        initializing = true;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var skin in SkinCatalog.All)
        {
            if (!skin.free) builder.AddProduct(skin.ProductId, ProductType.NonConsumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>Localized price ("0,99 €") or null if the store is not ready.</summary>
    public string GetPrice(SkinDefinition skin)
    {
        var product = GetProduct(skin);
        return product != null && product.availableToPurchase ? product.metadata.localizedPriceString : null;
    }

    public bool CanBuy(SkinDefinition skin)
    {
        var product = GetProduct(skin);
        return product != null && product.availableToPurchase && !IsPurchasing;
    }

    /// <summary>Starts a purchase. Callback: (success, message for the user or null).</summary>
    public void Buy(SkinDefinition skin, Action<bool, string> callback)
    {
        if (!IsInitialized)
        {
            callback?.Invoke(false, "La tienda no está disponible. Revisa tu conexión.");
            InitializePurchasing();
            return;
        }
        if (IsPurchasing) return;

        var product = GetProduct(skin);
        if (product == null || !product.availableToPurchase)
        {
            callback?.Invoke(false, "Este producto no está disponible.");
            return;
        }

        pendingCallback = callback;
        pendingProductId = product.definition.id;
        controller.InitiatePurchase(product);
    }

    public void RestorePurchases(Action<bool, string> callback)
    {
        if (!IsInitialized)
        {
            callback?.Invoke(false, "La tienda no está disponible.");
            return;
        }
        // Restored products arrive through ProcessPurchase
        extensions.GetExtension<IAppleExtensions>().RestoreTransactions((ok, error) =>
            callback?.Invoke(ok, ok ? "Compras restauradas" : "No se pudieron restaurar las compras"));
    }

    Product GetProduct(SkinDefinition skin)
    {
        if (!IsInitialized || skin == null || skin.free) return null;
        return controller.products.WithID(skin.ProductId);
    }

    void CompletePending(string productId, bool ok, string message)
    {
        // Restored/deferred purchases of other products must not finish the current one
        if (pendingCallback == null || (productId != null && productId != pendingProductId)) return;
        var cb = pendingCallback;
        pendingCallback = null;
        pendingProductId = null;
        cb?.Invoke(ok, message);
    }

    // ------------------------------------------------------------- listener

    public void OnInitialized(IStoreController storeController, IExtensionProvider extensionProvider)
    {
        controller = storeController;
        extensions = extensionProvider;
        initializing = false;

        // Non-consumables already bought (reinstall / new device on Google Play)
        foreach (var product in controller.products.all)
        {
            if (product.hasReceipt) SkinManager.Unlock(SkinCatalog.GetByProductId(product.definition.id));
        }
        StateChanged?.Invoke();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        initializing = false;
        Debug.LogWarning($"IAP initialization failed: {error} {message}");
        StateChanged?.Invoke();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var skin = SkinCatalog.GetByProductId(purchaseEvent.purchasedProduct.definition.id);
        if (skin != null)
        {
            SkinManager.Unlock(skin);
            CompletePending(skin.ProductId, true, null);
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        CompletePending(product?.definition.id, false, MessageFor(failureReason));
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        CompletePending(product?.definition.id, false, MessageFor(failureDescription.reason));
    }

    static string MessageFor(PurchaseFailureReason reason)
    {
        switch (reason)
        {
            case PurchaseFailureReason.UserCancelled: return null; // no message needed
            case PurchaseFailureReason.DuplicateTransaction: return "Ya tenías esta skin";
            case PurchaseFailureReason.PaymentDeclined: return "El pago ha sido rechazado";
            default: return "No se pudo completar la compra";
        }
    }
}
