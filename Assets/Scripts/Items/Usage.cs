using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inventory {
	public abstract class Usage : ScriptableObject {
		public abstract void UseOnActor(GameObject actor);
		public abstract void UseAtPosition(Vector2 position);
	}
}
