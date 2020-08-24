using PaintIn3D;
using UnityEngine;

public class ObjectDistanceChanger : MonoBehaviour
{
    [SerializeField] private float damping = 0.1f;
    [SerializeField] private Vector2 distanceClamp;

    [SerializeField] private Transform target;
    private float zoom;
    private float currentZoom;

	private void Start()
	{
        zoom = target.localPosition.z;
        currentZoom = zoom;
    }

    void Update()
    {
        zoom += Input.mouseScrollDelta.y;
        zoom = Mathf.Clamp(zoom, distanceClamp.x, distanceClamp.y);

        float lerpFactor = P3dHelper.DampenFactor(damping, Time.deltaTime);
        currentZoom = Mathf.Lerp(currentZoom, zoom, lerpFactor);

        target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, currentZoom);
    }
}
