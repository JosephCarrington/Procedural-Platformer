using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	public bool Check() {
		foreach (Transform child in transform) {
			Collider2D c = Physics2D.OverlapPoint (child.transform.position);
			if (c == null) {
				continue;
			}

			if (c.gameObject.tag == "Bad Wall") {
				gameObject.SendMessageUpwards ("Die");
			} else {
				return true;
			}
		}
		return false;
	}
}
