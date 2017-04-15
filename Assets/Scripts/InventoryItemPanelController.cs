using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Inventory;

public class InventoryItemPanelController : MonoBehaviour {

	// Use this for initialization
	Text itemKey;
	Image itemIcon;
	Text itemCount;
	private string itemName;

	private int count = 0;

	Button button;

	public Item item;

	void Awake () {
		itemIcon = transform.Find ("Icon").gameObject.GetComponent<Image> ();
		itemCount = transform.Find ("Count").gameObject.GetComponent<Text> ();

		button = gameObject.GetComponent<Button> ();
		button.onClick.AddListener (UseItem);

		HideCount ();
		HideIcon ();
	}

	public void AddItem(Item newItem) {
		SetIcon (newItem.icon);
		SetCount (count + 1);
		item = newItem;
		ShowCount ();
		ShowIcon ();
	}

	public void SetIcon(Sprite newIcon) {
		itemIcon.sprite = newIcon;
		itemIcon.enabled = true;
	}

	public void SetCount(int newCount) {
		if (newCount < 1) {
			HideCount ();

		} else
			ShowCount ();
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
	void ShowIcon() {
		Color newColor = itemIcon.color;
		newColor.a = 1;
		itemIcon.color = newColor;
	}

	void HideIcon() {
		Color newColor = itemIcon.color;
		newColor.a = 0;
		itemIcon.color = newColor;
	}

	void UseItem() {
		item.UseOnActor (GameObject.Find("Player"));
		SetCount (count - 1);
		if (count <= 0) {
			HideIcon ();
			HideCount ();
			item = null;
		}
	}
		
}
