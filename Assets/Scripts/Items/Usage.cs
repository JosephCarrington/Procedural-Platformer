using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items {
	public abstract class Usage : ScriptableObject {
		public abstract void UseOn (GameObject o);
	}
}