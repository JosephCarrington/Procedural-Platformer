using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockfallTrapRockController : MonoBehaviour {

	public void OnCollisionEnter2D(Collision2D col) {
		int damageVelocity = 2;
		if (Mathf.Abs(col.relativeVelocity.y) > damageVelocity) {
			if (col.gameObject.tag == "Player") {
				if (col.contacts [0].normal.y > 0.5) {
					col.gameObject.SendMessage("TakeDamage", 10);
				}
			}
		}
		if (col.gameObject.tag == "Enemy") {
			col.gameObject.SendMessage("TakeDamage", 10);
		}
	}
}
