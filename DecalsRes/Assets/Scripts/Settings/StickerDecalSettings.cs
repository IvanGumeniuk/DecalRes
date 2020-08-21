using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerDecalSettings : MonoBehaviour
{
	[SerializeField] private List<StickerDecalConfig> stickersConfigs = new List<StickerDecalConfig>();

	public Texture GetTexture(int id)
	{
		var config = stickersConfigs.Find(x => x.id == id);

		return config == null ? null : config.texture;
	}
}
