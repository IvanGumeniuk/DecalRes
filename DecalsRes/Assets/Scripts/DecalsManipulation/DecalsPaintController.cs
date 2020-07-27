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

    [SerializeField] private DecalManipulationController currectActive;		// Currently active decal
	[SerializeField] private P3dDragPitchYaw cameraPivot;					// Camera controller

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

		if (currectActive == null)
			return;
		
		// Update camera position and rotation for active decal
		currectActive.SetCameraTransformData(cameraPivot.Pitch, cameraPivot.Yaw);
		manipulatorUIController.SetReflectionText($"{currectActive.Reflected}");

		if (isMoving)
		{
			Move(manipulatorUIController.difference);
		}

		if (isRotatingAndScaling)
		{
			AddScale(manipulatorUIController.difference.x);
			AddAngle(manipulatorUIController.difference.y);
		}
	}

	private void OnConfirmDecalCreation(bool confirm)
	{
		if (currectActive == null)
			return;

		if (confirm)
		{
			decal.Add(currectActive);
			currectActive.transform.SetParent(staticDecalsHolder);
        }
		else
		{
			if (decal.Contains(currectActive))
				decal.Remove(currectActive);

            Destroy(currectActive.gameObject);
		}

        currectActive = null;
		Unsubscribe();
	}

	private void OnNewDecalChosen(Sprite decalSprite, int id)
	{
        if(currectActive == null)
		{
			currectActive = Instantiate(prefab, dynamicDecalsHolder);
			currectActive.SetID(id);
			currectActive.SetPriority(maxPriority + 1);

			Subscribe();
		}

        currectActive.SetTexture(decalSprite.texture);
    }

	private void ChangeCurrent(int newItemID)
	{
		if (currectActive != null)
		{
			if (currectActive.id == newItemID)
				return;

			currectActive.transform.SetParent(staticDecalsHolder);
			Unsubscribe();
		}

		currectActive = GetItemWithID(newItemID);

		if (currectActive != null)
		{
			cameraPivot.Pitch = currectActive.verticalOffset == 0 ? cameraPivot.Pitch : currectActive.verticalOffset;
			cameraPivot.Yaw = currectActive.horizontalOffset == 0 ? cameraPivot.Yaw : currectActive.horizontalOffset; ;

			// Need a frame to update stored camera position and rotation
			StartCoroutine(SetParentWithDelay(dynamicDecalsHolder));

			Subscribe();
		}
	}

	private IEnumerator SetParentWithDelay(Transform parent)
	{
		yield return null;
		currectActive.transform.SetParent(parent);
	}

	private DecalManipulationController GetItemWithID(int id)
	{
		return decal.Find(x => x.id == id);
	}

	private void OnLayerItemSelected(int id)
	{
		if (currectActive != null && !decal.Contains(currectActive))
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
		if (currectActive != null)
		{
			isMoving = true;
			currectActive.OnStartMoving();
		}
	}

	private void Move(Vector3 vector)
	{
		currectActive.Move(vector);
	}

	private void OnFinishMoving()
	{
		isMoving = false;
	}

	private void OnStartRotationAndScaling()
	{
		if (currectActive != null)
		{
			isRotatingAndScaling = true;
			currectActive.OnStartRotationAndScaling();
		}
	}

	private void AddAngle(float angle)
	{
		currectActive.AddAngle(angle);
	}

	private void AddScale(float multiplier)
	{
		currectActive.Scale(multiplier);
	}

	private void OnFinishRotationAndScaling()
	{
		isRotatingAndScaling = false;
	}

	private void OnReflection()
	{
		if (currectActive != null)
			currectActive.Reflect();
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
