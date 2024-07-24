using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject MenuSettings;
    public GameObject pressScreen;
    // Start is called before the first frame update
    void Start()
    {
        //MenuSettings = GameObject.Find("MenuSettings");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OpenSettings()
    {
        pressScreen.GetComponent<PressScreen>().inSettings = true;
        MenuSettings.SetActive(true);
    }

    public void CloseSettings()
    {
        pressScreen.GetComponent<PressScreen>().inSettings = false;
        MenuSettings.SetActive(false);
    }
}
