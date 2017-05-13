using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChudHeadDamageController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			gameObject.SendMessageUpwards ("TakeDamage", 1);
			col.gameObject.GetComponent<PlayerController> ().BounceOffEnemy (1f);

		}
	}
}
