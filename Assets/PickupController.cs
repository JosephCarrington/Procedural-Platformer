using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
public class PickupController : MonoBehaviour {

	float dontPickupTime = 0.5f;
	public float startTime;
	public Item[] itemsToGet;

	public void Start() {
		startTime = Time.time;
		Physics2D.IgnoreCollision (gameObject.GetComponent<Collider2D> (), GameObject.Find ("Player").GetComponent<Collider2D> ());
	}

	public void OnTriggerEnter2D(Collider2D col) {
		if (Time.time < startTime + dontPickupTime)
			return;
		bool addedItem = false;
		switch (col.gameObject.tag) {
		case "Player":
			foreach (Item item in itemsToGet) {
				addedItem = col.gameObject.GetComponent<InventoryController> ().AddItem (item);
			}
			break;
		}
		if (addedItem) {
			Destroy (gameObject);
		}
	}

	bool onFire = false;
	public void OnCollisionEnter2D(Collision2D col) {
		if (!onFire && col.gameObject.layer == LayerMask.NameToLayer ("BadWall")) {
			onFire = true;
			GameObject.Instantiate (Resources.Load ("Fire"), gameObject.transform);
			Destroy (gameObject, 1f);
		}
	}
}
