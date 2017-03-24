using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SlimeBallController : MonoBehaviour {
	public int slimeRadius = 1;
	void Use() {
		GameObject owner = gameObject.GetComponent<InventoryItemController> ().Owner;
		Vector2 playerPos = owner.transform.position;
		Coordinates coords = new Coordinates (Mathf.RoundToInt(playerPos.x), Mathf.RoundToInt(playerPos.y));
		Vector2 mapSize = Utils.Utils.mapSize;
		coords.x += (int)mapSize.x / 2;
		coords.y += (int)mapSize.y / 2;

		TileMapController mapController = GameObject.Find ("TileMap").GetComponent<TileMapController> ();
		for(int x = coords.x - slimeRadius; x <= coords.x + slimeRadius; x ++) {
			for(int y = coords.y - slimeRadius; y <= coords.y + slimeRadius; y ++) {
				if (mapController.IsWallAtCoords (new Coordinates(x, y))) {
					mapController.CreateSlimeAt (new Coordinates(x, y));
				}
			}
		}
		mapController.Build ();


		Destroy (gameObject);

	}
}
