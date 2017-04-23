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
		switch (col.gameObject.tag) {
		case "Player":
			foreach (Item item in itemsToGet) {
				col.gameObject.GetComponent<InventoryController> ().AddItem (item);
			}
			Destroy (gameObject);
			break;
		}
	}

}
