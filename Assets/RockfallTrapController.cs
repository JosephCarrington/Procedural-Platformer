using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class RockfallTrapController : MonoBehaviour {

	// Use this for initialization
	GameObject holder, plate, rock;
	TileMapController mapController;
	void Start () {
		holder = transform.Find ("Holder").gameObject;
		plate = transform.Find ("Plate").gameObject;
		rock = transform.Find ("Rock").gameObject;
		mapController = GameObject.Find ("TileMap").GetComponent<TileMapController>();
		int minHeightCheck = 4;
		int maxHeightCheck = 32;
		bool madeTrap = false;
//		for (int y = Mathf.RoundToInt(transform.position.y) + minHeightCheck; y < Mathf.RoundToInt(transform.position.y) + maxHeightCheck; y++) {
//			TileMapController.TileInfo tile = mapController.GetTileAtPosition (transform.position);
//			if (tile.type == TileMapController.TileType.Empty) {
//				madeTrap = true;
//				SetTopOffset (y - Mathf.RoundToInt(transform.position.y));
//				break;
//			}	
//		}
		for (int y = 1; y < maxHeightCheck; y++) {
			Vector2 checkPos = gameObject.transform.position;
			checkPos.y += y;
			TileMapController.TileInfo tile = mapController.GetTileAtPosition (checkPos);
			if (y <= minHeightCheck) {
				if (tile.type == TileMapController.TileType.Empty) {
					continue;
				} else {
					break;
				}
			} else if (y <= maxHeightCheck) {
				if (tile.type == TileMapController.TileType.Wall) {
					madeTrap = true;
					SetTopOffset (y - 1);
					break;
				}
			}
		}
		if (!madeTrap) {
			print ("Trap not made");
			Destroy (gameObject);
		}

	}
	
	void SetTopOffset(int height) {
		Vector3 newPos = holder.transform.localPosition;
		newPos.y += height;
		holder.transform.localPosition = newPos;
		newPos = rock.transform.localPosition;
		newPos.y += height;
		rock.transform.localPosition = newPos;

	}

	bool triggered = false;
	void TriggerTrap() {
		if (!triggered) {
			rock.gameObject.GetComponent<Rigidbody2D> ().isKinematic = false;
			rock.gameObject.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (-10, 10), ForceMode2D.Impulse);
		}
		triggered = true;
	}
}
