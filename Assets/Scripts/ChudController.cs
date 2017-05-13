using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChudController : MonoBehaviour {
	public void FacePlayer(GameObject player) {
		Vector2 dir = player.transform.position - gameObject.transform.position;
		if(Mathf.Sign(dir.x) != Mathf.Sign(transform.localScale.x)) {
			Vector3 newScale = transform.localScale;
			newScale.x = -newScale.x;
			gameObject.transform.localScale = newScale;
		}
	}

	public void Die() {
		gameObject.GetComponent<Rigidbody2D> ().AddTorque (20f);
		gameObject.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.None;
		gameObject.GetComponent<Animator> ().SetTrigger ("Die");
	}

	public void Destroy() {
		DestroyInSeconds (5f);
	}

	IEnumerator DestroyInSeconds(float s) {
		yield return new WaitForSeconds (s);
		GameObject.Destroy (gameObject);

	}
}
