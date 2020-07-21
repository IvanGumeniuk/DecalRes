using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationViewUIController : MonoBehaviour
{
    public Button backButton;
    public Button customizatonButton;

	public List<GameObject> openedViews = new List<GameObject>();
	public List<SubviewType> openedViewTypes = new List<SubviewType>();

	public GameObject categoryUI;
	public StickerDecalUIController stickerDecalUIController;

	private void Awake()
	{
		backButton.onClick.AddListener(OnBackPressed);
		customizatonButton.onClick.AddListener(OnCustomizationButtonPressed);
	}

	private void OnDestroy()
	{
		backButton.onClick.RemoveListener(OnBackPressed);
		customizatonButton.onClick.RemoveListener(OnCustomizationButtonPressed);
	}

	private void OnCustomizationButtonPressed()
	{
		OpenView(categoryUI);
		IngameUIManager.Instance.decalLayers.gameObject.SetActive(true);
		customizatonButton.interactable = false;
		backButton.gameObject.SetActive(true);
	}

	public void OpenView(GameObject view, SubviewType subviewType = SubviewType.None)
	{
		if (openedViews.Count > 0)
		{
			SubviewType previousView = openedViewTypes[openedViews.Count - 1];

			if (previousView == SubviewType.Stickers || previousView == SubviewType.CustomPainting)
			{
				openedViews[openedViews.Count - 1].SetActive(false);

				openedViews.RemoveAt(openedViews.Count - 1);
				openedViewTypes.RemoveAt(openedViewTypes.Count - 1);
			}
		}

		openedViews.Add(view);
		openedViewTypes.Add(subviewType);

		openedViews[openedViews.Count - 1].SetActive(true);
	}

	private void OnBackPressed()
	{
		if (openedViews.Count > 0)
		{
			openedViews[openedViews.Count - 1].SetActive(false);
			openedViews.RemoveAt(openedViews.Count - 1);
			openedViewTypes.RemoveAt(openedViewTypes.Count - 1);

			if (openedViews.Count > 0)
				openedViews[openedViews.Count - 1].SetActive(true);
			else
			{
				customizatonButton.interactable = true;
				IngameUIManager.Instance.decalLayers.gameObject.SetActive(false);
				backButton.gameObject.SetActive(false);
			}
		}
	}
}
