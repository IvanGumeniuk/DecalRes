using System;
using UnityEngine;

public class DecalsUIController : MonoBehaviour
{
	public Action<DecalType, int, int, Texture> OnDecalCreated;	          // <decalType, decalID, textureID, Texture>
    public Action<int, int, string> OnTextDecalCreated;           // <decalID, fontID, decalText>

    public StickerDecalUIController stickerDecalUIController;
    public CustomPaintingDecalUIController paintingDecalUIController;
    public CustomTextDecalUIController textDecalUIController;

    public SubcategoryUIView customizationManipulatorView;
    public SubcategoryUIView stickerDecalsView;
    public SubcategoryUIView customTextDecalsView;

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
		if (opened)
		{
            if (view != SubviewType.CustomPainting)
                paintingDecalUIController.Activate(false);
        }

        DeselectButtons();
        textDecalUIController.SetActiveInput(false);
    }

    public void OnCreatingDecal(DecalType decalType, int textureID, Texture texture)
    {
        IngameUIManager.Instance.decalLayers.DeselectItems();
        OnDecalCreated?.Invoke(decalType, decalID, textureID, texture);
    }

    private void OnConfirmDecal(bool confirm)
    {
        // When user confirms sticker creation then increment ID. It will be next item ID
        if (confirm)
        {
            textDecalUIController.OnDecalCreated(decalID);
            textDecalUIController.GetTextDecalInfo(decalID, out int fontID, out string text);
            OnTextDecalCreated?.Invoke(decalID, fontID, text);

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

    // New decal chosen or layer item selected 
    public void DecalChoosen(DecalType type, int id)
	{
		switch (type)
		{
			case DecalType.None:
                {
                    textDecalUIController.SetActiveInput(false);   
                    break;
                }
			case DecalType.Sticker:
                {
                    IngameUIManager.Instance.customizationViewUIController.OpenView(stickerDecalsView.animationUIController, SubviewType.Stickers);
                    textDecalUIController.SetActiveInput(false);

                    break;
                }
			case DecalType.Text:
                {
                    IngameUIManager.Instance.customizationViewUIController.OpenView(customTextDecalsView.animationUIController, SubviewType.CustomText);
                    textDecalUIController.OnDecalChoosen(id);

                    break;
                }
			default:
				break;
		}
	}
}
