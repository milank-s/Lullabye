using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public Transform Camera;

    public float dissembleSpeed = 0.5f;
    // Start is called before the first frame update
    [SerializeField] private Transform room;
    [SerializeField] private List<Transform> walls;
    [SerializeField] private List<Transform> props;
    
    void Start()
    {
        foreach (Transform t in room.GetComponentsInChildren<Transform>())
        {
            props.Add(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Camera.Rotate(0, 10 * Time.deltaTime, 0, Space.World);
        foreach (Transform t in props)
        {
            t.localScale -= Vector3.one * Time.deltaTime * 0.01f;
        }

        foreach (Transform t in walls)
        {
            Vector3 dir = (t.position - transform.position).normalized;
            
            if(Vector3.Dot(dir, Vector3.up) > 0.75f)
            {
                t.position += Vector3.up * Time.deltaTime * dissembleSpeed;
            }
            
            if(Vector3.Dot(dir, Vector3.right) > 0.75f)
            {
                t.position += Vector3.right * Time.deltaTime* dissembleSpeed;
            }
            
            if(Vector3.Dot(dir, Vector3.right) < -0.75f)
            {
                t.position -= Vector3.right * Time.deltaTime* dissembleSpeed;
            }
            
            if(Vector3.Dot(dir, Vector3.up) > 0.75f)
            {
                t.position += Vector3.up * Time.deltaTime* dissembleSpeed;
            }
            
            if(Vector3.Dot(dir, Vector3.up) < -0.75f)
            {
                t.position -= Vector3.up * Time.deltaTime* dissembleSpeed;
            }
            
            if(Vector3.Dot(dir, Vector3.forward) > 0.75f)
            {
                t.position += Vector3.forward * Time.deltaTime* dissembleSpeed;
            }
            
            if(Vector3.Dot(dir, Vector3.forward) < -0.75f)
            {
                t.position -= Vector3.forward * Time.deltaTime* dissembleSpeed;
            }
           
        }
    }
}
