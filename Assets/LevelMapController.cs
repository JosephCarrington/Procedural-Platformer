using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMapController : MonoBehaviour {

	// Use this for initialization
	GameObject player;
	void Start () {
		player = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newPos = gameObject.transform.position;
		newPos.x = player.transform.position.x;
		newPos.y = player.transform.position.y;
		gameObject.transform.position = newPos;
	}
}
