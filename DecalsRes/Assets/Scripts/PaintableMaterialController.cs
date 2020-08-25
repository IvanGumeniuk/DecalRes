using PaintIn3D;
using System.Collections.Generic;
using UnityEngine;

public class PaintableMaterialController : MonoBehaviour
{
	private const string TEXTURE_PROPERTY_NAME = "_MainTex";

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

		//p3DMaterialCloner.Index = materials.Count - 1;
		p3DPaintableTexture.Slot = new P3dSlot(materials.Count - 1, p3DPaintableTexture.Slot.Name);
		p3DPaintableTexture.Coord = P3dCoord.Second;

		meshRenderer.materials = materials.ToArray();

		SetID(id);
	}

	public void SetID(int id)
	{
		this.id = id;
	}

	public void SetMaterialTexture(Texture texture)
	{
		Material material = new Material(paintableMaterial);
		material.SetTexture(TEXTURE_PROPERTY_NAME, texture);
		
		List<Material> materials = new List<Material>();
		meshRenderer.GetMaterials(materials);

		materials.RemoveAt(materials.Count - 1);
		materials.Add(material);

		meshRenderer.materials = materials.ToArray();
	}

	public byte[] Serialize()
	{
		return p3DPaintableTexture.GetPngData();
	}

	public void Deserialize(byte[] textureData)
	{
		Texture2D buffer = new Texture2D(1, 1);
		buffer.LoadImage(textureData);
		SetMaterialTexture(buffer);
	}
}
