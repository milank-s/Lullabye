using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer r;

    public Sprite[] idle;
    public Sprite[] walkForward;
    public Sprite[] walkBack;
    public Sprite hit;
    public Tree tree;
    public float framerate = 0.5f;

    public Animator surpriseAnim;
    public float speed;
    public float pauseTime;
    public float moveTime;
    private float timeUntilMove;
    private float timeUntilPause;
    private float hitFrames;
    private bool hitCopter;
    private bool moving;
    private Vector3 target;
    private float interval;
    private float hitInterval;
    private Vector3 velocity;
    private int index;
    public Sprite[] curSprites;

    private bool facingCamera;
    private bool hasCopter;
    public AudioSource hitSound;

    private Vector3 dirToCamera;
    // Update is called once per frame

    void Start()
    {
        curSprites = idle;
    }
    void Update()
    {
        Vector3 toCam = transform.position - tree.mainCam.transform.position;
        toCam.y = 0;
        transform.right = toCam;

        Vector3 toTarget = (target - transform.position).normalized;
        Vector3 camForward = tree.mainCam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        
        
        
            if (Vector3.Dot(toTarget, -camForward) < 0)
            {
                if (facingCamera)
                {
                    interval = 1f / framerate;
                }
                facingCamera = false;
            }
            else
            {
                if (!facingCamera)
                {
                    interval = 1f / framerate;
                }
                facingCamera = true;
            }
        

        surpriseAnim.transform.forward = -tree.mainCam.transform.forward;
        
        Vector3 toTargetFromCam = target - tree.mainCam.transform.position;
       
        if (toTarget.magnitude > 0.1f)
        {
          
            toTargetFromCam = tree.mainCam.transform.InverseTransformDirection(toTarget);
            if (toTargetFromCam.x > 0)
            {
                r.flipX = false;
            }
            else
            {
                r.flipX = true;
            }
                
        }
        
        if (hitCopter)
        {
            hitInterval -= Time.deltaTime;
            if (hitInterval < 0)
            {
                hitCopter = false;
            }
            return;
        }
        
        if (moving)
        {
//            Vector3 lookDir = target - transform.position;
//
//            if (lookDir.magnitude > 0)
//            {
//                transform.forward = lookDir;
//            }

            if (toTarget.magnitude > 0.1f)
            {
                if (facingCamera)
                {
                    curSprites = walkForward;
                }
                else
                {
                    curSprites = walkBack;
                }                
            }
            timeUntilPause -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if ((Vector3.Distance(transform.position, target) < 0.1f) || (!hasCopter && timeUntilPause < 0))
            {
                
                moving = false;
                timeUntilMove = moveTime;
                r.sprite = idle[0];
                curSprites = idle;
                index = 0;
                interval = 2f / framerate;
               
//                Vector3 lookPos = tree.mainCam.transform.position;
//                lookPos.y = transform.position.y;
//                transform.LookAt(lookPos);
            }
            
        }
        else
        {

            
            
            if (timeUntilMove < 0)
            {
                moving = true;
                timeUntilPause = pauseTime;
                
                if (tree.curCopter != null && tree.curCopter.falling)
                {
                        hasCopter = true;
                        Vector3 copterPos = tree.curCopter.transform.position;
                        copterPos.y = transform.position.y;
                        Vector3 treeBase = tree.transform.position;
                        treeBase.y = transform.position.y;

                        Vector3 diff = copterPos - treeBase;
                        float distanceToTree = diff.magnitude;
                        target = Random.onUnitSphere;
                        //target = diff;
                        target.y = 0;
                        target = target.normalized * distanceToTree;
                        target.y = transform.position.y;

//                    if (!hasCopter)
//                    {
//                        hasCopter = true;
//                        Vector3 copterPos = tree.curCopter.transform.position;
//                        copterPos.y = transform.position.y;
//                        Vector3 treeBase = tree.transform.position;
//                        treeBase.y = transform.position.y;
//
//                        Vector3 diff = copterPos - treeBase;
//                        float distanceToTree = diff.magnitude;
//                        target = Random.onUnitSphere;
//                        //target = diff;
//                        target.y = 0;
//                        target = target.normalized * distanceToTree;
//                        target.y = transform.position.y;
//                    }
//                    else if(false)
//                    {
//                        hasCopter = true;
//                        Vector3 copterPos = tree.curCopter.transform.position;
//                        copterPos.y = transform.position.y;
//                        Vector3 treeBase = tree.transform.position;
//                        treeBase.y = transform.position.y;
//
//                        float distanceToTree = (copterPos - treeBase).magnitude;
//                        
//                        Vector3 myPos = transform.position;
//                        treeBase.y = transform.position.y;
//
//                        Vector3 diff = myPos - treeBase;
//                        target = diff;
//                        target.y = 0;
//                        target = target.normalized * distanceToTree;
//                        target.y = transform.position.y;
//                    }
                }
                else
                {
                    target = Random.onUnitSphere * Random.Range(1f, 5f);
                    target.y = transform.position.y;
                }

                index = 0;
                if (facingCamera)
                {
                    curSprites = walkForward;
                }
                else
                {
                    curSprites = walkBack;
                }
                interval = 1f / framerate;
                r.sprite = curSprites[0];

            }

            timeUntilMove -= Time.deltaTime;
        }

        interval -= Time.deltaTime;
        
        if (interval < 0)
        {
            index = (index + 1) % curSprites.Length;

            if (moving)
            {
                interval = 1f / framerate;
            }
            else
            {
                interval = 2f / framerate;
            }

            r.sprite = curSprites[index];
        }
        
    }

    public void OnCollisionEnter(Collision col)
    {
        Rigidbody rb = col.rigidbody;
        MapleCopter c = rb.GetComponent<MapleCopter>();

        if (c != null && c.isPlayer && c.falling)
        {
            Vector3 lookPos = -tree.mainCam.transform.forward;
            transform.LookAt(lookPos);
            
            r.sprite = hit;
            hitSound.Play();
            c.LoseControl();
            c.RagdollOn();
            StartCoroutine(HitDuck(c));
           
            
        }

    }

    IEnumerator HitDuck(MapleCopter c)
    {
        hasCopter = false;
        surpriseAnim.SetTrigger("surprise");
        c.falling = false;
        
        
        if (moving)
        {
            target = Random.onUnitSphere * Random.Range(1f, 5f);
            target.y = transform.position.y;
        }
        else
        {
            timeUntilMove = 0;
        }
        
        yield return new WaitForSeconds(0.5f);
        
        if (c.flying)
        {
            //c.Plant();
        }
    }
}
