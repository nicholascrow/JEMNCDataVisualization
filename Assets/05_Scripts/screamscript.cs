using UnityEngine;
using System.Collections;

public class screamscript : MonoBehaviour
{
    public GameObject manyscreams;
    bool played = false;
    void OnCollisionEnter(Collision collision)
    {
        print(collision.other.tag);
        if (!played && collision.other.tag.ToString() == "car")
        {
            if (manyscreams) {
                foreach (var item in manyscreams.GetComponents<AudioSource>())
                {
                    item.Play();
                }
            }
           GetComponent<AudioSource>().Play();
            played = true;
            StartCoroutine(screamEnd());
        }
        
    }

    IEnumerator screamEnd()
    {
        yield return new WaitForSeconds(.5f);
        played = false;
    }
}
