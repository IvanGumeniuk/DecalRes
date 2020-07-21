using PaintIn3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalScaler : MonoBehaviour
{
    public P3dPaintDecal decal;
    public Vector3 defaultScale;
  
    public float scaleSpeed = 1;
    public float currentScale = 1;
    public float startScale = 1;
	public Vector2 scaleClamp = new Vector2(0.05f, 10);

	private void Start()
	{
		decal = GetComponent<P3dPaintDecal>();
		defaultScale = decal.Scale;
	}

	public void StartScaling()
	{
		startScale = decal.Scale.x / defaultScale.x;
		currentScale = startScale;
	}

	public void AddScale(float scale)
	{
		currentScale = startScale + scale * scaleSpeed;
		currentScale = Mathf.Clamp(currentScale, scaleClamp.x, scaleClamp.y);
		decal.Scale = defaultScale * currentScale;
	}

	public void SetScale(float scale)
	{
		decal.Scale = defaultScale * scale;
	}

	public void RevertToDefault()
	{
		decal.Scale = defaultScale;
	}
}
