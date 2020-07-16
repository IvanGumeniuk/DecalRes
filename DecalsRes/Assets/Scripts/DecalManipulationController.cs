using PaintIn3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalManipulationController : MonoBehaviour
{
    public P3dHitBetween p3DHit;
    public P3dPaintDecal p3DPaint;
    public P3dPaintableTexture p3DPaintableTexture;
	public DecalRotator decalRotator;
    public DecalScaler decalScaler;
	public DecalMover decalMover;

	[SerializeField] private List<ShotData> shots = new List<ShotData>();

	public int oldLayer;
	public int newLayer;

    private CustomizationManipulatorViewUIController manipulatorUIController;

	[SerializeField] private bool isMoving;
	[SerializeField] private bool isRotatingAndScaling;

	[ContextMenu("ChangeLayer")]
	public void ChangeLayer()
	{
		//p3DPaintableTexture.ChangeLayer(oldLayer, newLayer);
		

		StartCoroutine(Try());

	}

	private IEnumerator Try()
	{
		bool wasEnabled = true;
		if (!p3DPaint.gameObject.activeSelf)
		{
			wasEnabled = false;
			p3DPaint.gameObject.SetActive(true);
		}

		for (int i = 0; i < shots.Count; i++)
		{
			P3dStateManager.UndoAll();
		}

		P3dStateManager.ClearAllStates();
		
		Texture current = p3DPaint.Texture;
		
		for (int i = shots.Count - 1; i >= 0; i--)
		{
			p3DPaint.Texture = shots[i].texture;
			decalScaler.SetScale(shots[i].size);
			decalRotator.SetAngle(shots[i].angle);
			
			yield return null;

			p3DHit.MakeShot(shots[i].pointA, shots[i].pointB);

			//yield return new WaitForSeconds(1);
		}

		p3DPaint.Texture = current;
		if (!wasEnabled)
		{
			p3DPaint.gameObject.SetActive(false);
		}
	}

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
			shots.Add(new ShotData() 
			{
				pointA = p3DHit.PointA.position, 
				pointB = p3DHit.PointB.position,
				angle = decalRotator.currentAngle,
				size = decalScaler.currentScale,
				texture = p3DPaint.Texture
			});

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
			decalScaler.AddScale(manipulatorUIController.difference.x);
			decalRotator.AddRotationAngle(manipulatorUIController.difference.y);
		}
    }

	[System.Serializable]
	private class ShotData
	{
		public Vector3 pointA;
		public Vector3 pointB;
		public float angle;
		public float size;
		public Texture texture;
	}

}
