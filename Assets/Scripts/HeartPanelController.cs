using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartPanelController : MonoBehaviour {

	List<GameObject> hearts = new List<GameObject>();

	void Awake() {
		foreach (Transform child in transform) {
			hearts.Add (child.gameObject);
		}
		
	}

	public void SetHeartCount(int hp) {
		for(int i = 0; i < hearts.Count; i++) {
			Color newColor = hearts[i].GetComponent<Image> ().color;
			newColor.a = i < hp ? 1 : 0;
			hearts[i].GetComponent<Image> ().color = newColor;
		}
	}
}
