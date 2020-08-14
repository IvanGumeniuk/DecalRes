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

        UpdateColor();
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

        s = saturation.value;
        v = 1 - brightness.value;
        float a = 1 - alpha.value;

        saturationImage.color = Color.HSVToRGB(h, 1, v);
        brightnessImage.color = Color.HSVToRGB(h, s, 1);

        color = Color.HSVToRGB(h, s, v);
        alphaImage.color = color;

        color.a = a;

        colorPreviewImage.color = color;
	}

    public void SetColorToPanel(DecalColorDataContainer decalColorData)
	{
        radialMovingUIController.SetHandlePosition(decalColorData.hueHandleValue);
        saturation.value = decalColorData.saturationSliderValue;
        brightness.value = decalColorData.brightnessSliderValue;
        alpha.value = decalColorData.alphaSliderValue;
        UpdateColor();
    }

    public DecalColorDataContainer GetDecalColorDataContainer()
	{
        DecalColorDataContainer container = new DecalColorDataContainer()
        {
            hueHandleValue = radialMovingUIController.WorldPosition,
            saturationSliderValue = saturation.value,
            brightnessSliderValue = brightness.value,
            alphaSliderValue = alpha.value,
            rgbColor = color
        };

        return container;
    }

    public void ResetValues()
	{
        radialMovingUIController.ResetPosition();
        saturation.value = 0;
        brightness.value = 0;
        alpha.value = 0;
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
