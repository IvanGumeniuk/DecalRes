using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeDecalSettings : MonoBehaviour
{
	[SerializeField] private List<ShapeDecalConfig> shapesConfigs = new List<ShapeDecalConfig>();

	public Texture GetTexture(int id)
	{
		var config = shapesConfigs.Find(x => x.id == id);

		return config == null ? null : config.texture;
	}
}
