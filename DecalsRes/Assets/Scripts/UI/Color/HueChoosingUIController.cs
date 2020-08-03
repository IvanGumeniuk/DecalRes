using UnityEngine;
using UnityEngine.UI;

public class HueChoosingUIController : MonoBehaviour
{
    public Image hueImage;
    public Color hueColor;

    public Texture2D texture2D;
    public Vector2 onRectPosition;
    public RectTransform rect;
	public int width;
	public int height;

	private void Start()
	{
		rect = hueImage.GetComponent<RectTransform>();
		texture2D = hueImage.mainTexture as Texture2D;

		width = (int)rect.rect.width;
		height = (int)rect.rect.height;
	}
    
	public Color GetColor(Vector3 screenPoint)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, null, out onRectPosition);

		onRectPosition.x = width - (width / 2 - onRectPosition.x);
		onRectPosition.y = Mathf.Abs((height / 2 - onRectPosition.y) - height);

		if (onRectPosition.x > width || onRectPosition.x < 0)
			onRectPosition.x = -1;

		if (onRectPosition.y > height)
			onRectPosition.y = -1;

		if (onRectPosition.x > -1 && onRectPosition.y > -1)
		{
			hueColor = texture2D.GetPixel((int)onRectPosition.x, (int)onRectPosition.y);
		}

		return hueColor;
	}
}


