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
		int collidersTouchingBad = 0;
		int collidersTouchingGood = 0;
		foreach (Transform child in transform) {
			RaycastHit2D h = Physics2D.Raycast (gameObject.transform.position, child.position - gameObject.transform.position, 0.7f, defaultWalls);
			if (h.transform == null) {
				continue;
			} else {
				if (h.transform.gameObject.layer == LayerMask.NameToLayer ("BadWall")) {
					collidersTouchingBad++;
				} else {
					collidersTouchingGood++;
				}
			}
		}
		if (collidersTouchingGood > 0) {
			return true;
		}

		if (collidersTouchingBad == transform.childCount) {
//			gameObject.SendMessageUpwards ("TakeDamage", 1);
//			gameObject.transform.parent.GetComponent<PlayerController>().KnockBack(new Vector2(0, 10f));
		}

		return false;
	}
}
