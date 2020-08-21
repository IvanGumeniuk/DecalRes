﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDecalSettings : MonoBehaviour
{
    [SerializeField] private List<FontDecalConfig> fonts = new List<FontDecalConfig>();

    public Font GetFont(int id)
	{
		var font = fonts.Find(x => x.id == id);
		return font == null ? null : font.font;
	}
}
