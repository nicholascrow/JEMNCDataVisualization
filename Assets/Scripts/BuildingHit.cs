using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BuildingHit : MonoBehaviour {

    // For seeing if collison with car
    void OnCollisionEnter(Collision col)
    {
         // If the car has hit the building
        if (col.gameObject.tag.Equals("car"))
        {
            
            StartCoroutine(ChangeLevel()); // Call ChangeLevel
        }
        
    }

    IEnumerator ChangeLevel()
    {
        // Wait for three seconds before beginning the fade
        yield return new WaitForSeconds(3.0f); 
        // Fade out and load new level
        float fadeTime = GameObject.Find("Quad").GetComponent<FadingEyeblink>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        // Change it to the next level (when you are in jail)
        SceneManager.LoadScene(1);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
