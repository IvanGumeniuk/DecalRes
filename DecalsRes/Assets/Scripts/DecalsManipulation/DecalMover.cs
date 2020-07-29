using PaintIn3D;
using UnityEngine;

public class DecalMover : MonoBehaviour
{
	public P3dHitBetween hitController;

	public Transform pointA;
	public Transform pointB;

	public Vector3 pointADefaultPosition;
	public Vector3 pointAStartPosition;

	public Vector3 pointBDefaultPosition;
	public Vector3 pointBStartPosition;

	public float movingSpeed = 0.5f;
	public float distanceBetweenPoints;

	private void Start()
	{
		if(hitController == null)
			hitController = GetComponent<P3dHitBetween>();

		pointADefaultPosition = pointA.localPosition;
		pointBDefaultPosition = pointB.localPosition;
	}

	public void StartMoving()
	{
		pointAStartPosition = pointA.localPosition;
		pointBStartPosition = pointB.localPosition;
	}

	public void UpdateRaycastPosition(Transform target)
	{
		pointA.position = target.position;
		pointA.rotation = target.rotation;

		pointB.position = pointA.position + pointA.forward * distanceBetweenPoints;
		pointB.rotation = target.rotation;
	}

	public Vector3 movingVector;

	public void Move(Vector3 vector)
	{
		movingVector = vector;
		pointA.localPosition = pointAStartPosition + vector * movingSpeed;
		pointB.localPosition = pointBStartPosition + vector * movingSpeed;
	}

	public void RevertToDefault()
	{
		pointA.localPosition = pointADefaultPosition;
		pointB.localPosition = pointBDefaultPosition;
	}
}
