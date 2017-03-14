using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableController : MonoBehaviour {

	// Use this for initialization
	public Sprite destroyedSprite;
	private bool destroyed = false;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (!destroyed) {
			destroyed = true;
			gameObject.GetComponent<SpriteRenderer> ().sprite = destroyedSprite;
			if (gameObject.GetComponentInChildren<ParticleSystem> ()) {
				gameObject.GetComponentInChildren<ParticleSystem> ().Play ();
			}
		}
	}
}
