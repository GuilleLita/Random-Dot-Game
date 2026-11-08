using UnityEngine;

public class PressScreen : MonoBehaviour
{
    public GameObject pressScreen;
    public GameObject settingsButton;
    public GameObject settingsButton2;
    public GameObject menuSetings;

    public GameObject dots;
    public bool inSettings = false;

    private DotsLogic dotsLogic;
    private MEnuAnimator menuAnimator;

    void Start()
    {
        dots = GameObject.Find("Dots");
        pressScreen = GameObject.Find("InitialText");
        dotsLogic = dots.GetComponent<DotsLogic>();
        menuAnimator = settingsButton != null ? settingsButton.GetComponent<MEnuAnimator>() : null;
    }

    private bool IsInMenu(Vector2 position)
    {
        return (menuSetings.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint((RectTransform)menuSetings.transform, position))
            || (settingsButton2.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint((RectTransform)settingsButton2.transform, position));
    }

    void Update()
    {
        int touchCount = Input.touchCount;

        for (int i = 0; i < touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Touches that start on the menu belong to the UI, ignore them
                    // (but keep processing the rest of the touches of this frame).
                    if (dotsLogic.isWinner || SkinShopUI.IsOpen || IsInMenu(touch.position)) break;

                    // Hide press screen and settings when the game starts
                    pressScreen.SetActive(false);
                    if (inSettings && menuAnimator != null) menuAnimator.CloseSettings();
                    else menuSetings.SetActive(false);

                    dotsLogic.AddDot(touch);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (!dotsLogic.isWinner || dotsLogic.IsWinnerFinger(touch.fingerId))
                        dotsLogic.MoveDot(touch);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    // Canceled is sent by the OS when many fingers touch at once,
                    // on palm rejection, etc. Not handling it left dots stuck on screen.
                    dotsLogic.RemoveDot(touch.fingerId);
                    break;
            }
        }

        // Safety net: drop any dot whose finger is no longer on the screen
        dotsLogic.SyncWithTouches();

        if (touchCount == 0)
        {
            if (dotsLogic.isWinner) dotsLogic._Reset();

            if (!inSettings)
            {
                pressScreen.SetActive(true);
                menuSetings.SetActive(true);
            }
        }
    }
}
