using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationViewUIController : MonoBehaviour
{
	public Action<SubviewType, bool> OnViewOpened;
	public Action OnSaveClicked;
	public Action OnLoadClicked;
	public Action OnBackClicked;

    public Button backButton;
    public Button customizatonButton;
    public Button saveButton;
    public Button loadButton;

	public List<OpenAnimationUIController> openedViews = new List<OpenAnimationUIController>();
	public List<SubviewType> openedViewTypes = new List<SubviewType>();

	public SubviewType LastOpened { get { return openedViewTypes.Count > 0 ? openedViewTypes[openedViewTypes.Count - 1] : SubviewType.None; } }

	public SubcategoryUIView categoryUI;
	public SubcategoryUIView categoryDecals;

	private void Awake()
	{
		backButton.onClick.AddListener(OnBackPressed);
		saveButton.onClick.AddListener(OnSavePressed);
		loadButton.onClick.AddListener(OnLoadPressed);
		customizatonButton.onClick.AddListener(OnCustomizationButtonPressed);
	}

	private void OnDestroy()
	{
		backButton.onClick.RemoveListener(OnBackPressed);
		saveButton.onClick.RemoveListener(OnSavePressed);
		loadButton.onClick.RemoveListener(OnLoadPressed);
		customizatonButton.onClick.RemoveListener(OnCustomizationButtonPressed);
	}

	private void OnCustomizationButtonPressed()
	{
		OpenView(categoryUI.animationUIController, SubviewType.CustomizationCategory);
		
		customizatonButton.interactable = false;
		backButton.gameObject.SetActive(true);
	}

	public void OpenView(OpenAnimationUIController view, SubviewType subviewType = SubviewType.None)
	{
		if (view == null || (LastOpened == subviewType && LastOpened != SubviewType.None))
		{
			return;
		}

		if(subviewType == SubviewType.Decals)
		{
			IngameUIManager.Instance.decalLayers.gameObject.SetActive(true);
		}

		if (openedViews.Count > 0)
		{
			bool disablePrevious = LastOpened == SubviewType.Stickers
				|| LastOpened == SubviewType.CustomPainting
				|| LastOpened == SubviewType.CustomText
				|| LastOpened == SubviewType.CustomizationCategory
				|| LastOpened == SubviewType.Shapes
				|| LastOpened == SubviewType.Logos
				|| LastOpened == SubviewType.Stripes;

			bool keepInHistory = LastOpened == SubviewType.CustomizationCategory;

			if (disablePrevious)
			{
				openedViews[openedViews.Count - 1].Close();

				if (!keepInHistory)
				{
					openedViews.RemoveAt(openedViews.Count - 1);
					openedViewTypes.RemoveAt(openedViewTypes.Count - 1);
				}
			}
		}

		openedViews.Add(view);
		openedViewTypes.Add(subviewType);

		openedViews[openedViews.Count - 1].Open();
		OnViewOpened?.Invoke(subviewType, true);
	}

	private void OnBackPressed()
	{
		OnBackClicked?.Invoke();

		if (openedViews.Count > 0)
		{
			openedViews[openedViews.Count - 1].Close();

			OnViewOpened?.Invoke(openedViewTypes[openedViewTypes.Count - 1], false);

			if(openedViewTypes[openedViewTypes.Count - 1] == SubviewType.Decals)
			{
				IngameUIManager.Instance.decalLayers.gameObject.SetActive(false);
			}

			openedViews.RemoveAt(openedViews.Count - 1);
			openedViewTypes.RemoveAt(openedViewTypes.Count - 1);

			if (openedViews.Count > 0)
				openedViews[openedViews.Count - 1].Open();
			else
			{
				customizatonButton.interactable = true;
				backButton.gameObject.SetActive(false);
			}
		}
	}

	private void OnSavePressed()
	{
		OnSaveClicked?.Invoke();
	}

	private void OnLoadPressed()
	{
		if (!openedViewTypes.Contains(SubviewType.Decals))
		{
			OpenView(categoryDecals.animationUIController, SubviewType.Decals);
		}
		OnLoadClicked?.Invoke();
	}
}
