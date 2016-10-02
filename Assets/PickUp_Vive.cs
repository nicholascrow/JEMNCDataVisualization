using UnityEngine;
using System.Collections;

public class PickUp_Vive : MonoBehaviour {

    //the controllers
    SteamVR_TrackedObject trackedObj;

    //the joint to keep this object attached to the controller
    FixedJoint joint;

    //object that can be selected
    GameObject selectableObj;

    //object that is currently selected
    GameObject selectedObj;

    //should we destroy this object?
    public static bool destroySelected = false;

    //when the scene starts
    void Awake() {

        //we can't select anything until the controllers are tracked
        selectableObj = null;
        selectedObj = null;

        //controller tracking
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update() {
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        //break the joint if the object we are holding is destroyed
        if(destroySelected && joint != null) {
            //destroy the joint
            Object.DestroyImmediate(joint);
            //set it to null
            joint = null;
        }

        //if we can select an object then grab it.
        if(joint == null && selectableObj != null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip)) {
            AddJoint();
        }

        //if we are holding an object lets stop holding it
        else if(joint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Grip)) {
            RemoveJoint();
        }
    }

    /// <summary>
    /// When the controller enters an object, we need to mark it so we can grab it. 
    /// </summary>
    /// <param name="collision">The object we can grab.</param>
    /// 
    void OnTriggerEnter(Collider collision) {
        print(collision.gameObject.name);
        if(collision.gameObject.GetComponent<Can_Pickup>()
        && collision.gameObject.GetComponent<Can_Pickup>().isBeingHeld == 0) {
            selectableObj = collision.gameObject;
        }
    }

    /// <summary>
    /// When the controller leaves the object.
    /// </summary>
    /// <param name="collisionInfo">The object left.</param>
    void OnTriggerExit(Collider collisionInfo) {
        selectableObj = null;
    }

    /// <summary>
    /// This method adds a joint between the 2 objects (this object and the one we are grabbing)
    /// </summary>
    void AddJoint() {
        selectedObj = selectableObj;
        selectableObj.GetComponent<Can_Pickup>().isBeingHeld++;
        joint = selectableObj.AddComponent<FixedJoint>();
        joint.connectedBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Removes the joint between the 2 objects.
    /// </summary>
    void RemoveJoint() {
        
        //is the object we want to grab being held still
        selectedObj.GetComponent<Can_Pickup>().isBeingHeld--;

        //the controller
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        // the rigidbody on the joint
        Rigidbody r = joint.gameObject.GetComponent<Rigidbody>();

        //destroy the join
        Object.DestroyImmediate(joint);

        //set it to null
        joint = null;

        //selected object is now null
        selectedObj = null;

        //the next part applies physics.
        var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
        if(origin != null) {
            r.velocity = origin.TransformVector(device.velocity);
            r.angularVelocity = origin.TransformVector(device.angularVelocity);
        }
        else {
            r.velocity = device.velocity;
            r.angularVelocity = device.angularVelocity;
        }

        r.maxAngularVelocity = r.angularVelocity.magnitude;
    }
}
