using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripeDecalSettings : MonoBehaviour
{
	[SerializeField] private List<StripeDecalConfig> stripesConfigs = new List<StripeDecalConfig>();

	public Texture GetTexture(int id)
	{
		var config = stripesConfigs.Find(x => x.id == id);

		return config == null ? null : config.texture;
	}
}
