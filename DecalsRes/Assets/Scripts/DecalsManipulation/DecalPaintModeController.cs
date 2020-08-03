using PaintIn3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalPaintModeController : MonoBehaviour
{
    public List<P3dPaintable> bodyParts = new List<P3dPaintable>();
    public List<P3dPaintable> windowsParts = new List<P3dPaintable>();
    private CustomizationManipulatorViewUIController manipulatorView;
    [SerializeField] private PaintTarget paintMode;
    private int currentlySelectedIndex = -1;
    private int Index
    {
        set
        {
            currentlySelectedIndex = value;

            if (currentlySelectedIndex >= 3)
                currentlySelectedIndex = 0;

            paintMode = (PaintTarget)currentlySelectedIndex;
        }

        get
        {
            return currentlySelectedIndex;
        }
    }

    private void Awake()
    {
        manipulatorView = IngameUIManager.Instance.manipulatorViewUIController;
        manipulatorView.OnPaintableTargetPressed += ChangeMode;
    }

    private void OnDestroy()
    {
        if(manipulatorView != null)
            manipulatorView.OnPaintableTargetPressed -= ChangeMode;
    }

    private IEnumerator Start()
	{
        yield return null;
        ChangeMode();
    }

    public void ChangeMode()
	{
        Index++;

        manipulatorView.SetPaintableTargetStatus(paintMode);

        switch (paintMode)
		{
			case PaintTarget.Body:
                {
                    bodyParts.ForEach(x => x.enabled = true);
                    windowsParts.ForEach(x => x.enabled = false);
                    break;
                }
			case PaintTarget.Windows:
                {
                    windowsParts.ForEach(x => x.enabled = true); 
                    bodyParts.ForEach(x => x.enabled = false);
                    break;
                }
			case PaintTarget.BodyAndWindows:
                {
                    windowsParts.ForEach(x => x.enabled = true);
                    bodyParts.ForEach(x => x.enabled = true); 
                    break;
                }
			default:
				break;
		}
	}

    public enum PaintTarget
	{
        Body,
        Windows,
        BodyAndWindows
	}
}
