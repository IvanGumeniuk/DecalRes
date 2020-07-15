using System.Collections.Generic;
using UnityEngine;

public class SubcategoryUIOpener : MonoBehaviour
{
	public List<GameObject> views = new List<GameObject>();

	public void OnClick(int buttonIndex)
	{
		if(buttonIndex >= 0 && buttonIndex < views.Count)
		{
			IngameUIManager.Instance.customizationViewUIController.OpenView(views[buttonIndex]);
		}
	}
}
