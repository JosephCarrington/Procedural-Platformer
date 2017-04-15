using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

using Vaults;

namespace Utils {
	public class Utils {
		public static float GetBetweenValue(float min, float max, float inputValue) {
			return(inputValue - min) / (max - min);
		}
		public static Vector2 mapSize = Vector2.one * 256f;

		public static Vault XMLToVault(XmlDocument vaultData) {
			
			XmlElement root = vaultData ["map"];

			XmlNodeList properties = root.GetElementsByTagName ("property");
			int minDepth = -1, maxDepth = -1;
			string vaultType;
			foreach (XmlNode p in properties) {
				switch(p.Attributes ["name"].Value) {
				case "Min Depth":
					minDepth = int.Parse (p.Attributes ["value"].Value);
					break;
				case "Max Depth":
					maxDepth = int.Parse (p.Attributes ["value"].Value);
					break;
				case "vault Type":
					vaultType = p.Attributes ["value"].Value;
					break;
				}
			}


			int width =  int.Parse(root.GetAttribute ("width"));
			int height = int.Parse(root.GetAttribute ("height"));
			Coordinates tileSize = new Coordinates(
				int.Parse(root.GetAttribute("tilewidth")),
				int.Parse(root.GetAttribute("tileheight"))
			);

			Coordinates size = new Coordinates (width, height);

			string csv = root.GetElementsByTagName("data")[0].InnerText;
			StringBuilder b = new StringBuilder (csv);
			b.Replace ("\r", "").Replace ("\n", "");

			Vault va = new Vault();
			va.minDepth = minDepth;
			va.maxDepth = maxDepth;
			va.size = size;
			va.csv = b.ToString();


			va.objects = new List<TiledObject> ();
			int exitCount = 0;
			XmlNodeList objects = root.GetElementsByTagName ("object");
			foreach (XmlNode o in objects) {
				TiledObject obj = new TiledObject();
				switch (o.Attributes ["type"].Value) {
				case "Exit":
					obj = new ExitObject ();
					exitCount++;
					break;
				}

				obj.position = new Coordinates (
					int.Parse (o.Attributes ["x"].Value) / tileSize.x,
					int.Parse (o.Attributes ["y"].Value) / tileSize.y
				);

				obj.size = new Coordinates (
					int.Parse (o.Attributes ["width"].Value) / tileSize.x,
					int.Parse (o.Attributes ["height"].Value) / tileSize.y
				);
				va.objects.Add (obj);
			}

			if (exitCount > 0) {
				va.type = VaultType.Configured;
			}
			return va;
		}



		public static List<Vault> ShuffleVaults(List<Vault> list)  
		{  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = Random.Range(0, n + 1);  
				Vault value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  

			return list;
		}

//		public static TileType[,] TransposeMatrix(TileType[,] matrix)
//		{
//			int w = matrix.GetLength(0);
//			int h = matrix.GetLength(1);
//
//			TileType[,] result = new TileType[h, w];
//
//			for (int i = 0; i < w; i++)
//			{
//				for (int j = 0; j < h; j++)
//				{
//					result[j, i] = matrix[i, j];
//				}
//			}
//
//			return result;
//
//		}

	}


}
