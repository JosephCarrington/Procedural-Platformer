using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChudChaseColliderController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			gameObject.SendMessageUpwards ("FacePlayer", col.gameObject);
			gameObject.transform.parent.GetComponent<Animator> ().SetTrigger ("ChasePlayer");

		}
	}
	void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			gameObject.SendMessageUpwards ("FacePlayer", col.gameObject);
			gameObject.transform.parent.GetComponent<Animator> ().SetTrigger ("WalkToPlayer");

		}
	}
}
