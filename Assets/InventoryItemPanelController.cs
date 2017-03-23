using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class InventoryItemPanelController : MonoBehaviour {

	// Use this for initialization
	Text itemKey;
	Image itemIcon;
	Text itemCount;
	void Awake () {
		itemKey = transform.Find ("ItemKeyPanel/ItemKey").gameObject.GetComponent<Text> ();
		itemIcon = transform.Find ("ItemIconPanel/Icon").gameObject.GetComponent<Image> ();
		itemCount = transform.Find ("ItemIconPanel/Count").gameObject.GetComponent<Text> ();
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
	// Update is called once per frame
	void Update () {
		
	}
}
