using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PositionTracker : MonoBehaviour {
    public float recordTime = 1f;

    public List<GameObject> trackedObjects = new List<GameObject>();

    [SerializeField]
    public List<Time_TrackedObj>[] timeTracker;

    float currTime = 0f;


    void Start() {
        timeTracker = new List<Time_TrackedObj>[trackedObjects.Count];
        for(int i = 0; i < timeTracker.Length; i++) {
            timeTracker[i] = new List<Time_TrackedObj>();
        }
    }
    void FixedUpdate() {
        currTime += Time.fixedDeltaTime;
        if(currTime > recordTime) {
            for(int i = 0; i < trackedObjects.Count; i++) {
                timeTracker[i].Add(new Time_TrackedObj(trackedObjects[i].transform.position, trackedObjects[i].transform.rotation));
                print(trackedObjects[i].name + " Added " + trackedObjects[i].transform.position);
            }
            currTime = 0;
        }
    }

}
[System.Serializable]
public class Time_TrackedObj {
    public Vector3 position;
    public Quaternion rotation;

    public Time_TrackedObj(Vector3 pos, Quaternion rot) {
        position = pos;
        rotation = rot;
    }
}