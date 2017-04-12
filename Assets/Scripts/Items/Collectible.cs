using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Collectible : MonoBehaviour {

	// Use this for initialization
	public Sprite itemIcon;

	float startTime;
	void Start () {
		startTime = Time.time;
		// Ignore our non-trigger collider, to avoid weird physics
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

	void OnTriggerEnter2D(Collider2D col) {
		if (Time.time < startTime + dontCollectTime) {
			return;
		}

		if (col.gameObject.tag == "Player") {
			InventoryController inventory = col.gameObject.GetComponent<InventoryController> ();
			if (inventory.HasRoom ()) {
				Owner = col.gameObject;
				inventory.AddItem (gameObject);
			}
		}
	}
}
