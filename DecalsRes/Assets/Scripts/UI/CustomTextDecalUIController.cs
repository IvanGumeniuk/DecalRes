﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomTextDecalUIController : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private DecalCustomTextController textDecalContoller;

	public List<Button> fontButtons = new List<Button>();
	private List<CanvasGroup> fontCanvasGroups = new List<CanvasGroup>();

	[SerializeField] private List<Font> fonts = new List<Font>();

	[SerializeField] private List<int> textDecalsIDs = new List<int>();

	private DecalsUIController decalsUIController;
	
	[SerializeField] private int currentID;
	private int selectedFontIndex = 0;

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

		decalsUIController.OnCreatingDecal(DecalType.Text, GetTexture(decalsUIController.ID));
	}

	public void DeselectButtons()
	{
		foreach (var group in fontCanvasGroups)
		{
			group.alpha = 0.5f;
		}

		//inputField.gameObject.SetActive(false);
	}

	public void SetActiveInput(bool active)
	{
		inputField.gameObject.SetActive(active);
	}

	public bool IsTextDecal (int id)
	{
        return textDecalsIDs.Contains(id);
	}

    public void OnInputTextChanged(string text)
	{
        textDecalContoller.SetText(text);
    }

    public void CreateTextDecal(int id)
    {
        if (!IsAnyButtonEnabled)
            return;

		textDecalContoller.CreateNewTexture(id, fonts[selectedFontIndex]);

		textDecalContoller.SetTextureToCamera(id);
    }

    public RenderTexture GetTexture(int id)
    {
        return textDecalContoller.GetTexture(id);
    }

	public void OnDecalCreated(int id)
	{
		if (inputField.gameObject.activeSelf)
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
		Debug.Log($"OnDecalChoosen {id}");
        textDecalContoller.SetTextureToCamera(id);
		inputField.text = textDecalContoller.GetText(id);
		SetActiveInput(true);
	}

	public void OnDecalRemoved(int id)
    {
        textDecalContoller.RemoveTexture(id);
        if (textDecalsIDs.Contains(id))
            textDecalsIDs.Remove(id);
    }
}