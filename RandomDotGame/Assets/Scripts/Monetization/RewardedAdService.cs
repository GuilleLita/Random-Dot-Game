using System;
using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// Loads and shows rewarded ads and tells the caller whether the ad was fully watched.
/// Used by the skin shop ("watch 3 ads to unlock").
/// </summary>
public class RewardedAdService : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static RewardedAdService Instance { get; private set; }

    [SerializeField] string androidAdUnitId = "Rewarded_Android";
    [SerializeField] string iOSAdUnitId = "Rewarded_iOS";

    /// <summary>Raised when an ad becomes ready or stops being ready.</summary>
    public event Action ReadyChanged;

    public bool IsReady => loaded && !showing;

    string adUnitId;
    bool loaded, loading, showing;
    float nextLoadTime;
    Action<bool> pendingCallback;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
#if UNITY_IOS
        adUnitId = iOSAdUnitId;
#else
        adUnitId = androidAdUnitId; // Android and Editor
#endif
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (!loaded && !loading && !showing && Advertisement.isInitialized && Time.unscaledTime >= nextLoadTime)
        {
            loading = true;
            Advertisement.Load(adUnitId, this);
        }
    }

    /// <summary>
    /// Shows a rewarded ad. The callback receives true only if the user watched it completely.
    /// Returns false (and does not call the callback) if no ad is ready.
    /// </summary>
    public bool Show(Action<bool> onFinished)
    {
        if (!IsReady) return false;
        pendingCallback = onFinished;
        showing = true;
        loaded = false;
        ReadyChanged?.Invoke();
        Advertisement.Show(adUnitId, this);
        return true;
    }

    void Finish(bool completed)
    {
        showing = false;
        var cb = pendingCallback;
        pendingCallback = null;
        cb?.Invoke(completed);
        ReadyChanged?.Invoke();
    }

    // ------------------------------------------------------------- listeners

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId != adUnitId) return;
        loading = false;
        loaded = true;
        ReadyChanged?.Invoke();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        if (placementId != adUnitId) return;
        Debug.Log($"Rewarded ad failed to load: {error} - {message}");
        loading = false;
        nextLoadTime = Time.unscaledTime + 15f; // retry later
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState state)
    {
        if (placementId != adUnitId) return;
        Finish(state == UnityAdsShowCompletionState.COMPLETED);
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        if (placementId != adUnitId) return;
        Debug.Log($"Rewarded ad failed to show: {error} - {message}");
        Finish(false);
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }
}
