using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Vaults;

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
//			print("Coords out of bounds : " + coords.ToString());
			return false;
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

	public bool IsLavaAtCoords(Coordinates coords) {
		if (coords.x < 0 || coords.x >= map.width || coords.y < 0 || coords.y >= map.height) {
			print("Coords out of bounds : " + coords.ToString());
			return false;
		}
		int wall = map.GetTile (coords.x, coords.y, 1);
		switch (wall) {
		case 8:
			return true;
		default:
			return false;
		}
	}

	public void CarveOutRoom(Vector3 bl, Vector3 tr) {
		for (int x = (int)bl.x; x <= (int)tr.x; x++) {
			for (int y = (int)bl.y; y <= (int)tr.y; y++) {
				if (x >= 1 && x <= map.width - 1 && y >= 1 && y <= map.height - 1) {
					CreateEmptyTileAt(new Coordinates(x, y));
				}
			}
		}
	}

	public void CreateVaultInRoom(Vault v, Room r) {
		TileType[,] tiles = v.tiles;
		uint[,] tileInts = v.tileInts;



		for (int x = 0; x < tiles.GetLength(0); x++) {
			for (int y = 0; y < tiles.GetLength(1); y++) {
				uint rawTile = tileInts [x, y];
				int tile = (int)(rawTile & ~(0xE0000000)); // ignore flipping and rotating
				Coordinates newCoords = new Coordinates (x, tiles.GetLength(0) - 1 - y);
				newCoords.x += r.pos.x;
				newCoords.y += r.pos.y;
				newCoords.x -= r.size.x / 4;
//				newCoords.y -= r.size.y;
				newCoords.y -= r.size.y / 4;

				int layer = 0;
				switch (tile) {
				case 5:
				case 6:
				case 7:
				case 8:
					CreateDebugTileAt(newCoords);
					break;
				case 9:
					break;
				case 11: 
					layer = spikeLayer;
					CreateSpikeAt (newCoords);
					break;
				default:
					break;
				}

				// Set tile flags
				bool flipHorizontal = (rawTile & 0x80000000) != 0;
				bool flipVertical = (rawTile & 0x40000000) != 0;
				bool flipDiagonal = (rawTile & 0x20000000) != 0;
				tk2dTileFlags tileFlags = 0;
				if (flipDiagonal) tileFlags |= (tk2dTileFlags.Rot90 | tk2dTileFlags.FlipX);
				if (flipHorizontal) tileFlags ^= tk2dTileFlags.FlipX;
				if (flipVertical) tileFlags ^= tk2dTileFlags.FlipY;
				map.SetTileFlags(newCoords.x, newCoords.y, layer, tileFlags);
			}
		}
	}

	public int GetWallColumnHeight(Coordinates a) {
		int wallHeight = 0;
		bool wallAtPos = true;
		while (wallAtPos) {
			if (IsWallAtCoords (
				new Coordinates(a.x, a.y + wallHeight)
			)) {
				wallHeight++;
				wallAtPos = true;
			} else {
				wallAtPos = false;
			}
		}

		return wallHeight;
	}

	public void CreateLava(Coordinates a, Coordinates b) {
		for (int x = a.x; x <= b.x; x++) {
			for (int y = a.y; y <= b.y; y++) {
				map.SetTile (x, y, 1, 8);
			}
		}
	}

	public void CreateEmptyTileAt(Coordinates c) {
		map.SetTile (c.x, c.y, 0, Random.Range (0, Random.Range (0, 4)));
//		map.ColorChannel.SetColor (c.x, c.y, Color.white);
//		map.ColorChannel.SetColor (c.x-1, c.y, Color.white);
////		map.ColorChannel.SetColor (c.x+1, c.y, Color.white);
//		map.ColorChannel.SetColor (c.x, c.y-1, Color.white);
////		map.ColorChannel.SetColor (c.x, c.y+1, Color.white);
////
////		map.ColorChannel.SetColor (c.x +1, c.y+1, Color.white);
////		map.ColorChannel.SetColor (c.x +1, c.y-1, Color.white);
//
////		map.ColorChannel.SetColor (c.x -1, c.y+1, Color.white);
//		map.ColorChannel.SetColor (c.x -1, c.y-1, Color.white);
	}

	public void CreateWallTileAt(Coordinates c) {
		map.SetTile (c.x, c.y, 0, Random.Range (4, 7));
	}
	public void CreateDebugTileAt(Coordinates c) {
		map.SetTile (c.x, c.y, 0, 15);
	}

	public int spikeLayer = 3;
	public void CreateSpikeAt(Coordinates c) {
		map.SetTile (c.x, c.y, spikeLayer, 10);
	}

	public int slimeLayer = 4;
	public void CreateSlimeAt(Coordinates c) {
		CreateEmptyTileAt (c);
		map.SetTile (c.x, c.y, slimeLayer, 12);
	}
	public bool DoesContinuousWallExist(Coordinates a, Coordinates b) {
//		Color lineColor = a.x != b.x ? Color.red : Color.blue;
//		Debug.DrawLine (new Vector3 (a.x - 128, a.y - 128, -2), new Vector3 (b.x - 128, b.y - 128, -2), lineColor);
//		Debug.Break();
		for (int x = a.x; x <= b.x; x++) {
			for (int y = a.y; y <= b.y; y++) {
				if(!IsWallAtCoords(new Coordinates(x, y))) {
					return false;
				}
			}
		}
		return true;
	}

	public void Build() {
		map.Build ();
	}
}
