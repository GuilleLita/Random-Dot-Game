using UnityEngine;

public class MEnuAnimator : MonoBehaviour
{

    public GameObject MenuSettings;
    public GameObject pressScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSettings()
    {
        //pressScreen.GetComponent<PressScreen>().inSettings = !pressScreen.GetComponent<PressScreen>().inSettings;
        if (!pressScreen.GetComponent<PressScreen>().inSettings)
        {
            pressScreen.GetComponent<PressScreen>().inSettings = true;  
            MenuSettings.GetComponent<Animation>().Play("Open");
            }
        else
        {
            MenuSettings.GetComponent<Animation>().Play("Close");
            //pressScreen.GetComponent<PressScreen>().inSettings = false;
        }
        
    }

    public void CloseSettings()
    {
        
        MenuSettings.GetComponent<Animation>().Play("Close");
        //this.gameObject.SetActive(false);
    }

    public void FalseActiveParent()
    {
        pressScreen.GetComponent<PressScreen>().inSettings = false;
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }
}
