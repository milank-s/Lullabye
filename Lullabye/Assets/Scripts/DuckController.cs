using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer r;

    public Sprite[] idle;
    public Sprite[] walk;
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

    private bool hasCopter;
    public AudioSource hitSound;

    // Update is called once per frame
    void Update()
    {
        
        
        
        Vector3 toCam = transform.position - tree.mainCam.transform.position;
        toCam.y = 0;
        transform.right = toCam;
        
        
        surpriseAnim.transform.forward = -tree.mainCam.transform.forward;
        
        
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

            timeUntilPause -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (timeUntilPause < 0)
            {
                moving = false;
                timeUntilMove = moveTime;
                r.sprite = idle[0];
                curSprites = idle;
                index = 0;
                
               
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
                    if (!hasCopter)
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
                    }
                    else
                    {
                        hasCopter = true;
                        Vector3 myPos = transform.position;
                        myPos.y = transform.position.y;
                        Vector3 treeBase = tree.transform.position;
                        treeBase.y = transform.position.y;

                        Vector3 diff = myPos - treeBase;
                        float distanceToTree = diff.magnitude;
                        target = diff;
                        target.y = 0;
                        target = target.normalized * distanceToTree;
                        target.y = transform.position.y;
                    }
                }
                else
                {
                    target = Random.onUnitSphere * Random.Range(1f, 5f);
                    target.y = transform.position.y;
                }

                curSprites = walk;
                r.sprite = walk[0];
                index = 0;

            }

            timeUntilMove -= Time.deltaTime;
        }
        
        
        if (interval < 0)
        {
            index++;
            if (index >= curSprites.Length)
            {
                index = 0;
            }

            interval = 1f / framerate;
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
        surpriseAnim.transform.position = c.transform.position;
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
