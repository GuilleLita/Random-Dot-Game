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
    private SkinDefinition skin;

    /// <summary>Number of colors of the selected skin (each active dot gets a different one)</summary>
    public static int ColorCount => SkinManager.Current.palette.Length;
   

    Vector3 scale;
    Vector3 scaleTarget;
    void Awake()
    {
        innerCircle = this.transform.GetChild(0).gameObject;
        outerCircle = this.transform.GetChild(1).gameObject;
        scale = transform.localScale;
        ApplySkin(SkinManager.Current);
    }

    void ApplySkin(SkinDefinition s)
    {
        skin = s;
        Image innerImage = innerCircle.GetComponent<Image>();
        Image outerImage = outerCircle.GetComponent<Image>();
        SkinVisuals.CaptureOriginals(innerImage.sprite, outerImage.sprite);
        innerImage.sprite = SkinVisuals.Inner(s);
        outerImage.sprite = SkinVisuals.Outer(s);
        degreesPerSec = s.rotationSpeed;
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
        SetColor(Random.Range(0, skin.palette.Length));
    }

    public void SetColor(int index)
    {
        Color color = skin.palette[Mathf.Clamp(index, 0, skin.palette.Length - 1)];
        innerCircle.GetComponent<Image>().color = color;
        outerCircle.GetComponent<Image>().color = color;
    }

    public void Uwin()
    {
        winner = true;
    }
}
