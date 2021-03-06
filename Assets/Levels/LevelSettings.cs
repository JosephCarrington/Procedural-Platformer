﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

[CreateAssetMenu(fileName = "levelData", menuName = "LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject {
	[Header("Vault Settings")]
	public int depth = 1;
	[Range(0f, 1f)]
	public float chanceToPlaceVault = 0;

		
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
	public WeightedSpawn[] enemies;

	[Header("Exit")]
	public GameObject exit;

	[Header("Treasure")]
	[Range(0f, 1f)]
	public float chanceToSpawnTreasure = 0.01f;
	public GameObject[] treasure;

	[Header("TileSet")]
	public int firstTileId = 0;

	public void Awake() {
		
	}
	public GameObject GetEnemy() {
		GameObject enemy = null;
		float totalSum = 0f;
		foreach (WeightedSpawn spawn in enemies) {
			totalSum += spawn.weight;
		}
		float roll = Random.Range (0, totalSum);
		float amountLeft = roll;
		foreach (WeightedSpawn spawn in enemies) {
			if (amountLeft > spawn.weight) {
				amountLeft -= spawn.weight;
			} else {
				enemy = spawn.prefab;
				break;
			}
			
		}
		return enemy;
	}
}
