using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockfallTrapPlateController : MonoBehaviour {

	public void OnTriggerEnter2D(Collider2D col) {
		if (col.isTrigger) {
			return;
		}
		gameObject.SendMessageUpwards ("TriggerTrap");	
	}
}
