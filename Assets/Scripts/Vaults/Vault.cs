using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Vaults {
	public enum VaultType {
		Entrance,
		Exit,
		Floating,
		Configured
	}
		
	public class Vault {
		public string name;
		public Coordinates size;
		public string csv;
		public VaultType type = VaultType.Floating;
		public List<TiledObject> objects = new List<TiledObject>();
		public int minDepth, maxDepth;

		public uint[,] tileInts;

		public void ParseCSV() {

			const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
			const uint FLIPPED_VERTICALLY_FLAG   = 0x40000000;
			const uint FLIPPED_DIAGONALLY_FLAG   = 0x20000000;

			tileInts = new uint[size.x, size.y];
			int i = 0;
			string[] tileIds = csv.Split (',');
			List<uint> values = new List<uint>();
			foreach (string id in tileIds) {
				values.Add (uint.Parse (id, System.Globalization.NumberFormatInfo.InvariantInfo));
			}

			uint[] tileData = values.ToArray ();

			for (int y = 0; y < size.y; y++) {
				for (int x = 0; x < size.x; x++) {
					uint c = tileData [i];
					tileInts[x, y] = c;
					i++;
				}
			}
		}
	}
}