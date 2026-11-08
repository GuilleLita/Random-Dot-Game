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
    // Distinct, easy to tell apart colors (each active dot gets a different one)
    static readonly Color[] colors = new Color[]
    {
        new Color(0.0f, 0.4f, 1.0f),  // blue
        new Color(0.2f, 1.0f, 0.4f),  // green
        new Color(1.0f, 0.2f, 0.2f),  // red
        new Color(1.0f, 0.8f, 0.0f),  // yellow
        new Color(0.0f, 1.0f, 1.0f),  // cyan
        new Color(1.0f, 0.0f, 1.0f),  // magenta
        new Color(1.0f, 0.5f, 0.0f),  // orange
        new Color(0.6f, 0.3f, 1.0f),  // purple
        new Color(0.6f, 1.0f, 0.0f),  // lime
        new Color(1.0f, 0.4f, 0.7f),  // pink
        new Color(0.2f, 1.0f, 0.8f),  // turquoise
        new Color(1.0f, 1.0f, 1.0f)   // white
    };

    public static int ColorCount => colors.Length;
   

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
        scaleTarget = new Vector3(0.9f, 0.9f, 0.9f);
        
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
            // Frame-rate independent growth (0.6 units/second, same as 0.01/frame at 60 fps)
            for (int i = 0; i < 3; ++i) scale[i] = Mathf.Min(transform.localScale[i] + 0.6f * Time.deltaTime, scaleTarget[i]);
            transform.localScale = scale;
        }

        float rotAmount = degreesPerSec * Time.deltaTime;
        float curRot = outerCircle.transform.localRotation.eulerAngles.z;
        outerCircle.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, curRot + rotAmount));
    }

    public void SetRandomColor()
    {
        SetColor(Random.Range(0, colors.Length));
    }

    public void SetColor(int index)
    {
        Color color = colors[Mathf.Clamp(index, 0, colors.Length - 1)];
        innerCircle.GetComponent<Image>().color = color;
        outerCircle.GetComponent<Image>().color = color;
    }

    public void Uwin()
    {
        winner = true;
    }
}
