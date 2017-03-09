using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
	public class Coordinates {
		public int x, y;
		public Coordinates(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public override string ToString() {
			return "[" + this.x + "," + this.y + "]";
		}
			
	}
}