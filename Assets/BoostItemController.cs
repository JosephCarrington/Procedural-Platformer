using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostItemController : MonoBehaviour {

	public Vector2 boostForce;
	void Use() {
		GameObject owner = gameObject.GetComponent<InventoryItemController>().Owner;
		Vector2 newVel = owner.GetComponent<Rigidbody2D> ().velocity;
		newVel.y = 0;
		newVel.x = 0;
		owner.GetComponent<Rigidbody2D> ().velocity = newVel;
		owner.GetComponent<Rigidbody2D> ().AddForce (boostForce, ForceMode2D.Impulse);
		owner.transform.Find ("DoubleJump Particles").GetComponent<ParticleSystem> ().Play ();
	}
}
