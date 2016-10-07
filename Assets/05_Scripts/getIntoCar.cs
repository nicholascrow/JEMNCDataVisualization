using UnityEngine;
using System.Collections;

public class getIntoCar : MonoBehaviour
{

    SteamVR_TrackedObject trackedObj;

    public GameObject car;

    Transform reference
    {
        get
        {
            var top = SteamVR_Render.Top();
            return (top != null) ? top.origin : null;
        }
    }


    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    bool incar = false;
    void FixedUpdate()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && !incar)
        {
            incar = true;
            reference.position = car.transform.position - new Vector3(0,1.5f,1.5f);
        }
    }
}
