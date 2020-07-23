using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalLayersUIController : MonoBehaviour
{
    public Action<int> OnLayerItemRemoved;              // int - item`s ID 
    public Action<int> OnLayerItemSelected;              // int - item`s ID 
    public Action<Dictionary<int, int>> OnLayerItemsPrioritiesChanged;              // int - item`s ID 

    [SerializeField] private List<DecalLayerItem> layerItems = new List<DecalLayerItem>();
    [SerializeField] private RectTransform layerItemsHolder;
    [SerializeField] private DecalLayerItem itemPrefab;

    private Sprite newItemSprite;
    private int newItemID;

    private void Start()
    {
        IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.OnStickerClicked += OnStickerClicked;
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalPainting += OnConfirmDecalCreating;
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalChanging += OnConfirmDecalChanging;
    }

    private void OnDestroy()
    {
        IngameUIManager.Instance.customizationViewUIController.stickerDecalUIController.OnStickerClicked -= OnStickerClicked;
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalPainting -= OnConfirmDecalCreating;
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalChanging -= OnConfirmDecalChanging;
    }

    private void OnConfirmDecalCreating(bool confirm)
	{
        if (!confirm)
            return;

        DecalLayerItem item = Instantiate(itemPrefab, layerItemsHolder);
        layerItems.Add(item);
        item.Initialize(this, layerItemsHolder, newItemID, newItemSprite);
    }

    private void OnConfirmDecalChanging(bool confirm)
    {
        DeselectItems();
    }


    private void OnStickerClicked(Sprite sprite, int id)
	{
        newItemSprite = sprite;
        newItemID = id;
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

    public void OnItemSelected(DecalLayerItem item)
	{
        DeselectItems();

        item.Selected = true;
        OnLayerItemSelected?.Invoke(item.id);
    }

    public void DeselectItems()
	{
        foreach (var layerItem in layerItems)
        {
            layerItem.Selected = false;   
        }

        OnLayerItemSelected?.Invoke(-1);
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

        Dictionary<int, int> priorities = new Dictionary<int, int>();

        for (int i = 0; i < layerItems.Count; i++)
		{
            layerItems[i].transform.SetSiblingIndex(layerItems[i].priority);
            priorities.Add(layerItems[i].id, layerItems[i].priority);
        }

        OnLayerItemsPrioritiesChanged?.Invoke(priorities);
    }
}
