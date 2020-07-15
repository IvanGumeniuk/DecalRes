using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationManipulatorViewUIController : MonoBehaviour
{
    public event Action<bool> OnConfirmDecalPainting;
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
        if (!StickerIsAvailable)
            return;

        OnConfirmDecalPainting?.Invoke(true);
    }

    public void OnCancelButtonClick()
	{
        if (!StickerIsAvailable)
            return;

        OnConfirmDecalPainting?.Invoke(false);
    }

    public void OnMoveButtonPointerDown()
    {
        if (!StickerIsAvailable)
            return;

        startPosition = Input.mousePosition;
        difference = Vector3.zero;

        OnStartMoving.Invoke();
    }

    public void OnMoveButtonDrag()
    {
        if (!StickerIsAvailable)
            return;

        difference = Input.mousePosition - startPosition;
    }

    public void OnMoveButtonPointerUp()
    {
        if (!StickerIsAvailable)
            return;

        startPosition = Vector3.zero;
        difference = Vector3.zero;

        OnFinishMoving.Invoke();
    }

    public void OnSizeButtonPointerDown()
    {
        if (!StickerIsAvailable)
            return;

        startPosition = Input.mousePosition;
        difference = Vector3.zero;

        OnStartRotationAndScaling?.Invoke();
    }

    public void OnSizeButtonDrag()
    {
        if (!StickerIsAvailable)
            return;

        difference = Input.mousePosition - startPosition;
    }

    public void OnSizeButtonPointerUp()
    {
        if (!StickerIsAvailable)
            return;

        startPosition = Vector3.zero;
        difference = Vector3.zero;

        OnFinishRotationAndScaling?.Invoke();
    }
}
