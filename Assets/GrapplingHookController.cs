using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D col) {
		gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		gameObject.GetComponent<SpringJoint2D> ().connectedBody = GameObject.Find ("Player").GetComponent<Rigidbody2D> ();
		gameObject.GetComponent<SpringJoint2D> ().enabled = true;
	}
}
