using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.name == "Player" || col.gameObject.tag == "Enemy") {
			// Compare other objects y location to this
			if(Mathf.Sign(col.gameObject.GetComponent<Rigidbody2D>().velocity.y) != Mathf.Sign(gameObject.transform.localScale.y)) {
//			if (col.gameObject.transform.position.y > gameObject.transform.position.y) {
				col.gameObject.SendMessage ("TakeDamage", 2);
				Vector2 knockBack = col.gameObject.transform.position - transform.position;
				knockBack.y = 3;
				knockBack.x = -knockBack.x;
				knockBack *= 10f;
				col.gameObject.GetComponent<PlayerController> ().KnockBack (knockBack);
			}
		}
	}
}
