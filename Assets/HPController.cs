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
		gameObject.GetComponent<SpriteRenderer> ().color = hurtColor;
		gameObject.GetComponent<ParticleSystem> ().Play ();
		StartCoroutine(ChangeColorBack(0.25f));

		if (hp < 1) {
			gameObject.SendMessage ("Die");
		}
	}

	IEnumerator ChangeColorBack(float t) {
		yield return new WaitForSeconds (t);
		gameObject.GetComponent<SpriteRenderer> ().color = startColor;

	}
}
