using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureController : MonoBehaviour {

	// Use this for initialization
	public Sprite openSprite;
	public GameObject[] randomContents;
	public int minContentCount = 1;
	public int maxContentCount = 4;
	private List<GameObject> contents;
	void Start () {
		contents = new List<GameObject> ();
		int c = Random.Range (minContentCount, maxContentCount + 1);
		for (int i = 0; i < c; i++) {
			contents.Add(randomContents[Random.Range(0, randomContents.Length)]);
		}
	}
	bool opened = false;
	void OnTriggerStay2D(Collider2D col) {
		if (!opened && col.gameObject.name == "Player") {
			if (Input.GetButtonDown ("Interact")) {
				opened = true;
				gameObject.GetComponent<SpriteRenderer> ().sprite = openSprite;
				foreach (GameObject item in contents) {
					GameObject newItem = GameObject.Instantiate (item, transform.position, Quaternion.identity);
					newItem.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-1f, 1f), Random.Range (1f, 2f)), ForceMode2D.Impulse);
				}
			}
		}
	}

}
