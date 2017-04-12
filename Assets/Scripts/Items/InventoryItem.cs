using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class InventoryItem : MonoBehaviour {
	public abstract void UseOnSelf ();
	public void Throw () {
		print ("throw");
	}
}
