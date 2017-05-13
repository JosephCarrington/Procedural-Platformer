using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChudWakeColliderController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			gameObject.transform.parent.GetComponent<Animator> ().SetTrigger ("WakeUp");
		}
	}
	void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			gameObject.transform.parent.GetComponent<Animator> ().SetTrigger ("Sleep");
		}
	}
}
