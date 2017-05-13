using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChudHandDamagerController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			col.gameObject.GetComponent<PlayerController> ().TakeDamage (1);
			Vector2 dir = col.gameObject.transform.position - gameObject.transform.position;
			dir.y += 1f;
			col.gameObject.GetComponent<PlayerController> ().KnockBack (dir * 20f);

		}
	}
}
