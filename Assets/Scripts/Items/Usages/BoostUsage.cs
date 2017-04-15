using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
public class BoostUsage : Usage {

	public Vector2 velocity = new Vector2(0, 40);
	public override void UseOnActor(GameObject actor) {
		Vector2 newVel = actor.GetComponent<Rigidbody2D> ().velocity;
		newVel.y = 0;
		actor.GetComponent<Rigidbody2D> ().velocity = newVel;
		actor.GetComponent<Rigidbody2D> ().AddForce (velocity, ForceMode2D.Impulse);
	}

	public override void UseAtPosition(Vector2 position) {
	}
}
