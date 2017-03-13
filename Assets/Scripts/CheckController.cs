using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckController : MonoBehaviour {

	// Use this for initialization
	public LayerMask defaultWalls;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	public bool Check() {
		foreach (Transform child in transform) {
			RaycastHit2D h = Physics2D.Raycast (gameObject.transform.position, child.position - gameObject.transform.position, 0.7f, defaultWalls);
			if (h.transform == null) {
				continue;
			} else {
				if (h.transform.gameObject.layer == LayerMask.NameToLayer ("BadWall")) {
					gameObject.SendMessageUpwards ("Die");
				}
				return true;
			}
		}
		return false;
	}
}
