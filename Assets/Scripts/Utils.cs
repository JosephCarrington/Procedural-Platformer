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

	public class Utils {
		public static float GetBetweenValue(float min, float max, float inputValue) {
			return(inputValue - min) / (max - min);
		}
		public static Vector2 mapSize = Vector2.one * 256f;

	}

	public class Room : MonoBehaviour {
		public static Vector2 mapSize = Vector2.one * 256f;
		public Coordinates pos, size, topLeft, topRight, bottomRight, bottomLeft;

		void Awake() {
			pos = new Coordinates (
				Mathf.RoundToInt(gameObject.transform.position.x + mapSize.x / 2),
				Mathf.RoundToInt(gameObject.transform.position.y + mapSize.y / 2)
			);
			size = new Coordinates (
				Mathf.RoundToInt (gameObject.transform.lossyScale.x),
				Mathf.RoundToInt (gameObject.transform.lossyScale.y)
			);

			topLeft = new Coordinates (
				pos.x - size.x / 2,
				pos.y + size.y / 2
			);
			topRight = new Coordinates (
				pos.x + size.x / 2,
				pos.y + size.y / 2
			);
			bottomRight = new Coordinates (
				pos.x + size.x / 2,
				pos.y - size.y / 2
			);
			bottomLeft = new Coordinates (
				pos.x - size.x / 2,
				pos.y - size.y / 2
			);

		}
	}
}