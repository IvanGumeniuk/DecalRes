using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalsPaintController : MonoBehaviour
{
    [SerializeField] private DecalManipulationController prefab;			// Decal prefab
	[SerializeField] private Transform staticDecalsHolder;					// Parent of inactive decals
    [SerializeField] private Transform dynamicDecalsHolder;                 // Active decal`s parent
	[SerializeField] private Transform dynamicReflectedDecalsHolder;		// Active reflected decal`s parent
    [SerializeField] private List<DecalManipulationController> decals = new List<DecalManipulationController>();

	public DecalManipulationController _currentActive;
	public DecalManipulationController currentActive
	{ 
		get { return _currentActive; }
		set {
			_currentActive = value;
			if (_currentActive == null)
			{
				Debug.Log($"null");
			}
			else
			{
				Debug.Log($"{_currentActive.decalType}");
			}
		}
	}
	// Currently active decal
	[SerializeField] private CameraController cameraController;					

	[SerializeField] private bool isMoving;
	[SerializeField] private bool isRotatingAndScaling;

	private CustomizationManipulatorViewUIController manipulatorUIController;
	private DecalsUIController decalUIController;
	private DecalLayersUIController decalLayersUIController;
	private ColorPanelUIController colorPanelUIController;
	private CustomizationViewUIController customizationView;

	private int maxPriority;												//This will be used as new decal initial priority

	private void Start()
    {
        manipulatorUIController = IngameUIManager.Instance.manipulatorViewUIController;
		decalUIController = IngameUIManager.Instance.decalsController;
		decalLayersUIController = IngameUIManager.Instance.decalLayers;
		colorPanelUIController = IngameUIManager.Instance.colorPanelUIController;
		customizationView = IngameUIManager.Instance.customizationViewUIController;

		decalUIController.OnDecalCreated += OnNewDecalChosen;
        decalLayersUIController.OnLayerItemRemoved += OnLayerItemRemoved;
        decalLayersUIController.OnLayerItemSelected += OnLayerItemSelected;
		decalLayersUIController.OnLayerItemsPrioritiesChanged += OnLayerItemsPrioritiesChanged;

		manipulatorUIController.OnConfirmDecalPainting += OnConfirmDecalCreation;

		customizationView.OnViewOpened += OnViewOpened;

		cameraController.OnCameraPositionChanged += OnCameraPositionChanged;
    }

	private void OnDestroy()
	{
		if (decalUIController != null)
		{
			decalUIController.OnDecalCreated -= OnNewDecalChosen;
		}

		if (decalLayersUIController != null)
		{
			decalLayersUIController.OnLayerItemRemoved -= OnLayerItemRemoved;
			decalLayersUIController.OnLayerItemSelected -= OnLayerItemSelected;
			decalLayersUIController.OnLayerItemsPrioritiesChanged -= OnLayerItemsPrioritiesChanged;
		}

		if (manipulatorUIController != null)
		{
			manipulatorUIController.OnConfirmDecalPainting -= OnConfirmDecalCreation;
		}

		if (customizationView != null)
		{
			customizationView.OnViewOpened -= OnViewOpened;
		}

		if (cameraController != null)
		{
			cameraController.OnCameraPositionChanged -= OnCameraPositionChanged;
		}
	}

	private void Update()
	{
		UpdateDynamicReflectedHolderTransform();
		
		if (currentActive == null)
			return;

		// Update camera position and rotation for active decal
		currentActive.SetCameraTransformData(cameraController.Pitch, cameraController.Yaw);

		manipulatorUIController.SetFlipButtonStatus(currentActive.Flipped);
		manipulatorUIController.SetMirrorButtonStatus(currentActive.Reflected);

		currentActive.SetColor(colorPanelUIController.GetDecalColorDataContainer());
		
		if (isMoving)
		{
			Move(manipulatorUIController.difference);
		}
		else
		{
			currentActive.decalMover.UpdateRaycastPosition(cameraController.mainCamera.transform);
		}

		if (isRotatingAndScaling)
		{
			AddScale(manipulatorUIController.difference.x);
			AddAngle(manipulatorUIController.difference.y);
		}
	}

	private void UpdateDynamicReflectedHolderTransform()
	{
		dynamicReflectedDecalsHolder.localPosition = new Vector3(-cameraController.transform.localPosition.x, cameraController.transform.localPosition.y, cameraController.transform.localPosition.z);
		dynamicReflectedDecalsHolder.localRotation = Quaternion.Euler(new Vector3(cameraController.transform.localRotation.eulerAngles.x, -cameraController.transform.localRotation.eulerAngles.y, cameraController.transform.localRotation.eulerAngles.z));
	}

	private void OnConfirmDecalCreation(bool confirm)
	{
		if (currentActive == null)
		{
			cameraController.ResetToDefaultPosition();
			manipulatorUIController.floatingUIController.SetTarget(null);
			return;
		}
		

		if (confirm)
		{
			decals.Add(currentActive);
			currentActive.SetParent(staticDecalsHolder, staticDecalsHolder);
			decalLayersUIController.CreateLayerElement(currentActive.decalType, currentActive.id, currentActive.Texture);
		}
		else
		{
			if (decals.Contains(currentActive))
				decals.Remove(currentActive);

			Destroy(currentActive.gameObject);
		}

        currentActive = null;
		cameraController.ResetToDefaultPosition();
		manipulatorUIController.floatingUIController.SetTarget(null);
		colorPanelUIController.ResetValues();
		colorPanelUIController.gameObject.SetActive(false);

		Unsubscribe();
	}

	private void OnNewDecalChosen(DecalType decalType, int id, Texture texture)
	{
		if (currentActive == null)
		{
			currentActive = Instantiate(prefab, dynamicDecalsHolder);
			currentActive.SetIdentifier(decalType, id);
			currentActive.SetPriority(maxPriority + 1);
			cameraController.SetPivot(currentActive.HitPoint, true);
			manipulatorUIController.floatingUIController.SetTarget(currentActive.HitPoint);
			colorPanelUIController.gameObject.SetActive(true);

			Subscribe();
		}

		decalUIController.DecalChoosen(decalType, id);
		currentActive.SetTexture(texture);
    }

	private void ChangeCurrent(int newItemID)
	{
		if (currentActive != null)
		{
			if (currentActive.id == newItemID)
				return;

			currentActive.SetParent(staticDecalsHolder, staticDecalsHolder);
			Unsubscribe();
		}

		currentActive = GetItemWithID(newItemID); 

		if (currentActive != null)
		{
			float vertical = currentActive.verticalOffset == 0 ? cameraController.Pitch : currentActive.verticalOffset;
			float horizontal = currentActive.horizontalOffset == 0 ? cameraController.Yaw : currentActive.horizontalOffset;
			
			cameraController.SetPivot(currentActive.HitPoint);
			manipulatorUIController.floatingUIController.SetTarget(currentActive.HitPoint);
			cameraController.SetCameraPosition(vertical, horizontal);

			// Need a frame to update stored camera position and rotation
			StartCoroutine(SetParentWithDelay(dynamicDecalsHolder, dynamicReflectedDecalsHolder));
			colorPanelUIController.SetColorToPanel(currentActive.decalColorData);
			colorPanelUIController.gameObject.SetActive(true);
		
			decalUIController.DecalChoosen(currentActive.decalType, currentActive.id);

			Subscribe();
		}
		else
		{
			cameraController.SetPivot(null);
			manipulatorUIController.floatingUIController.SetTarget(null);
			colorPanelUIController.ResetValues();
			colorPanelUIController.gameObject.SetActive(false);

			decalUIController.DecalChoosen(DecalType.None, -1);
		}
	}

	private IEnumerator SetParentWithDelay(Transform parent, Transform reflectedParent)
	{
		yield return null;
		currentActive.SetParent(parent, reflectedParent);
	}

	private DecalManipulationController GetItemWithID(int id)
	{
		return decals.Find(x => x.id == id);
	}

	private void OnCameraPositionChanged()
	{
		if (currentActive != null)
		{
			currentActive.decalMover.UpdateRaycastPosition(cameraController.mainCamera.transform);
		}
	}

	private void OnViewOpened(SubviewType view, bool opened)
	{
		if(!decalLayersUIController.IsLayerSelected)
			OnConfirmDecalCreation(false);
	}

	private void OnLayerItemSelected(int id)
	{
		//Debug.Log($"OnLayerItemSelected");

		if (currentActive != null && !decals.Contains(currentActive))
		{
			//Debug.Log($"OnLayerItemSelected DESTROY");
			OnConfirmDecalCreation(false);
		}

		ChangeCurrent(id);
	}

	private void OnLayerItemRemoved(int id)
	{
		var decal = GetItemWithID(id);
		if(decal != null)
		{
			decalUIController.DecalRemoved(id);

			this.decals.Remove(decal);
			Destroy(decal.gameObject);
		}
	}

	private void OnLayerItemsPrioritiesChanged(Dictionary<int, int> priorities)
	{
		for (int i = 0; i < decals.Count; i++)
		{
			int newPriority = priorities[decals[i].id];
			decals[i].SetPriority(newPriority);

			// Update max priority. 
			maxPriority = Mathf.Max(newPriority, maxPriority);
		}
	}

	private void OnStartMoving()
	{
		if (currentActive != null)
		{
			isMoving = true;
			currentActive.OnStartMoving();
		}
	}

	private void Move(Vector3 vector)
	{
		currentActive.Move(vector);
	}

	private void OnFinishMoving()
	{
		isMoving = false;

		if (Vector3.Distance(currentActive.HitPoint.position, currentActive.decalMover.pointB.position) > 0.1f) 
			cameraController.UpdateCameraPosition();
	}

	private void OnStartRotationAndScaling()
	{
		if (currentActive != null)
		{
			isRotatingAndScaling = true;
			currentActive.OnStartRotationAndScaling();
		}
	}

	private void AddAngle(float angle)
	{
		currentActive.AddAngle(angle);
	}

	private void AddScale(float multiplier)
	{
		currentActive.Scale(multiplier);
	}

	private void OnFinishRotationAndScaling()
	{
		isRotatingAndScaling = false;
	}

	private void OnFlip()
	{
		if (currentActive != null)
			currentActive.Flip();
	}

	private void OnReflection()
	{
		if (currentActive != null)
		{
			if (!currentActive.Reflected)
			{
				currentActive.StartReflection(dynamicReflectedDecalsHolder);
			}
			else
			{
				currentActive.StopReflection();
			}
		}
	}

	// Subscribing to UI decal`s manipulations events
	private void Subscribe()
	{
        manipulatorUIController.OnStartMoving += OnStartMoving;
        manipulatorUIController.OnStartRotationAndScaling += OnStartRotationAndScaling;
        manipulatorUIController.OnFinishMoving += OnFinishMoving;
        manipulatorUIController.OnFinishRotationAndScaling += OnFinishRotationAndScaling;
		manipulatorUIController.OnFlipPressed += OnFlip;
		manipulatorUIController.OnMirrorPressed += OnReflection;
	}

	private void Unsubscribe()
	{
        manipulatorUIController.OnStartMoving -= OnStartMoving;
        manipulatorUIController.OnStartRotationAndScaling -= OnStartRotationAndScaling;
        manipulatorUIController.OnFinishMoving -= OnFinishMoving;
        manipulatorUIController.OnFinishRotationAndScaling -= OnFinishRotationAndScaling;
		manipulatorUIController.OnFlipPressed -= OnFlip;
		manipulatorUIController.OnMirrorPressed -= OnReflection;
	}
}
