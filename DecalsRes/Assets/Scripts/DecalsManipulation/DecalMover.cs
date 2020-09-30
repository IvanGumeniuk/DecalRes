using PaintIn3D;
using UnityEngine;

public class DecalMover : MonoBehaviour
{
	public P3dHitBetween hitController;
	
	public Vector3 transformDefaultPosition;
	public Vector3 transformStartMovingPosition;

	public float movingSpeed = 0.5f;
	public float distanceBetweenPoints;
	
	private void Start()
	{
		if(hitController == null)
			hitController = GetComponent<P3dHitBetween>();

		transformDefaultPosition = transform.position;
		transformStartMovingPosition = transform.position;
	}

	public void StartMoving()
	{
		transformStartMovingPosition = transform.position;
	}

	public void UpdateRaycastPosition(Transform target, float cameraToTargetDistance)
	{
		transform.position = target.position + transform.forward * cameraToTargetDistance;
		transform.rotation = target.rotation;
	}

	public void Move(Vector3 vector)
	{
		transform.position = transformStartMovingPosition + vector * movingSpeed;
	}

	public void FinishMoving(bool followsCamera)
	{
		if (Vector3.Distance(hitController.Point.position, hitController.PointB.position) < 1)
		{
			if (followsCamera)
			{
				hitController.Point.position = transformStartMovingPosition;
			}
			else
			{
				transform.position = transformStartMovingPosition;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.DrawSphere(transformStartMovingPosition, 0.2f);
		}
	}

	public void RevertToDefault()
	{
		transform.position = transformDefaultPosition;
	}
}
