using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ChickenController : MonoBehaviour {

	// Use this for initialization
	Animator animator;
	Vector2 destination;
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		SelectFacing ();
		StartCoroutine (SelectNextAction ());
	}

	void SelectFacing() {
		Vector3 newScale = gameObject.transform.localScale;
		if (Random.value < 0.5f) {
			newScale.x = -newScale.x;
		}
		gameObject.transform.localScale = newScale;
	}
	// Update is called once per frame
	float lastFlapTime = 0f;
	public float flapSpeed = 0.25f;
	public Vector2 flapForce = Vector2.up;
	void Update () {
		if (!IsGrounded ()) {
			animator.SetBool ("Flying", true);
			animator.SetBool ("Walking", false);

			if (Time.time > lastFlapTime + flapSpeed) {
				lastFlapTime = Time.time;
				gameObject.GetComponent<Rigidbody2D> ().AddForce (flapForce, ForceMode2D.Impulse);
			}
		} else {
			animator.SetBool ("Flying", false);
			if (gameObject.GetComponent<Rigidbody2D> ().velocity.x != 0f) {
				animator.SetBool ("Walking", true);
			} else {
				animator.SetBool ("Walking", false);

			}
		}

		if(gameObject.GetComponent<Rigidbody2D>().velocity.x != 0f && Mathf.Sign(gameObject.GetComponent<Rigidbody2D>().velocity.x) != Mathf.Sign(transform.localScale.x)) {
			Vector3 newScale = gameObject.transform.localScale;
			newScale.x = -newScale.x;
			gameObject.transform.localScale = newScale;

		}

		
	}

	bool IsGrounded() {
		RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector2.up, 0.51f);
		if (hit.collider != null) {
			return true;
		}
		return false;
	}
	IEnumerator SelectNextAction() {
		yield return null;
	}

	float moveStartTime;
	public float moveTime = 1f;
	IEnumerator Walk(Vector2 targetPos) {
		moveStartTime = Time.time;
		animator.SetBool ("Idle", false);
		Vector2 startPos = gameObject.transform.position;
		while (Vector2.Distance (gameObject.transform.position, targetPos) < 0.1) {
			Vector2 currentPos = gameObject.transform.position;
			currentPos = Vector2.Lerp(startPos, targetPos, Utils.Utils.GetBetweenValue(moveStartTime, moveStartTime + moveTime, Time.time));
			yield return null;
		}
	}
}
