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
		
	public class Vault {
		public Coordinates size;
		public string csv;
		public VaultType type = VaultType.Floating;
		public List<TiledObject> objects;
		public int minDepth, maxDepth;
	}
}