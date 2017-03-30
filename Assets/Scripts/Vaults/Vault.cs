using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Vaults {
	public enum VaultType {
		Entrance,
		Exit,
		Floating
	}
		
	public enum TileType {
		Empty,
		Wall,
		Lava,
		Spike
	}

	public class Vault : MonoBehaviour {
		public Coordinates size {
			get {
				return size;
			}
			set {
				size = value;
//				tiles = new TileType[size.x, size.y];
			}
		}
		public string csv;
		public VaultType type = VaultType.Floating;
		public List<TiledObject> objects = new List<TiledObject>();
		public int minDepth, maxDepth;

//		public TileType[,] tiles { get; set;}

		public void ParseCSV() {
//			int i = 0;
//			char[] chars = csv.ToCharArray ();
//			for (int x = 0; x < size.x; x++) {
//				for (int y = 0; y < size.y; y++) {
//					char c = chars [i];
//					print (c);
//					i++;
//				}
//			}
		}
	}
}