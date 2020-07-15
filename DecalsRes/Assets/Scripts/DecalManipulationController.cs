using PaintIn3D;
using System.Collections;
using UnityEngine;

public class DecalManipulationController : MonoBehaviour
{
    public P3dHitBetween p3DHit;
    public P3dPaintDecal p3DPaint;
    public DecalRotator decalRotator;
    public DecalScaler decalScaler;
	public DecalMover decalMover;

    private CustomizationManipulatorViewUIController manipulatorUIController;

	[SerializeField] private bool isMoving;
	[SerializeField] private bool isRotatingAndScaling;

    private void Awake()
    {
        manipulatorUIController = IngameUIManager.Instance.manipulatorViewUIController;
		manipulatorUIController.OnConfirmDecalPainting += OnConfirmDecal;
		manipulatorUIController.OnStartMoving += OnStartMoving;
		manipulatorUIController.OnStartRotationAndScaling += OnStartRotationAndScaling;
		manipulatorUIController.OnFinishMoving += OnFinishMoving;
		manipulatorUIController.OnFinishRotationAndScaling += OnFinishRotationAndScaling;
	}

	private void OnDestroy()
	{
		manipulatorUIController.OnConfirmDecalPainting -= OnConfirmDecal;
		manipulatorUIController.OnStartMoving -= OnStartMoving;
		manipulatorUIController.OnStartRotationAndScaling -= OnStartRotationAndScaling;
		manipulatorUIController.OnFinishMoving -= OnFinishMoving;
		manipulatorUIController.OnFinishRotationAndScaling -= OnFinishRotationAndScaling;
	}

	private void OnConfirmDecal(bool confirm)
	{
		StartCoroutine(MakeDecal(confirm));
	}

	private IEnumerator MakeDecal(bool make)
	{
		if (make && p3DHit.gameObject.activeSelf)
		{
			p3DHit.MakeShot();
		}

		IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.DisableAllButtons();
		yield return null;

		decalMover.RevertToDefault();
		decalScaler.RevertToDefault();
		decalRotator.RevertToDefault();
	}

	private void OnStartMoving()
	{
		isMoving = true;
		decalMover.StartMoving();
	}

	private void OnFinishMoving()
	{
		isMoving = false;
	}

	private void OnStartRotationAndScaling()
	{
		isRotatingAndScaling = true;
		decalScaler.StartScaling();
		decalRotator.StartRotation();
	}

	private void OnFinishRotationAndScaling()
	{
		isRotatingAndScaling = false;
	}

	private void Update()
    {
		if (isMoving)
		{
			decalMover.Move(manipulatorUIController.difference);
		}

		if (isRotatingAndScaling)
		{
			decalScaler.Scale(manipulatorUIController.difference.x);
			decalRotator.Rotate(manipulatorUIController.difference.y);
		}
    }

}
