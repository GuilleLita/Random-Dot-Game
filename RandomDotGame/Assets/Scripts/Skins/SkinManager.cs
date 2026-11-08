using System;
using UnityEngine;

/// <summary>
/// Stores which skins are owned, the ad progress of each one and the selected skin.
/// </summary>
public static class SkinManager
{
    const string SelectedKey = "skin_selected";
    const string OwnedPrefix = "skin_owned_";
    const string AdsPrefix = "skin_ads_";

    /// <summary>Raised when a skin is unlocked, selected or its ad progress changes.</summary>
    public static event Action Changed;

    public static SkinDefinition Current
    {
        get
        {
            var skin = SkinCatalog.Get(PlayerPrefs.GetString(SelectedKey, SkinCatalog.Default.id));
            return skin != null && IsOwned(skin) ? skin : SkinCatalog.Default;
        }
    }

    public static bool IsOwned(SkinDefinition skin)
    {
        return skin.free || PlayerPrefs.GetInt(OwnedPrefix + skin.id, 0) == 1;
    }

    public static bool IsSelected(SkinDefinition skin) => Current.id == skin.id;

    public static int AdsWatched(SkinDefinition skin)
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(AdsPrefix + skin.id, 0), 0, SkinCatalog.AdsToUnlock);
    }

    public static void Unlock(SkinDefinition skin)
    {
        if (skin == null || IsOwned(skin)) return;
        PlayerPrefs.SetInt(OwnedPrefix + skin.id, 1);
        PlayerPrefs.Save();
        Changed?.Invoke();
    }

    /// <summary>Adds one fully watched ad to the skin. Returns true if this unlocked it.</summary>
    public static bool RegisterAdWatched(SkinDefinition skin)
    {
        if (IsOwned(skin)) return false;
        int watched = AdsWatched(skin) + 1;
        PlayerPrefs.SetInt(AdsPrefix + skin.id, watched);
        PlayerPrefs.Save();

        if (watched >= SkinCatalog.AdsToUnlock)
        {
            Unlock(skin);
            return true;
        }
        Changed?.Invoke();
        return false;
    }

    public static void Select(SkinDefinition skin)
    {
        if (skin == null || !IsOwned(skin)) return;
        PlayerPrefs.SetString(SelectedKey, skin.id);
        PlayerPrefs.Save();
        Changed?.Invoke();
    }
}
