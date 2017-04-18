using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class InventoryDisplayController : MonoBehaviour {
	private int slotCount = 2;
	private List<GameObject> slots = new List<GameObject>();

	public void Start() {
		foreach (Transform child in transform) {
			slots.Add (child.gameObject);
		}
		UpdateDisplayCount ();

	}
	public void UpdateDisplayCount() {
		int i = 1;


		foreach (GameObject slot in slots) {
			slot.SetActive (false);
			if (i > slotCount) {
				continue;
			}

			slot.SetActive (true);
			i++;
		}


		Vector2 newSize = gameObject.GetComponent<RectTransform> ().sizeDelta;
		newSize.x = slotCount * 100f;
		gameObject.GetComponent<RectTransform> ().sizeDelta = newSize;
	}

	public bool AddItem(Item item) {

		GameObject newSlot = GetSlotForItem (item);
		if (newSlot == null) {
			return false;
		}
		newSlot.GetComponent<InventoryItemPanelController> ().AddItem (item);
		return true;
	}

	private GameObject GetSlotForItem(Item item) {
		GameObject returnSlot = null;
		foreach (GameObject slot in slots) {
			if (slot.GetComponent<InventoryItemPanelController> ().item == item) {
				returnSlot = slot;
				break;
			}
		}

		if (returnSlot == null) {
			returnSlot = GetFirstEmptySlot ();
		}

		return returnSlot;
	}

	private bool HasEmptySlot() {
		foreach (GameObject slot in slots) {
			if (!slot.activeSelf) {
				continue;
			}
			if(slot.GetComponent<InventoryItemPanelController>().IsEmpty()) {
				return true;
			}
		}
		return false;
	}

	private GameObject GetFirstEmptySlot() {
		GameObject returnSlot = null;
		foreach (GameObject slot in slots) {
			if (!slot.activeSelf) {
				continue;
			}
			if(slot.GetComponent<InventoryItemPanelController>().IsEmpty()) {
				returnSlot = slot;
				break;
			}
		}
		return returnSlot;
	}
}
