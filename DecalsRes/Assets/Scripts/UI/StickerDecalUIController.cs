using PaintIn3D;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerDecalUIController : MonoBehaviour
{
	public Action<Sprite, int> OnStickerClicked;

	public GameObject stickerSubcategoryObject;
	public GameObject customizationManipulatorView;

	public List<Button> stickerButtons = new List<Button>();
	private List<CanvasGroup> stickerCanvasGroups = new List<CanvasGroup>();

	public P3dPaintDecal stickerDecal;
	private int stickerID = 0;

	private void Start()
	{
		IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalPainting += OnConfirmSticker;

		for (int i = 0; i < stickerButtons.Count; i++)
		{
			stickerCanvasGroups.Add(stickerButtons[i].GetComponent<CanvasGroup>());
		}
	}

	private void OnDestroy()
	{
		IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalPainting -= OnConfirmSticker;
	}

	public bool IsAnyButtonEnabled
	{
		get
		{
			for (int i = 0; i < stickerCanvasGroups.Count; i++)
			{
				if (Mathf.Approximately(stickerCanvasGroups[i].alpha,1))
					return true;
			}

			return false;
		}
	}

	private void Update()
	{
		customizationManipulatorView.SetActive(stickerSubcategoryObject.activeSelf);
	}


	public void OnStickerButtonClick(int stickerIndex)
	{
		if (stickerIndex >= 0 && stickerIndex < stickerButtons.Count)
		{
			foreach (var group in stickerCanvasGroups)
				group.alpha = 0.5f;

			stickerCanvasGroups[stickerIndex].alpha = 1;
		}

		if (IsAnyButtonEnabled)
		{
			IngameUIManager.Instance.decalLayers.DeselectItems();

			OnStickerClicked?.Invoke(stickerButtons[stickerIndex].GetComponent<Image>().sprite, stickerID);
		}
	}

	public void DisableAllButtons()
	{
		foreach (var group in stickerCanvasGroups)
		{
			group.alpha = 0.5f;
		}
	}

	private void OnConfirmSticker(bool confirm)
	{
		if(confirm) stickerID++;
	}
}
