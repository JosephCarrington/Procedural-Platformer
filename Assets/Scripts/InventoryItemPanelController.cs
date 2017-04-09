using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPanelController : MonoBehaviour {

	// Use this for initialization
	Text itemKey;
	Image itemIcon;
	Text itemCount;

	Button button;
	bool empty = true;
	void Awake () {
		itemKey = transform.Find ("Key").gameObject.GetComponent<Text> ();
		itemIcon = transform.Find ("Icon").gameObject.GetComponent<Image> ();
		itemCount = transform.Find ("Count").gameObject.GetComponent<Text> ();

		button = gameObject.GetComponent<Button> ();
		button.onClick.AddListener (UseItem);
	}

	public void SetKey(string newKey) {
		itemKey.text = newKey;
	}

	public void SetIcon(Sprite newIcon) {
		itemIcon.sprite = newIcon;
	}

	public void SetCount(int newCount) {
		itemCount.text = newCount.ToString ();
	}

	public bool IsEmpty() {
		return empty;
	}

	public void UseItem() {
		print ("used");
	}
}
