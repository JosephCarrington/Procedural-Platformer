using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartPanelController : MonoBehaviour {

	public GameObject heartPrefab;
	GameObject[] hearts = new GameObject[10];
	public Vector2 heartSize = Vector2.one * 32;
	public float bufferPixels = 16;
//	private int lastHP = 3;
	// Use this for initialization
	void Awake () {
		for (int i = 0; i < 10; i++) {
			hearts [i] = GameObject.Instantiate (heartPrefab, transform);
			Vector2 heartPos = hearts [i].GetComponent<RectTransform> ().position;
			heartPos.x += (heartSize.x * i);
			if (i > 0) {
				heartPos.x += (bufferPixels * i);
			}
			hearts [i].GetComponent<RectTransform> ().position = heartPos;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetHeartCount(int hp) {
		for(int i = 0; i < hearts.Length; i++) {
			Color newColor = hearts[i].GetComponent<Image> ().color;
			newColor.a = i < hp ? 1 : 0;
			hearts[i].GetComponent<Image> ().color = newColor;
		}
	}
}
