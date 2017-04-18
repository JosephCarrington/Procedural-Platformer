using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using Utils;
public class SlimeUsage : Usage {
	TileMapController controller;
	public int slimeRadius = 1;
	public void Awake() {
		controller = GameObject.Find ("TileMap").GetComponent<TileMapController> ();
	}
	public override void UseOnActor(GameObject actor) {
		UseAtPosition (actor.transform.position);
	}

	public override void UseAtPosition(Vector2 position) {
		for (float x = position.x - slimeRadius; x <= position.x + slimeRadius; x++) {
			for (float y = position.y - slimeRadius; y <= position.y + slimeRadius; y++) {
				TileMapController.TileInfo tile = controller.GetTileAtPosition(new Vector2(x, y));
				if(tile.type == TileMapController.TileType.Wall) {
					controller.CreateSlimeAt(new Coordinates(Mathf.RoundToInt(x + Utils.Utils.mapSize.x / 2), Mathf.RoundToInt(y + Utils.Utils.mapSize.y / 2)));
				}
			}
		}
	}
}
