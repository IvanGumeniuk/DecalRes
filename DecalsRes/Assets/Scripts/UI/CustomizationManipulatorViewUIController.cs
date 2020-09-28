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
    public event Action OnCameraFollowPressed;
    public event Action OnFlipPressed;
    public event Action OnPaintableTargetPressed;
    public event Action OnMirrorPressed;

    public FloatingUIComponentController floatingUIController;

    public Button moveButton;
    public Button sizeButton;
    public Button confirmButton;
    public Button cancelButton;

    public Button cameraSideButton;
    public Button cameraFollowButton;
    public Button flipButton;
    public Button mirrorButton;
    public Button paintableTargetButton;

    public Text cameraSideText;
    public Text cameraFollowText;
    public Text flipText;
    public Text mirrorText;
    public Text paintableTargetText;
        
    public Image cameraFollowImage;
    public Image reflectionImage;
    public Image mirrorImage;
    public Image paintableTargetImage;

    [SerializeField] private Sprite cameraFollowOnSprite;
    [SerializeField] private Sprite cameraFollowOffSprite;

    [SerializeField] private Sprite flipEnabledSprite;
    [SerializeField] private Sprite flipDisabledSprite;

    [SerializeField] private Sprite mirrorEnabledSprite;
    [SerializeField] private Sprite mirrorDisabledSprite;

    [SerializeField] private Sprite targetBodyWindowsSprite;
    [SerializeField] private Sprite targetBodySprite;
    [SerializeField] private Sprite targetWindowsSprite;

    public Vector3 startPosition;
    public Vector3 difference;

    private DecalsUIController decalsUIController;

    private bool DecalIsChosen { get { return decalsUIController.DecalIsChoosing; } }


	private void Start()
	{
        decalsUIController = IngameUIManager.Instance.decalsController;
    }

	public void OnConfirmButtonClick()
	{
        if (DecalIsChosen)
        {
            decalsUIController.DeselectButtons();
            OnConfirmDecalPainting?.Invoke(true);
            return;
        }

        OnConfirmDecalChanging?.Invoke(true);
    }

    public void OnCancelButtonClick()
    {
        if (DecalIsChosen)
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

    public void SetCameraFollowStatus(bool follow)
	{
        cameraFollowText.text = $"CameraFollow: {follow}";
        cameraFollowImage.sprite = follow ? cameraFollowOnSprite : cameraFollowOffSprite;
    }

    public void SetFlipButtonStatus(bool flipped)
    {
        flipText.text = $"Flipped: {flipped}";
        reflectionImage.sprite = flipped ? flipEnabledSprite : flipDisabledSprite;
    }

    public void SetMirrorButtonStatus(bool reflected)
    {
        mirrorText.text = $"Reflected: {reflected}";
        mirrorImage.sprite = reflected ? mirrorEnabledSprite : mirrorDisabledSprite;
    }

    public void SetPaintableTargetStatus(DecalPaintableTarget target)
    {
        paintableTargetText.text = $"Target: {target}";

        switch (target)
        {
            case DecalPaintableTarget.Body:
                {
                    paintableTargetImage.sprite = targetBodySprite;
                    break;
                }
            case DecalPaintableTarget.Windows:
                {
                    paintableTargetImage.sprite = targetWindowsSprite;
                    break;
                }
            case DecalPaintableTarget.BodyAndWindows:
                {
                    paintableTargetImage.sprite = targetBodyWindowsSprite;
                    break;
                }
        }
    }

    public void OnCameraSideClick()
	{
        OnCameraSidePressed?.Invoke();
	}

    public void OnCameraFollowClick()
	{
        OnCameraFollowPressed?.Invoke();
	}

    public void OnFlipClicked()
	{
        OnFlipPressed?.Invoke();
    }
    
    public void OnPaintableTargetClicked()
	{
        OnPaintableTargetPressed?.Invoke();
    }

    public void OnMirrorClicked()
	{
        OnMirrorPressed?.Invoke();
    }
}
