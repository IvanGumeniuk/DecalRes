using PaintIn3D;
using UnityEngine;

public class DecalRotator : MonoBehaviour
{
	public P3dPaintDecal decal;
	public float defaultAngle;

	public float rotationSpeed = 0.75f;
	public float currentAngle = 1;
	public float startAngle = 1;

	public Vector2 angleClamp = new Vector2(-180, 180);

	private void Start()
	{
		if (decal == null)
			decal = GetComponent<P3dPaintDecal>();

		defaultAngle = decal.Angle;
	}

	public void StartRotation()
	{
		startAngle = decal.Angle;
		currentAngle = startAngle;
	}

	public void AddRotationAngle(float angle)
	{
		currentAngle = startAngle + angle * rotationSpeed;
		decal.Angle =  Mathf.Clamp(currentAngle, angleClamp.x, angleClamp.y);
	}

	public void SetAngle(float angle)
	{
		float currentAngle = defaultAngle + angle;
		decal.Angle = Mathf.Clamp(currentAngle, angleClamp.x, angleClamp.y);
	}

	public void RevertToDefault()
	{
		decal.Angle = defaultAngle;
	}
}
