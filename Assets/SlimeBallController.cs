using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBallController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		startTime = Time.time;
		Physics2D.IgnoreCollision (gameObject.transform.GetComponent<CircleCollider2D> (), GameObject.Find ("Player").GetComponent<BoxCollider2D> ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public float dontCollectTime = 0.25f;
	float startTime;
	void OnTriggerEnter2D(Collider2D col) {
		if (Time.time < startTime + dontCollectTime) {
			return;
		}
		if (col.gameObject.tag == "Player") {
			col.gameObject.SendMessage ("TransferToInventory", gameObject);
			GameObject.Destroy (gameObject);
		}
	}
}
