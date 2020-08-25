using PaintIn3D;
using UnityEngine;

public class ObjectDistanceChanger : MonoBehaviour
{
    [SerializeField] private float damping = 0.1f;
    [SerializeField] private Vector2 distanceClamp;

    [SerializeField] private Transform target;
    private float zoom;
    private float currentZoom;

    [SerializeField] private float startDistance;
    [SerializeField] private float difference;
    [SerializeField] private float multiplier = 0.01f;

	private void Start()
	{
        zoom = target.localPosition.z;
        currentZoom = zoom;
    }

    void Update()
    {
        /*if (Application.isEditor)
        {
            zoom += Input.mouseScrollDelta.y;     
		}
		else*/
		{
            if(Input.touches.Length > 1)
			{
                var touches = Input.touches;
                difference = 0;
                if (Input.GetTouch(1).phase == TouchPhase.Began)
				{
                    startDistance = Vector3.Distance(touches[0].position, touches[1].position) * multiplier;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                {
					float distance = Vector3.Distance(touches[0].position, touches[1].position) * multiplier;
                    difference = distance - startDistance;
                    startDistance = distance;
                    zoom += Mathf.Clamp(difference, -1, 1);
                }
            }
		}

        zoom = Mathf.Clamp(zoom, distanceClamp.x, distanceClamp.y);

        float lerpFactor = P3dHelper.DampenFactor(damping, Time.deltaTime);
        currentZoom = Mathf.Lerp(currentZoom, zoom, lerpFactor);

        target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, currentZoom);
    }
}
