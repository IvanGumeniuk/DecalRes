using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PaintablePartsMaterialController : MonoBehaviour
{
    public List<PaintableMaterialController> paintableMaterialControllers = new List<PaintableMaterialController>();

	private Dictionary<int, byte[]> serializedData = new Dictionary<int, byte[]>(); // Send it via net <materialID, pngData>

	public void Initialize()
	{
		for (int i = 0; i < paintableMaterialControllers.Count; i++)
		{
			paintableMaterialControllers[i].InitializePaintableMaterial(i);
		}
	}

    public Dictionary<int, byte[]> Serialize()
	{
		serializedData.Clear();
		for (int i = 0; i < paintableMaterialControllers.Count; i++)
		{
			int id = i;
			byte[] data = paintableMaterialControllers[i].Serialize();

			serializedData.Add(id, data);

			string path = GetPath(i);
			File.WriteAllBytes(path, data);
		}

		return serializedData;
	}

	public void Deserialize()
	{
		for (int i = 0; i < paintableMaterialControllers.Count; i++)
		{
			string path = GetPath(i);
			byte[] data = File.ReadAllBytes(path);
			
			if(data != null)
				paintableMaterialControllers[i].Deserialize(data);
		}
	}

	public void Deserialize(Dictionary<int, byte[]> serializedData)
	{
		if(serializedData.Keys.Count != paintableMaterialControllers.Count)
		{
			Debug.Log($"Wrong data for deserialization");
			return;
		}

		for (int i = 0; i < paintableMaterialControllers.Count; i++)
		{
			paintableMaterialControllers[i].Deserialize(serializedData[i]);
		}
	}

	private string GetPath(int id)
	{
		return @"d:\MyTest_" + id + ".png";
	}
}
