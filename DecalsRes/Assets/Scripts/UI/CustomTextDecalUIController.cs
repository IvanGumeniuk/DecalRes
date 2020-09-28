using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomTextDecalUIController : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private DecalCustomTextController textDecalContoller;

	public List<Button> fontButtons = new List<Button>();
	private List<CanvasGroup> fontCanvasGroups = new List<CanvasGroup>();

	[SerializeField] private Button showInputFieldButton;
	[SerializeField] private Button cancelInputButton;
	[SerializeField] private Button confirmInputButton;

	[SerializeField] private OpenAnimationUIController inputViewAnimationController;

	[SerializeField] private List<int> textDecalsIDs = new List<int>();

	private DecalsUIController decalsUIController;

	
	[SerializeField] private int currentID;
	[SerializeField] private int selectedFontIndex = 0;

	public bool IsAnyButtonEnabled
	{
		get
		{
			for (int i = 0; i < fontCanvasGroups.Count; i++)
			{
				if (Mathf.Approximately(fontCanvasGroups[i].alpha, 1))
					return true;
			}

			return false;
		}
	}

	private void Start()
	{
		decalsUIController = IngameUIManager.Instance.decalsController;

		for (int i = 0; i < fontButtons.Count; i++)
		{
			fontCanvasGroups.Add(fontButtons[i].GetComponent<CanvasGroup>());
		}

		textDecalsIDs = new List<int>(SettingsManager.Instance.textDecalSettings.GetFontsIDs());

		showInputFieldButton.onClick.AddListener(OnShowInputFieldButtonPressed);
		confirmInputButton.onClick.AddListener(OnConfirmInputButtonPressed);
		cancelInputButton.onClick.AddListener(OnCancelInputButtonPressed);
	}

	private void OnDestroy()
	{
		showInputFieldButton.onClick.RemoveListener(OnShowInputFieldButtonPressed);
		confirmInputButton.onClick.RemoveListener(OnConfirmInputButtonPressed);
		cancelInputButton.onClick.RemoveListener(OnCancelInputButtonPressed);
	}

	public void OnFontButtonClick(int fontIndex)
	{
		if (fontIndex >= 0 && fontIndex < fontButtons.Count)
		{
			foreach (var group in fontCanvasGroups)
				group.alpha = 0.5f;

			fontCanvasGroups[fontIndex].alpha = 1;
		}

		selectedFontIndex = fontIndex;

		CreateTextDecal(decalsUIController.ID);

		SetActiveInput(true);

		decalsUIController.OnCreatingDecal(DecalType.Text, -1, GetTexture(decalsUIController.ID));
	}

	public void DeselectButtons()
	{
		foreach (var group in fontCanvasGroups)
		{
			group.alpha = 0.5f;
		}
	}

	private void OnShowInputFieldButtonPressed()
	{
		inputViewAnimationController.Open();
	}

	private void OnConfirmInputButtonPressed()
	{
		inputViewAnimationController.Close();
		textDecalContoller.SetText(inputField.text);
	}

	private void OnCancelInputButtonPressed()
	{
		inputViewAnimationController.Close();
	}

	public void SetActiveInput(bool active)
	{
		showInputFieldButton.gameObject.SetActive(active);
	}

    public void OnInputTextChanged(string text)
	{
        textDecalContoller.SetText(text);
    }

    public void CreateTextDecal(int id)
    {
        if (!IsAnyButtonEnabled)
            return;

		HandleTextureCreating(id, selectedFontIndex);
	}

	public void SetText(string text, int fontID = -1)
	{
		textDecalContoller.SetText(text, fontID);
	}

	public void HandleTextureCreating(int id, int fontID)
	{
		textDecalContoller.CreateNewTexture(id, fontID);
		textDecalContoller.SetTextureToCamera(id, fontID);
	}


    public RenderTexture GetTexture(int id)
    {
        return textDecalContoller.GetTexture(id);
    }

	public void GetTextDecalInfo(int decalID, out int fontID, out string text)
	{
		textDecalContoller.GetText(decalID, out fontID, out text);
	}

	public void OnDecalCreated(int id)
	{
		if (showInputFieldButton.gameObject.activeSelf)
		{
			textDecalContoller.StoreDecalText(id);
			SetActiveInput(false);

			if (!textDecalsIDs.Contains(id))
			{
				textDecalsIDs.Add(id);
			}
		}
	}

    public void OnDecalChoosen(int id)
    {
		textDecalContoller.GetText(id, out int fontID, out string text);
        textDecalContoller.SetTextureToCamera(id, fontID);
		inputField.text = text;
		SetActiveInput(true);
	}

	public void OnDecalRemoved(int id)
    {
        textDecalContoller.RemoveTexture(id);
        if (textDecalsIDs.Contains(id))
            textDecalsIDs.Remove(id);
    }
}
