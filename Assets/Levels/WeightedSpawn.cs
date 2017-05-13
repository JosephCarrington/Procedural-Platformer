using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedSpawn : System.Object {
	public GameObject prefab;
	[Range(0f, 1f)]
	public float weight;
}
