using UnityEngine;
using System.Collections;

public class collidething : MonoBehaviour {
    public GameObject enablething;
    // Use this for initialization
    public GameObject crashsound;
    void OnCollisionEnter(Collision other) {
        if(other.transform.parent.gameObject.GetComponent<AudioSource>()) {
            enablething.SetActive(true);
            GetComponent<AudioSource>().Play();
            other.transform.parent.gameObject.GetComponent<AudioSource>().Stop();
            crashsound.GetComponent<AudioSource>().Play();
        }
    }
}
