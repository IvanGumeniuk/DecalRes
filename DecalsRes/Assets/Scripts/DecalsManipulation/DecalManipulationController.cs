using PaintIn3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DecalManipulationController : MonoBehaviour
{
	private static string BODY_LAYER = "Body";
	private static string WINDOWS_LAYER = "Windows";

	public int id;
	public DecalType decalType;
	public DecalPaintableTarget decalTarget;
	public int decalTextureID;

	// Camera transform data. It stores to prevent wrong projection when other decal was selected, camera position and rotation changed and this object selected again
	public float verticalOffset;
	public float horizontalOffset;

	public float followCameraStoredPitch;
	public float followCameraStoredYaw;

	public P3dHitBetween p3DHitBetweenController;
	public P3dPaintDecal p3DPaintDecalController;

	public DecalRotator decalRotator;
	public DecalScaler decalScaler;
	public DecalMover decalMover;
	public DecalColorDataContainer decalColorData;
	public DecalManipulationController reflectedDecal;
	public LayerMask targetlayers;
	public string decalText;
	public int decalFontID;

	public bool Reflected { get { return reflectedDecal != null; } }
	public bool Flipped { get { return decalScaler.Flipped; } }
	public bool FollowsCamera { get; set; } = true;
	public int Priority { get { return p3DHitBetweenController.Priority; } }

	public Transform HitPoint { get { return p3DHitBetweenController.Point; } }
	public Texture DecalTexture { get { return p3DPaintDecalController.Texture; } }


	private int currentlySelectedIndex = -1;
	private int Index
	{
		set
		{
			currentlySelectedIndex = value;

			if (currentlySelectedIndex >= Enum.GetValues(typeof(DecalPaintableTarget)).Length)
				currentlySelectedIndex = 0;

			decalTarget = (DecalPaintableTarget)currentlySelectedIndex;
		}

		get
		{
			return currentlySelectedIndex;
		}
	}

	private void Start()
	{
		// Initialize target
		NextTarget();
	}

	public void NextTarget()
	{
		Index++;
		SetDecalTarget(decalTarget);
	}

	public void SetDecalTarget(DecalPaintableTarget target)
	{
		decalTarget = target;

		switch (target)
		{
			case DecalPaintableTarget.Body:
				targetlayers = LayerMask.GetMask(BODY_LAYER);
				break;
			case DecalPaintableTarget.Windows:
				targetlayers = LayerMask.GetMask(WINDOWS_LAYER);
				break;
			case DecalPaintableTarget.BodyAndWindows:
				targetlayers = LayerMask.GetMask(BODY_LAYER) | LayerMask.GetMask(WINDOWS_LAYER);
				break;
		}

		p3DPaintDecalController.Layers = targetlayers;

		if (Reflected)
			reflectedDecal.SetDecalTarget(target);
	}

	public DecalPaintableTarget DecalTarget()
	{
		return decalTarget;
	}

	public void SetCameraTransformData(float verticalOffset, float horizontalOffset)
	{
		this.verticalOffset = verticalOffset;
		this.horizontalOffset = horizontalOffset;
	}

	public void SetIdentifier(DecalType decalType, int id)
	{
		this.decalType = decalType;
		this.id = id;
	}

	public void SetScale(Vector3 scale)
	{
		decalScaler.SetScale(scale);
	}

	public void SetColor(DecalColorDataContainer decalColorData)
	{
		p3DPaintDecalController.Color = decalColorData.rgbColor;

		if (Reflected)
			reflectedDecal.SetColor(decalColorData);

		this.decalColorData = decalColorData;
	}

	public void SetPriority(int priority)
	{
		p3DHitBetweenController.Priority = priority;
		if (Reflected)
			reflectedDecal.SetPriority(priority);
	}

	public void SetTexture(int textureID, DecalType decalType)
	{
		decalTextureID = textureID;
		p3DPaintDecalController.Texture = GetTextureByType(decalTextureID, decalType);

		if (Reflected)
			reflectedDecal.SetTexture(decalTextureID, decalType);
	}

	private Texture GetTextureByType(int textureID, DecalType decalType)
	{
		switch (decalType)
		{
			case DecalType.Sticker: return SettingsManager.Instance.stickerDecalSettings.GetTexture(decalTextureID);
			case DecalType.Shape: return SettingsManager.Instance.shapeDecalSettings.GetTexture(decalTextureID);
			case DecalType.Logo: return SettingsManager.Instance.logoDecalSettings.GetTexture(decalTextureID);
			case DecalType.Stripe: return SettingsManager.Instance.stripeDecalSettings.GetTexture(decalTextureID);
			default: return null;
		}
	}

	public void SetTexture(Texture texture)
	{
		p3DPaintDecalController.Texture = texture;

		if (Reflected)
			reflectedDecal.SetTexture(texture);
	}

	private void OnConfirm()
	{
		StartCoroutine(MakeDecal());
	}

	private IEnumerator MakeDecal()
	{
		p3DHitBetweenController.MakeShot();

		IngameUIManager.Instance.decalsController.DeselectButtons();
		yield return null;

		decalMover.RevertToDefault();
		decalScaler.RevertToDefault();
		decalRotator.RevertToDefault();
	}

	private void Update()
	{
		if (Reflected)
			UpdateReflection();
	}

	public void FinishDecalCustomization(bool finish)
	{
		if(finish) 
			p3DHitBetweenController.MakeShot();
	}

	public void StopFollowCamera(float pitch, float yaw)
	{
		followCameraStoredPitch = pitch;
		followCameraStoredYaw = yaw;
	}

	public void StartFollowCamera(out float storedPitch, out float storedYaw)
	{
		storedPitch = followCameraStoredPitch;
		storedYaw = followCameraStoredYaw;

		followCameraStoredPitch = 0;
		followCameraStoredYaw = 0;
	}

	public void OnStartMoving()
	{
		decalMover.StartMoving();
	}

	public void Move(Vector3 vector)
	{
		decalMover.Move(vector);
	}

	public void FinishMoving(bool followsCamera)
	{
		decalMover.FinishMoving(followsCamera);
	}

	public void OnStartRotationAndScaling()
	{
		decalScaler.StartScaling();
		decalRotator.StartRotation();
	}

	public void AddAngle(float angle)
	{
		decalRotator.AddRotationAngle(angle);
	}

	public void Scale(float multiplier)
	{
		decalScaler.AddScale(multiplier);
	}

	public void Flip()
	{
		decalScaler.Flip();
	}

	public void SetText(int fontID, string text)
	{
		decalText = text;
		decalFontID = fontID;
	}


	public void StartReflection(Transform reflectedDecalsHolder)
	{
		reflectedDecal = Instantiate(gameObject, reflectedDecalsHolder).GetComponent<DecalManipulationController>();
		reflectedDecal.gameObject.name = $"{gameObject.name}_Reflected";

		UpdateReflection();
	}

	private void UpdateReflection()
	{
		reflectedDecal.transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
		reflectedDecal.transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, -transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z));
		
		if(decalType == DecalType.Text)
			reflectedDecal.decalScaler.SetScale(new Vector3(decalScaler.Scale.x, decalScaler.Scale.y, decalScaler.Scale.z));
		else
			reflectedDecal.decalScaler.SetScale(new Vector3(-decalScaler.Scale.x, decalScaler.Scale.y, decalScaler.Scale.z));

		reflectedDecal.decalRotator.SetAngle(-decalRotator.Angle);
		reflectedDecal.SetDecalTarget(decalTarget);
	}

	public void StopReflection()
	{
		if (Reflected)
		{
			Destroy(reflectedDecal.gameObject);
			reflectedDecal = null;
		}
	}

	public void SetParent(Transform parent, Transform reflectedParent)
	{
		transform.parent = parent;
		if (Reflected)
		{
			reflectedDecal.transform.parent = reflectedParent;
		}
	}

	private void OnDestroy()
	{
		StopReflection();
	}

	public string Serialize()
	{
		return new SerializableDecalsData(this).Serialize();
	}

	public void Deserialize(string json)
	{
		SerializableDecalsData.Deserialize(json, this);
	}


	[Serializable]
	private class SerializableDecalsData
	{
		public int id;
		public DecalType decalType;
		public DecalPaintableTarget decalTarget;

		public float[] position;
		public float[] rotation;

		public float verticalOffset;
		public float horizontalOffset;

		public float followCameraStoredPitch;
		public float followCameraStoredYaw;

		public string reflectedData;

		public string decalsRotationData;
		public string decalsMovingData;
		public string decalsScaleData;
		public string decalsColorData;

		public bool reflected;
		public bool flipped;
		public bool followsCamera;

		public int decalTextureID;
		public int decalPriority;
		public string decalText;
		public int decalFontID;

		public SerializableDecalsData(DecalManipulationController decal)
		{
			id = decal.id;
			decalType = decal.decalType;
			decalTarget = decal.decalTarget;
			position = decal.transform.position.ToArray();
			rotation = decal.transform.rotation.ToArray();

			verticalOffset = decal.verticalOffset;
			horizontalOffset = decal.horizontalOffset;

			followCameraStoredPitch = decal.followCameraStoredPitch;
			followCameraStoredYaw = decal.followCameraStoredYaw;

			reflected = decal.Reflected;
			flipped = decal.Flipped;
			followsCamera = decal.FollowsCamera;
 
			decalTextureID = decal.decalTextureID;
			decalPriority = decal.Priority;

			decalText = decal.decalText;
			decalFontID = decal.decalFontID;

			decalsRotationData = new SerializableDecalsRotationData(decal.decalRotator).Serialize();
			decalsMovingData = new SerializableDecalsMovingData(decal.decalMover).Serialize();
			decalsScaleData = new SerializableDecalsScaleData(decal.decalScaler).Serialize();
			decalsColorData = new SerializableDecalsColorData(decal.decalColorData).Serialize();

			if (decal.Reflected)
			{
				reflectedData = new SerializableDecalsData(decal.reflectedDecal).Serialize();
			}
		}

		public string Serialize()
		{
			return JsonUtility.ToJson(this);
		}

		public static void Deserialize(string json, DecalManipulationController decal)
		{
			var deserialized = JsonUtility.FromJson<SerializableDecalsData>(json);

			decal.id = deserialized.id;
			decal.decalType = deserialized.decalType;
			decal.SetDecalTarget(deserialized.decalTarget);
			decal.verticalOffset = deserialized.verticalOffset;
			decal.horizontalOffset = deserialized.horizontalOffset;
			decal.followCameraStoredPitch = deserialized.followCameraStoredPitch;
			decal.followCameraStoredYaw = deserialized.followCameraStoredYaw;
			
			decal.decalText = deserialized.decalText;
			decal.decalFontID = deserialized.decalFontID;

			decal.transform.position = deserialized.position.ToVector3();
			decal.transform.rotation = deserialized.rotation.ToQuaternion();

			if(decal.decalType == DecalType.Text)
			{
				IngameUIManager.Instance.decalsController.textDecalUIController.SetText(decal.decalText, decal.decalFontID);
				IngameUIManager.Instance.decalsController.textDecalUIController.HandleTextureCreating(decal.id, decal.decalFontID);
				var texture = IngameUIManager.Instance.decalsController.textDecalUIController.GetTexture(decal.id);
				decal.SetTexture(texture);
			}
			else
				decal.SetTexture(deserialized.decalTextureID, deserialized.decalType);

			decal.SetPriority(deserialized.decalPriority);

			SerializableDecalsRotationData.Deserialize(deserialized.decalsRotationData, decal.decalRotator);			
			SerializableDecalsMovingData.Deserialize(deserialized.decalsMovingData, decal.decalMover);		
			SerializableDecalsScaleData.Deserialize(deserialized.decalsScaleData, decal.decalScaler);

			DecalColorDataContainer decalColorDataContainer = new DecalColorDataContainer();
			SerializableDecalsColorData.Deserialize(deserialized.decalsColorData, decalColorDataContainer);
			decal.SetColor(decalColorDataContainer);

			if (deserialized.flipped)
			{
				decal.Flip();
			}

			decal.followCameraStoredPitch = deserialized.followCameraStoredPitch;
			decal.followCameraStoredYaw = deserialized.followCameraStoredYaw;
			decal.FollowsCamera = deserialized.followsCamera;

			// deserialize reflected data
			if (deserialized.reflected)
			{
				decal.StartReflection(DecalHoldersManager.Instance.staticReflectedDecalsHolder);
				Deserialize(deserialized.reflectedData, decal.reflectedDecal);
				decal.reflectedDecal.SetDecalTarget(deserialized.decalTarget);
			}
		}
	}

	[Serializable]
	private class SerializableDecalsRotationData
	{
		public float defaultAngle;

		public float rotationSpeed;
		public float currentAngle;
		public float startAngle;

		public float[] angleClamp;

		public float angle;

		public SerializableDecalsRotationData(DecalRotator decalRotator)
		{
			defaultAngle = decalRotator.defaultAngle;
			rotationSpeed = decalRotator.rotationSpeed;
			currentAngle = decalRotator.currentAngle;
			startAngle = decalRotator.startAngle;
			angle = decalRotator.angle;

			angleClamp = decalRotator.angleClamp.ToArray();
		}

		public string Serialize()
		{
			return JsonUtility.ToJson(this);
		}

		public static void Deserialize(string json, DecalRotator decalRotatorReference)
		{
			var deserialized = JsonUtility.FromJson<SerializableDecalsRotationData>(json);

			decalRotatorReference.defaultAngle = deserialized.defaultAngle;
			decalRotatorReference.rotationSpeed = deserialized.rotationSpeed;
			decalRotatorReference.currentAngle = deserialized.currentAngle;
			decalRotatorReference.startAngle = deserialized.startAngle;
			decalRotatorReference.SetAngle(deserialized.angle);
			decalRotatorReference.angleClamp = deserialized.angleClamp.ToVector2();
		}
	}

	[Serializable]
	private class SerializableDecalsScaleData
	{
		public float scaleSpeed;
		public float currentScale;
		public float startScale;

		public float[] defaultScale;
		public float[] scale;
		public float[] scaleClamp;

		public SerializableDecalsScaleData(DecalScaler decalScaler)
		{
			scaleSpeed = decalScaler.scaleSpeed;
			currentScale = decalScaler.currentScale;
			startScale = decalScaler.startScale;
			
			defaultScale = decalScaler.defaultScale.ToArray();
			scale = decalScaler.Scale.ToArray();
			scaleClamp = decalScaler.scaleClamp.ToArray();
		}

		public string Serialize()
		{
			return JsonUtility.ToJson(this);
		}

		public static void Deserialize(string json, DecalScaler decalScalerReference)
		{
			var deserialized = JsonUtility.FromJson<SerializableDecalsScaleData>(json);

			decalScalerReference.scaleSpeed = deserialized.scaleSpeed;
			decalScalerReference.currentScale = deserialized.currentScale;
			decalScalerReference.startScale = deserialized.startScale;

			decalScalerReference.defaultScale = deserialized.defaultScale.ToVector3();
			decalScalerReference.SetScale(deserialized.scale.ToVector3());
			decalScalerReference.scaleClamp = deserialized.scaleClamp.ToVector2();
		}
	}

	[Serializable]
	private class SerializableDecalsMovingData
	{
		public float[] transformDefaultPosition;
		public float[] transformStartMovingPosition;

		public float movingSpeed;
		public float distanceBetweenPoints;

		public SerializableDecalsMovingData() { }
		public SerializableDecalsMovingData(DecalMover decalMover)
		{
			movingSpeed = decalMover.movingSpeed;
			distanceBetweenPoints = decalMover.distanceBetweenPoints;

			transformDefaultPosition = decalMover.transformDefaultPosition.ToArray();
			transformStartMovingPosition = decalMover.transformStartMovingPosition.ToArray();
		}

		public string Serialize()
		{
			return JsonUtility.ToJson(this);
		}

		public static void Deserialize(string json, DecalMover decalMoverReference)
		{
			var deserialized = JsonUtility.FromJson<SerializableDecalsMovingData>(json);

			decalMoverReference.movingSpeed = deserialized.movingSpeed;
			decalMoverReference.distanceBetweenPoints = deserialized.distanceBetweenPoints;

			decalMoverReference.transformDefaultPosition = deserialized.transformDefaultPosition.ToVector3();
			decalMoverReference.transformStartMovingPosition = deserialized.transformStartMovingPosition.ToVector3();
		}
	}

	[Serializable]
	private class SerializableDecalsColorData
	{
		public float saturationSliderValue;
		public float brightnessSliderValue;
		public float alphaSliderValue;
		
		public float[] hueHandleValue;
		public float[] rgbColor;

		public SerializableDecalsColorData(DecalColorDataContainer decalColorContainer)
		{
			saturationSliderValue = decalColorContainer.saturationSliderValue;
			brightnessSliderValue = decalColorContainer.brightnessSliderValue;
			alphaSliderValue = decalColorContainer.alphaSliderValue;

			hueHandleValue = decalColorContainer.hueHandleValue.ToArray();
			rgbColor = decalColorContainer.rgbColor.ToArray();
		}

		public string Serialize()
		{
			return JsonUtility.ToJson(this);
		}

		public static void Deserialize(string json, DecalColorDataContainer decalColorContainerReference)
		{
			var deserialized = JsonUtility.FromJson<SerializableDecalsColorData>(json);

			decalColorContainerReference.saturationSliderValue = deserialized.saturationSliderValue;
			decalColorContainerReference.brightnessSliderValue = deserialized.brightnessSliderValue;
			decalColorContainerReference.alphaSliderValue = deserialized.alphaSliderValue;

			decalColorContainerReference.hueHandleValue = deserialized.hueHandleValue.ToVector3();
			decalColorContainerReference.rgbColor = deserialized.rgbColor.ToColor();
			
		}
	}
}
