using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] GameObject Camera;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Random.InitState(System.DateTime.Now.Millisecond);
        Camera.transform.position = new Vector3(Screen.width/2, Screen.height/2, -10);
        Camera.GetComponent<Camera>().orthographicSize = (float)Screen.width;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
