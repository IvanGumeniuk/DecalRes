using System;
using UnityEngine;

public class RadialMovingUIController : MonoBehaviour
{
    public Action OnMoving;

    [SerializeField] private Transform center;
    [SerializeField] private RectTransform handle;
   
    [SerializeField] private float radius;
    [SerializeField] private Vector3 defaultPosition;

    public Vector3 WorldPosition { get; private set; }

	private void Awake()
	{
        radius = Vector3.Distance(center.position, handle.position);

        defaultPosition = handle.position;
    }

    public void SetHandlePosition(Vector3 position)
	{
        if (position == Vector3.zero)
            position = defaultPosition;

        WorldPosition = position;
        handle.position = WorldPosition;

        OnMoving?.Invoke();
    }

    public void ResetPosition()
	{
        if(defaultPosition != Vector3.zero)
            SetHandlePosition(defaultPosition);
    }

	public void OnDrag()
	{
		Vector3 direction = (Input.mousePosition - center.position).normalized;
        WorldPosition = center.position + direction * radius; 
        handle.position = WorldPosition;

        OnMoving?.Invoke();
    }
}
