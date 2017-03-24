using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {
	public int numSlots = 9;

	public GameObject inventoryPanel;
	public GameObject inventoryItemPanelPrefab;
	private List<GameObject>[] slots;
	void Start() {
		slots = new List<GameObject>[numSlots];
		for(int i = 0; i < numSlots; i ++) {
			slots [i] = new List<GameObject> ();
		}

	}

	void Update() {
		for (int i = 1; i <= numSlots; i++) {
			if (Input.GetKeyDown (i.ToString())) {
				GameObject item = GetItemAtSlot (i);
				if (item == null) {
					return;
				}
				item.GetComponent<InventoryItemController> ().UseItem ();
				UpdateInventoryDisplay ();

			}
		}
	}

	public void UpdateInventoryDisplay() {
		foreach (Transform child in inventoryPanel.transform) {
			Destroy (child.gameObject);
		}
		for (int i = 0; i < slots.Length; i++) {
			List<GameObject> slot = slots [i];
	
			if (slot.Count != 0) {
				GameObject newPanel = GameObject.Instantiate (inventoryItemPanelPrefab, inventoryPanel.transform);
				Vector3 newPos = newPanel.GetComponent<RectTransform> ().position;
				newPos.x = i * inventoryItemPanelPrefab.GetComponent<RectTransform> ().rect.width;
				newPanel.GetComponent<RectTransform> ().position = newPos;
				newPanel.GetComponent<InventoryItemPanelController> ().SetKey ((i + 1).ToString ());
				newPanel.GetComponent<InventoryItemPanelController> ().SetIcon (slot[i].GetComponent<SpriteRenderer>().sprite);
				newPanel.GetComponent<InventoryItemPanelController> ().SetCount (slot.Count);

			}
		}
	}

	public void AddToInventory(GameObject newItem) {
		int itemSlot = FindItemSlot (newItem.name);
		if (itemSlot != -1) {
			slots [itemSlot].Add (newItem);
		} else if(HasRoomInInventory()) {
			itemSlot = GetEmptySlot ();
			if (itemSlot != -1) {
				slots [itemSlot].Add (newItem);
			}
		}

		UpdateInventoryDisplay ();
	}
	


	GameObject GetItemAtSlot(int slotId) {
		slotId--;
		if (slots [slotId].Count > 0) {
			GameObject r = slots [slotId] [slots [slotId].Count - 1];
			slots [slotId].Remove (r);
			return r;
		}
		return null;
	}

	int FindItemSlot(string itemName) {
		for (int i = 0; i < slots.Length; i++) {
			if (slots [i].Count > 0 && slots[i][0].name == itemName) {
				return i;
			}

		}
		return -1;
	}

	private int GetEmptySlot() {
		for (int i = 0; i < slots.Length; i++) {
			if (slots [i].Count == 0) {
				return i;
			}
		}
		return -1;
	}

	public bool HasRoomInInventory()  {
		foreach (List<GameObject> slot in slots) {
			if (slot.Count == 0) {
				return true;
			}
		}
		return false;
	}
}
