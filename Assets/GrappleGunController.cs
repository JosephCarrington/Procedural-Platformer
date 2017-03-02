using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGunController : MonoBehaviour {

	// Use this for initialization
	LineRenderer line;
	public GameObject hook;
	void Start () {
		
		line = gameObject.GetComponent<LineRenderer> ();

	}
	
	// Update is called once per frame
	public float fireSpeed = 4f;

	bool fired = false;
	public LayerMask allButSelf;
	void Update () {
		if (fired) {
			Vector3[] points = new Vector3[2];
			points [0] = transform.position;
			points [1] = hook.transform.position;
			line.positionCount = 2;
			line.SetPositions (points);
		}
		else {
			if (Input.GetButton ("Fire1")) {
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
				line.positionCount = 2;

				RaycastHit2D hit = Physics2D.Raycast (transform.position, mousePos - transform.position, Mathf.Infinity, allButSelf);
				print (hit.collider.name);



				Vector2 hookPos = mousePos - transform.position;
				hookPos.Normalize ();

				Vector2 playerPos = transform.position;
				hook.transform.position = playerPos + (hookPos * 2f);

				Vector3[] points = new Vector3[2];
				points [0] = transform.position;
				points [1] = playerPos + (hookPos * 2f);
				line.SetPositions (points);

			}


			if (Input.GetButtonUp ("Fire1")) {
				hook = GameObject.Instantiate (hook);
				Rigidbody2D body = hook.GetComponent<Rigidbody2D> ();
				body.velocity = (line.GetPosition (1) - transform.position) * fireSpeed;
				line.positionCount = 0;

//
				fired = true;

			}
		}	
	}
}
