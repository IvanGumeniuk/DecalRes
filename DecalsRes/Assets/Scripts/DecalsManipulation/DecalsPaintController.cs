using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalsPaintController : MonoBehaviour
{
    [SerializeField] private DecalManipulationController prefab;
    [SerializeField] private Transform staticDecalsHolder;
    [SerializeField] private Transform dynamicDecalsHolder;
    [SerializeField] private List<DecalManipulationController> decalPainters = new List<DecalManipulationController>();

    [SerializeField] private DecalManipulationController currectActive;
    
	[SerializeField] private bool isMoving;
	[SerializeField] private bool isRotatingAndScaling;

	private CustomizationManipulatorViewUIController manipulatorUIController;
	private StickerDecalUIController stickerDecalUIController;
	private DecalLayersUIController decalLayersUIController;

	private void Start()
    {
        manipulatorUIController = IngameUIManager.Instance.manipulatorViewUIController;
		stickerDecalUIController = IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController;
		decalLayersUIController = IngameUIManager.Instance.decalLayers;

		stickerDecalUIController.OnStickerClicked += OnNewDecalChosen;
        decalLayersUIController.OnLayerItemRemoved += OnLayerItemRemoved;
        decalLayersUIController.OnLayerItemSelected += OnLayerItemSelected;
		decalLayersUIController.OnLayerItemsPrioritiesChanged += OnLayerItemsPrioritiesChanged;

		manipulatorUIController.OnConfirmDecalPainting += OnConfirmDecalCreation;
    }

	private void OnDestroy()
	{
		if (stickerDecalUIController != null)
		{
			stickerDecalUIController.OnStickerClicked -= OnNewDecalChosen;
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
		if (isMoving)
		{
			Move(manipulatorUIController.difference);
		}

		if (isRotatingAndScaling)
		{
			MultiplyScale(manipulatorUIController.difference.x);
			AddAngle(manipulatorUIController.difference.y);
		}
	}

	private void OnConfirmDecalCreation(bool confirm)
	{
		if (currectActive == null)
			return;

		if (confirm)
		{
            currectActive.transform.SetParent(staticDecalsHolder);
        }
		else
		{
			if (decalPainters.Contains(currectActive))
				decalPainters.Remove(currectActive);

            Destroy(currectActive.gameObject);
		}

        currectActive = null;
		Unsubscribe();
	}

	private void OnNewDecalChosen(Sprite decalSprite, int id)
	{
		// Search decal with "id" in earlier created decal
		ChangeCurrent(id);

        if(currectActive == null)
		{
			currectActive = Instantiate(prefab, dynamicDecalsHolder);
			currectActive.SetID(id);

			decalPainters.Add(currectActive);

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
		}

		Unsubscribe();

		currectActive = GetItemWithID(newItemID);

		if (currectActive != null)
		{
			currectActive.transform.SetParent(dynamicDecalsHolder);
			Subscribe();
		}
	}

	private DecalManipulationController GetItemWithID(int id)
	{
		return decalPainters.Find(x => x.id == id);
	}

	private void OnLayerItemSelected(int id)
	{
		ChangeCurrent(id);
	}

	private void OnLayerItemRemoved(int id)
	{
		var decal = decalPainters.Find(x => x.id == id);
		if(decal != null)
		{
			decalPainters.Remove(decal);
			Destroy(decal.gameObject);
		}
	}

	private void OnLayerItemsPrioritiesChanged(Dictionary<int, int> priorities)
	{
		for (int i = 0; i < decalPainters.Count; i++)
		{
			decalPainters[i].SetPriority(priorities[decalPainters[i].id]);
		}
	}

	private void OnStartMoving()
	{
		isMoving = true;
		currectActive.OnStartMoving();
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
		isRotatingAndScaling = true;
		currectActive.OnStartRotationAndScaling();
	}

	private void AddAngle(float angle)
	{
		currectActive.AddAngle(angle);
	}

	private void MultiplyScale(float multiplier)
	{
		currectActive.Scale(multiplier);
	}

	private void OnFinishRotationAndScaling()
	{
		isRotatingAndScaling = false;
	}

	private void Subscribe()
	{
        manipulatorUIController.OnStartMoving += OnStartMoving;
        manipulatorUIController.OnStartRotationAndScaling += OnStartRotationAndScaling;
        manipulatorUIController.OnFinishMoving += OnFinishMoving;
        manipulatorUIController.OnFinishRotationAndScaling += OnFinishRotationAndScaling;
    }

    private void Unsubscribe()
	{
        manipulatorUIController.OnStartMoving -= OnStartMoving;
        manipulatorUIController.OnStartRotationAndScaling -= OnStartRotationAndScaling;
        manipulatorUIController.OnFinishMoving -= OnFinishMoving;
        manipulatorUIController.OnFinishRotationAndScaling -= OnFinishRotationAndScaling;
    }
}
