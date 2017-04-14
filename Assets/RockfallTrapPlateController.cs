using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockfallTrapPlateController : MonoBehaviour {

	public void OnTriggerEnter2D(Collider2D col) {
		gameObject.SendMessageUpwards ("TriggerTrap");	
	}
}
