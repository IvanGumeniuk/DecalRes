using PaintIn3D.Examples;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    [SerializeField] private P3dDragPitchYaw cameraController;

    [SerializeField] private List<CameraPositionData> cameraPositions = new List<CameraPositionData>();
    private CustomizationManipulatorViewUIController manipulatorView;

    [SerializeField] private CarSide carSide;
   
    private int currentlySelectedIndex = -1;
    private int Index
	{
		set
		{
            currentlySelectedIndex = value;

            if (currentlySelectedIndex >= cameraPositions.Count)
                currentlySelectedIndex = 0;

            carSide = (CarSide)currentlySelectedIndex;
        }

        get
		{
            return currentlySelectedIndex;
        }
	}
    private void Awake()
    {
        manipulatorView = IngameUIManager.Instance.manipulatorViewUIController;
        manipulatorView.OnCameraSidePressed += ChangeCarSide;
    }

    private void OnDestroy()
    {
        if (manipulatorView != null)
            manipulatorView.OnCameraSidePressed -= ChangeCarSide;
    }

    private void Update()
	{
        if(Application.isEditor && !Application.isPlaying)
		{
            cameraPositions.ForEach(x => x.Update());
		}

        if (Application.isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ChangeCarSide();
        }
    }

	private void ChangeCarSide()
	{
        Index++;
        manipulatorView.SetCameraText($"{carSide}");
        SetCameraPosition(cameraPositions[Index].vertical, cameraPositions[Index].horizontal);
    }

    public void SetCameraPosition(float vertical, float horizontal)
	{
        cameraController.Pitch = vertical;
        cameraController.Yaw = horizontal;
    }

    [Serializable]
    private class CameraPositionData
	{
        public string name;
        public CarSide side;
        public float vertical;
        public float horizontal;

        public void Update()
		{
            name = $"{side}";
        }
	}

    private enum CarSide
	{
        LeftDoor,
        RightDoor,
        Hood,
        Trunk,
        Roof
	}
}
