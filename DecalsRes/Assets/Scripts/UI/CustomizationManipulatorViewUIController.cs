using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationManipulatorViewUIController : MonoBehaviour
{
    public event Action<bool> OnConfirmDecalPainting;
    public event Action<bool> OnConfirmDecalChanging;
    public event Action OnStartMoving;
    public event Action OnFinishMoving;
    public event Action OnStartRotationAndScaling;
    public event Action OnFinishRotationAndScaling;

    public Button moveButton;
    public Button sizeButton;
    public Button confirmButton;
    public Button cancelButton;

    public Vector3 startPosition;
    public Vector3 difference;

    private bool StickerIsAvailable { get { return IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.IsAnyButtonEnabled; } }

    public void OnConfirmButtonClick()
	{
        if (StickerIsAvailable)
        {
            IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.DisableAllButtons();
            OnConfirmDecalPainting?.Invoke(true);
            return;
        }

        OnConfirmDecalChanging?.Invoke(true);
    }

    public void OnCancelButtonClick()
    {
        if (StickerIsAvailable)
        {
            OnConfirmDecalPainting?.Invoke(false);
            return;
        }
        OnConfirmDecalChanging?.Invoke(false);
    }

    public void OnMoveButtonPointerDown()
    {
        startPosition = Input.mousePosition;
        difference = Vector3.zero;

        OnStartMoving?.Invoke();
    }

    public void OnMoveButtonDrag()
    {
        difference = Input.mousePosition - startPosition;
    }

    public void OnMoveButtonPointerUp()
    {
        startPosition = Vector3.zero;
        difference = Vector3.zero;

        OnFinishMoving?.Invoke();
    }

    public void OnSizeButtonPointerDown()
    {
        startPosition = Input.mousePosition;
        difference = Vector3.zero;

        OnStartRotationAndScaling?.Invoke();
    }

    public void OnSizeButtonDrag()
    {
        difference = Input.mousePosition - startPosition;
    }

    public void OnSizeButtonPointerUp()
    {
        startPosition = Vector3.zero;
        difference = Vector3.zero;

        OnFinishRotationAndScaling?.Invoke();
    }
}
