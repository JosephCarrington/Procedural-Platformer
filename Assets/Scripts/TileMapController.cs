using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Vaults;
namespace TileMap {
	public class TileMapController : MonoBehaviour {

		// Use this for initialization
		tk2dTileMap map;
		void Start () {
			map = gameObject.GetComponent<tk2dTileMap> ();
			FillWithWalls ();

		}

		public int wallLayer = 2;
		void FillWithWalls() {
			for (int x = 0; x < map.width; x++) {
				for (int y = 0; y < map.width; y++) {
					map.SetTile (x, y, wallLayer, 33);
				}
			}
		}

		public bool IsWallAtCoords(Coordinates coords) {
			if (coords.x < 0 || coords.x >= map.width || coords.y < 0 || coords.y >= map.height) {
	//			print("Coords out of bounds : " + coords.ToString());
				return false;
			}
			int wall = map.GetTile (coords.x, coords.y, wallLayer);
			if(wall == -1 ) {
				return false;
			}
			return true;
		}

		public bool IsLavaAtCoords(Coordinates coords) {
			if (coords.x < 0 || coords.x >= map.width || coords.y < 0 || coords.y >= map.height) {
				print("Coords out of bounds : " + coords.ToString());
				return false;
			}
			int wall = map.GetTile (coords.x, coords.y, 2);
			if (wall == -1) {
				return false;
			}
			return true;
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
			uint[,] tileInts = v.tileInts;



			for (int x = 0; x < tileInts.GetLength(0); x++) {
				for (int y = 0; y < tileInts.GetLength(1); y++) {
					uint rawTile = tileInts [x, y];
					int tile = (int)(rawTile & ~(0xE0000000)); // ignore flipping and rotating
					Coordinates newCoords = new Coordinates (x, tileInts.GetLength(0) - 1 - y);
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
						CreateWallTileAt(newCoords);
						break;
					case 9:
						break;
					case 11: 
						if(!IsWallAtCoords(newCoords)) {
							CreateSpikeAt (newCoords);
						}
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
					map.SetTileFlags(newCoords.x, newCoords.y, 0, tileFlags);
				}
			}
		}

		public TileDirection TileFlagsToTileDirection(tk2dTileFlags flags) {
			bool flipVertical = (flags & tk2dTileFlags.FlipY) != 0;
			bool flipHorizontal = (flags & tk2dTileFlags.FlipX) != 0;
			bool flipDiagonal = (flags & tk2dTileFlags.Rot90) != 0;
			if (flipDiagonal) {
				if (flipHorizontal) {
					return TileDirection.Left;
				}
				return TileDirection.Right;
			} else {
				if (flipVertical) {
					return TileDirection.Down;
				}
			}
			return TileDirection.Up;
		}
		public enum TileType{
			Empty,
			Wall,
			Lava,
			Spike
		}

		public enum TileDirection{
			Up,
			Down,
			Left,
			Right
		}
		public class TileInfo {
			public TileType type;
			public TileDirection direction;
		}

		public Coordinates PositionToCoordinates(Vector2 pos) {
			return new Coordinates(Mathf.RoundToInt(pos.x + Utils.Utils.mapSize.x / 2),
				Mathf.RoundToInt(pos.y + Utils.Utils.mapSize.y / 2)
			);

		}
		public TileInfo GetTileAtPosition(Vector2 pos, TileLayer l) {
			int x, y, tileId = -1;
			TileDirection dir = TileDirection.Up;
			if (map.GetTileAtPosition (pos, out x, out y)) {		
				// Default to spike
				tileId = map.GetTile(x, y, (int)l);
				if (tileId != -1) {

				}
			}
			tk2dTileFlags flags = map.GetTileFlags(x, y, (int)l);
			dir = (TileFlagsToTileDirection (flags));

			TileInfo tile = new TileInfo ();
			switch (tileId) {
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
			case 64:
			case 65:
			case 66:
				tile.type = TileType.Wall;
				break;
			case 8:
				tile.type = TileType.Lava;
				break;
			default:
				tile.type = TileType.Empty;
				break;
			}
			if (tileId == spikeTile) {
				tile.type = TileType.Spike;
			}
			tile.direction = dir;
			return tile;
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
		public int lavaLayer = 1;
		public void CreateLava(Coordinates a, Coordinates b) {
			for (int x = a.x; x <= b.x; x++) {
				for (int y = a.y; y <= b.y; y++) {
					if (y == b.y) {
						map.SetTile (x, y, lavaLayer, 288);
					}
					else
					map.SetTile (x, y, lavaLayer, 320);
				}
			}
		}

		public void CreateEmptyTileAt(Coordinates c) {
			map.SetTile (c.x, c.y, wallLayer, -1);
		}

		public void CreateWallTileAt(Coordinates c) {
			map.SetTile (c.x, c.y, wallLayer, 33);
		}
		public void CreateDebugTileAt(Coordinates c) {
			map.SetTile (c.x, c.y, wallLayer, 15);
		}

		public int spikeTile = 256;
		public void CreateSpikeAt(Coordinates c) {
//			map.SetTile (c.x, c.y, 1, spikeTile);
		}
		public void CreateSpikeAt(Coordinates c, TileDirection d) {
//			tk2dTileFlags tileFlags = 0;
//			map.SetTile (c.x, c.y, 1, spikeTile);
//			switch (d) {
//			case TileDirection.Down:
//				tileFlags ^= tk2dTileFlags.FlipY;
//				break;
//			}
//			map.SetTileFlags(c.x, c.y, 1, tileFlags);
//
		}

		public int slimeLayer = 4;
		public void CreateSlimeAt(Coordinates c) {
	//		CreateEmptyTileAt (c);
			map.SetTile (c.x, c.y, slimeLayer, 12);
//			Build ();
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
			// Build our bool map first
			bool[,] walls = new bool[map.width, map.height];
			for (int x = 0; x < map.width; x++) {
				for (int y = 0; y < map.height; y++) {
					if (BaseWallExists (new Coordinates (x, y))) {
						walls [x, y] = true;
					} else {
						walls [x, y] = false;
					}
				}
			}
			for (int x = 0; x < map.width; x++) {
				for (int y = 0; y < map.height; y++) {
//					 Get the tile neighbors and assign a tile based on that. Levels grow outwards from wall tiles already placed
					TileNeighbors ns = GetSurroundingTiles(new Coordinates(x, y), walls);
					if (!ns.C) {
						// Empty tile
						if (ns.S) {
							if (ns.E) {
								if (ns.NE && ns.SW && ns.SE) {
									SetTileIfNotSet (x, y, wallLayer, 3);
								} else {
									SetTileIfNotSet (x, y, wallLayer, 0);
								}
							} else if (ns.W) {
								SetTileIfNotSet (x, y, wallLayer, 4);

							} else {
								SetTileIfNotSet (x, y, wallLayer, 1);
							}
						} else if (ns.E) {
							if (ns.N) {
								SetTileIfNotSet (x, y, wallLayer, 35);

							} else {
								SetTileIfNotSet (x, y, wallLayer, 32);
							}
						} else if (ns.W) {
							if (ns.N) {
								SetTileIfNotSet (x, y, wallLayer, 36);

							} else {
								SetTileIfNotSet (x, y, wallLayer, 34);
							}
						} else if (ns.N) {
							SetTileIfNotSet (x, y, wallLayer, 65);
						} else if (ns.NE) {
							SetTileIfNotSet (x, y, wallLayer, 64);
						} else if (ns.SE) {
							SetTileIfNotSet (x, y, wallLayer, 0);
						} else if (ns.NW) {
							SetTileIfNotSet (x, y, wallLayer, 66);
						} else if (ns.SW) {
							SetTileIfNotSet (x, y, wallLayer, 2);
						}
					}
				}
			}
			map.Build ();
			// MAy be able to do this before first build
			// Make marching squares


		}

		public void SetTileIfNotSet(int x, int y, int layer, int tileId) {
			int currentTile = map.GetTile (x, y, layer);
			if (currentTile != tileId) {
				map.SetTile (x, y, layer, tileId);
			}
		}

		public enum TileLayer {
			BG = 0,
			Floor = 2,
			Lava = 1,
			Effects = 3
		}


		TileNeighbors GetSurroundingTiles(Coordinates c, int l) {
			// Remember it's bottom left origin!
			TileNeighbors n = new TileNeighbors();
			n.C = TileExists (c, l);
			n.N = TileExists (new Coordinates (c.x, c.y + 1), l);
			n.NE = TileExists (new Coordinates (c.x + 1, c.y + 1), l);
			n.E = TileExists (new Coordinates (c.x + 1, c.y), l);
			n.SE = TileExists (new Coordinates (c.x + 1, c.y - 1), l);
			n.S = TileExists (new Coordinates (c.x, c.y - 1), l);
			n.SW = TileExists (new Coordinates (c.x - 1, c.y - 1), l);
			n.W = TileExists (new Coordinates (c.x - 1, c.y), l);
			n.NW = TileExists (new Coordinates (c.x - 1, c.y + 1), l);
			return n;
		}

		TileNeighbors GetSurroundingTiles(Coordinates c, bool[,] boolMap) {
			// Remember it's bottom left origin!
			TileNeighbors n = new TileNeighbors();
			n.C = TileExists (c, boolMap);
			n.N = TileExists (new Coordinates (c.x, c.y + 1), boolMap);
			n.NE = TileExists (new Coordinates (c.x + 1, c.y + 1), boolMap);
			n.E = TileExists (new Coordinates (c.x + 1, c.y), boolMap);
			n.SE = TileExists (new Coordinates (c.x + 1, c.y - 1), boolMap);
			n.S = TileExists (new Coordinates (c.x, c.y - 1), boolMap);
			n.SW = TileExists (new Coordinates (c.x - 1, c.y - 1), boolMap);
			n.W = TileExists (new Coordinates (c.x - 1, c.y), boolMap);
			n.NW = TileExists (new Coordinates (c.x - 1, c.y + 1), boolMap);
			return n;
		}

		bool TileExists(Coordinates c, int l) {
			if (c.x < 0 || c.x >= map.width || c.y < 0 || c.y >= map.height) {
				return false;
			}
			int t = map.GetTile (c.x, c.y, l);
			if (t == -1) {
				return false;
			} else {
				return true;
			}
		}

		bool BaseWallExists(Coordinates c) {
			if (c.x < 0 || c.x >= map.width || c.y < 0 || c.y >= map.height) {
				return false;
			}
			int t = map.GetTile (c.x, c.y, wallLayer);
			if (t == 33) {
				return true;
			}
			return false;
		}

		bool TileExists(Coordinates c, bool[,] boolMap) {
			if (c.x < 0 || c.x >= boolMap.GetLength(0) || c.y < 0 || c.y >= boolMap.GetLength(1)) {
				return false;
			}
			return boolMap [c.x, c.y];
		}

		public void RecreateLiquids() {
	//		GameObject lava = GameObject.Find ("Lava");
	//		foreach (Transform child in lava.transform) {
	//			// Remove any boyouncy effectors
	////			Debug.Break();
	//			GameObject.DestroyImmediate(child.gameObject.GetComponent<BuoyancyEffector2D>());
	//
	//			child.gameObject.GetComponent<EdgeCollider2D> ().isTrigger = true;
	//			child.gameObject.GetComponent<EdgeCollider2D> ().usedByEffector = true;
	//			child.gameObject.AddComponent<BuoyancyEffector2D> ();
	//		}
		}
	}
}