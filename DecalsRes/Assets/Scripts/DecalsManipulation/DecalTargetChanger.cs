using System.Collections.Generic;
using UnityEngine;

public class DecalTargetChanger : MonoBehaviour
{
    // List of decal`s target game objects
    public List<GameObject> targets = new List<GameObject>();
    public int activeIndex = 0;

    void Update()
    {
		/*if (Input.GetKeyDown(KeyCode.Alpha1))
		{
            activeIndex = 0;
            ChangeTarget();
        }

        /*if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeIndex = 1;
            ChangeTarget();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            activeIndex = 2;
            ChangeTarget();
        }*/
    }

	private void ChangeTarget()
	{
        IngameUIManager.Instance.decalsController.DeselectButtons();

        for (int i = 0; i < targets.Count; i++)
        {
            if (i != activeIndex)
            {
                targets[i].SetActive(false);
            }
            else
            {
                targets[i].SetActive(true);
            }
        }
    }
}
