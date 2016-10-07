using UnityEngine;
using System.Collections;

public class collidething : MonoBehaviour {
    public GameObject enablething;
    // Use this for initialization
    public GameObject crashsound;
    void OnCollisionEnter(Collision other) {
        enablething.SetActive(true);
        GetComponent<AudioSource>().Play();
        other.transform.parent.gameObject.GetComponent<AudioSource>().Stop();
        crashsound.GetComponent<AudioSource>().Play();
    }
}
