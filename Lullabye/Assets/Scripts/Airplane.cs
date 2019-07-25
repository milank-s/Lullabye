using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{
    
    public Transform targetPosition;
    public Transform airplaneParent;
    public float speedModifier = .1f; 
    public float altitudeModifier = .001f; 
    [HideInInspector] public Vector3 worldRot; 
    public float latitude;
    public float longitude;
    public float altitude;
    public float direction;    
    public float horizontal;
    public bool isGround;
    public float vertical;
    bool IsInit = false;

    public void Init() {
        if(isGround) {
            gameObject.SetActive(false);
            return;
        }

        worldRot = new Vector3(latitude,longitude,direction);
        SetTransform(targetPosition, worldRot);

        targetPosition.position += targetPosition.up * altitude * altitudeModifier; 

        transform.parent = Manager.instance.earth.earthTarget; 
        transform.position = Manager.instance.earth.earthTarget.position; 
        transform.rotation = Manager.instance.earth.earthTarget.rotation; 
        
        IsInit = true; 
    }

    void Update() {
        if(IsInit == false)
            return;

        MoveTarget();
    }

    void MoveTarget() {
        targetPosition.Rotate(Vector3.right * horizontal * Time.deltaTime * speedModifier, Space.Self);
    }

    public static void SetTransform(Transform worldTransform, Vector3 rot) {
        worldTransform.Rotate(Vector3.right * rot.x, Space.World);
        worldTransform.Rotate(Vector3.up * -rot.y, Space.World);
        worldTransform.Rotate(Vector3.forward * -rot.z, Space.Self);
    }
}
