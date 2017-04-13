using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {
	public GameObject inventoryDisplay;
	InventoryDisplayController inventory;

	void Start() {
		inventory = inventoryDisplay.GetComponent<InventoryDisplayController> ();
	}
	public bool HasRoom() {
		return true;
	}

	public void AddItem(GameObject item) {
//		// First find if we already have this item
//		int slot = inventory.GetItemSlot(item);
//		if (slot != -1) {
//			inventory.AddItemAtSlot (item, slot);
//		}

	}


}