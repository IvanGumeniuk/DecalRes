using UnityEngine;

public class FloatingUIComponentController : MonoBehaviour
{
    public Transform target;
	public Vector3 offset;
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
		if (target != null) self.position = mainCamera.WorldToScreenPoint(target.position + offset);
		else self.position = defaulPosition;
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
	}
}
