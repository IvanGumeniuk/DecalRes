using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalsUIController : MonoBehaviour
{
	public Action<DecalType, int, Texture> OnDecalCreated;	
    
    public StickerDecalUIController stickerDecalUIController;
    public CustomPaintingDecalUIController paintingDecalUIController;
    public CustomTextDecalUIController textDecalUIController;

    public GameObject customizationManipulatorView;
    public GameObject stickerDecalsView;
    public GameObject customTextDecalsView;

    // Unique ID for each sticker. 
    [SerializeField] private int decalID = 0;
    public int ID { get { return decalID; } }

    public bool DecalIsChoosing { get { return stickerDecalUIController.IsAnyButtonEnabled || textDecalUIController.IsAnyButtonEnabled; } }


	private void Start()
	{
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalPainting += OnConfirmDecal;
        IngameUIManager.Instance.customizationViewUIController.OnViewOpened += OnViewOpened;
    }

    private void OnDestroy()
    {
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalPainting -= OnConfirmDecal;
        IngameUIManager.Instance.customizationViewUIController.OnViewOpened -= OnViewOpened;
    }

    private void OnViewOpened(SubviewType view, bool opened)
    {
        bool stickersViewOpened = (view == SubviewType.Stickers || view == SubviewType.CustomText) && opened;
        customizationManipulatorView.SetActive(stickersViewOpened);
       
        DeselectButtons();
        textDecalUIController.SetActiveInput(false);
    }

    public void OnCreatingDecal(DecalType decalType, Texture texture)
    {
        IngameUIManager.Instance.decalLayers.DeselectItems();
        OnDecalCreated?.Invoke(decalType, decalID, texture);
    }

    private void OnConfirmDecal(bool confirm)
    {
        // When user confirms sticker creation then increment ID. It will be next item ID
        if (confirm)
        {
            textDecalUIController.OnDecalCreated(decalID);

            decalID++;
        }
        // Otherwise deselect all buttons
        else
        {
            DecalRemoved(decalID);
            DeselectButtons();
            textDecalUIController.SetActiveInput(false);
        }
    }

    public void DeselectButtons()
	{
        stickerDecalUIController.DeselectButtons();
        textDecalUIController.DeselectButtons();
    }

    public void DecalRemoved(int id)
	{
        textDecalUIController.OnDecalRemoved(id);
    }

    public void DecalChoosen(DecalType type, int id)
	{
		switch (type)
		{
			case DecalType.None:
                {
                    //DeselectButtons();
                    textDecalUIController.SetActiveInput(false);   
                    break;
                }
			case DecalType.Sticker:
                {
                    IngameUIManager.Instance.customizationViewUIController.OpenView(customTextDecalsView, SubviewType.Stickers);
                    textDecalUIController.SetActiveInput(false); 
                    break;
                }
			case DecalType.Text:
                {
                    IngameUIManager.Instance.customizationViewUIController.OpenView(customTextDecalsView, SubviewType.CustomText);
                    textDecalUIController.OnDecalChoosen(id); 
                    break;
                }
			default:
				break;
		}
	}
}
