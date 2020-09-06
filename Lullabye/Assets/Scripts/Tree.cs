using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public Camera mainCam;
    [SerializeField] private Camera copterCam;
    [SerializeField] private Camera copterIsolateCam;
    [SerializeField] private AudioSource popSound;
    [SerializeField] private AudioSource paperSound;
    [SerializeField] private AudioSource rainSound;    
    [SerializeField] private AudioSource bendSound;
    [SerializeField] private AudioClip[] creaks;
    public Transform instructions;
    public GameObject title;
    public float zoom = 10;
    public float spawnRadius = 5;
    public float spawnMinRadius = 2;
    private int curIndex;
    private int index;

    public float clipDistance;
    public float clipPlaneZoom;

    public float timeSinceSpawned;

    public float timeSincePressed;
    private float blinkTimer;
    public float copterZoom;
    public float defaultZoom;
    public Transform magnifyMesh;

    public bool left;
    
    private List<MapleCopter> copters = new List<MapleCopter>();
    [SerializeField] GameObject RightCopter;
    [SerializeField] GameObject LeftCopter;
    [SerializeField] private Transform target;
    [SerializeField] private int copterCount = 100;
    [SerializeField] private Transform leafParent;
    public static Tree instance;
    [SerializeField] public BoxCollider col;
    [SerializeField] private Transform container;
    
    public List<Transform> treeLeaves;
    public ParticleSystem leafParticles;
    public MapleCopter curCopter;
    private float timer;
    public float dropRate = 1;
    private bool isCreatingCopter;
    private bool isPilotingCopter;
    private Vector3 velocity;
    private float timeSpacebarDown;

    private float spacebarTimer;
    // Start is called before the first frame update
        void Start()
        {
            foreach (Transform t in leafParent.transform)
            {
                
                treeLeaves.Add(t);
            }
            instance = this;
            Initialize();
        }


        void Initialize()
        {
            for (int i = 0; i < copterCount; i++)
            {
                GameObject copter = Random.Range(0, 100) > 50 ? RightCopter : LeftCopter;
                
                
                Vector3 spawnPos = Random.insideUnitSphere;
                Vector3 spawnMin = (spawnPos.normalized * spawnMinRadius);
                
                spawnPos = spawnPos * spawnRadius + new Vector3(spawnMin.x, 0, spawnMin.y);
                
                
                
                GameObject newCopter = Instantiate(copter, target.position + spawnPos,
                    Quaternion.identity);
                
                
                
                newCopter.transform.Rotate(0, Random.Range(0, 360), 0, Space.World);
                newCopter.transform.up = Random.onUnitSphere;
                copters.Add(newCopter.GetComponent<MapleCopter>());
            }
        }

        IEnumerator PopSeed()
        {

            bendSound.PlayOneShot(creaks[Random.Range(0,creaks.Length)]);
            isCreatingCopter = true;
            
            int index = Random.Range(0, treeLeaves.Count);
            
            Transform l = treeLeaves[index];
            Vector3 originalScale = l.localScale;
            
            Vector3 lookDir = (new Vector3(transform.position.x, l.position.y, transform.position.z) - l.position).normalized;
            if (Vector3.Dot(new Vector3(-lookDir.z, lookDir.y, lookDir.x), mainCam.transform.parent.forward) >
                Vector3.Dot(new Vector3(lookDir.z, lookDir.y, -lookDir.x), mainCam.transform.parent.forward))
            {
                left = true;
            }
            else
            {
                left = false;
            }

            GameObject copter = Random.Range(0, 100) > 50 ? RightCopter : LeftCopter;

            GameObject newCopter = Instantiate(copter, l.position,
                Quaternion.identity);
            newCopter.transform.up = Random.onUnitSphere;
            float t = 0;
            while (Input.GetKey(KeyCode.Space))
            {
                copterCam.transform.forward = newCopter.transform.position - copterCam.transform.position;
                
                magnifyMesh.position = l.transform.position - mainCam.transform.forward * 5f;
                magnifyMesh.rotation = Quaternion.LookRotation(mainCam.transform.forward, mainCam.transform.up);
                
                //mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, copterZoom, Time.deltaTime);
                
                l.localScale = Vector3.Lerp(originalScale, new Vector3(originalScale.x, 0.5f, originalScale.z), Mathf.Pow(t, 0.3f));
                t += Time.deltaTime * 3;
                if (t > 0.5f)
                {
                    mainCam.transform.parent.position = Vector3.SmoothDamp(mainCam.transform.parent.position, new Vector3(transform.position.x, l.position.y + 3f, transform.position.z), ref velocity, 0.3f);

                    lookDir = (new Vector3(transform.position.x, l.position.y, transform.position.z) - l.position).normalized;
                
                    lookDir = left
                        ? new Vector3(-lookDir.z, lookDir.y, lookDir.x)
                        : new Vector3(lookDir.z, lookDir.y, -lookDir.x);
                
                    mainCam.transform.parent.forward = Vector3.Lerp(mainCam.transform.parent.forward,  lookDir, Time.deltaTime * 3);
                    
                    magnifyMesh.gameObject.SetActive(true);
                    
                    if (instructions.gameObject.activeSelf)
                    {
                        instructions.gameObject.SetActive(false);
                    }
                   
                }
                yield return null;
            }

            if (Input.GetKeyUp(KeyCode.Space) && t < 0.5f)
            {
                Vector3 curScale = l.localScale;
                t = 1;
                magnifyMesh.gameObject.SetActive(false);
                
                bendSound.Stop();
                while (t > 0)
                {
                    l.localScale = Vector3.Lerp(originalScale, curScale, t);
                    t -= Time.deltaTime * 2;
                    yield return null;
                }

                    isCreatingCopter = false;
                    yield break;
            }

            popSound.Play();
            
            
            timeSinceSpawned = 0;
//            magnifyMesh.gameObject.SetActive(false);

            leafParticles.transform.position = l.position;
            leafParticles.transform.forward = l.position - transform.position;
            leafParticles.Emit(50);
            
            isPilotingCopter = true;
            curCopter = newCopter.GetComponent<MapleCopter>();
//            curCopter.enabled = false;
            foreach (Transform t2 in curCopter.GetComponentsInChildren<Transform>())
            {
                t2.gameObject.layer = LayerMask.NameToLayer("Copter"); 
            }

            curCopter.gameObject.layer = LayerMask.NameToLayer("Copter"); 
            curCopter.isPlayer = true;

            Vector3 trajectory = (l.position - transform.position).normalized;

           
            Vector3 lastPos = newCopter.transform.position;
            float force = 75 - l.position.y;
            t = 0;

            float rotSpeed = curCopter.lefty ? -100 : 100;
            //SynthSound.i.PlayNote(60);
            
            while (t < 1)
            {
                mainCam.nearClipPlane = Mathf.Lerp(mainCam.nearClipPlane, clipPlaneZoom, t);
                Vector3 targetPos = new Vector3(trajectory.x, 0, trajectory.z) * 10f + (Vector3.up * trajectory.y * (Mathf.Cos(t * Mathf.PI)) * (1-t) * force);
                newCopter.transform.position += targetPos * Time.deltaTime;
                
                newCopter.transform.RotateAround(Tree.instance.transform.position, Vector3.up,
                    Time.deltaTime * Mathf.Clamp01(1 - t * 2f) * rotSpeed); 
                newCopter.transform.right = (lastPos - newCopter.transform.position);
                lastPos = newCopter.transform.position;
                l.localScale = Vector3.Lerp(l.localScale, originalScale, t);
                t += Time.deltaTime * 0.75f;
                yield return null;
            }

            curCopter.enabled = true;
            isCreatingCopter = false;
            curCopter.Drop();
            
            
            //curCopter.GetComponentInChildren<ParticleSystem>().Play();
            
            
            TargetCopter();
        }

        IEnumerator ShootSeed(MapleCopter copter)
        {
            popSound.Play();
            
            leafParticles.transform.position = copter.transform.position;
            leafParticles.transform.forward = copter.transform.position - transform.position;
            leafParticles.Emit(50);
            
            
            
            foreach (Transform t2 in copter.GetComponentsInChildren<Transform>())
            {
                t2.gameObject.layer = LayerMask.NameToLayer("Copter"); 
            }

            copter.gameObject.layer = LayerMask.NameToLayer("Copter"); 

            Vector3 trajectory = (copter.transform.position - transform.position).normalized;

           
            Vector3 lastPos = copter.transform.position;
            float force = 75 - copter.transform.position.y;
            float t = 0;

            float rotSpeed = copter.lefty ? -100 : 100;
            //SynthSound.i.PlayNote(60);
            
            while (t < 1)
            {
               
                Vector3 targetPos = new Vector3(trajectory.x, 0, trajectory.z) * 10f + (Vector3.up * trajectory.y * (Mathf.Cos(t * Mathf.PI)) * (1-t) * force);
                copter.transform.position += targetPos * Time.deltaTime;
                
                copter.transform.RotateAround(Tree.instance.transform.position, Vector3.up,
                    Time.deltaTime * Mathf.Clamp01(1 - t * 2f) * rotSpeed); 
                copter.transform.right = (lastPos - copter.transform.position);
                lastPos = copter.transform.position;
                
                t += Time.deltaTime * 0.75f;
                yield return null;
            }

            copter.enabled = true;
            copter.Drop();
        }
       
        void Update()
        {

            rainSound.volume = Mathf.Clamp(Time.time / 20f, 0, 0.1f);
            blinkTimer = Time.time % 2;
            bool titleOn = blinkTimer > 0.66f;
            title.SetActive(titleOn);
            
//            copterCam.orthographicSize = 
            copterIsolateCam.orthographicSize = mainCam.orthographicSize;
  
            if (index < copterCount)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = Random.Range(dropRate, dropRate * 2);
                    //StartCoroutine(ShootSeed(copters[index]));
                    copters[index].Drop();
                    index++;

                }

                if (Input.GetKey(KeyCode.Space) && !isPilotingCopter && !isCreatingCopter)

                {
                    StartCoroutine(PopSeed());
                    
                    
//                    if (index < copters.Count)
//                    {
//                        curIndex = index;
//                        copters[index].Drop();
//                        copters[index].isPlayer = true;
//                        copterCam.enabled = true;
//                        copters[index].isPlayer = true;
//                        isPilotingCopter = true;
//                        index++;
//                    }
                }
            }

            timeSincePressed += Time.deltaTime;
            if (timeSincePressed > 30)
            {
                instructions.gameObject.SetActive(true);
                timeSincePressed = 0;
            }

            if (isPilotingCopter)
            {
                timeSincePressed = 0;
                
                TargetCopter();
            }
            else
            {
                if (!isCreatingCopter)
                {
                    
                    mainCam.transform.parent.position = Vector3.Lerp(mainCam.transform.parent.position,
                        transform.position + Vector3.up * 23f, Time.deltaTime);
                    mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, defaultZoom, Time.deltaTime);

                    mainCam.nearClipPlane = Mathf.Lerp(mainCam.nearClipPlane, clipDistance, Time.deltaTime / 2f);
                    mainCam.transform.parent.Rotate(Vector3.up * Time.deltaTime * 15, Space.World);
                }
            }
        }
    
   
    public void GetNextCopter()
    {
        StartCoroutine(PlaceSeed());
    }

    IEnumerator PlaceSeed()
    {
        float t = 0;

        yield return null;
        magnifyMesh.gameObject.SetActive(false);
        curCopter = null;
        isPilotingCopter = false;
//        copterCam.enabled = false;

         //this is where the planting FX go
    }

    public void OnCollisionEnter(Collision col)
    {

        if (col.rigidbody.tag == "copter")
        {
            col.rigidbody.BroadcastMessage("LoseControl");
        }
    }
  void TargetCopter()
  {
      timeSinceSpawned += Time.deltaTime;
      magnifyMesh.position =  curCopter.transform.position - mainCam.transform.forward * 5f;
      magnifyMesh.rotation = Quaternion.LookRotation(mainCam.transform.forward, mainCam.transform.up);
      copterCam.transform.forward = curCopter.transform.position - copterCam.transform.position;
//      copterCam.transform.forward
      int sign = curCopter.lefty ? 1 : -1;
      Transform c = curCopter.transform;
      float clip = Mathf.Lerp(clipPlaneZoom, clipDistance, 1-(curCopter.transform.position.y/15));
      
      mainCam.nearClipPlane = Mathf.Lerp(mainCam.nearClipPlane, clip, Time.deltaTime /2f);
      mainCam.transform.parent.position = Vector3.SmoothDamp(mainCam.transform.parent.position, new Vector3(transform.position.x, c.position.y + 4f + Mathf.Clamp01(1-(curCopter.transform.position.y/20)) * 10f , transform.position.z), ref velocity, 0.3f);
      copterCam.transform.localPosition = Vector3.up * Mathf.Clamp01(1 - (curCopter.transform.position.y / 20)) * -5f;

      if ((curCopter.transform.position.y/18) < 1f && timeSinceSpawned > 5f)
      {
          magnifyMesh.gameObject.SetActive(false);
      }
      Vector3 forwardlookDir = (new Vector3(transform.position.x, c.position.y, transform.position.z) - c.position)
          .normalized;
      
          Vector3 lookDir = forwardlookDir.normalized;

          lookDir = left
              ? new Vector3(-lookDir.z, lookDir.y, lookDir.x)
              : new Vector3(lookDir.z, lookDir.y, -lookDir.x);

          
          Vector3 finalLookDir = Vector3.Lerp(lookDir, forwardlookDir, timeSinceSpawned);
          
          mainCam.transform.parent.forward = Vector3.Lerp(mainCam.transform.parent.forward, finalLookDir, Time.deltaTime * 3);

          float zoom = Mathf.Lerp(copterZoom, defaultZoom, 1-(curCopter.transform.position.y/20));
      mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, zoom, Time.deltaTime);
     // mainCam.transform.parent.forward = Vector3.Lerp(mainCam.transform.parent.forward, (new Vector3(transform.position.x, transform.position.y, transform.position.z) - c.position).normalized, Time.deltaTime * 3);
//      copterCam.transform.parent.Rotate(Vector3.up * Time.deltaTime * 10 * sign, Space.World); 
  }
}
