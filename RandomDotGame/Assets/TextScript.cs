using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TextScript : MonoBehaviour
{
    float maxAmp;
    float minAmp;
    Vector3 scale;
    // Start is called before the first frame update
    void Start()
    {
        maxAmp = 1f;
        minAmp = 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 3; ++i) scale[i] = maxAmp + minAmp * (Mathf.Sin(Time.time) + 1);
        transform.localScale = scale;
    }
}
