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
    [SerializeField] private AudioSource leafSound;
    public bool isPlayer;
    public bool lefty;

    public float loseControlTime = 1;
    public float loseControlTimer;

    public float loseControlNormalized
    {
        get { return (1 - loseControlTimer / loseControlTime); }
    }
    private ParticleSystem p;
    [SerializeField] private AudioSource popSound;
    private float xOffset;
    private float yOffset;
    private float fallTimer;
    public bool falling;
    public bool flying;
    private float fallingCoefficient;
    private float flyingCoefficient;
    private Rigidbody r;
    private bool lostControl;
    private Vector3 lastPos;
    private float xMovement;
    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
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
        
        if (isPlayer)
        {
            leafSound.Play();
        }

        falling = true;
        
        float t = 0;

        Vector3 forwardDir = child.right;
        while (t < 1)
        {
            if (isPlayer)
            {
                //SynthSound.i.controller.SetParameterPercent(AudioHelm.Param.kVolume, t / 5f);
            }

            t += Time.deltaTime;
            yield return null;
        }
//        while (t < 1)
//        {
//            //Vector3 newDir = Vector3.RotateTowards(child.right, Vector3.up, Time.deltaTime, 0.0f);
//            //child.rotation = Quaternion.LookRotation(newDir, Vector3.right);
//            //child.right = Vector3.Lerp(forwardDir, transform.InverseTransformDirection(Vector3.up), t);
//            t += Time.deltaTime * 5;
//            yield return null;
//        }
//        float freeFallTime = Random.Range(0.05f, 0.15f);
//
//        t = 0;
//        while (t < freeFallTime)
//        {
//            t += Time.deltaTime;
//            yield return null;
//        }
//
//        t = 0;
//        
//        forwardDir = child.right;
//        float angle = Random.Range(0.1f, 0.15f);
//        while (t < 1f)
//        {
////            Quaternion targetRot = Quaternion.LookRotation()
////            Quaternion.Slerp()
//    
//            //child.right = Vector3.Lerp(forwardDir, transform.InverseTransformDirection(Vector3.right), t);
//            t += Time.deltaTime * 10;
//            yield return null;
//        }

        if (isPlayer)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
        flying = true;
    }
    // Update is called once per frame
    void Update()
    {

        lastPos = transform.position;

        flyingCoefficient = Mathf.Clamp01(Mathf.Abs(child.right.x) + Mathf.Abs(child.right.z));
        
        
        float zMovement = 0;
        
        float time = Time.time / 5f;
        
        float offset = Mathf.PerlinNoise(time + xOffset, time + yOffset) * 2 - 1f;
        
       
        
        CheckFloor();
        
        if (falling)
        {
            if (!isPlayer)
            {
            
                xMovement = Mathf.PerlinNoise(time + xOffset, time + yOffset) * 2 - 1f;
            
            }
            else
            {
                xMovement = Mathf.Lerp(xMovement, Input.GetAxis("Horizontal"), Time.deltaTime * 3);
                zMovement = Input.GetAxis("Vertical");
            }

            
            if (!lostControl)
            {
                fallingCoefficient = Mathf.Abs(Vector3.Dot(child.right, Vector3.up));
            }
            else
            {
                loseControlTimer -= Time.deltaTime;
                fallingCoefficient = Mathf.Lerp(fallingCoefficient, 2 -loseControlNormalized, Time.deltaTime * 10);
                if (loseControlTimer < 0 && lostControl)
                {
                    if (isPlayer)
                    {
                        leafSound.Play();
                    }
                    loseControlTimer = 0;
                    lostControl = false;
                }
            }
            
            if (flying)
            {
                    transform.RotateAround(Tree.instance.transform.position, Vector3.up,
                        Time.deltaTime * -(xMovement) * flyingCoefficient * 25 * loseControlNormalized); 
                    
                //child.position -= new Vector3(xMovement * flySpeed, fallSpeed * (0.25f + fallingCoefficient), zMovement * flySpeed) * Time.deltaTime;
            }
           
            if (!isPlayer)
            {
                Vector3 treePos = Tree.instance.transform.position;
                Vector3 dir = new Vector3(treePos.x, transform.position.y, treePos.z) - child.position;
                transform.position += dir.normalized * xMovement * fallingCoefficient * Time.deltaTime;
            }
            else
            {
                leafSound.pitch = 1 + (Mathf.Clamp01(Mathf.Abs(zMovement)) + offset + Mathf.Abs(xMovement))/5f;
                leafSound.volume = offset / 10f + 0.15f;
                //SynthSound.i.controller.SetPitchWheel(zMovement + offset - xMovement);
            }


            transform.position += (Vector3.down * fallSpeed * (0.1f + fallingCoefficient)) * Time.deltaTime;
            Vector3 lastVel = velocity;
            velocity = lastPos - transform.position;
            if (lostControl)
            {
                r.AddTorque(Random.onUnitSphere * Time.deltaTime * 100);
            }
            else
            {
                child.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(15, 90, -zMovement));
                //child.right = (Vector3.Lerp(rotateChild.InverseTransformDirection(transform.right), Vector3.up, -zMovement));
                transform.up = Vector3.Lerp(Vector3.up, velocity, Mathf.Abs(xMovement)/1.5f);
                rotateChild.Rotate(0, loseControlNormalized * rotationSpeed * Time.deltaTime * (1 + offset / 5), 0, Space.Self);
            }

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

    public void RagdollOn()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
    }
    
    void CheckFloor()
    {
        if (Tree.instance.col.transform.position.y + 0.2f > child.position.y)
        {

            Plant();
        }
    }

    public void Plant()
    {
        flying = false;
        leafSound.Stop();
        if (isPlayer)
        {
            //SynthSound.i.controller.NoteOff(60);
        }

        falling = false;
        if (isPlayer)
        {
            isPlayer = false;
            Tree.instance.GetNextCopter();
        }

        GetComponent<Rigidbody>().isKinematic = true;
        
        this.enabled = false;
    }

    public void LoseControl()
    {
        if (isPlayer)
        {
            leafSound.Stop();
        }
        Debug.Log("hit tree");
        lostControl = true;
        loseControlTimer = loseControlTime;
    }
}
