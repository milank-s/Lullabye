using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLights : MonoBehaviour
{
    public Light[] lights;

    public Gradient lightColor;

    public float speed;

    public AnimationCurve intensity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            float val = Mathf.PingPong(speed * Time.time + i, 1);
            lights[i].color = lightColor.Evaluate(val);
            lights[i].intensity = intensity.Evaluate(val);
        }
    }
}
