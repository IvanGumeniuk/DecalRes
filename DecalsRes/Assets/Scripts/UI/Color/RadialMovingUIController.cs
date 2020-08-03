using System;
using UnityEngine;

public class RadialMovingUIController : MonoBehaviour
{
    public Action OnMoving;

    [SerializeField] private Transform center;
    [SerializeField] private RectTransform handle;
    private float radius;

    public Vector3 WorldPosition { get; private set; }

	private void Start()
	{
        radius = Vector3.Distance(center.position, handle.position);
    }

	public void OnDrag()
	{
		Vector3 direction = (Input.mousePosition - center.position).normalized;
        WorldPosition = center.position + direction * radius; ;
        handle.position = WorldPosition;

        OnMoving?.Invoke();
    }
}
