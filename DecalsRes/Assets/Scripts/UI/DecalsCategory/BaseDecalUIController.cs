using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseDecalUIController : MonoBehaviour
{
	public List<Button> buttons = new List<Button>();
	protected List<CanvasGroup> canvasGroups = new List<CanvasGroup>();

	protected DecalsUIController decalsUIController;

	public bool IsAnyButtonEnabled
	{
		get
		{
			for (int i = 0; i < canvasGroups.Count; i++)
			{
				if (Mathf.Approximately(canvasGroups[i].alpha, 1))
					return true;
			}

			return false;
		}
	}

	protected void Start()
	{
		decalsUIController = IngameUIManager.Instance.decalsController;

		for (int i = 0; i < buttons.Count; i++)
		{
			canvasGroups.Add(buttons[i].GetComponent<CanvasGroup>());
		}
	}

	public virtual void OnDecalButtonClick(int index)
	{
		if (index >= 0 && index < buttons.Count)
		{
			foreach (var group in canvasGroups)
				group.alpha = 0.5f;

			canvasGroups[index].alpha = 1;
		}
	}

	public void DeselectButtons()
	{
		foreach (var group in canvasGroups)
		{
			group.alpha = 0.5f;
		}
	}
}
