using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inventory {
	[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]

	public class Item : ScriptableObject {
		public Sprite icon;
		public float weight = 0f;
		public List<Items.Usage> usages;

		public void UseOn(GameObject o) {
			foreach (Items.Usage usage in usages) {
				usage.UseOn (o);

			}
		}
	}


}