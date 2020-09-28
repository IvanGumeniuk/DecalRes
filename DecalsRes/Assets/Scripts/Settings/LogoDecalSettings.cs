using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoDecalSettings : MonoBehaviour
{
	[SerializeField] private List<LogoDecalConfig> logosConfigs = new List<LogoDecalConfig>();

	public Texture GetTexture(int id)
	{
		var config = logosConfigs.Find(x => x.id == id);

		return config == null ? null : config.texture;
	}
}
