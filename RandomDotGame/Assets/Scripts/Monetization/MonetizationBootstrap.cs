using UnityEngine;

/// <summary>
/// Creates the ad service, the IAP manager and the skin shop automatically when the
/// game starts, so nothing has to be added to the scene by hand.
/// If any of them is already in the scene, that one is used instead.
/// </summary>
public static class MonetizationBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        GameObject host = null;
        GameObject Host()
        {
            if (host == null)
            {
                host = new GameObject("Monetization");
                Object.DontDestroyOnLoad(host);
            }
            return host;
        }

        if (Object.FindFirstObjectByType<RewardedAdService>() == null) Host().AddComponent<RewardedAdService>();
        if (Object.FindFirstObjectByType<IAPManager>() == null) Host().AddComponent<IAPManager>();
        if (Object.FindFirstObjectByType<SkinShopUI>() == null) Host().AddComponent<SkinShopUI>();
    }
}
