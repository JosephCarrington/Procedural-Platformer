using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Utils;
using Vaults;

using Delaunay;
using Delaunay.Geo;

public class LevelCreator : MonoBehaviour {

	GameObject player;
	tk2dTileMap tileMap;
	public LevelSettings level;


	GameObject[] rooms;
	List<GameObject> finalRooms;
	TileMapController map;
	Rect levelBounds;
	public GameObject boostPrefab;


	void Start () {
		finalRooms = new List<GameObject> ();
		map = GameObject.Find ("TileMap").GetComponent<TileMapController> ();
		player = GameObject.Find ("Player");
		player.gameObject.SetActive (false);
		StartCoroutine(CreateRooms(level.roomCount));

		groundDecorations = new GameObject[(int)level.mapSize.x, (int)level.mapSize.y];
	}

	IEnumerator CreateRooms(int numRooms) {
		ReadVaults ();
		//		ShuffleVaults ();
		yield return StartCoroutine(CreateRects (numRooms));
		SetMainRooms ();
		CreateDelauneyAndTree ();
		CreateCorridors ();

		levelBounds = new Rect();
		bool firstRoomSet = false;
		foreach (GameObject room in rooms) {
			if (room.layer != LayerMask.NameToLayer ("UnusedRoom")) {
				if (!firstRoomSet) {
					levelBounds = GetRect (room.transform);
					firstRoomSet = true;
				}
				//				room.GetComponent<BoxCollider2D> ().usedByComposite = true;
				finalRooms.Add (GameObject.Instantiate (room, gameObject.transform));

				Rect roomRect = GetRect (room.transform);
				if (roomRect.xMin < levelBounds.xMin) {
					levelBounds.xMin = roomRect.xMin;
				}
				if (roomRect.xMax > levelBounds.xMax) {
					levelBounds.xMax = roomRect.xMax;
				}
				if (roomRect.yMin < levelBounds.yMin) {
					levelBounds.yMin = roomRect.yMin;
				}
				if (roomRect.yMax > levelBounds.yMax) {
					levelBounds.yMax = roomRect.yMax;
				}

			}
			GameObject.Destroy (room);
		}

		foreach (Transform child in transform) {
			child.GetComponent<SpriteRenderer> ().enabled = false;
		}

		levelBounds.xMin -= 1;
		levelBounds.xMax += 1;
		levelBounds.yMin -= 1;
		levelBounds.yMax += 1;

		//		tiles = new Tile[mapSize.x, mapSize.y];
		foreach (Transform child in transform) {
			//			WallInRoom (child.gameObject);
			child.gameObject.AddComponent<Room>();
			CarveOutRoom(child.gameObject);
			Destroy (child.GetComponent<Rigidbody2D> ());
			Destroy (child.GetComponent<BoxCollider2D> ());


		}

		player.SetActive (true);

		Coordinates entranceRoomPos = null;
		Room entranceRoom = null;
		foreach(Transform child in transform) {
			Room room = child.gameObject.GetComponent<Room> ();
			if(map.DoesContinuousWallExist(
				new Coordinates(room.bottomLeft.x, room.bottomLeft.y -1),
				new Coordinates(room.bottomRight.x, room.bottomRight.y - 1)
			)) {
				entranceRoomPos = room.pos;
				entranceRoom = room;
				break;
			}
		}




		bool wallUnder = false;
		Coordinates playerPos = entranceRoomPos;
		int currentYCheck = playerPos.y - 1;
		while (!wallUnder) {
			if (map.IsWallAtCoords (new Coordinates(playerPos.x, currentYCheck))) {
				wallUnder = true;
			} else {
				currentYCheck--;
			}
		}
		Vector2 newPlayerPos = new Vector2 (playerPos.x - (level.mapSize.x / 2), currentYCheck - (level.mapSize.y / 2) + 1);
		player.transform.position = newPlayerPos;
		Vector3 boostPos = newPlayerPos;
		boostPos.z = -2;
		boostPos.y += 3;
		for (int i = 0; i < 3; i++) {
			GameObject.Instantiate (boostPrefab, boostPos, Quaternion.identity);
		}


		foreach (Transform child in transform) {
			Room room = child.gameObject.GetComponent<Room>();
			//			 Check to fill with vault
			bool madeVault = false;
			if (room != entranceRoom && Random.value < level.chanceToPlaceVault) {
				foreach (Vault v in floatingVaults) {
					if (v.size.x <= room.size.x && v.size.y <= room.size.y) {
						v.ParseCSV ();
						room.vault = v;
						floatingVaults = Utils.Utils.ShuffleVaults (floatingVaults);
						map.CreateVaultInRoom (room.vault, room);

						break;
					}
				}
			}

			if (madeVault) {
			}

			if (!madeVault && Random.value < level.chanceToAttemptLava) {
				if(room != entranceRoom) {

					// Check if there is lavaDepth depth of room cuppyness
					Coordinates bottomLeftWall = new Coordinates(room.bottomLeft.x, room.bottomLeft.y),
					bottomRightWall = new Coordinates(room.bottomRight.x, room.bottomRight.y);
					bottomLeftWall.x -= 1;
					bottomLeftWall.y -= 1;
					bottomRightWall.x += 1;
					bottomRightWall.y -= 1;

					Coordinates leftCheck = new Coordinates(bottomLeftWall.x, bottomLeftWall.y),
					rightCheck = new Coordinates(bottomRightWall.x, bottomRightWall.y);

					leftCheck.y += level.lavaDepth;
					rightCheck.y += level.lavaDepth;

					if(map.DoesContinuousWallExist(bottomLeftWall, bottomRightWall)) {

						int leftColHeight = map.GetWallColumnHeight (
							new Coordinates (bottomLeftWall.x, bottomLeftWall.y + 1)
						);
						int rightColHeight = map.GetWallColumnHeight (
							new Coordinates (bottomRightWall.x, bottomRightWall.y + 1)
						);
						if (leftColHeight > 0 && rightColHeight > 0) {
							int lavaHeight = leftColHeight > rightColHeight ? rightColHeight : leftColHeight;
							lavaHeight = lavaHeight > level.lavaDepth ? level.lavaDepth : lavaHeight;
							map.CreateLava (
								room.bottomLeft,
								new Coordinates (room.bottomRight.x, room.bottomRight.y + lavaHeight - 1)
							);
						}

					}
				}
			}

			for(int x = room.bottomLeft.x; x < room.bottomRight.x; x++) {
				if(map.IsWallAtCoords(new Coordinates(x, room.bottomLeft.y - 1)) && !IsWallOrLavaAtCoords(new Coordinates(x, room.bottomLeft.y))) {

					bool spawnedEnemy = false;
					if (room != entranceRoom && Random.value < level.chanceToSpawnEnemy) {
						GameObject newEnemy = GameObject.Instantiate (level.enemies [Random.Range(0, level.enemies.Length)], new Vector3 (x - level.mapSize.x / 2, room.bottomLeft.y - level.mapSize.y / 2, -1), Quaternion.identity);
						enemies.Add (newEnemy);
						spawnedEnemy = true;
					}

					bool spawnedSpike = false;
					if (!spawnedEnemy && room != entranceRoom && Random.value < level.chanceToSpawnSpike) {
						spawnedSpike = true;
						map.CreateSpikeAt (new Coordinates (x, room.bottomLeft.y), TileMapController.TileDirection.Up);
//						GameObject.Instantiate(level.spike, new Vector3(x - level.mapSize.x / 2, room.bottomLeft.y - level.mapSize.y / 2, -1), Quaternion.identity);
					}

					//					bool spawnedTrap = false;
					if (!spawnedSpike && room != entranceRoom && Random.value < level.chanceToSpawnTrap) {
						//						spawnedTrap = true;
						GameObject.Instantiate(level.traps[Random.Range(0, level.traps.Length)], new Vector3(x - level.mapSize.x / 2, room.bottomLeft.y - level.mapSize.y / 2, -1), Quaternion.identity);
					}

					if(!spawnedSpike && Random.value < level.chanceToSpawnGroundDecoration) {
						GameObject newDeco = GameObject.Instantiate (
							level.groundDecorations [Random.Range (0, level.groundDecorations.Length)],
							new Vector3 (x - level.mapSize.x / 2, room.bottomLeft.y - level.mapSize.y / 2),
							Quaternion.identity
						);
						if (Random.value > 0.5) {
							Vector3 newScale = newDeco.gameObject.transform.localScale;
							newScale.x = -1;
							newDeco.gameObject.transform.localScale = newScale;
						}
						groundDecorations [x, room.bottomLeft.y] = newDeco;
					}


				}
			}
			// Top Spikes
			for (int x = room.topLeft.x; x < room.topRight.x; x++) {
				if (map.IsWallAtCoords (new Coordinates (x, room.topLeft.y + 1)) && !IsWallOrLavaAtCoords (new Coordinates (x, room.topLeft.y))) {
					//					bool spawnedSpike = false;
					if (room != entranceRoom && Random.value < level.chanceToSpawnSpike) {
						//						spawnedSpike = true;
//						GameObject newSpike = GameObject.Instantiate(level.spike, new Vector3(x - level.mapSize.x / 2, room.topLeft.y - level.mapSize.y / 2, -1), Quaternion.identity);
//						Vector2 newScale = newSpike.transform.localScale;
//						newScale.y = -1;
//						newSpike.transform.localScale = newScale;
						map.CreateSpikeAt (new Coordinates (x, room.topLeft.y), TileMapController.TileDirection.Down);

					}
				}
			}
		}

		// Exit
		Vector2 minExitRoomSize = new Vector2(8, 8);
		bool placedExit = false;
		foreach (Transform child in transform) {
			Room room = child.gameObject.GetComponent<Room> ();
			// Don't spawn the exit in the room we start in
			if (room != entranceRoom) {
				// Don't spawn the exit in a room that is too small
				if (room.size.x >= minExitRoomSize.x && room.size.y >= minExitRoomSize.y) {
					GameObject.Instantiate (level.exit, new Vector3 (room.pos.x - (level.mapSize.x / 2) + 0.5f, room.pos.y - ((level.mapSize.y / 2) -1) - 0.5f, -1), Quaternion.identity);
					// Make a floor beneath it
					map.CreateWallTileAt(new Coordinates(room.pos.x, room.pos.y -1));
					map.CreateWallTileAt(new Coordinates(room.pos.x + 1, room.pos.y -1));
					placedExit = true;
				}
				if (placedExit) {
					break;
				}
			}
		}

		map.Build ();



		Vector3 camPos = Camera.main.transform.position;
		camPos.x = newPlayerPos.x;
		camPos.y = newPlayerPos.y;

		Camera.main.transform.position = camPos;
		Camera.main.GetComponent<CameraController> ().enabled = true;

	}


	float meanHeight,
		meanWidth;

	Vector2[] mainRoomPoints;

	private List<LineSegment> m_delaunayTriangulation,
		m_spanningTree,
		corridors;

	List<Vector2> midPoints;

	GameObject[,] groundDecorations;

	private List<Vault> floatingVaults =  new List<Vault>();
	private List<Vault> entranceVaults = new List<Vault> ();
	private List<Vault> exitVaults = new List<Vault> ();


	private void ReadVaults() {
		Object[] raws = Resources.LoadAll ("Vaults", typeof(TextAsset));
		TextAsset[] rawXMLs = new TextAsset[raws.Length];
		int i = 0;
		foreach (Object raw in raws) {
			rawXMLs [i] = raw as TextAsset;
			i++;
		}

		foreach (TextAsset rawXML in rawXMLs) {
			XmlDocument vaultXML = new XmlDocument ();
			vaultXML.LoadXml (rawXML.text);
			Vault newVault = Utils.Utils.XMLToVault (vaultXML);
			if (newVault.minDepth == -1 || newVault.minDepth >= level.depth) {
				switch (newVault.type) {
				case VaultType.Entrance:
					entranceVaults.Add (newVault);
					break;
				case VaultType.Exit:
					exitVaults.Add (newVault);
					break;
				case VaultType.Floating:
					floatingVaults.Add (newVault);
					break;
				}
			}
		}
	}

	private void ShuffleVaults() {
		floatingVaults = Utils.Utils.ShuffleVaults (floatingVaults);
		entranceVaults = Utils.Utils.ShuffleVaults (entranceVaults);
		exitVaults = Utils.Utils.ShuffleVaults (exitVaults);
	}

	private IEnumerator CreateRects(int numRooms) {
		meanWidth = level.minWidth + (level.widthVariance / 2);
		meanHeight = level.minWidth + (level.heightVariance / 2);
		rooms = new GameObject[level.roomCount];
		Time.fixedDeltaTime = 0.001f;
		for (int i = 0; i < numRooms; i++) {
			int wv = Random.Range (0, level.widthVariance);
			int hv = Random.Range (0, level.heightVariance);
			// only round numbers please
			if (wv % 2 != 0) {
				wv++;
			}
			if (hv % 2 != 0) {
				hv++;
			}
			GameObject newRoom = GameObject.Instantiate (level.defaultRoom);
			newRoom.transform.position = Random.insideUnitCircle * (level.mapSize.x / 4);
			newRoom.transform.localScale = new Vector3 (level.minWidth + wv, level.minHeight + hv, 1);
			newRoom.layer = LayerMask.NameToLayer ("UnusedRoom");
			rooms [i] = newRoom;
		}

		bool allSleeping = false;
		while (!allSleeping) {
			foreach (GameObject room in rooms) {
				allSleeping = true;
				if (!room.GetComponent<Rigidbody2D> ().IsSleeping()) {
					allSleeping = false;
					yield return null;
					break;
				}
			}
		}

//		yield return null;
	}

	List<GameObject> mainRooms;
	void SetMainRooms() {
		Time.fixedDeltaTime = 0.02f;
		// Round positions of rooms to int and get 'main rooms'?
		mainRooms = new List<GameObject>();
		foreach (GameObject room in rooms) {
			room.GetComponent<Rigidbody2D> ().isKinematic = true;
			Vector2 newPos = room.transform.position;
			newPos.x = Mathf.RoundToInt (newPos.x);
			newPos.y = Mathf.RoundToInt (newPos.y);
			room.transform.position = newPos;
			if (room.transform.localScale.x > (meanWidth * 1.1f) && room.transform.localScale.y > (meanHeight * 1.1f)) {
				room.layer = LayerMask.NameToLayer ("MainRoom");
				room.name = "Main Room";
				mainRooms.Add (room);
			}
		}
	}

	void CreateDelauneyAndTree() {
		mainRoomPoints = new Vector2[mainRooms.Count];
		for (int i = 0; i < mainRooms.Count; i++) {
			mainRoomPoints[i] = mainRooms [i].transform.position;
		}

		List<uint> colors = new List<uint> ();
		foreach (Vector2 point in mainRoomPoints) {
			colors.Add (0);
		}

		Delaunay.Voronoi v = new Delaunay.Voronoi (mainRoomPoints.ToList (), colors, new Rect (0, 0, 100, 50));
		m_delaunayTriangulation = v.DelaunayTriangulation ();
		m_spanningTree = v.SpanningTree (KruskalType.MINIMUM);
		corridors = m_spanningTree;
		for (int i = 0; i < m_delaunayTriangulation.Count; i++) {
			if (Random.value < level.chanceToAddExtraCorridor) {
				corridors.Add(m_delaunayTriangulation[i]);
			}
		}
	}

	void CreateCorridors() {
		corridors = corridors.Distinct ().ToList();
		// For each corridor, add connections. TODO: This is a dumb
		for (int i = 0; i < corridors.Count; i++) {
			LineSegment c = corridors [i];
			Vector2 start = (Vector2)c.p0;
			Vector2 end = (Vector2)c.p1;
			GameObject startRoom = GetRoomAtCoords (start);
			GameObject endRoom = GetRoomAtCoords (end);
			startRoom.GetComponent<BaseRoomController> ().AddConnection (endRoom);
		}

		// Try to create corridors
		midPoints = new List<Vector2>();
		foreach (GameObject room in rooms) {
			List<GameObject> connections = room.GetComponent<BaseRoomController> ().GetConnections ();
			for (int i = 0; i < connections.Count; i++) {
				// First see if we can create a straight connection
				Vector2 startCoords = room.GetComponent<BaseRoomController>().GetCoords();
				Vector2 startScale = room.transform.localScale;
				Vector2 endCoords = connections[i].GetComponent<BaseRoomController>().GetCoords();
				Vector2 endScale = connections [i].transform.localScale;
				// Check X coords
				Vector3 midPoint = (startCoords + endCoords) / 2;
				midPoints.Add (midPoint);

				// I was doing rects wrong, no longer!
				Rect startRect = GetRect(room.transform);
				Rect endRect = GetRect (connections [i].transform);

				// let's try drawing a line
				bool createdCorridor = false;
				Vector2 lineStart = new Vector2 (midPoint.x, startCoords.y),
				lineEnd = new Vector2 (midPoint.x, endCoords.y);
				// Vertical Corridor
				if (startRect.Contains (lineStart) && endRect.Contains (lineEnd)) {
					CreateCorridor (lineStart, lineEnd);
					createdCorridor = true;
				}
				// Horizontal Corridor
				if (!createdCorridor) {
					lineStart = new Vector2 (startCoords.x, midPoint.y);
					lineEnd = new Vector2 (endCoords.x, midPoint.y);
					if (startRect.Contains (lineStart) && endRect.Contains (lineEnd)) {
						CreateCorridor (lineStart, lineEnd);
						createdCorridor = true;
					}
				}
				// L Corridor
				if (!createdCorridor) {
					CreateCorridor (
						startCoords,
						new Vector2 (startCoords.x, endCoords.y)
					);
					CreateCorridor (
						new Vector2 (startCoords.x, endCoords.y),
						endCoords
					);
					createdCorridor = true;
				}
			}
		}
	}


	void CarveOutRoom(GameObject room) {
//		Rect bounds = GetRect (room.transform);
		// the points should be offset from the center of the levelBounds, so that levelBOunds center is in the middle of the map
		Vector2 bottomLeft = room.transform.position;
		Vector2 topRight = room.transform.position;
		bottomLeft -= (Vector2)room.transform.lossyScale / 2;
		topRight += (Vector2)room.transform.lossyScale / 2;

		bottomLeft += level.mapSize / 2;
		topRight += level.mapSize / 2;
		map.CarveOutRoom (bottomLeft, topRight);
	}
		
	void OnDrawGizmos() {
		if (mainRoomPoints == null) {
			return;
		}
//		foreach (Vector2 point in mainRoomPoints) {
//			Gizmos.DrawSphere (point, 1f);
//		}
//
		Gizmos.color = Color.magenta;
		if (m_delaunayTriangulation != null) {
			for (int i = 0; i< m_delaunayTriangulation.Count; i++) {
				Vector2 left = (Vector2)m_delaunayTriangulation [i].p0;
				Vector2 right = (Vector2)m_delaunayTriangulation [i].p1;
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}
		if (m_spanningTree != null) {
			Gizmos.color = Color.green;
			for (int i = 0; i< m_spanningTree.Count; i++) {
				LineSegment seg = m_spanningTree [i];				
				Vector2 left = (Vector2)seg.p0;
				Vector2 right = (Vector2)seg.p1;
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}

//		if (corridors != null) {
//			Gizmos.color = Color.yellow;
//			for (int i = 0; i< corridors.Count; i++) {
//				LineSegment seg = corridors [i];				
//				Vector2 left = (Vector2)seg.p0;
//				Vector2 right = (Vector2)seg.p1;
//				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
//			}
//		}
//		if (midPoints != null) {
//			Gizmos.color = Color.white;
//			for (int i = 0; i < midPoints.Count; i++) {
//				Gizmos.DrawSphere (midPoints [i], 2f);
//			}
//		}
	}

	GameObject GetRoomAtCoords(Vector2 coords) {
		foreach (GameObject room in rooms) {
			if (room.GetComponent<BaseRoomController> ().GetCoords () == coords) {
				return room;
			}
		}
		return null;
	}

	Rect GetRect(Transform t) {
		return new Rect (
			t.position.x - t.localScale.x / 2,
			t.position.y - t.localScale.y / 2,
			t.localScale.x,
			t.localScale.y
		);
	}

	void CreateCorridor(Vector2 start, Vector2 end) {
		Vector2 midPoint = (start + end) / 2;
		GameObject corridor = GameObject.Instantiate (level.defaultRoom, gameObject.transform);
//		corridor.GetComponent<BoxCollider2D> ().usedByComposite = true;
		corridor.GetComponent<Rigidbody2D> ().isKinematic = true;
		corridor.name = "Corridor";
		corridor.transform.position = midPoint;
		corridor.layer = LayerMask.NameToLayer ("Corridor");

		if (start.x == end.x) {
			corridor.transform.position = new Vector2 (start.x, midPoint.y);

			corridor.transform.localScale = new Vector2 (4, Vector2.Distance(start, end));
		}
		if (start.y == end.y) {
			corridor.transform.position = new Vector2 (midPoint.x, start.y);
			corridor.transform.localScale = new Vector2 (Vector2.Distance(start, end), 4);

		}

//		Collider2D[] hits = Physics2D.OverlapAreaAll (start, end, 1 << LayerMask.NameToLayer("ConnectingRoom"));
		RaycastHit2D[] hits = Physics2D.LinecastAll(start, end, 1 << LayerMask.NameToLayer("UnusedRoom"));
		foreach (RaycastHit2D hit in hits) {
			// Layermask does not work?
			if (hit.transform.gameObject.layer != LayerMask.NameToLayer ("UnusedRoom")) {
				continue;
			}
			hit.transform.gameObject.layer = LayerMask.NameToLayer ("ConnectingRoom");
		}
	}
		
	private List<GameObject> enemies = new List<GameObject>();

	void SpawnExitAtCoords(Coordinates c) {
		
	}

	bool IsWallOrLavaAtCoords(Coordinates c) {
		if (map.IsWallAtCoords (c) || map.IsLavaAtCoords (c)) {
			return true;
		}
		return false;
	}


}
		