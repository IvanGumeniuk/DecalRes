using UnityEngine;
using UnityEngine.UI;

public class DecalLayerItem : MonoBehaviour
{
	[SerializeField] private RectTransform selectionOutline;

	[SerializeField] private RawImage image;
	[SerializeField] private Button closeButton;

	private RectTransform rectTransform;
	private RectTransform content;

	private DecalLayersUIController layersUIController;

	public int ID { get; set; }
	public int Priority { get; set; }
	public DecalType DecalType { get; set; }

	public bool selected;
	public bool Selected 
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			selectionOutline.gameObject.SetActive(selected);
			closeButton.gameObject.SetActive(selected);
		}
	}

	public Vector2 ElementWorldCenter
	{
		get
		{
			Vector3[] corners = new Vector3[4];
			rectTransform.GetWorldCorners(corners);

			Vector3 center = Vector3.zero;
			for (int i = 0; i < corners.Length; i++)
			{
				center += corners[i];
			}

			return center / corners.Length;
		}
	}

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		Priority = transform.GetSiblingIndex();
		closeButton.onClick.AddListener(OnRemoveClick);
	}

	private void OnDestroy()
	{
		closeButton.onClick.RemoveListener(OnRemoveClick);
	}

	public void Initialize(DecalLayersUIController layersUIController, RectTransform content, DecalType decalType, int id, Texture itemSprite)
	{
		this.layersUIController = layersUIController;
		this.content = content;
		ID = id;
		DecalType = decalType;

		image.texture = itemSprite;
		image.SizeToParent();
	}

	public void OnDrag()
	{
		rectTransform.position = Input.mousePosition;
		ClampInArea(rectTransform, content);
		layersUIController.OnItemDrag(this);
	}

	public void OnEndDrag()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(content);
	}

	public void OnRemoveClick()
	{
		layersUIController.OnRemoveItem(this);
	}

	public void OnPointerClick()
	{
		layersUIController.OnItemSelected(this);
	}

	// Clamps "element" position in "area" rectangle
	private void ClampInArea(RectTransform element, RectTransform area)
	{
		Vector3 pos = area.localPosition;

		Vector3 minPosition = area.rect.min - element.rect.min;
		Vector3 maxPosition = area.rect.max - element.rect.max;

		pos.x = Mathf.Clamp(element.localPosition.x, minPosition.x, maxPosition.x);
		pos.y = Mathf.Clamp(element.localPosition.y, minPosition.y, maxPosition.y);

		element.localPosition = pos;
	}
}
