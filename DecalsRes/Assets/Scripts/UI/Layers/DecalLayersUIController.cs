using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalLayersUIController : MonoBehaviour
{
    public Action<int> OnLayerItemRemoved;              // int - item`s ID 

    [SerializeField] private List<DecalLayerItem> layerItems = new List<DecalLayerItem>();
    [SerializeField] private RectTransform layerItemsHolder;
    [SerializeField] private DecalLayerItem itemPrefab;

    private Sprite newItemSprite;

    private void Start()
    {
        IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.OnStickerClicked += OnStickerClicked;
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalPainting += OnConfirmDecalCreating;
    }

    private void OnDestroy()
    {
        IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.OnStickerClicked -= OnStickerClicked;
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalPainting -= OnConfirmDecalCreating;
    }

    private void OnConfirmDecalCreating(bool confirm)
	{
        if (!confirm)
            return;

        DecalLayerItem item = Instantiate(itemPrefab, layerItemsHolder);
        layerItems.Add(item);
        item.Initialize(this, layerItemsHolder, layerItems.Count - 1, newItemSprite);
    }

	private void OnStickerClicked(Sprite sprite)
	{
        newItemSprite = sprite;   
    }

    public void OnBeginDrag(DecalLayerItem item)
	{
        item.transform.SetSiblingIndex(layerItems.Count - 1);
    }

    public void OnRemoveItem(DecalLayerItem item)
	{
        layerItems.Remove(item);
        OnLayerItemRemoved?.Invoke(item.id);

        Destroy(item.gameObject);
	}

    public void OnItemDrag(DecalLayerItem item)
	{
        for (int i = 0; i < layerItems.Count; i++)
		{
            if (layerItems[i] == item)
                continue;

            bool mustBeOnBottom = item.ElementWorldCenter.y <= layerItems[i].ElementWorldCenter.y && item.priority < layerItems[i].priority;
            bool mustBeOnTop = item.ElementWorldCenter.y >= layerItems[i].ElementWorldCenter.y && item.priority > layerItems[i].priority;

            if (mustBeOnBottom || mustBeOnTop)
			{
                int temp = item.priority;
                item.priority = layerItems[i].priority;
                layerItems[i].priority = temp;
            }
        }

        for (int i = 0; i < layerItems.Count; i++)
		{
            layerItems[i].transform.SetSiblingIndex(layerItems[i].priority);
        }
    }
}
