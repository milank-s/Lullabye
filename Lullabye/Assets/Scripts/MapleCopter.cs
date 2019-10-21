using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class MapleCopter : MonoBehaviour
{
    public float rotationSpeed;
    public float fallSpeed;
    public float flySpeed  = 2;
    [SerializeField] Transform child;
    [SerializeField] private Transform rotateChild;
    public bool isPlayer;
    public bool lefty;


    private ParticleSystem p;
    private float xOffset;
    private float yOffset;
    private float fallTimer;
    private bool falling;
    private bool flying;
    private float fallingCoefficient;
    private float flyingCoefficient;

    private Vector3 lastPos;

    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        flySpeed = Random.Range(3f, 5f);
        rotationSpeed = 2000;
        fallSpeed = 3f;
        if (lefty)
        {
            rotationSpeed = -rotationSpeed;
        }

        fallTimer = Random.Range(1f, 10f);
        yOffset = Random.Range(-1000f, 1000f);
        xOffset = Random.Range(-1000f, 1000f);
        //child.forward = Vector3.down;
    }

    public void Drop()
    {
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        falling = true;
        
        float t = 0;

        Vector3 forwardDir = child.right;
        while (t < 1)
        {
            //Vector3 newDir = Vector3.RotateTowards(child.right, Vector3.up, Time.deltaTime, 0.0f);
            //child.rotation = Quaternion.LookRotation(newDir, Vector3.right);
            //child.right = Vector3.Lerp(forwardDir, transform.InverseTransformDirection(Vector3.up), t);
            t += Time.deltaTime * 5;
            yield return null;
        }
        float freeFallTime = Random.Range(0.05f, 0.15f);

        t = 0;
        while (t < freeFallTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        
        forwardDir = child.right;
        float angle = Random.Range(0.1f, 0.15f);
        while (t < 1f)
        {
//            Quaternion targetRot = Quaternion.LookRotation()
//            Quaternion.Slerp()
    
            //child.right = Vector3.Lerp(forwardDir, transform.InverseTransformDirection(Vector3.right), t);
            t += Time.deltaTime * 10;
            yield return null;
        }

        flying = true;
    }
    // Update is called once per frame
    void Update()
    {

        lastPos = transform.position;
        fallingCoefficient = Mathf.Abs(Vector3.Dot(child.right, Vector3.up));
        flyingCoefficient = Mathf.Clamp01(Mathf.Abs(child.right.x) + Mathf.Abs(child.right.z));
        
        float xMovement = 0;
        float zMovement = 0;
        
        float time = Time.time / 5f;
        
        float offset = Mathf.PerlinNoise(time + xOffset, time + yOffset) * 2 - 1f;
        
        if (!isPlayer)
        {
            xMovement = Mathf.PerlinNoise(time + xOffset, time + yOffset) * 2 - 1f;
            //zMovement = Mathf.PerlinNoise(time - xOffset, time - yOffset) * 2 - 1f;
        }
        else
        {
            xMovement = Input.GetAxis("Horizontal");
            zMovement = Input.GetAxis("Vertical");
        }

        
        if (falling)
        {
            
           
            if (flying)
            {
               
                transform.RotateAround(Tree.instance.transform.position, Vector3.up, Time.deltaTime * -(xMovement) * flyingCoefficient *  50);
                //child.position -= new Vector3(xMovement * flySpeed, fallSpeed * (0.25f + fallingCoefficient), zMovement * flySpeed) * Time.deltaTime;
            }
           
                
            
            
            CheckFloor();


            if (!isPlayer)
            {
                transform.position += (Vector3.down * fallSpeed * (0.1f + fallingCoefficient)) * Time.deltaTime;
                Vector3 treePos = Tree.instance.transform.position;
                Vector3 dir = new Vector3(treePos.x, transform.position.y, treePos.z) - child.position;
                transform.position += dir.normalized * xMovement * fallingCoefficient * Time.deltaTime;
            }
            else
            {
                transform.position += (Vector3.down * fallSpeed * (0.1f + fallingCoefficient)) * Time.deltaTime;
            }

            
            child.localEulerAngles = new Vector3(0,0, Mathf.Lerp(15, 90, -zMovement));
            //child.right = (Vector3.Lerp(rotateChild.InverseTransformDirection(transform.right), Vector3.up, -zMovement));
            rotateChild.Rotate(0, rotationSpeed * Time.deltaTime * (1 + offset/5), 0, Space.Self);

            Vector3 lastVel = velocity;
            velocity = lastPos - transform.position;
            transform.up = Vector3.Lerp(Vector3.up, velocity, Mathf.Abs(xMovement));
//            if (lastVel.magnitude <= velocity.magnitude)
//            {
//                transform.up = Vector3.Lerp(transform.up, velocity, Time.deltaTime * 10);
//            }
//            else
//            {
//                transform.up = Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * 10);
//            }
//            new Vector3(Mathf.Sin(Time.time), 2, Mathf.Cos(Time.time));
        }
    }

    void RagdollOn()
    {
        
    }
    void CheckFloor()
    {
        if (Tree.instance.col.transform.position.y > child.position.y)
        {

            flying = false;
            falling = false;
            if (isPlayer)
            {
                Tree.instance.GetNextCopter();
            }

            RagdollOn();
            this.enabled = false;
        }
    }
}
