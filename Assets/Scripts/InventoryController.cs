using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class InventoryController : MonoBehaviour {
	GameObject inventoryDisplay;
	InventoryDisplayController inventory;

	void Start() {
		inventoryDisplay = GameObject.Find ("Inventory");
		inventory = inventoryDisplay.GetComponent<InventoryDisplayController> ();
	}
	public bool HasRoom() {
		return true;
	}

	public bool AddItem(Item item) {
		return inventory.AddItem (item);
	}


}