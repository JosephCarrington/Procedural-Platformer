using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMapController : MonoBehaviour {

	// Use this for initialization
	GameObject player;
	Rect startingViewportRect;
	public Rect fullscreenViewportRect;

	float startingZoomFactor;
	public float fullScreenZoomFactor;

	void Start () {
		player = GameObject.Find ("Player");
		startingViewportRect = gameObject.GetComponent<tk2dCamera> ().CameraSettings.rect;
		startingZoomFactor = gameObject.GetComponent<tk2dCamera> ().ZoomFactor;

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Map")) {
			print ("maximizing map");
			gameObject.GetComponent<tk2dCamera> ().CameraSettings.rect = fullscreenViewportRect;
			gameObject.GetComponent<tk2dCamera> ().ZoomFactor = fullScreenZoomFactor;
		}
		if (Input.GetButtonUp ("Map")) {
			print ("minimizing map");
			gameObject.GetComponent<tk2dCamera> ().CameraSettings.rect = startingViewportRect;
			gameObject.GetComponent<tk2dCamera> ().ZoomFactor = startingZoomFactor;
		}
	}
}
