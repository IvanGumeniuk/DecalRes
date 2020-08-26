using PaintIn3D;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomPaintingDecalUIController : MonoBehaviour
{
	public GameObject paintingSubcategoryObject;

	public List<Button> paintingButtons = new List<Button>();
	public P3dPaintSphere paintingDecal;

	private float paintingRadius = 0.1f;
	private float eraiserRadius = 0.2f;

	private bool canRotateCamera = false;
	public bool CanRotateCamera
	{
		get { return !paintingSubcategoryObject.activeSelf || canRotateCamera; }
		private set { canRotateCamera = value; }
	}

	public Button rotateCameraButton;
	public Image rotateCameraButtonImage;
	public Sprite rotateCameraButtonSpriteOn;
	public Sprite rotateCameraButtonSpriteOff;

	private void Start()
	{
		rotateCameraButton.onClick.AddListener(OnRotateCameraButtonClick);
	}

	private void OnDestroy()
	{
		rotateCameraButton.onClick.RemoveListener(OnRotateCameraButtonClick);
	}

	public void Activate(bool activate)
	{
		paintingDecal.gameObject.SetActive(activate);
	}

	public void OnPaintingButtonsButtonClick(int paintingIndex)
	{
		if(paintingIndex >= 0 && paintingIndex < paintingButtons.Count)
		{
			foreach (var button in paintingButtons)
				button.GetComponent<CanvasGroup>().alpha = 0.5f;

			paintingButtons[paintingIndex].GetComponent<CanvasGroup>().alpha = 1;
			paintingDecal.Color = paintingButtons[paintingIndex].GetComponent<Image>().color;
			paintingDecal.gameObject.SetActive(!CanRotateCamera);

			// Eraiser
			if (paintingIndex == 0)
			{
				paintingDecal.BlendMode = P3dBlendMode.SubtractiveSoft(Vector4.one);
				paintingDecal.Radius = eraiserRadius;
			}
			else
			{
				paintingDecal.BlendMode = P3dBlendMode.AlphaBlend(Vector4.one);
				paintingDecal.Radius = paintingRadius;
			}
		}
	}

	private void OnRotateCameraButtonClick()
	{
		CanRotateCamera = !CanRotateCamera;
		rotateCameraButtonImage.sprite = CanRotateCamera ? rotateCameraButtonSpriteOn : rotateCameraButtonSpriteOff;

		paintingDecal.gameObject.SetActive(!CanRotateCamera);
	}
}
