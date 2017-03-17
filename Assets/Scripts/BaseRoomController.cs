using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoomController : MonoBehaviour {

	// Use this for initialization
	List<GameObject> connections;
	void Start () {
		connections = new List<GameObject> ();
	}

	public void AddConnection(GameObject room) {
		connections.Add (room);
	}

	public List<GameObject> GetConnections() {
		return connections;
	}
		

	public Vector2 GetCoords() {
		return  new Vector2 (transform.position.x, transform.position.y);
	}
}
