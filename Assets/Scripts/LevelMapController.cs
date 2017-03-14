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
			gameObject.GetComponent<tk2dCamera> ().CameraSettings.rect = fullscreenViewportRect;
			gameObject.GetComponent<tk2dCamera> ().ZoomFactor = fullScreenZoomFactor;
//			StartCoroutine(MaximizeMap());
		}
		if (Input.GetButtonUp ("Map")) {
			gameObject.GetComponent<tk2dCamera> ().CameraSettings.rect = startingViewportRect;
			gameObject.GetComponent<tk2dCamera> ().ZoomFactor = startingZoomFactor;
		}
	}
//	float wVel, hVel, xVel, yVel, zoomVel;
//	public float smoothTime = 0.5f;
//
//	IEnumerator MaximizeMap() {
//		Rect currentRect = gameObject.GetComponent<tk2dCamera> ().CameraSettings.rect;
//		float currentZoom = gameObject.GetComponent<tk2dCamera> ().ZoomFactor;
//
//		while (currentRect != fullscreenViewportRect && currentZoom != fullScreenZoomFactor) {
//			float newW = Mathf.SmoothDamp (currentRect.width, fullscreenViewportRect.width, ref wVel, smoothTime);
//			float newH = Mathf.SmoothDamp (currentRect.height, fullscreenViewportRect.height, ref hVel, smoothTime);
//			float newX = Mathf.SmoothDamp (currentRect.x, fullscreenViewportRect.x, ref xVel, smoothTime);
//			float newY = Mathf.SmoothDamp (currentRect.y, fullscreenViewportRect.y, ref yVel, smoothTime);
//			float newZoom = Mathf.SmoothDamp (currentZoom, fullScreenZoomFactor, ref zoomVel, smoothTime);
//			gameObject.GetComponent<tk2dCamera> ().CameraSettings.rect = new Rect (newX, newY, newW, newH);
//			gameObject.GetComponent<tk2dCamera> ().ZoomFactor = newZoom;
//			yield return null;
//		}
//	}
}
