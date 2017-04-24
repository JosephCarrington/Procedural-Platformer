using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaPeelController : MonoBehaviour {

	void Start() {
		Physics2D.IgnoreCollision (gameObject.GetComponent<Collider2D> (), GameObject.Find ("Player").GetComponent<Collider2D> ());
	}
	void OnTriggerEnter2D(Collider2D col) {
		print (col.gameObject.name);
		if(col.gameObject.GetComponent<Rigidbody2D>() != null) {
			Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
			Vector2 v = rb.velocity;
			float x = v.x;

			Vector2 newVel = new Vector2 (x < 0 ? Mathf.Clamp (x * 10, -20f, -2f) : Mathf.Clamp (x * 10, 2f, 20f), 0);
			rb.AddForce (newVel, ForceMode2D.Impulse);
		}
	}

}
