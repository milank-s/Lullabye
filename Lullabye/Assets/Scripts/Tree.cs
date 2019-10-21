using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TreeEditor;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private Camera copterCam;

    public float spawnRadius = 5;
    public float spawnMinRadius = 2;
    private int curIndex;
    private int index;
    
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
    private MapleCopter curCopter;
    private float timer;
    public float dropRate = 1;
    private bool isCreatingCopter;
    private bool isPilotingCopter;
    
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
                copters.Add(newCopter.GetComponent<MapleCopter>());
            }
        }

        IEnumerator PopSeed()
        {

            isCreatingCopter = true;
            int index = Random.Range(0, treeLeaves.Count);
            
            Transform l = treeLeaves[index];
            Vector3 originalScale = l.localScale;
            
            float t = 0;
            while (Input.GetKey(KeyCode.Space))
            {
                
                l.localScale = Vector3.Lerp(originalScale, new Vector3(originalScale.x, 0.5f, originalScale.z), Mathf.Pow(t, 0.3f));
                t += Time.deltaTime * 3;
                yield return null;
            }

            if (Input.GetKeyUp(KeyCode.Space) && t < 1)
            {
                Vector3 curScale = l.localScale;
                t = 1;
                while (t > 0)
                {
                    l.localScale = Vector3.Lerp(originalScale, curScale, t);
                    t -= Time.deltaTime * 2;
                    yield return null;
                }

                    isCreatingCopter = false;
                yield break;
            }

            leafParticles.transform.position = l.position;
            leafParticles.transform.forward = l.position - transform.position;
            leafParticles.Emit(50);
            GameObject copter = Random.Range(0, 100) > 50 ? RightCopter : LeftCopter;


            Vector3 spawnPos = l.position;
                
            GameObject newCopter = Instantiate(copter, l.position,
                Quaternion.identity);
//            
            curCopter = newCopter.GetComponent<MapleCopter>();
            curCopter.enabled = false;
            curCopter.isPlayer = true;

            Vector3 trajectory = (l.position - transform.position).normalized;

            float force = 60 - l.position.y;
            t = 0;
            while (t < 1)
            {
                Vector3 targetPos = new Vector3(trajectory.x, 0, trajectory.z) * 10f + (Vector3.up * trajectory.y * (Mathf.Cos(t * Mathf.PI)) * (1-t) * force);
                newCopter.transform.position += targetPos * Time.deltaTime;
                l.localScale = Vector3.Lerp(l.localScale, originalScale, t);
                t += Time.deltaTime;
                yield return null;
            }

            curCopter.enabled = true;
            copterCam.enabled = true;
            isPilotingCopter = true;
            isCreatingCopter = false;
            curCopter.Drop();
            curCopter.GetComponentInChildren<ParticleSystem>().Play();
            TargetCopter();
        }
        
        void Update()
        {
            mainCam.transform.parent.Rotate(Vector3.up * Time.deltaTime * 15, Space.World);

            if (index < copterCount)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = Random.Range(dropRate, dropRate * 2);
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


            if (isPilotingCopter)
            {
                TargetCopter();
            }
        }
    
   
    public void GetNextCopter()
    {
        StartCoroutine(PlaceSeed());
    }

    IEnumerator PlaceSeed()
    {
        yield return new WaitForSeconds(0.5f);
        curCopter = null;
        isPilotingCopter = false;
        copterCam.enabled = false;
        mainCam.transform.parent.position = transform.position;

    }
  void TargetCopter()
  {
      int sign = curCopter.lefty ? 1 : -1;
      Transform c = curCopter.transform;
      mainCam.transform.parent.position = c.position;
      mainCam.transform.parent.forward = Vector3.Lerp(mainCam.transform.parent.forward, (new Vector3(transform.position.x, mainCam.transform.parent.position.y, transform.position.z) - curCopter.transform.position).normalized, Time.deltaTime * 3);
//      copterCam.transform.parent.Rotate(Vector3.up * Time.deltaTime * 10 * sign, Space.World); 
  }
}
