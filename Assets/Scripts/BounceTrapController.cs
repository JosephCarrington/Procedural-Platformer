﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceTrapController : MonoBehaviour {

	// Use this for initialization
	Animator animator;
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		Vector3 newPos = gameObject.transform.position;
		newPos.y -= 0.25f;
		gameObject.transform.position = newPos;
	}
		
	bool sprung = false;
	void OnTriggerEnter2D(Collider2D col) {
		if (col.isTrigger) {
			return;
		}
		if (sprung && col.gameObject.GetComponent<Rigidbody2D>() != null && col.gameObject.GetComponent<Rigidbody2D>().velocity.y >= 0) {
			return;
		}
		if (col.gameObject.tag == "Player" || col.gameObject.GetComponent<Rigidbody2D>() != null) {
			if (sprung) {
				animator.SetTrigger ("Fire");
			}
			col.gameObject.GetComponent<Rigidbody2D> ().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
			sprung = true;
			Vector2 newVel = col.gameObject.GetComponent<Rigidbody2D> ().velocity;
			newVel.y = 0;
			col.gameObject.GetComponent<Rigidbody2D> ().velocity = newVel;
			col.gameObject.GetComponent<Rigidbody2D> ().AddForce (Vector2.up * 40f, ForceMode2D.Impulse);
//			Vector2 newColliderOffset = gameObject.GetComponent<BoxCollider2D> ().offset;
//			newColliderOffset.y = 0.12f;
//			gameObject.GetComponent<BoxCollider2D> ().offset = newColliderOffset;
			animator.SetTrigger("Fire");
		}
	}
}
