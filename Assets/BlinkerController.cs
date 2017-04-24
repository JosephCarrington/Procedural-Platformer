using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BlinkerController : MonoBehaviour {

	// Use this for initialization
	bool off = false;
	void Start () {
		StartCoroutine (Blink ());
	}

	void Update() {
		if (Input.GetButtonDown ("Jump")) {
			SceneManager.LoadScene ("Instructions");
		}
	}

	IEnumerator Blink() {
		while (true) {
			yield return new WaitForSeconds (0.25f);
			if (off) {
				gameObject.GetComponent<Text>().enabled = false;	
			} else {
				gameObject.GetComponent<Text>().enabled = true;	
			}
			off = !off;
		}
	}
}
