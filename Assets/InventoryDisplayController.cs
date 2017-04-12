using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayController : MonoBehaviour {

	private InventoryItemPanelController[] slots;
	void Start() {
		slots = new InventoryItemPanelController[9];
		int i = 0;
		foreach (Transform child in transform) {
			slots [i] = child.GetComponent<InventoryItemPanelController> ();
			i++;
		}
	}

	public int GetItemSlot() {
		return GetFirstEmptySlot ();
	}
	public int GetItemSlot(GameObject item) {
		int i = 0;
		foreach(InventoryItemPanelController slot in slots) {
			if (slot.item == item.GetComponent<InventoryItem>()) {
				return i;
			}	
			i++;
		}
		return(GetItemSlot ());
	}

	int GetFirstEmptySlot() {
		int i = 0;
		foreach(InventoryItemPanelController slot in slots) {
			if (slot.IsEmpty ()) {
				return i;
			}	
			i++;
		}

		return -1;
	}

	public void AddItemAtSlot(GameObject item, int slot) {
		slots [slot].AddItem (item);
	}
}
