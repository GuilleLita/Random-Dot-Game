using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] GameObject Camera;
    [SerializeField] GameObject BotonVibra;
    [SerializeField] GameObject BotonSonido;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("Vibracion", 1) == 1)
        {
            BotonVibra.transform.GetChild(0).gameObject.SetActive(true);
            BotonVibra.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            BotonVibra.transform.GetChild(0).gameObject.SetActive(false);
            BotonVibra.transform.GetChild(1).gameObject.SetActive(true);
        }

        if (PlayerPrefs.GetInt("Sonido", 1) == 1)
        {
            BotonSonido.transform.GetChild(0).gameObject.SetActive(true);
            BotonSonido.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            BotonSonido.transform.GetChild(0).gameObject.SetActive(false);
            BotonSonido.transform.GetChild(1).gameObject.SetActive(true);
        }

        Application.targetFrameRate = 60;
        Random.InitState(System.DateTime.Now.Millisecond);
        Camera.transform.position = new Vector3(Screen.width/2, Screen.height/2, -10);
        Camera.GetComponent<Camera>().orthographicSize = (float)Screen.height/2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
