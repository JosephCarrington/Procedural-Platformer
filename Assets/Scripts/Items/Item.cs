using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory {
	[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]

	public class Item : ScriptableObject {
		public Sprite icon;
		public float weight = 0f;
		public string usageName;

		private Usage usage;

		public void UseOnActor(GameObject actor) {
			usage = ScriptableObject.CreateInstance(usageName) as Usage;
			usage.UseOnActor (actor);
			Destroy (usage);
		}
	}


}