using System;
using UnityEngine;

public class RadialMovingUIController : MonoBehaviour
{
    public Action OnMoving;

    [SerializeField] private Transform center;
    [SerializeField] private RectTransform handle;
    [SerializeField] private RectTransform pointOnCircle;

    [SerializeField] private float radius;
    [SerializeField] private Vector3 defaultPosition;

    public Vector3 WorldPosition { get { return handle.position; } }

    public void Initialize()
	{
        radius = Vector3.Distance(center.position, handle.position);
        defaultPosition = handle.position;
    }

    public void SetHandlePosition(Vector3 position)
	{
        handle.position = position;

        OnMoving?.Invoke();
    }

    public void ResetPosition()
	{
        SetHandlePosition(defaultPosition);
    }

	public void OnDrag()
	{
		Vector3 direction = (Input.mousePosition - center.position).normalized;
        handle.position = center.position + direction * radius; 

        OnMoving?.Invoke();
    }
}
