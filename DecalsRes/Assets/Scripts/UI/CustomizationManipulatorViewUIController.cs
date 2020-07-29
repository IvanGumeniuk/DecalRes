using System;
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

    public event Action OnCameraSidePressed;
    public event Action OnReflectionPressed;
    public event Action OnPaintableTargetPressed;

    public FloatingUIComponentController floatingUIController;

    public Button moveButton;
    public Button sizeButton;
    public Button confirmButton;
    public Button cancelButton;

    public Button cameraSideButton;
    public Button reflectionButton;
    public Button mirrorButton;
    public Button paintableTargetButton;

    public Text cameraSideText;
    public Text reflectionText;
    public Text paintableTargetText;

    public Vector3 startPosition;
    public Vector3 difference;

    private bool StickerIsChosen { get { return IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.IsAnyButtonEnabled; } }


    public void OnConfirmButtonClick()
	{
        if (StickerIsChosen)
        {
            IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.DeselectButtons();
            OnConfirmDecalPainting?.Invoke(true);
            return;
        }

        OnConfirmDecalChanging?.Invoke(true);
    }

    public void OnCancelButtonClick()
    {
        if (StickerIsChosen)
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

    public void SetCameraText(string text)
    {
        cameraSideText.text = "Camera: " + text;
    }

    public void SetReflectionText(string text)
    {
        reflectionText.text = "Reflected: " + text;
    }

    public void SetPaintableTargetText(string text)
    {
        paintableTargetText.text = "Target: " + text;
    }

    public void OnCameraSideClick()
	{
        OnCameraSidePressed?.Invoke();
	}

    public void OnReflectionClicked()
	{
        OnReflectionPressed?.Invoke();
    }
    
    public void OnPaintableTargetClicked()
	{
        OnPaintableTargetPressed?.Invoke();
    }
}
