using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationViewUIController : MonoBehaviour
{
    public Button backButton;
    public Button customizatonButton;

	public List<GameObject> openedViews = new List<GameObject>();

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
		customizatonButton.gameObject.SetActive(false);
	}

	public void OpenView(GameObject view)
	{
		if (openedViews.Count > 0)
		{
			openedViews[openedViews.Count - 1].SetActive(false);
		}

		openedViews.Add(view);
		openedViews[openedViews.Count - 1].SetActive(true);
	}

	private void OnBackPressed()
	{
		if (openedViews.Count > 0)
		{
			openedViews[openedViews.Count - 1].SetActive(false);
			openedViews.RemoveAt(openedViews.Count - 1);

			if (openedViews.Count > 0)
				openedViews[openedViews.Count - 1].SetActive(true);
			else
				customizatonButton.gameObject.SetActive(true);
		}
	}
}
