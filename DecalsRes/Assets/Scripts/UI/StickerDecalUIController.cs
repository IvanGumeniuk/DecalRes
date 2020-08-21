using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerDecalUIController : MonoBehaviour
{
	public List<Button> stickerButtons = new List<Button>();
	private List<CanvasGroup> stickerCanvasGroups = new List<CanvasGroup>();

	private DecalsUIController decalsUIController;

	public bool IsAnyButtonEnabled
	{
		get
		{
			for (int i = 0; i < stickerCanvasGroups.Count; i++)
			{
				if (Mathf.Approximately(stickerCanvasGroups[i].alpha, 1))
					return true;
			}

			return false;
		}
	}

	private void Start()
	{
		decalsUIController = IngameUIManager.Instance.decalsController;

		for (int i = 0; i < stickerButtons.Count; i++)
		{
			stickerCanvasGroups.Add(stickerButtons[i].GetComponent<CanvasGroup>());
		}
	}

	public void OnStickerButtonClick(int stickerIndex)
	{
		if (stickerIndex >= 0 && stickerIndex < stickerButtons.Count)
		{
			foreach (var group in stickerCanvasGroups)
				group.alpha = 0.5f;

			stickerCanvasGroups[stickerIndex].alpha = 1;
		}

		decalsUIController.OnCreatingDecal(DecalType.Sticker, stickerIndex, null);
	}

	public void DeselectButtons()
	{
		foreach (var group in stickerCanvasGroups)
		{
			group.alpha = 0.5f;
		}
	}
}
