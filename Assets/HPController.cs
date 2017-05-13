using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPController : MonoBehaviour {

	// Use this for initialization
	public int hp = 1;
	public Color hurtColor = Color.magenta;
	private Color startColor;
	public void Start() {
		startColor = gameObject.GetComponent<SpriteRenderer> ().color;
	}
	void TakeDamage(int amount) {
		hp -= amount;
		if (gameObject.GetComponent<SpriteRenderer> () != null) {
			gameObject.GetComponent<SpriteRenderer> ().color = hurtColor;
			StartCoroutine(ChangeColorBack(0.25f));

		}
		if (gameObject.GetComponent<ParticleSystem> () != null) {
			gameObject.GetComponent<ParticleSystem> ().Play ();
		}

		if (hp < 1) {
			gameObject.SendMessage ("Die");
		}
	}

	IEnumerator ChangeColorBack(float t) {
		yield return new WaitForSeconds (t);
		gameObject.GetComponent<SpriteRenderer> ().color = startColor;

	}
	private bool onFire = false;
	void OnCollisionEnter2D(Collision2D col) {
		if (!onFire && col.gameObject.layer == LayerMask.NameToLayer("BadWall")) {
			onFire = true;
			GameObject.Instantiate (Resources.Load ("Fire"), gameObject.transform);
			StartCoroutine (DieEventually (1f));

		}
	}

	IEnumerator DieEventually(float hurtTime) {
		while (hp > 0) {
			TakeDamage (1);
			yield return new WaitForSeconds (hurtTime);
		}
	}
}
