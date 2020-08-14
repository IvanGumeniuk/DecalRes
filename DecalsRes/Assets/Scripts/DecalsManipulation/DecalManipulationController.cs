using PaintIn3D;
using System.Collections;
using UnityEngine;

public class DecalManipulationController : MonoBehaviour
{
	public int id;
	public DecalType decalType;

	// Camera transform data. It stores to prevent wrong projection when other decal was selected, camera position and rotation changed and this object selected again
	public float verticalOffset;
	public float horizontalOffset;

    public P3dHitBetween p3DHitBetweenController;
    public P3dPaintDecal p3DPaintDecalController;
	
	public DecalRotator decalRotator;
    public DecalScaler decalScaler;
	public DecalMover decalMover;
	public DecalColorDataContainer decalColorData;
	public DecalManipulationController reflected;

	public bool Active { get; set; }
	public bool Reflected { get { return reflected != null; } }
	public bool Flipped { get { return decalScaler.Flipped; } }

	public Transform HitPoint { get { return p3DHitBetweenController.Point; } }

	public Texture Texture { get { return p3DPaintDecalController.Texture; } }

	public void SetCameraTransformData(float verticalOffset, float horizontalOffset)
	{
		this.verticalOffset = verticalOffset;
		this.horizontalOffset = horizontalOffset;
	}

	public void SetIdentifier(DecalType decalType, int id)
	{
		this.decalType = decalType;
		this.id = id;
	}

	public void SetColor(DecalColorDataContainer decalColorData)
	{
		p3DPaintDecalController.Color = decalColorData.rgbColor;

		if(Reflected)
			reflected.SetColor(decalColorData);

		this.decalColorData = decalColorData;
	}

	public void SetPriority(int priority)
	{
		p3DHitBetweenController.Priority = priority;
		if (Reflected)
			reflected.SetPriority(priority);
	}

	public void SetTexture(Texture texture)
	{
		p3DPaintDecalController.Texture = texture; 
		if (Reflected)
			reflected.SetTexture(texture);
	}

	private void OnConfirm()
	{
		StartCoroutine(MakeDecal());
	}

	private IEnumerator MakeDecal()
	{
		p3DHitBetweenController.MakeShot();

		IngameUIManager.Instance.decalsController.DeselectButtons();
		yield return null;

		decalMover.RevertToDefault();
		decalScaler.RevertToDefault();
		decalRotator.RevertToDefault();
	}

	private void Update()
	{
		if (Reflected)
			UpdateReflection();

		/*if (Input.GetKeyDown(KeyCode.Space))
		{
			p3DHitBetweenController.MakeShot();
		}*/
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

	public void Flip()
	{
		decalScaler.Flip();
	}

	public void StartReflection(Transform reflectedDecalsHolder)
	{
		reflected = Instantiate(gameObject, reflectedDecalsHolder).GetComponent<DecalManipulationController>();
		reflected.gameObject.name = $"{gameObject.name}_Reflected";

		UpdateReflection();
	}

	private void UpdateReflection()
	{
		reflected.transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
		reflected.transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, -transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z));

		reflected.decalScaler.SetScale(new Vector3(-decalScaler.Scale.x, decalScaler.Scale.y, decalScaler.Scale.z));
		reflected.decalRotator.SetAngle(-decalRotator.Angle);

		//reflected.decalMover.pointB.localPosition =  decalMover.pointB.localPosition;
	}

	public void StopReflection()
	{
		if (Reflected)
		{
			Destroy(reflected.gameObject);
			reflected = null;
		}
	}

	public void SetParent(Transform parent, Transform reflectedParent)
	{
		transform.parent = parent;
		if (Reflected)
		{
			reflected.transform.parent = reflectedParent;
		}
	}

	private void OnDestroy()
	{
		StopReflection();
	}
}
