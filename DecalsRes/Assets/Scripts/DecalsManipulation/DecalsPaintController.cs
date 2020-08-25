using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DecalsPaintController : MonoBehaviour
{
    [SerializeField] private DecalManipulationController prefab;			// Decal prefab
    [SerializeField] private List<DecalManipulationController> decals = new List<DecalManipulationController>();

	// Currently active decal
	public DecalManipulationController currentActive;

	[SerializeField] private CameraController cameraController;
	[SerializeField] private PaintablePartsMaterialController paintablePartMaterialController;

	[SerializeField] private bool isMoving;
	[SerializeField] private bool isRotatingAndScaling;

	private CustomizationManipulatorViewUIController manipulatorUIController;
	private DecalsUIController decalUIController;
	private DecalLayersUIController decalLayersUIController;
	private ColorPanelUIController colorPanelUIController;
	private CustomizationViewUIController customizationView;

	private int maxPriority;                                                //This will be used as new decal initial priority

	private DecalHoldersManager decalHolders;

	private void Awake()
	{
		paintablePartMaterialController.Initialize();
	}

	private void Start()
	{
		decalHolders = DecalHoldersManager.Instance;

		manipulatorUIController = IngameUIManager.Instance.manipulatorViewUIController;
		decalUIController = IngameUIManager.Instance.decalsController;
		decalLayersUIController = IngameUIManager.Instance.decalLayers;
		colorPanelUIController = IngameUIManager.Instance.colorPanelUIController;
		customizationView = IngameUIManager.Instance.customizationViewUIController;

		decalUIController.OnDecalCreated += OnNewDecalChosen;
		decalUIController.OnTextDecalCreated += OnTextDecalCreationConfimed;

		decalLayersUIController.OnLayerItemRemoved += OnLayerItemRemoved;
		decalLayersUIController.OnLayerItemSelected += OnLayerItemSelected;
		decalLayersUIController.OnLayerItemsPrioritiesChanged += OnLayerItemsPrioritiesChanged;

		manipulatorUIController.OnConfirmDecalPainting += OnConfirmDecalCreation;

		customizationView.OnViewOpened += OnViewOpened;
		customizationView.OnSaveClicked += OnSaveButtonPressed;
		customizationView.OnLoadClicked += OnLoadButtonPressed;

		cameraController.OnCameraPositionChanged += OnCameraPositionChanged;
	}

	private void OnDestroy()
	{
		if (decalUIController != null)
		{
			decalUIController.OnDecalCreated -= OnNewDecalChosen;
			decalUIController.OnTextDecalCreated -= OnTextDecalCreationConfimed;
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
			customizationView.OnSaveClicked -= OnSaveButtonPressed;
			customizationView.OnLoadClicked -= OnLoadButtonPressed;
		}

		if (cameraController != null)
		{
			cameraController.OnCameraPositionChanged -= OnCameraPositionChanged;
		}
	}


	private void LocalSerialization()
	{
		List<string> buffer = new List<string>();

		for (int i = 0; i < decals.Count; i++)
		{
			buffer.Add(decals[i].Serialize());
		}

		SerializableDecalsData decalsData = new SerializableDecalsData(0, buffer.ToArray());

		string path = Application.persistentDataPath + "/DecalsData.txt";
		File.WriteAllText(path, decalsData.Serialize());

		paintablePartMaterialController.Serialize();
	}

	private IEnumerator LocalDeserialization()
	{
		string path = Application.persistentDataPath + "/DecalsData.txt";
		string deserialized = File.ReadAllText(path);

		if (deserialized != null && deserialized.Length > 0)
		{
			SerializableDecalsData.Deserialize(deserialized, out int vehicleID, out string[] data);

			ClearDecals();
			decalLayersUIController.ClearLayers();

			if (data.Length != 0)
			{
				for (int i = 0; i < data.Length; i++)
				{
					var decal = Instantiate(prefab, decalHolders.staticDecalsHolder);
					decals.Add(decal);

					yield return null;

					decal.Deserialize(data[i]);
					decalLayersUIController.CreateLayerElement(decal.decalType, decal.id, decal.DecalTexture);
				}
			}
		}

		paintablePartMaterialController.Deserialize();
	}
	private IEnumerator GlobalSerialization()
	{
		decals.ForEach(x => x.FinishDecalCustomization(true));
		
		yield return null;

		paintablePartMaterialController.Serialize();
	}

	private void GlobalDeserialization()
	{
		paintablePartMaterialController.Deserialize();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			LocalSerialization();
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			StartCoroutine(LocalDeserialization());
		}

		UpdateDynamicReflectedHolderTransform();

		if (currentActive == null)		
			return;
		

		// Update camera position and rotation for active decal
		currentActive.SetCameraTransformData(cameraController.Pitch, cameraController.Yaw);

		manipulatorUIController.SetFlipButtonStatus(currentActive.Flipped);
		manipulatorUIController.SetMirrorButtonStatus(currentActive.Reflected);
		manipulatorUIController.SetCameraFollowStatus(currentActive.FollowsCamera);
		manipulatorUIController.SetPaintableTargetStatus(currentActive.decalTarget);

		currentActive.SetColor(colorPanelUIController.GetDecalColorDataContainer());
		
		if (isMoving)
		{
			Move(manipulatorUIController.difference);
		}
		else
		{
			if(currentActive.FollowsCamera)
				currentActive.decalMover.UpdateRaycastPosition(cameraController.mainCamera.transform, cameraController.DistanceToPivot);
		}

		if (isRotatingAndScaling)
		{
			AddScale(manipulatorUIController.difference.x);
			AddAngle(manipulatorUIController.difference.y);
		}
	}

	public void ClearDecals()
	{
		int count = decals.Count;

		for (int i = count - 1; i >= 0; i--)
		{
			Destroy(decals[i].gameObject);
			decals.RemoveAt(i);
		}
	}

	private void UpdateDynamicReflectedHolderTransform()
	{
		decalHolders.dynamicReflectedDecalsHolder.localPosition = new Vector3(-cameraController.transform.localPosition.x, cameraController.transform.localPosition.y, cameraController.transform.localPosition.z);
		decalHolders.dynamicReflectedDecalsHolder.localRotation = Quaternion.Euler(new Vector3(cameraController.transform.localRotation.eulerAngles.x, -cameraController.transform.localRotation.eulerAngles.y, cameraController.transform.localRotation.eulerAngles.z));
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
			currentActive.SetParent(decalHolders.staticDecalsHolder, decalHolders.staticDecalsHolder);
			decalLayersUIController.CreateLayerElement(currentActive.decalType, currentActive.id, currentActive.DecalTexture);
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

	private void OnTextDecalCreationConfimed(int decalID, int fontID, string text)
	{
		currentActive.SetText(fontID, text);
	}


	private void OnNewDecalChosen(DecalType decalType, int id, int textureID, Texture texture)
	{
		if (currentActive == null)
		{
			currentActive = Instantiate(prefab, decalHolders.dynamicDecalsHolder);
			currentActive.SetIdentifier(decalType, id);
			currentActive.SetPriority(maxPriority + 1);
			
			cameraController.SetPivot(currentActive.HitPoint, true);
			manipulatorUIController.floatingUIController.SetTarget(currentActive.HitPoint);
			colorPanelUIController.gameObject.SetActive(true);

			Subscribe();
		}

		decalUIController.DecalChoosen(decalType, id);
		
		if(decalType == DecalType.Sticker)
			currentActive.SetTexture(textureID);
		else
			currentActive.SetTexture(texture);
	}

	private void ChangeCurrent(int newItemID)
	{
		if (currentActive != null)
		{
			if (currentActive.id == newItemID)
				return;

			currentActive.SetParent(decalHolders.staticDecalsHolder, decalHolders.staticDecalsHolder);
			Unsubscribe();
		}

		currentActive = GetItemWithID(newItemID); 

		if (currentActive != null)
		{
			float vertical;
			float horizontal;

			if (currentActive.FollowsCamera)
			{
				vertical = currentActive.verticalOffset == 0 ? cameraController.Pitch : currentActive.verticalOffset;
				horizontal = currentActive.horizontalOffset == 0 ? cameraController.Yaw : currentActive.horizontalOffset; ;
			}
			else
			{
				vertical = currentActive.followCameraStoredPitch;
				horizontal = currentActive.followCameraStoredYaw;
			}

			cameraController.SetPivot(currentActive.HitPoint);
			manipulatorUIController.floatingUIController.SetTarget(currentActive.HitPoint);
			cameraController.SetCameraPosition(vertical, horizontal);

			// Need a frame to update stored camera position and rotation
			if (currentActive.FollowsCamera)
			{
				StartCoroutine(SetParentWithDelay(decalHolders.dynamicDecalsHolder, decalHolders.dynamicReflectedDecalsHolder));
			}
			else
			{
				StartCoroutine(SetParentWithDelay(decalHolders.staticDecalsHolder, decalHolders.staticDecalsHolder));
			}

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
		if (currentActive != null && currentActive.FollowsCamera)
		{
			currentActive.decalMover.UpdateRaycastPosition(cameraController.mainCamera.transform, cameraController.DistanceToPivot);
		}
	}

	private void OnViewOpened(SubviewType view, bool opened)
	{
		if(!decalLayersUIController.IsLayerSelected || view == SubviewType.CustomPainting)
			OnConfirmDecalCreation(false);
	}

	private void OnLayerItemSelected(int id)
	{
		if (currentActive != null && !decals.Contains(currentActive))
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

	private void ChangeTarget()
	{
		if (currentActive == null)
			return;

		currentActive.NextTarget();
	}

	private void ChangeCameraFollowing()
	{
		if (currentActive == null)
			return;

		if (currentActive.FollowsCamera)
		{
			currentActive.StopFollowCamera(cameraController.Pitch, cameraController.Yaw);
			currentActive.FollowsCamera = false;

			currentActive.SetParent(decalHolders.staticDecalsHolder, decalHolders.staticDecalsHolder);
			cameraController.SetPivot(null);
		}
		else
		{
			cameraController.SetPivot(currentActive.HitPoint);

			currentActive.StartFollowCamera(out float pitch, out float yaw);
			currentActive.FollowsCamera = true;

			cameraController.SetCameraPosition(pitch, yaw);

			StartCoroutine(SetParentWithDelay(decalHolders.dynamicDecalsHolder, decalHolders.dynamicReflectedDecalsHolder));
		}
	}


	private void OnStartMoving()
	{
		if (currentActive != null)
		{
			isMoving = true;
			currentActive.OnStartMoving();

			if (!currentActive.FollowsCamera)
			{
				currentActive.SetParent(decalHolders.dynamicDecalsHolder, decalHolders.dynamicReflectedDecalsHolder);

				currentActive.decalMover.UpdateRaycastPosition(cameraController.mainCamera.transform, cameraController.DistanceToPivot);

				currentActive.StopFollowCamera(cameraController.Pitch, cameraController.Yaw);
				currentActive.SetParent(decalHolders.staticDecalsHolder, decalHolders.staticDecalsHolder);
			}
		}
	}

	private void Move(Vector3 vector)
	{
		currentActive.Move(cameraController.transform.TransformVector(vector));
	}

	private void OnFinishMoving()
	{
		isMoving = false;
		currentActive.FinishMoving(currentActive.FollowsCamera);

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
				Transform reflectionParent = currentActive.FollowsCamera ? decalHolders.dynamicReflectedDecalsHolder : decalHolders.staticDecalsHolder;

				currentActive.StartReflection(reflectionParent);
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
		manipulatorUIController.OnCameraFollowPressed += ChangeCameraFollowing;
		manipulatorUIController.OnPaintableTargetPressed += ChangeTarget;

	}

	private void Unsubscribe()
	{
        manipulatorUIController.OnStartMoving -= OnStartMoving;
        manipulatorUIController.OnStartRotationAndScaling -= OnStartRotationAndScaling;
        manipulatorUIController.OnFinishMoving -= OnFinishMoving;
        manipulatorUIController.OnFinishRotationAndScaling -= OnFinishRotationAndScaling;
		manipulatorUIController.OnFlipPressed -= OnFlip;
		manipulatorUIController.OnMirrorPressed -= OnReflection;
		manipulatorUIController.OnCameraFollowPressed -= ChangeCameraFollowing;
		manipulatorUIController.OnPaintableTargetPressed -= ChangeTarget;
	}

	private void OnSaveButtonPressed()
	{
		LocalSerialization();
	}

	private void OnLoadButtonPressed()
	{
		StartCoroutine(LocalDeserialization());
	}

	[Serializable]
	private class SerializableDecalsData
	{
		public int vehicleID;
		public string[] data;

		public SerializableDecalsData(int vehicleID, string[] data)
		{
			this.vehicleID = vehicleID;
			this.data = data;
		}

		public string Serialize()
		{
			return JsonUtility.ToJson(this);
		}

		public static void Deserialize(string json, out int vehicleID, out string[] data)
		{
			var deserialized = JsonUtility.FromJson<SerializableDecalsData>(json);
			vehicleID = deserialized.vehicleID;
			data = deserialized.data;
		}
	}
}
