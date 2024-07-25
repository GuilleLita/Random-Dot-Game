using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressScreen : MonoBehaviour
{   
    public GameObject pressScreen;
    private GameObject settingsButton;
    public GameObject menuSetings;

    public GameObject dots;
    public bool inSettings = false;

    private bool isInSomething = false;

    // Start is called before the first frame update
    void Start()
    {
        dots = GameObject.Find("Dots");
        pressScreen = GameObject.Find("InitialText");
        settingsButton = GameObject.Find("SettingsButton");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount >= 1 && !inSettings)
        {          

            for (int i = 0; i < Input.touchCount; ++i)
            {
                Touch touch = Input.GetTouch(i);

                bool isInMenu = RectTransformUtility.RectangleContainsScreenPoint((RectTransform)menuSetings.transform, touch.position);

                //If touch in settigns button, ignore
                if ((RectTransformUtility.RectangleContainsScreenPoint((RectTransform)settingsButton.transform, touch.position) && settingsButton.activeInHierarchy) ||
                    (isInMenu && menuSetings.activeInHierarchy)) {
                    isInSomething = true;
                    return;
                }
                
                
                //Else, do touch logic
                switch (touch.phase)
                {
                    case TouchPhase.Began:

                        Debug.Log("Touch detected");
                        if (!dots.GetComponent<DotsLogic>().isWinner && !isInSomething) {
                            //Hide press screen and settings button if there is a touch
                            pressScreen.SetActive(false);
                            settingsButton.SetActive(false);
                            dots.GetComponent<DotsLogic>().AddDot(touch);
                        }
                        break;
                    case TouchPhase.Moved:
                        if (isInSomething) return;
                        Debug.Log("Touch moved");
                        if (!dots.GetComponent<DotsLogic>().isWinner) {
                            dots.GetComponent<DotsLogic>().MoveDot(touch);
                        }
                        else if (dots.GetComponent<DotsLogic>().isWinnnerFinger(touch))
                        {
                            dots.GetComponent<DotsLogic>().MoveDot(touch);
                        }   
                        
                        break;
                    case TouchPhase.Ended:
                        Debug.Log("Touch ended");
                        if (isInSomething) {
                            isInSomething = false;
                            return; 
                        } 
                        dots.GetComponent<DotsLogic>().RemoveDot(touch.fingerId);
                        
                        break;
                }
            }
            
        }
        else if (Input.touchCount == 0 && !inSettings)
        {
            pressScreen.SetActive(true);
            settingsButton.SetActive(true);
            if (dots.GetComponent<DotsLogic>().isWinner)
            {
                dots.GetComponent<DotsLogic>()._Reset();
            }
        }


   
    }
}

