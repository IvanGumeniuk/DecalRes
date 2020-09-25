using PaintIn3D;
using UnityEngine;

public class FloatingUIComponentController : MonoBehaviour
{
    public Transform target;
	public Vector3 offset;
	public float dampening = 10;

	public RectTransform self;

	public Camera mainCamera;
	private Vector2 defaulPosition;

	private void Start()
	{
		self = GetComponent<RectTransform>();
		defaulPosition = self.position;
		mainCamera = Camera.main;
	}

	void Update()
    {
		if (target != null)
		{
			//var factor = dampening; // P3dHelper.DampenFactor(dampening, Time.deltaTime);

			//Vector3 position = Vector3.Lerp(self.position, mainCamera.WorldToScreenPoint(target.position + offset), factor);
			self.position = mainCamera.WorldToScreenPoint(target.position + offset);
		}
		else self.position = defaulPosition;
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
	}
}
