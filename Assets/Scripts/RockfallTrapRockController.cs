using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockfallTrapRockController : MonoBehaviour {
	bool didDamage = false;
	public bool triggered = false;
	public void OnCollisionEnter2D(Collision2D col) {
		int damageVelocity = 2;
		if (triggered && !didDamage && Mathf.Abs(col.relativeVelocity.y) > damageVelocity) {
			if (col.gameObject.tag == "Player") {
				if (col.contacts [0].normal.y > 0.5) {
					didDamage = true;
					col.gameObject.GetComponent<Animator> ().SetTrigger ("Squish");
					col.gameObject.SendMessage("TakeDamage", 10);
				}
			}
		}
		if (triggered && col.gameObject.tag == "Enemy") {
			col.gameObject.SendMessage("TakeDamage", 10);
		}
	}
}
