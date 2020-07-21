using PaintIn3D;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerDecalUIController : MonoBehaviour
{
	public Action<Sprite> OnStickerClicked;

	public GameObject stickerSubcategoryObject;
	public GameObject customizationManipulatorView;

	public List<Button> stickerButtons = new List<Button>();
	public P3dPaintDecal stickerDecal;

	public bool IsAnyButtonEnabled
	{
		get
		{
			for (int i = 0; i < stickerButtons.Count; i++)
			{
				if (stickerButtons[i].gameObject.activeSelf)
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
		if(stickerIndex >= 0 && stickerIndex < stickerButtons.Count)
		{
			foreach (var button in stickerButtons)
				button.GetComponent<CanvasGroup>().alpha = 0.5f;

			stickerButtons[stickerIndex].GetComponent<CanvasGroup>().alpha = 1;
			stickerDecal.Texture = stickerButtons[stickerIndex].GetComponent<Image>().mainTexture;
		}

		if (IsAnyButtonEnabled)
		{
			stickerDecal.gameObject.SetActive(true);
			OnStickerClicked?.Invoke(stickerButtons[stickerIndex].GetComponent<Image>().sprite);
		}
	}

	public void DisableAllButtons()
	{
		foreach (var button in stickerButtons)
		{
			button.GetComponent<CanvasGroup>().alpha = 0.5f;
		}
		stickerDecal.gameObject.SetActive(false);
	}
}
