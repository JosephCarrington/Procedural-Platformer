using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	[SerializeField] SpriteRenderer sr;
	[SerializeField] tk2dSpriteCollectionData scd;
	[SerializeField] int spriteIndex;

	void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
		if (sr == null)
		{
			sr = gameObject.AddComponent<SpriteRenderer>();
		}
	}


	void OnValidate()
	{
		if (scd == null || !scd.IsValidSpriteId(spriteIndex))
		{
			return;
		}

		Debug.Log("valid");

		var sprtk = scd.spriteDefinitions[spriteIndex];
		var tex = scd.textures[0];
		Sprite spr = Sprite.Create(tex as Texture2D, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
	}
}
