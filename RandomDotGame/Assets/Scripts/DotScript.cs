using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotScript : MonoBehaviour
{

    private GameObject innerCircle;
    private GameObject outerCircle;
    private float degreesPerSec = 110f;

    float maxAmp;
    float minAmp;

    private bool winner = false;
    Color[] colors = new Color[]
        {
            new Color(0.0f, 0.4f, 1.0f),
            new Color(0.2f, 1.0f, 0.4f),
            new Color(1.0f, 0.2f, 0.2f),
            new Color(0.4f, 1.0f, 0.4f),
            new Color(0.0f, 1.0f, 1.0f),
            new Color(1.0f, 0.0f, 1.0f),
            new Color(1.0f, 0.8f, 0.0f),
            new Color(0.0f, 1.0f, 0.2f),
            new Color(0.6f, 1.0f, 0.0f),
            new Color(0.2f, 1.0f, 0.8f),
            new Color(1.0f, 0.6f, 0.2f),
            new Color(1.0f, 0.2f, 1.0f)
    };
   

    Vector3 scale;
    Vector3 scaleTarget;
    void Awake()
    {
        innerCircle = this.transform.GetChild(0).gameObject;
        outerCircle = this.transform.GetChild(1).gameObject;
        scale = transform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        maxAmp = 0.6f;
        minAmp = 0.05f;
        scaleTarget = new Vector3(0.8f, 0.8f, 1.6f);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!winner) { 
            for (int i = 0; i < 3; ++i) scale[i] = maxAmp + minAmp * (Mathf.Sin(Time.time) + 1);
            transform.localScale = scale;
        }
        else if(transform.localScale[2] < scaleTarget[2])
        {
            for (int i = 0; i < 3; ++i) scale[i] = transform.localScale[i] + 0.01f;
            transform.localScale = scale;
        }

        float rotAmount = degreesPerSec * Time.deltaTime;
        float curRot = outerCircle.transform.localRotation.eulerAngles.z;
        outerCircle.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, curRot + rotAmount));
    }

    public void SetRandomColor()
    {
        int index = Random.Range(0, colors.Length);
        Color color = colors[index];
        innerCircle.GetComponent<Image>().color = color;
        outerCircle.GetComponent<Image>().color = color;
    }

    public void Uwin()
    {
        winner = true;
    }
}
