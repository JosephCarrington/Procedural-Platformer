using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class InventoryItemController : MonoBehaviour {

	// Use this for initialization

	float startTime;
	void Start () {
		startTime = Time.time;
		Physics2D.IgnoreCollision (gameObject.transform.GetComponent<Collider2D> (), GameObject.Find ("Player").GetComponent<Collider2D> ());
	}
	public float dontCollectTime = 0.25f;

	public bool destroyOnUse = true;
	private GameObject owner;
	public GameObject Owner {
		get {
			return owner;
		}
		set {
			if (value.tag == "Player") {
				owner = value;
			}
		}
	}

	void PickUp(GameObject newOwner) {
		if (newOwner.GetComponent<InventoryController> ().HasRoomInInventory ()) {
			owner = newOwner;
			newOwner.GetComponent<InventoryController> ().AddToInventory (gameObject);
			Destroy (gameObject.GetComponent<Collider2D> ());
		}
	}

	public void UseItem() {
		gameObject.SendMessage ("Use");
		if (destroyOnUse) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (Time.time < startTime + dontCollectTime) {
			return;
		}

		if (col.gameObject.tag == "Player") {
			PickUp (col.gameObject);
		}
	}
}
