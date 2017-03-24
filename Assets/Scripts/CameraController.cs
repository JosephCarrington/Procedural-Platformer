using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	GameObject player;
	Text score;
	private Vector2 lookOffset;
	public Vector2 lookOffsetStrength = Vector2.one;

	void Start () {
		player = GameObject.Find ("Player");
	}

	void Update() {
		float h = Input.GetAxis ("Look Horizontal");
		float v = Input.GetAxis ("Look Vertical");
		lookOffset = new Vector2 (h * lookOffsetStrength.x, v * lookOffsetStrength.y);

	}
	// Update is called once per frame
	Vector2 currentVel;

	public float lookaheadFactor = 0.25f;
	public float smoothTime = 0.5f;
	void FixedUpdate () {
		Vector2 newPos = player.transform.position;
		newPos += lookOffset;
		newPos += (player.GetComponent<Rigidbody2D> ().velocity) * lookaheadFactor;
//		newPos.z = transform.position.z;
//		transform.position = newPos;
		newPos = Vector2.SmoothDamp (transform.position, newPos, ref currentVel, smoothTime, Mathf.Infinity, Time.deltaTime);
		transform.position = new Vector3 (newPos.x, newPos.y, transform.position.z);
	}

//		void SetScore(int newScore) {
//		score.text = newScore.ToString();
//	}


}
