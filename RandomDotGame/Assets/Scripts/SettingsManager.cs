using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject MenuSettings;
    public GameObject pressScreen;
    [SerializeField] GameObject BotonVibracion;
    [SerializeField] GameObject BotonSonido;
    // Start is called before the first frame update
    void Start()
    {
        //MenuSettings = GameObject.Find("MenuSettings");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Vibracion()
    {
        if (PlayerPrefs.GetInt("Vibracion", 1) == 1)
        {
            PlayerPrefs.SetInt("Vibracion", 0);
            BotonVibracion.transform.GetChild(0).gameObject.SetActive(false);
            BotonVibracion.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("Vibracion", 1);
            BotonVibracion.transform.GetChild(0).gameObject.SetActive(true);
            BotonVibracion.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void Sonido()
    {
        if (PlayerPrefs.GetInt("Sonido", 1) == 1)
        {
            PlayerPrefs.SetInt("Sonido", 0);
            BotonSonido.transform.GetChild(0).gameObject.SetActive(false);
            BotonSonido.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("Sonido", 1);
            BotonSonido.transform.GetChild(0).gameObject.SetActive(true);
            BotonSonido.transform.GetChild(1).gameObject.SetActive(false);
        }
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
