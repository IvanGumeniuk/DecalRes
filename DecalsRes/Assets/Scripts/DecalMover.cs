using PaintIn3D;
using UnityEngine;

public class DecalMover : MonoBehaviour
{
	public P3dHitBetween decal;

	public Transform movableHitPoint;

	public Vector3 movableHitPointDefaultPosition;
	public Vector3 movableHitPointStartPosition;
	public float movingSpeed = 0.5f;


	private void Start()
	{
		if(decal == null)
			decal = GetComponent<P3dHitBetween>();

		movableHitPointDefaultPosition = movableHitPoint.localPosition;
	}

	public void StartMoving()
	{
		movableHitPointStartPosition = movableHitPoint.localPosition;
	}

	public void Move(Vector3 vector)
	{
		movableHitPoint.localPosition = movableHitPointStartPosition + vector * movingSpeed;
	}

	public void RevertToDefault()
	{
		movableHitPoint.localPosition = movableHitPointDefaultPosition;
	}
}
