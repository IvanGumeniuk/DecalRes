using PaintIn3D;
using System.Collections.Generic;
using UnityEngine;

public class PaintableMaterialController : MonoBehaviour
{
	[SerializeField] private int id;
    [SerializeField] private Material paintableMaterial;
    [SerializeField] private P3dMaterialCloner p3DMaterialCloner;
    [SerializeField] private P3dPaintableTexture p3DPaintableTexture;
    [SerializeField] private MeshRenderer meshRenderer;

	public void InitializePaintableMaterial(int id)
	{
		meshRenderer = GetComponent<MeshRenderer>();
			
		List<Material> materials = new List<Material>(meshRenderer.materials);
		Material material = new Material(paintableMaterial);
		materials.Add(material);

		p3DPaintableTexture.Slot = new P3dSlot(materials.Count - 1, p3DPaintableTexture.Slot.Name);
		p3DPaintableTexture.Coord = P3dCoord.Second;

		meshRenderer.materials = materials.ToArray();

		SetID(id);
	}

	public void SetID(int id)
	{
		this.id = id;
	}

	public byte[] Serialize(string name)
	{
		p3DPaintableTexture.Save(name + id);
		return p3DPaintableTexture.GetPngData();
	}

	public void Deserialize(byte[] textureData)
	{
		p3DPaintableTexture.LoadFromData(textureData);

	}
}
