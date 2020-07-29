﻿using PaintIn3D.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    [SerializeField] private P3dDragPitchYaw cameraController;

    [SerializeField] private List<CameraPositionData> cameraPositions = new List<CameraPositionData>();

    [SerializeField] private CarSide carSide;

    private CustomizationManipulatorViewUIController manipulatorView;

    public Transform targetPivot;
    public Vector3 defaultPivot;
    public Camera mainCamera;

    public float Pitch { get { return cameraController.Pitch; }  private set { cameraController.Pitch = value; } }
    public float Yaw { get { return cameraController.Yaw; } private set { cameraController.Yaw = value; } }


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

        mainCamera = Camera.main;
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

    public void SetPivot(Transform target, bool updateCameraPositionWithDelay = false)
	{
        targetPivot = target;
        UpdateCameraPosition(updateCameraPositionWithDelay);
    }

    public void UpdateCameraPosition(bool withDelay = false)
	{
		if (withDelay)
		{
            StartCoroutine(UpdateCameraPosition());
            return;
		}

        if (targetPivot != null)
        {
            transform.position = targetPivot.position;
        }
        else
        {
            transform.position = defaultPivot;
        }
    }

    public void ResetToDefaultPosition(bool withDelay = false)
	{
        if (withDelay)
        {
            targetPivot = null;
            StartCoroutine(UpdateCameraPosition());
            return;
        }

        targetPivot = null;
        transform.position = defaultPivot;
    }

    private IEnumerator UpdateCameraPosition()
	{
        yield return null;
        yield return null;

        if (targetPivot != null)
        {
            transform.position = targetPivot.position;
        }
        else
        {
            transform.position = defaultPivot;
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
        Pitch = vertical;
        Yaw = horizontal;
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