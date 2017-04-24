using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
public class BananaUsage : Usage {

	public override void UseOnActor(GameObject actor) {
		actor.SendMessage ("HealDamage", 1);
		GameObject peel = GameObject.Instantiate (Resources.Load ("Banana Peel"), actor.transform.position + new Vector3(0, 1, -2f), Quaternion.identity) as GameObject;
		peel.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-2f, 2f), 1f), ForceMode2D.Impulse);
	}

	public override void UseAtPosition(Vector2 position) {
	}
}
