using UnityEngine;

public class IngameUIManager : Singleton<IngameUIManager>
{
    public CustomizationViewUIController customizationViewUIController;
    public CustomizationManipulatorViewUIController manipulatorViewUIController;
    public DecalLayersUIController decalLayers;

	private void Start()
	{
		Application.targetFrameRate = 60;
	}
}
