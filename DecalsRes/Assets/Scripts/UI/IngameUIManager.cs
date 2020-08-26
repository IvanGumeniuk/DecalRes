using UnityEngine;

public class IngameUIManager : Singleton<IngameUIManager>
{
    public CustomizationViewUIController customizationViewUIController;
    public CustomizationManipulatorViewUIController manipulatorViewUIController;
	public DecalsUIController decalsController;
    public DecalLayersUIController decalLayers;
	public ColorPanelUIController colorPanelUIController;

	public bool CanRotateCamera { get { return decalsController.paintingDecalUIController.CanRotateCamera; } }

	private void Start()
	{
		//Application.targetFrameRate = 60;
	}
}
