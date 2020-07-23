using PaintIn3D;
using System;
using System.Collections;
using UnityEngine;

public class DecalManipulationController : MonoBehaviour
{
	public int id;

    public P3dHitBetween p3DHit;
    public P3dPaintDecal p3DPaint;

	public DecalRotator decalRotator;
    public DecalScaler decalScaler;
	public DecalMover decalMover;

	public void SetID(int id)
	{
		this.id = id;
	}

	public void SetPriority(int priority)
	{
		p3DHit.Priority = priority;
	}

	public void SetTexture(Texture texture)
	{
		p3DPaint.Texture = texture;
	}

	private void OnConfirm()
	{
		StartCoroutine(MakeDecal());
	}

	private IEnumerator MakeDecal()
	{
		p3DHit.MakeShot();

		IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.DisableAllButtons();
		yield return null;

		decalMover.RevertToDefault();
		decalScaler.RevertToDefault();
		decalRotator.RevertToDefault();
	}

	public void OnStartMoving()
	{
		decalMover.StartMoving();
	}

	public void Move(Vector3 vector)
	{
		decalMover.Move(vector);
	}

	public void OnStartRotationAndScaling()
	{
		decalScaler.StartScaling();
		decalRotator.StartRotation();
	}

	public void AddAngle(float angle)
	{
		decalRotator.AddRotationAngle(angle);
	}

	public void Scale(float multiplier)
	{
		decalScaler.AddScale(multiplier);
	}
}
