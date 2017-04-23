using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPController : MonoBehaviour {

	// Use this for initialization
	public int hp = 1;
	void TakeDamage(int amount) {
		hp -= amount;
		if (hp < 1) {
			gameObject.SendMessage ("Die");
		}
	}
}
