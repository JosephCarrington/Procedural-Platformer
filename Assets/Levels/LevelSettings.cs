using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class RoomCreationSettings {

}

[CreateAssetMenu(fileName = "levelData", menuName = "LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject {
	[Header("Initial Room Creation")]
	public Vector2 mapSize = Vector2.one * 256;
	public GameObject defaultRoom;
	public int roomCount = 128;
	public int minWidth = 4;
	public int minHeight = 4;
	public int widthVariance = 16;
	public int heightVariance = 16;
	[Range(0f, 1f)]
	public float chanceToAddExtraCorridor = 0.25f;

	[Header("Vaults")]
	public TextAsset[] vaults;

	[Header("Ground Decoration")]
	[Range(0f, 1f)]
	public float chanceToSpawnGroundDecoration = 0.25f;
	public GameObject[] groundDecorations;

	[Header("Spikes")]
	public float chanceToSpawnSpike = 0.1f;
	public GameObject spike;

	[Header("Traps")]
	[Range(0f, 1f)]
	public float chanceToSpawnTrap = 0.1f;
	public GameObject[] traps;

	[Header("Lava")]
	[Range(0f, 1f)]
	public float chanceToAttemptLava = 1f;
	public int lavaDepth = 6;

	[Header("Enemies")]
	[Range(0f, 1f)]
	public float chanceToSpawnEnemy = 0.1f;
	public GameObject[] enemies;

	[Header("Exit")]
	public GameObject exit;
}
