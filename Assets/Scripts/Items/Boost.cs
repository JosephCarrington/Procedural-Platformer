using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
namespace Items.Usages {
	public class Boost : Items.Usage {
		public float boostStrength = 10f;
		public override void UseOn (GameObject o) {
			o.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, boostStrength), ForceMode2D.Impulse);
		}
	}
}