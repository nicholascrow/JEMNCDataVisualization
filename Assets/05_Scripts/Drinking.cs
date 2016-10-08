using UnityEngine;
using System.Collections;

public class Drinking: MonoBehaviour {
    public GameObject beer; // right var???
    public bool canPickUp = false;
    
    // Use this for initialization
	void Start () {
        beer.SetActive(true); //make the object interactive

	}
    //public GameObject prefab;
    public Rigidbody attachPoint;

    SteamVR_TrackedObject trackedObj;
    FixedJoint joint;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
	
	void FixedUpdate()
	{
		var device = SteamVR_Controller.Input((int)trackedObj.index);
		if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) )
		{
            print(NavMovement.goNow);
            beer.transform.SetParent(this.transform);
            beer.transform.localPosition = new Vector3(-.007f, -.05f, .018f);
            beer.transform.localRotation = Quaternion.Euler(-8.048f, 239.55f, -32.27f);
            NavMovement.goNow = true;
			//joint = beer.AddComponent<FixedJoint>();
           // joint.connectedBody = this.GetComponent<Rigidbody>();//attachPoint;
		}
	    // don't want to destroy it hmm
	}
}
