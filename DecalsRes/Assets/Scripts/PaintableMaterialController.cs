using PaintIn3D;
using System.Collections.Generic;
using UnityEngine;

public class PaintableMaterialController : MonoBehaviour
{
    [SerializeField] private Material paintableMaterial;
    [SerializeField] private P3dMaterialCloner p3DMaterialCloner;
    [SerializeField] private P3dPaintableTexture p3DPaintableTexture;
    [SerializeField] private MeshRenderer meshRenderer;

	private void Awake()
	{
		InitializePaintableMaterial();
	}

	private void InitializePaintableMaterial()
	{
		meshRenderer = GetComponent<MeshRenderer>();
	
		if(meshRenderer == null || paintableMaterial == null || p3DMaterialCloner == null || p3DPaintableTexture == null)
		{
			Debug.Log($"[PaintableMaterialController] You have to set all references correctly");
		}
		
		List<Material> materials = new List<Material>(meshRenderer.materials);
		materials.Add(paintableMaterial);

		p3DMaterialCloner.Index = materials.Count - 1;
		p3DPaintableTexture.Slot = new P3dSlot(materials.Count - 1, p3DPaintableTexture.Slot.Name);
		p3DPaintableTexture.Coord = P3dCoord.Second;

		meshRenderer.materials = materials.ToArray();
	}

}
