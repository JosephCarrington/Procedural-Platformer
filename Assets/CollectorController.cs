using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class CollectorController : MonoBehaviour {

	// Use this for initialization
	private Animator animator;
	public GameObject[] randomStartingItems;
	private List<GameObject> heldItems;
	GameObject player;
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		player = GameObject.Find ("Player");
		Physics2D.IgnoreLayerCollision (gameObject.layer, gameObject.layer);

		heldItems = new List<GameObject> ();

		GameObject startingItem = GameObject.Instantiate (randomStartingItems [Random.Range (0, randomStartingItems.Length)], gameObject.transform);
		startingItem.transform.localPosition = Vector3.up;
		heldItems.Add (startingItem);
		startingItem.SetActive (false);
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
				Vector3 newScale = gameObject.transform.localScale;
				newScale.x = -newScale.x;
				gameObject.transform.localScale = newScale;
			}
		} else {
			if (Mathf.Sign (gameObject.GetComponent<Rigidbody2D> ().velocity.x) != Mathf.Sign (gameObject.transform.localScale.x)) {
				Vector3 newScale = gameObject.transform.localScale;
				newScale.x = -newScale.x;
				gameObject.transform.localScale = newScale;
			}
				
		}

	}

	void OnTriggerEnter2D(Collider2D col) {
		Vector2 difference;
		if (col.gameObject.name == "Player") {
			animator.SetBool ("Cowering", true);
			difference = player.transform.position - transform.position;
			if (CheckGround ()) {
				gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-difference.x, 20 + Random.value), ForceMode2D.Impulse);
				lookingAtPlayer = false;
				StartCoroutine(LookAtPlayerAgain(5f));
			}


		}

	}

	bool pickingUp = false;
	void OnTriggerStay2D(Collider2D col) {
		if (!pickingUp && col.gameObject.GetComponent<PickupController> () != null) {
			lookingAtPlayer = false;
			//			pickingUp = true;
			Vector2 difference = col.gameObject.transform.position - gameObject.transform.position;
			float movement = difference.x * 100f * Time.deltaTime;
			gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (movement, 0f));


		}
	}


	void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			animator.SetBool ("Cowering", false);

		}
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.name == "Player") {
			if (col.contacts [0].normal.y < 0) {
				if (col.relativeVelocity.magnitude > 1f) {
					col.gameObject.GetComponent<PlayerController> ().BounceOffEnemy (1f);
					gameObject.SendMessage ("TakeDamage", 1);
				}
			}
		}
		if (col.gameObject.GetComponent<PickupController> () != null) {
			pickingUp = true;

			col.gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			animator.SetTrigger ("Picking Up");
			StartCoroutine (SetNotPickingUp (col.gameObject));

		}

	}

	IEnumerator SetNotPickingUp(GameObject item) {
		yield return new WaitForSeconds (0.5f);
		item.transform.parent = gameObject.transform;
		item.transform.localPosition = Vector3.up;
		heldItems.Add (item);
		item.SetActive (false);
		pickingUp = false;
	}

	void Die() {
		foreach (GameObject item in heldItems) {

			item.SetActive (true);
			item.transform.parent = null;
			item.GetComponent<PickupController> ().startTime = Time.time;
			item.gameObject.GetComponent<Rigidbody2D> ().AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 4f)), ForceMode2D.Impulse);

		}
		gameObject.GetComponent<ParticleSystem> ().Play ();
		gameObject.GetComponent<Collider2D> ().enabled = false;
		gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;

		Destroy (gameObject, 0.25f);
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
