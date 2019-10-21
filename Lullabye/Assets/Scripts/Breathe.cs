using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Breathe : MonoBehaviour
{
    private bool x;
    private bool y;
    
    private float timer;
    private float offset;
    private float interval;
    private float speed = 5;
    private float perlin;
    // Start is called before the first frame update
    void Start()
    {
        offset = Random.Range(0f, 1f);
        foreach (Transform t in transform)
        {
            t.gameObject.AddComponent<Breathe>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0)
        {
            timer = Random.Range(0.5f, 1f);
            if (Random.Range(0, 100) > 55)
            {
                y = false;
                x = !x;
            }
            else{
                y = !y;
            }
        }
        speed = 5 * Time.deltaTime;
        perlin = offset + Time.time/2f;
        timer -= Time.deltaTime;
        
        if (y)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, 0.5f + Mathf.PerlinNoise(perlin, -perlin),  transform.localScale.z), speed);
        }
        else
        {
            if (x)
            {
                transform.localScale = Vector3.Lerp(transform.localScale,
                    new Vector3(transform.localScale.x, transform.localScale.y, 0.5f + Mathf.PerlinNoise(-perlin, perlin)),
                    speed);
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3( 0.5f + Mathf.PerlinNoise(perlin, -perlin), transform.localScale.y, transform.localScale.z), speed);
            }
        }
    }
}
