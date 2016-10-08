using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NavMovement : MonoBehaviour {
    public static bool goNow = false;
    public GameObject destination;
    public GameObject eye;
    private NavMeshAgent agent;

    public List<GameObject> myList = new List<GameObject>();
   int i = -1;
    void go() {
  //  myList = new List<GameObject>();
     agent = gameObject.GetComponent<NavMeshAgent>();
        agent.SetDestination(myList[i].transform.position);
        destination = myList[0];
        transform.rotation = Quaternion.Euler(lockPos, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        // transform.LookAt(myList[i].transform);
    }
    void Start() {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }
    int lockPos = 0;
    bool done = false;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) {
            NavMovement.goNow = true;
        }


        if (goNow)
        {
            eye.GetComponent<CameraFilterPack_FX_Drunk>().gameObject.SetActive(true);
            if (!done)
            {
                GetComponent<AudioSource>().Play();
                done = true;
            }
                // print(agent.remainingDistance);
            if (i == -1 || (agent.remainingDistance < .001f && i < myList.Count-1))
            {
                transform.rotation = Quaternion.Euler(lockPos, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                i++;
                destination = myList[i];
                agent.SetDestination(myList[i].transform.position);
            }

        }
    }
}

