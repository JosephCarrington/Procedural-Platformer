using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorController : MonoBehaviour {

	// Use this for initialization
	private Animator animator;
	GameObject player;
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		player = GameObject.Find ("Player");
		Physics2D.IgnoreLayerCollision (gameObject.layer, gameObject.layer);
	}


	bool grounded = true;
	bool lookingAtPlayer = true;
	// Update is called once per frame
	void Update () {
		grounded = CheckGround ();
		if (!grounded) {
			animator.SetBool ("InAir", true);
		} else {
			animator.SetBool ("InAir", false);
		}

		if (Mathf.Abs(gameObject.GetComponent<Rigidbody2D> ().velocity.x) > 0.1f) {
			animator.SetBool ("Walking", true);
			animator.speed = Mathf.Clamp(Mathf.Abs(gameObject.GetComponent<Rigidbody2D> ().velocity.x / 2), 0.1f, 3f);

		} else {
			animator.SetBool ("Walking", false);

		}
		if (lookingAtPlayer) {
			Vector2 difference = player.transform.position - transform.position;
			if (Mathf.Sign (difference.x) != Mathf.Sign (gameObject.transform.localScale.x)) {
				Vector2 newScale = gameObject.transform.localScale;
				newScale.x = -newScale.x;
				gameObject.transform.localScale = newScale;
			}
		} else {
			if (Mathf.Sign (gameObject.GetComponent<Rigidbody2D> ().velocity.x) != Mathf.Sign (gameObject.transform.localScale.x)) {
				Vector2 newScale = gameObject.transform.localScale;
				newScale.x = -newScale.x;
				gameObject.transform.localScale = newScale;
			}
				
		}

	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			animator.SetBool ("Cowering", true);
			Vector2 difference = player.transform.position - transform.position;
			if (CheckGround ()) {
				gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-difference.x, 20), ForceMode2D.Impulse);
				lookingAtPlayer = false;
				StartCoroutine(LookAtPlayerAgain(5f));
			}


		}
	}


	void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			animator.SetBool ("Cowering", false);

		}
	}

	IEnumerator LookAtPlayerAgain(float t) {
		yield return new WaitForSeconds (t);
		lookingAtPlayer = true;
	}

	bool CheckGround() {
		RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector2.up, 0.6f);
		if (hit.collider != null) {
			return true;
		}

		return false;

	}
}
