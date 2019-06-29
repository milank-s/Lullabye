using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLights : MonoBehaviour
{
    public Light[] lights;
    public Camera c;
    public Gradient lightColor;

    public float intensityMult = 2;
    public float speed;

    public AnimationCurve intensity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
//        c.nearClipPlane = 0.5f + Mathf.Sin(Time.time) * 10f;
        
        for (int i = 0; i < lights.Length; i++)
        {
            float val = Mathf.PingPong(speed * Time.time + i, 1);
            lights[i].color = lightColor.Evaluate(val);
            lights[i].intensity = intensity.Evaluate(val) * intensityMult;
        }
    }
}
