using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadNotificationUIController : MonoBehaviour
{
	private const string SAVED = "Saved!";
	private const string LOADED = "Loaded!";

	[SerializeField] private Text notificationText;
	[SerializeField] private OpenAnimationUIController animationUIController;
	[SerializeField] private float waitTime = 1.5f;

	[SerializeField] private DecalsPaintController decalsPaintController;

	private CustomizationViewUIController customizationView;

	private void Start()
	{
		decalsPaintController = FindObjectOfType(typeof(DecalsPaintController)) as DecalsPaintController;

		customizationView = IngameUIManager.Instance.customizationViewUIController;
		decalsPaintController.OnSaved += OnSaveClicked;
		decalsPaintController.OnLoaded += OnLoadClicked;
	}

	private void OnDestroy()
	{
		decalsPaintController.OnSaved -= OnSaveClicked;
		decalsPaintController.OnLoaded -= OnLoadClicked;
	}

	private void OnSaveClicked()
	{
		StartCoroutine(ShowNotification(SAVED));
	}

	private void OnLoadClicked()
	{
		StartCoroutine(ShowNotification(LOADED));
	}

	private IEnumerator ShowNotification(string text)
	{
		notificationText.text = text;

		customizationView.saveButton.interactable = false;
		customizationView.loadButton.interactable = false;
		
		animationUIController.Open();
		yield return new WaitForSeconds(waitTime);
		animationUIController.Close();

		yield return new WaitForSeconds(animationUIController.animationDuration);

		customizationView.saveButton.interactable = true;
		customizationView.loadButton.interactable = true;
	}
}
