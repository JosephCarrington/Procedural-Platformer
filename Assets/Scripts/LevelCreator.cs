using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Utils;

using Delaunay;
using Delaunay.Geo;

public class LevelCreator : MonoBehaviour {

	// Use this for initialization
	public tk2dSprite baseWall;
	public Material pixelSnap;
	GameObject player;

	enum Tile {
		Blank,
		Wall
	}

	Tile[,] tiles;

	public tk2dTileMap tileMap;


	public int roomCount = 100;
	GameObject[] rooms;
	List<GameObject> finalRooms;
	TileMapController map;
	Rect levelBounds;

	void Start () {
		finalRooms = new List<GameObject> ();
		map = GameObject.Find ("TileMap").GetComponent<TileMapController> ();
		player = GameObject.Find ("Player");
		player.gameObject.SetActive (false);
		StartCoroutine(CreateRooms(roomCount));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int minWidth,
		minHeight,
		widthVarience,
		heightVarience;

	float meanHeight,
		meanWidth;
	public GameObject defaultRoom;
	Vector2[] mainRoomPoints;

	public Color mainRoomColor,
		connectingRoomColor,
		corridorColor;

	private List<LineSegment> m_delaunayTriangulation,
		m_spanningTree,
		corridors;

	public float chanceToAddExtraCorridor = 0.1f;

	List<Vector2> midPoints;
	public int lavaDepth = 4;


	public float chanceToSpawnGroundDecoration = 0.5f;

	public GameObject[] groundDecorationPrefabs;
	private GameObject[,] groundDecorations = new GameObject[(int)mapSize.x, (int)mapSize.y];

	public GameObject exit;


	IEnumerator CreateRooms(int numRooms) {
		meanWidth = minWidth + (widthVarience / 2);
		meanHeight = minHeight + (heightVarience / 2);
		rooms = new GameObject[roomCount];
		Time.fixedDeltaTime = 0.001f;
		for (int i = 0; i < numRooms; i++) {
			int wv = Random.Range (0, widthVarience);
			int hv = Random.Range (0, heightVarience);
			// only round numbers please
			if (wv % 2 != 0) {
				wv++;
			}
			if (hv % 2 != 0) {
				hv++;
			}
			GameObject newRoom = GameObject.Instantiate (defaultRoom);
			newRoom.transform.position = Random.insideUnitCircle * (mapSize.x / 4);
			newRoom.transform.localScale = new Vector3 (minWidth + wv, minHeight + hv, 1);
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

		Time.fixedDeltaTime = 0.02f;
		print ("all asleep at " + Time.time);
		// Round positions of rooms to int and get 'main rooms'?
		List<GameObject> mainRooms = new List<GameObject>();
		foreach (GameObject room in rooms) {
			room.GetComponent<Rigidbody2D> ().isKinematic = true;
			Vector2 newPos = room.transform.position;
			newPos.x = Mathf.RoundToInt (newPos.x);
			newPos.y = Mathf.RoundToInt (newPos.y);
			room.transform.position = newPos;
			if (room.transform.localScale.x > (meanWidth * 1.1f) && room.transform.localScale.y > (meanHeight * 1.1f)) {
				room.GetComponent<SpriteRenderer> ().color = mainRoomColor;
				room.layer = LayerMask.NameToLayer ("MainRoom");
				room.name = "Main Room";
				mainRooms.Add (room);
			}
		}

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
			if (Random.value < chanceToAddExtraCorridor) {
				corridors.Add(m_delaunayTriangulation[i]);
			}
		}

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

		levelBounds = new Rect();
		bool firstRoomSet = false;
		foreach (GameObject room in rooms) {
			if (room.layer != LayerMask.NameToLayer ("UnusedRoom")) {
				if (!firstRoomSet) {
					levelBounds = GetRect (room.transform);
					firstRoomSet = true;
				}
				room.GetComponent<BoxCollider2D> ().usedByComposite = true;
				finalRooms.Add (GameObject.Instantiate (room, gameObject.transform));
				print ("Adding room to final");
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
		Vector2 newPlayerPos = new Vector2 (playerPos.x - (mapSize.x / 2), currentYCheck - (mapSize.y / 2) + 1);
		player.transform.position = newPlayerPos;

		foreach (Transform child in transform) {
			if (Random.value < chanceToAttemptLava) {
				Room room = child.GetComponent<Room> ();
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

					leftCheck.y += lavaDepth;
					rightCheck.y += lavaDepth;

					if(map.DoesContinuousWallExist(bottomLeftWall, bottomRightWall)) {
						
						int leftColHeight = map.GetWallColumnHeight (
							new Coordinates (bottomLeftWall.x, bottomLeftWall.y + 1)
	                    );
						int rightColHeight = map.GetWallColumnHeight (
							new Coordinates (bottomRightWall.x, bottomRightWall.y + 1)
						);
						if (leftColHeight > 0 && rightColHeight > 0) {
							int lavaHeight = leftColHeight > rightColHeight ? rightColHeight : leftColHeight;
							lavaHeight = lavaHeight > lavaDepth ? lavaDepth : lavaHeight;
							map.CreateLava (
								room.bottomLeft,
								new Coordinates (room.bottomRight.x, room.bottomRight.y + lavaHeight - 1)
							);
						}

					}
				}
			}
		}
			
		// Ground deco, enemies
		foreach(Transform child in transform) {
			Room room = child.gameObject.GetComponent<Room>();
			for(int x = room.bottomLeft.x; x < room.bottomRight.x; x++) {
				if(map.IsWallAtCoords(new Coordinates(x, room.bottomLeft.y - 1)) && !map.IsWallAtCoords(new Coordinates(x, room.bottomLeft.y))) {
					
					if(Random.value < chanceToSpawnGroundDecoration) {
						GameObject newDeco = GameObject.Instantiate (
							groundDecorationPrefabs [Random.Range (0, groundDecorationPrefabs.Length)],
							new Vector3 (x - mapSize.x / 2, room.bottomLeft.y - mapSize.y / 2),
							Quaternion.identity
						);
						if (Random.value > 0.5) {
							Vector3 newScale = newDeco.gameObject.transform.localScale;
							newScale.x = -1;
							newDeco.gameObject.transform.localScale = newScale;
						}
						groundDecorations [x, room.bottomLeft.y] = newDeco;
					}
					if (room != entranceRoom && Random.value < chanceToSpawnEnemy) {
						GameObject newEnemy = GameObject.Instantiate (enemyPrefabs [0], new Vector3 (x - mapSize.x / 2, room.bottomLeft.y - mapSize.y / 2, -1), Quaternion.identity);
						enemies.Add (newEnemy);
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
					GameObject newExit = GameObject.Instantiate (exit, new Vector3 (room.pos.x - (mapSize.x / 2) + 0.5f, room.pos.y - ((mapSize.y / 2) -1) - 0.5f, -1), Quaternion.identity);
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

	public static Vector3 mapSize = Vector3.one * 256;
	public GameObject coin;
	public float chanceToSpawnCoin = 0.01f;
	public float chanceToAttemptLava = 1f;


	void CarveOutRoom(GameObject room) {
//		Rect bounds = GetRect (room.transform);
		// the points should be offset from the center of the levelBounds, so that levelBOunds center is in the middle of the map
		Vector3 bottomLeft = room.transform.position;
		Vector3 topRight = room.transform.position;
		bottomLeft -= room.transform.lossyScale / 2;
		topRight += room.transform.lossyScale / 2;



		bottomLeft += mapSize / 2;
		topRight += mapSize / 2;
		map.CarveOutRoom (bottomLeft, topRight);
		bottomLeft -= mapSize / 2;
		topRight -= mapSize / 2;

		// Coins and Enemies
		for (int x = (int)bottomLeft.x; x < (int)topRight.x; x++) {
			for (int y = (int)bottomLeft.y; y < (int)topRight.y; y++) {	
				if (Random.value < chanceToSpawnCoin) {
					GameObject.Instantiate (coin, new Vector3 (x, y, -1), Quaternion.identity);
				}
			}
		}
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
		GameObject corridor = GameObject.Instantiate (defaultRoom, gameObject.transform);
		corridor.GetComponent<BoxCollider2D> ().usedByComposite = true;
		corridor.GetComponent<Rigidbody2D> ().isKinematic = true;
		corridor.name = "Corridor";
		corridor.transform.position = midPoint;
		corridor.GetComponent<SpriteRenderer> ().color = corridorColor;
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
			hit.transform.gameObject.GetComponent<SpriteRenderer> ().color = connectingRoomColor;
			hit.transform.gameObject.layer = LayerMask.NameToLayer ("ConnectingRoom");
		}
	}

	public GameObject[] enemyPrefabs;
	private List<GameObject> enemies = new List<GameObject>();

	public float chanceToSpawnEnemy = 0.01f;

	void SpawnExitAtCoords(Coordinates c) {
		
	}


}
		