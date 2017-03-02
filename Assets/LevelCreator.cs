using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Delaunay;
using Delaunay.Geo;

public class LevelCreator : MonoBehaviour {

	// Use this for initialization
	public int levelWidth = 32;
	public int levelHeight = 32;

	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public Material pixelSnap;
	GameObject[,] tiles;


	public int roomCount = 100;
	GameObject[] rooms;
	void Start () {
		tiles = new GameObject[levelWidth, levelHeight];
//		FillLevelWithWalls ();
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

	public Color mainRoomColor;

	private List<LineSegment> m_delaunayTriangulation;
	private List<LineSegment> m_spanningTree;

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
			newRoom.transform.position = Random.insideUnitCircle * 100;
			newRoom.transform.localScale = new Vector3 (minWidth + wv, minHeight + hv, 1);
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
			newPos.x += 0.5f;
			newPos.y += 0.5f;
			room.transform.position = newPos;
			if (room.transform.localScale.x > (meanWidth * 1.1f) && room.transform.localScale.y > (meanHeight * 1.1f)) {
				room.GetComponent<SpriteRenderer> ().color = mainRoomColor;
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

			
	}

	void OnDrawGizmos() {
		if (mainRoomPoints == null) {
			return;
		}
		foreach (Vector2 point in mainRoomPoints) {
			Gizmos.DrawSphere (point, 1f);
		}

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
	}

}
		