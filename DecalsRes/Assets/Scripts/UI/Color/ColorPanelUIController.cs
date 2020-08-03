using UnityEngine;
using UnityEngine.UI;

public class ColorPanelUIController : MonoBehaviour
{
    public HueChoosingUIController hueChoosingUIController;
    public RadialMovingUIController radialMovingUIController;

    public Image colorPreviewImage;
    public Image saturationImage;
    public Image brightnessImage;
    public Image alphaImage;

    public Color color;
    public Slider saturation;
    public Slider brightness;
    public Slider alpha;


    private void Start()
    {
        radialMovingUIController.OnMoving += OnChoosingHue;
        saturation.onValueChanged.AddListener(OnColorModifierValueChanged);
        brightness.onValueChanged.AddListener(OnColorModifierValueChanged);
        alpha.onValueChanged.AddListener(OnColorModifierValueChanged);
    }

	private void OnDestroy()
	{
        radialMovingUIController.OnMoving -= OnChoosingHue;
        saturation.onValueChanged.RemoveListener(OnColorModifierValueChanged);
        brightness.onValueChanged.RemoveListener(OnColorModifierValueChanged);
        alpha.onValueChanged.RemoveListener(OnColorModifierValueChanged);
    }

    private void UpdateColor()
	{
        var screenPoint = RectTransformUtility.WorldToScreenPoint(null, radialMovingUIController.WorldPosition);
        color = hueChoosingUIController.GetColor(screenPoint);

        Color.RGBToHSV(color, out float h, out float s, out float v);

        s = 1 - saturation.value;
        v = 1 - brightness.value;
        float a = 1 - alpha.value;

        color = Color.HSVToRGB(h, s, v);

        saturationImage.color = color;
        brightnessImage.color = color;
        alphaImage.color = color;

        color.a = a;

        colorPreviewImage.color = color;
	}

    private void OnChoosingHue()
    {
        UpdateColor();
    }

    public void OnColorModifierValueChanged(float value)
	{
        UpdateColor();
    }
}
