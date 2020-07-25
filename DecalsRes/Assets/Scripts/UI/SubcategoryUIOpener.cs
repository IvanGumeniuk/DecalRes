using System.Collections.Generic;
using UnityEngine;

public class SubcategoryUIOpener : MonoBehaviour
{
	// Views can be opened
	public List<SubcategoryUIView> views = new List<SubcategoryUIView>();

	public void OnClick(int buttonIndex)
	{
		if (buttonIndex >= 0)
		{
			SubviewType subview = (SubviewType)buttonIndex;
			var view = views.Find(x => x.type == subview).gameObject;
			IngameUIManager.Instance.customizationViewUIController.OpenView(view, subview);
		}
	}	
}
