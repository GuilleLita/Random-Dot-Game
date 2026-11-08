# Random Dot Selector

Unity project for a mobile app where a random finger is selected, useful when you want to bet with your roommates who take out the trash.

<img src="https://github.com/user-attachments/assets/35d7c336-b8f2-40af-ba3a-fc417b0eed53" alt="Main Screen" width="200" height="400">
<img src="https://github.com/user-attachments/assets/8015cd38-39b5-438f-9bbf-a7eeb3a8a07f" alt="Main Screen" width="200" height="400">
<img src="https://github.com/user-attachments/assets/f02c9fe0-4c05-4d7d-ad6d-0e9a677accb9" alt="Dots Screen" width="200" height="400">


## Skins

Dot skins can be unlocked by buying them (in-app purchase) or by watching 3 rewarded ads.
The shop opens from the **Skins** button of the settings menu and is built from code
(`Assets/Scripts/Skins/SkinShopUI.cs`), so no scene setup is needed.

- **Add / edit skins:** `Assets/Scripts/Skins/SkinCatalog.cs`. Shapes are generated procedurally;
  to use your own art, add sprites to `Assets/Resources/Skins/` named `{id}_inner` and `{id}_outer`.
- **Store products:** create one *non-consumable* product per paid skin in Google Play Console /
  App Store Connect with id `skin_{id}` (`skin_neon`, `skin_pastel`, `skin_pixel`, `skin_estrella`,
  `skin_panal`, `skin_corazon`, `skin_flor`). The price shown in the shop comes from the store.
- **Ads:** uses the `Rewarded_Android` / `Rewarded_iOS` ad units of Unity Ads.
- Requires the Unity IAP package (`com.unity.purchasing`, already in `Packages/manifest.json`).
  Enable *In-App Purchasing* in *Project Settings > Services* and link the project.
