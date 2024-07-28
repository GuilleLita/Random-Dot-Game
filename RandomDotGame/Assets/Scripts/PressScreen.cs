using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressScreen : MonoBehaviour
{   
    public GameObject pressScreen;
    public GameObject settingsButton;
    public GameObject settingsButton2;
    public GameObject menuSetings;

    public GameObject dots;
    public bool inSettings = false;

    private bool isInSomething = false;

    // Start is called before the first frame update
    void Start()
    {
        dots = GameObject.Find("Dots");
        pressScreen = GameObject.Find("InitialText");
        //settingsButton = GameObject.Find("SettingsButton");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount >= 1)
        {          

            for (int i = 0; i < Input.touchCount; ++i)
            {
                Touch touch = Input.GetTouch(i);

                bool isInMenu = RectTransformUtility.RectangleContainsScreenPoint((RectTransform)menuSetings.transform, touch.position) || RectTransformUtility.RectangleContainsScreenPoint((RectTransform)settingsButton2.transform, touch.position);

                //If touch in settigns button, ignore
                //if ((RectTransformUtility.RectangleContainsScreenPoint((RectTransform)settingsButton.transform, touch.position) && settingsButton.activeInHierarchy) ||
                   if (isInMenu) {
                    //isInSomething = true;
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
                            if (inSettings)
                            {
                                settingsButton.GetComponent<MEnuAnimator>().CloseSettings();
                            }
                            else
                            {
                                menuSetings.SetActive(false);
                            }
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
                        } 
                        dots.GetComponent<DotsLogic>().RemoveDot(touch.fingerId);
                        
                        break;
                }
            }
            
        }
        else if (Input.touchCount == 0 && !inSettings)
        {
            pressScreen.SetActive(true);
            menuSetings.SetActive(true);
            if (dots.GetComponent<DotsLogic>().isWinner)
            {
                dots.GetComponent<DotsLogic>()._Reset();
            }
        }


   
    }
}

