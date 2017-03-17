using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ChickenController : MonoBehaviour {

	// Use this for initialization
	Animator animator;
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		StartCoroutine (SelectNextAction ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator SelectNextAction() {
		yield return null;
	}

	float moveStartTime;
	public float moveTime = 1f;
	IEnumerator Walk(Vector2 targetPos) {
		moveStartTime = Time.time;
		animator.SetBool ("Idle", false);
		Vector2 startPos = gameObject.transform.position;
		while (Vector2.Distance (gameObject.transform.position, targetPos) < 0.1) {
			Vector2 currentPos = gameObject.transform.position;
			currentPos = Vector2.Lerp(startPos, targetPos, Utils.Utils.GetBetweenValue(moveStartTime, moveStartTime + moveTime, Time.time));
			yield return null;
		}
	}
}
