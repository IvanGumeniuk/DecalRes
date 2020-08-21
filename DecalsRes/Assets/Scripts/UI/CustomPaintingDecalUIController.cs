using PaintIn3D;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomPaintingDecalUIController : MonoBehaviour
{
	public GameObject paintingSubcategoryObject;

	public List<Button> paintingButtons = new List<Button>();
	public P3dPaintSphere paintingDecal;

	private void Update()
	{
		paintingDecal.gameObject.SetActive(paintingSubcategoryObject.activeSelf);
	}

	public void OnPaintingButtonsButtonClick(int paintingIndex)
	{
		if(paintingIndex >= 0 && paintingIndex < paintingButtons.Count)
		{
			foreach (var button in paintingButtons)
				button.GetComponent<CanvasGroup>().alpha = 0.5f;

			paintingButtons[paintingIndex].GetComponent<CanvasGroup>().alpha = 1;
			paintingDecal.Color = paintingButtons[paintingIndex].GetComponent<Image>().color;
			paintingDecal.gameObject.SetActive(true);

			if (paintingIndex == 0)
				paintingDecal.BlendMode = P3dBlendMode.SubtractiveSoft(Vector4.one);
			else
				paintingDecal.BlendMode = P3dBlendMode.AlphaBlend(Vector4.one);

		}
	}
}
