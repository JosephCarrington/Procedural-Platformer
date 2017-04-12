using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPanelController : MonoBehaviour {

	// Use this for initialization
	Text itemKey;
	Image itemIcon;
	Text itemCount;
	private string itemName;

	private int count = 0;

	Button button;

	public GameObject item;

	void Awake () {
		itemIcon = transform.Find ("Icon").gameObject.GetComponent<Image> ();
		itemCount = transform.Find ("Count").gameObject.GetComponent<Text> ();

		button = gameObject.GetComponent<Button> ();
		button.onClick.AddListener (UseItem);

		HideCount ();
	}

	public void SetIcon(Sprite newIcon) {
		itemIcon.sprite = newIcon;
		itemIcon.enabled = true;
	}

	public void SetCount(int newCount) {
		itemCount.text = newCount.ToString ();
		count = newCount;
	}

	public bool IsEmpty() {
		if (count <= 0) {
			return true;
		}
		return false;
	}

	void ShowCount() {
		Color newColor = itemCount.color;
		newColor.a = 1;
		itemCount.color = newColor;
	}

	void HideCount() {
		Color newColor = itemCount.color;
		newColor.a = 0;
		itemCount.color = newColor;
	}

	public void AddItem(GameObject newItem) {
		// If we already have an item in here, make sure it matches the new one
 		if (item != null) {
			if (newItem.name == itemName) {
				Destroy (newItem);
				SetCount (count + 1);

			} else {
				Debug.LogError ("Cannot add item to inventory at slot. Slot contains a different inventory item");
			}
		} else {
			// We do not have an item in this slot, so add the new one
			SetIcon (newItem.GetComponent<Collectible> ().itemIcon);
			itemName = newItem.name;
			SetCount (1);
			item = newItem;
			ShowCount ();
		}
	}

	public void UseItem() {
		item.GetComponent<InventoryItem> ().UseOnSelf ();
		SetCount (count - 1);

	}
}
