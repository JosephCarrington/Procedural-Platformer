using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class TileMapController : MonoBehaviour {

	// Use this for initialization
	tk2dTileMap map;
	void Start () {
		map = gameObject.GetComponent<tk2dTileMap> ();
		FillWithWalls ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FillWithWalls() {
		for (int x = 0; x < map.width; x++) {
			for (int y = 0; y < map.width; y++) {
				map.SetTile (x, y, 0, Random.Range(4, 7));
			}
		}
	}

	public bool IsWallAtCoords(Coordinates coords) {
		if (coords.x < 0 || coords.x >= map.width || coords.y < 0 || coords.y >= map.height) {
			throw new UnityException ("Coords out of bounds : " + coords.ToString());
		}
		int wall = map.GetTile (coords.x, coords.y, 0);
		switch (wall) {
		case 4:
		case 5:
		case 6:
			return true;
		default:
			return false;
		}
	}

	public void CarveOutRoom(Vector3 bl, Vector3 tr) {
		for (int x = (int)bl.x; x <= (int)tr.x; x++) {
			for (int y = (int)bl.y; y <= (int)tr.y; y++) {
				if (x >= 1 && x <= map.width - 1 && y >= 1 && y <= map.height - 1) {
					map.SetTile (x, y, 0, Random.Range (0, Random.Range (0, 4)));
				}
			}
		}
	}

	public void Build() {
		map.Build ();
	}
}
