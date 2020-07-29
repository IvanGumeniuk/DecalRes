using PaintIn3D;
using System.Collections;
using UnityEngine;

public class DecalManipulationController : MonoBehaviour
{
	public int id;

	// Camera transform data. It stores to prevent wrong projection when other decal was selected, camera position and rotation changed and this object selected again
	public float verticalOffset;
	public float horizontalOffset;

    public P3dHitBetween p3DHitBetweenController;
    public P3dPaintDecal p3DPaintDecalController;
	
	public DecalRotator decalRotator;
    public DecalScaler decalScaler;
	public DecalMover decalMover;
	
	public bool Reflected { get { return decalScaler.Reflected; } }
	public Transform HitPoint { get { return p3DHitBetweenController.Point; } }


	public void SetCameraTransformData(float verticalOffset, float horizontalOffset)
	{
		this.verticalOffset = verticalOffset;
		this.horizontalOffset = horizontalOffset;
	}

	public void SetID(int id)
	{
		this.id = id;
	}

	public void SetPriority(int priority)
	{
		p3DHitBetweenController.Priority = priority;
	}

	public void SetTexture(Texture texture)
	{
		p3DPaintDecalController.Texture = texture;
	}

	private void OnConfirm()
	{
		StartCoroutine(MakeDecal());
	}

	private IEnumerator MakeDecal()
	{
		p3DHitBetweenController.MakeShot();

		IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.DeselectButtons();
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

	public void Reflect()
	{
		decalScaler.Reflect();
	}
}
