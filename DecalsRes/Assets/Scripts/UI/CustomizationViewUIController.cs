using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationViewUIController : MonoBehaviour
{
	public Action<SubviewType, bool> OnViewOpened;

    public Button backButton;
    public Button customizatonButton;

	public List<GameObject> openedViews = new List<GameObject>();
	public List<SubviewType> openedViewTypes = new List<SubviewType>();

	public SubviewType LastOpened { get { return openedViewTypes.Count > 0 ? openedViewTypes[openedViewTypes.Count - 1] : SubviewType.None; } }

	public GameObject categoryUI;

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
		if (view == null || (LastOpened == subviewType && LastOpened != SubviewType.None))
		{
			return;
		}

		if (openedViews.Count > 0)
		{
			bool disablePrevious = LastOpened == SubviewType.Stickers || LastOpened == SubviewType.CustomPainting || LastOpened == SubviewType.CustomText;

			if (disablePrevious)
			{
				openedViews[openedViews.Count - 1].SetActive(false);

				openedViews.RemoveAt(openedViews.Count - 1);
				openedViewTypes.RemoveAt(openedViewTypes.Count - 1);
			}
		}

		openedViews.Add(view);
		openedViewTypes.Add(subviewType);

		openedViews[openedViews.Count - 1].SetActive(true);
		OnViewOpened?.Invoke(subviewType, true);
	}

	private void OnBackPressed()
	{
		if (openedViews.Count > 0)
		{
			openedViews[openedViews.Count - 1].SetActive(false);

			OnViewOpened?.Invoke(openedViewTypes[openedViewTypes.Count - 1], false);

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
