using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChudWalkColliderController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			gameObject.SendMessageUpwards ("FacePlayer", col.gameObject);
			gameObject.transform.parent.GetComponent<Animator> ().SetTrigger ("WalkToPlayer");

		}
	}
}
