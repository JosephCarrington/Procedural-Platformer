﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableController : MonoBehaviour {

	// Use this for initialization
	public Sprite destroyedSprite;
	private bool destroyed = false;
	void OnTriggerEnter2D(Collider2D col) {
		if (col.isTrigger) {
			return;
		}
		if (!destroyed) {
			destroyed = true;
			gameObject.GetComponent<SpriteRenderer> ().sprite = destroyedSprite;
			if (gameObject.GetComponentInChildren<ParticleSystem> ()) {
				gameObject.GetComponentInChildren<ParticleSystem> ().Play ();
			}
		}
	}
}
