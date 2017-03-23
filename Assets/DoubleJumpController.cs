using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(InventoryItemController))]
public class DoubleJumpController : MonoBehaviour {
	InventoryItemController inventory;
	public float boostAmount = 20f;
	void Start() {
		inventory = gameObject.GetComponent<InventoryItemController> ();
	}
	void Use() {
		inventory.Owner.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, boostAmount), ForceMode2D.Impulse);
	}
}
