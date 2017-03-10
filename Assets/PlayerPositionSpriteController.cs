using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionSpriteController : MonoBehaviour {

	// Use this for initialization
	GameObject player;
	void Start () {
		player = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newPos = player.transform.position;
		gameObject.transform.position = newPos;
	}
}
