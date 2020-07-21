using UnityEngine;
using UnityEngine.UI;

public class DecalLayerItem : MonoBehaviour
{
    public int id;
    public int priority;
	public RectTransform rectTransform;
	public RectTransform content;
	public Image image;

	[SerializeField] private Button closeButton;

	[SerializeField] private DecalLayersUIController layersUIController;

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
		priority = transform.GetSiblingIndex();
		closeButton.onClick.AddListener(OnRemoveClick);
	}

	private void OnDestroy()
	{
		closeButton.onClick.RemoveListener(OnRemoveClick);
	}

	public void Initialize(DecalLayersUIController layersUIController, RectTransform content, int id, Sprite itemSprite)
	{
		this.layersUIController = layersUIController;
		this.content = content;
		this.id = id;
		image.sprite = itemSprite;
	}

	public void OnBeginDrag()
	{
		//layersUIController.OnBeginDrag(this);
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

	public void OnPointerEnter()
	{
		closeButton.gameObject.SetActive(true);
	}

	public void OnPointerExit()
	{
		closeButton.gameObject.SetActive(false);
	}

	public void OnRemoveClick()
	{
		layersUIController.OnRemoveItem(this);
	}

	void ClampInArea(RectTransform element, RectTransform area)
	{
		Vector3 pos = area.localPosition;

		Vector3 minPosition = area.rect.min - element.rect.min;
		Vector3 maxPosition = area.rect.max - element.rect.max;

		pos.x = Mathf.Clamp(element.localPosition.x, minPosition.x, maxPosition.x);
		pos.y = Mathf.Clamp(element.localPosition.y, minPosition.y, maxPosition.y);

		element.localPosition = pos;
	}
}
