using PaintIn3D.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalsPaintController : MonoBehaviour
{
    [SerializeField] private DecalManipulationController prefab;			// Decal prefab
	[SerializeField] private Transform staticDecalsHolder;					// Parent of inactive decals
    [SerializeField] private Transform dynamicDecalsHolder;					// Active decal`s parent
    [SerializeField] private List<DecalManipulationController> decal = new List<DecalManipulationController>();

    [SerializeField] private DecalManipulationController currentActive;		// Currently active decal
	[SerializeField] private CameraController cameraController;					

	[SerializeField] private bool isMoving;
	[SerializeField] private bool isRotatingAndScaling;

	private CustomizationManipulatorViewUIController manipulatorUIController;
	private StickerDecalUIController stickerDecalUIController;
	private DecalLayersUIController decalLayersUIController;

	private int maxPriority;												//This will be used as new decal initial priority

	private void Start()
    {
        manipulatorUIController = IngameUIManager.Instance.manipulatorViewUIController;
		stickerDecalUIController = IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController;
		decalLayersUIController = IngameUIManager.Instance.decalLayers;

		stickerDecalUIController.OnStickerChosen += OnNewDecalChosen;
        decalLayersUIController.OnLayerItemRemoved += OnLayerItemRemoved;
        decalLayersUIController.OnLayerItemSelected += OnLayerItemSelected;
		decalLayersUIController.OnLayerItemsPrioritiesChanged += OnLayerItemsPrioritiesChanged;

		manipulatorUIController.OnConfirmDecalPainting += OnConfirmDecalCreation;
    }

	private void OnDestroy()
	{
		if (stickerDecalUIController != null)
		{
			stickerDecalUIController.OnStickerChosen -= OnNewDecalChosen;
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
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			manipulatorUIController.SetCameraText("Free");
		}

		if (currentActive == null)
			return;
		
		// Update camera position and rotation for active decal
		currentActive.SetCameraTransformData(cameraController.Pitch, cameraController.Yaw);
		manipulatorUIController.SetReflectionText($"{currentActive.Reflected}");


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
			decal.Add(currentActive);
			currentActive.transform.SetParent(staticDecalsHolder);
        }
		else
		{
			if (decal.Contains(currentActive))
				decal.Remove(currentActive);

            Destroy(currentActive.gameObject);
		}

        currentActive = null;
		cameraController.ResetToDefaultPosition();
		manipulatorUIController.floatingUIController.SetTarget(null);

		Unsubscribe();
	}

	private void OnNewDecalChosen(Sprite decalSprite, int id)
	{
        if(currentActive == null)
		{
			currentActive = Instantiate(prefab, dynamicDecalsHolder);
			currentActive.SetID(id);
			currentActive.SetPriority(maxPriority + 1);
			cameraController.SetPivot(currentActive.HitPoint, true);
			manipulatorUIController.floatingUIController.SetTarget(currentActive.HitPoint);

			Subscribe();
		}

        currentActive.SetTexture(decalSprite.texture);
    }

	private void ChangeCurrent(int newItemID)
	{
		if (currentActive != null)
		{
			if (currentActive.id == newItemID)
				return;

			currentActive.transform.SetParent(staticDecalsHolder);
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
			StartCoroutine(SetParentWithDelay(dynamicDecalsHolder));

			Subscribe();
		}
		else
		{
			cameraController.SetPivot(null);
			manipulatorUIController.floatingUIController.SetTarget(null);
		}
	}

	private IEnumerator SetParentWithDelay(Transform parent)
	{
		yield return null;
		currentActive.transform.SetParent(parent);
	}

	private DecalManipulationController GetItemWithID(int id)
	{
		return decal.Find(x => x.id == id);
	}

	private void OnLayerItemSelected(int id)
	{
		if (currentActive != null && !decal.Contains(currentActive))
		{
			OnConfirmDecalCreation(false);
		}

		ChangeCurrent(id);
	}

	private void OnLayerItemRemoved(int id)
	{
		var decal = GetItemWithID(id);
		if(decal != null)
		{
			this.decal.Remove(decal);
			Destroy(decal.gameObject);
		}
	}

	private void OnLayerItemsPrioritiesChanged(Dictionary<int, int> priorities)
	{
		for (int i = 0; i < decal.Count; i++)
		{
			int newPriority = priorities[decal[i].id];
			decal[i].SetPriority(newPriority);

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

	private void OnReflection()
	{
		if (currentActive != null)
			currentActive.Reflect();
	}

	// Subscribing to UI decal`s manipulations events
	private void Subscribe()
	{
        manipulatorUIController.OnStartMoving += OnStartMoving;
        manipulatorUIController.OnStartRotationAndScaling += OnStartRotationAndScaling;
        manipulatorUIController.OnFinishMoving += OnFinishMoving;
        manipulatorUIController.OnFinishRotationAndScaling += OnFinishRotationAndScaling;
		manipulatorUIController.OnReflectionPressed += OnReflection;
	}

	private void Unsubscribe()
	{
        manipulatorUIController.OnStartMoving -= OnStartMoving;
        manipulatorUIController.OnStartRotationAndScaling -= OnStartRotationAndScaling;
        manipulatorUIController.OnFinishMoving -= OnFinishMoving;
        manipulatorUIController.OnFinishRotationAndScaling -= OnFinishRotationAndScaling;
		manipulatorUIController.OnReflectionPressed -= OnReflection;
	}
}
