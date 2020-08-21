using System;
using System.Collections.Generic;
using UnityEngine;

public class DecalLayersUIController : MonoBehaviour
{
    public Action<int> OnLayerItemRemoved;                                          // int - item`s ID 
    public Action<int> OnLayerItemSelected;                                         // int - item`s ID 
    public Action<Dictionary<int, int>> OnLayerItemsPrioritiesChanged;              // <item`s ID, priority>

    [SerializeField] private List<DecalLayerItem> layerItems = new List<DecalLayerItem>();
    [SerializeField] private RectTransform layerItemsHolder;
    [SerializeField] private DecalLayerItem itemPrefab;

    public bool IsLayerSelected { get { return layerItems.Find(x => x.Selected) != null; } }

    private void Start()
    {
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalPainting += OnConfirmDecalCreating;
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalChanging += OnConfirmDecalChanging;
    }

    private void OnDestroy()
    {
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalPainting -= OnConfirmDecalCreating;
        IngameUIManager.Instance.manipulatorViewUIController.OnConfirmDecalChanging -= OnConfirmDecalChanging;
    }

    public void CreateLayerElement(DecalType decalType, int id, Texture texture)
	{
        if (IsLayerSelected)
            return;

        DecalLayerItem item = Instantiate(itemPrefab, layerItemsHolder);
        layerItems.Add(item);
        item.Initialize(this, layerItemsHolder, decalType, id, texture);
    }

    private void OnConfirmDecalCreating(bool confirm)
	{
        /*if (!confirm || IsLayerSelected)
            return;

        DecalLayerItem item = Instantiate(itemPrefab, layerItemsHolder);
        layerItems.Add(item);
        item.Initialize(this, layerItemsHolder, newItemID, newItemTexture);*/
    }

    private void OnConfirmDecalChanging(bool confirm)
    {
        DeselectItems();
    }

    public void OnBeginDrag(DecalLayerItem item)
	{
        item.transform.SetSiblingIndex(layerItems.Count - 1);
    }

    public void OnRemoveItem(DecalLayerItem item)
    {
        layerItems.Remove(item);
        OnLayerItemRemoved?.Invoke(item.ID);
        Destroy(item.gameObject);

        for (int i = 0; i < layerItems.Count; i++)
        {
            layerItems[i].Priority = i;
        }

        UpdateLayerItemsPriorities();
    }

    public void OnItemSelected(DecalLayerItem item)
	{
        foreach (var layerItem in layerItems)
        {
            if(layerItem != item)
                layerItem.Selected = false;
        }

        item.Selected = !item.Selected;
        OnLayerItemSelected?.Invoke(item.Selected ? item.ID : -1);

        IngameUIManager.Instance.decalsController.DeselectButtons();
    }

    public void DeselectItems()
	{
        foreach (var layerItem in layerItems)
        {
            layerItem.Selected = false;   
        }

        // -1 - is there no selected layer items 
        OnLayerItemSelected?.Invoke(-1);
    }

    public void OnItemDrag(DecalLayerItem item)
	{
        for (int i = 0; i < layerItems.Count; i++)
		{
            bool itemIsHigher = item.ElementWorldCenter.y <= layerItems[i].ElementWorldCenter.y;
            bool itemIsLower = item.ElementWorldCenter.y >= layerItems[i].ElementWorldCenter.y;
            
            bool priorityIsHigher = item.Priority > layerItems[i].Priority;
            bool priorityIsLower = item.Priority < layerItems[i].Priority;

            bool mustMoveDown = itemIsHigher && priorityIsLower;
            bool mustMoveUp = itemIsLower && priorityIsHigher;

            if (mustMoveDown || mustMoveUp)
			{
                int temp = item.Priority;
                item.Priority = layerItems[i].Priority;
                layerItems[i].Priority = temp;
            }
        }

        UpdateLayerItemsPriorities();
    }

    private void UpdateLayerItemsPriorities()
	{
        Dictionary<int, int> priorities = new Dictionary<int, int>();

        for (int i = 0; i < layerItems.Count; i++)
        {
            layerItems[i].transform.SetSiblingIndex(layerItems[i].Priority);
            priorities.Add(layerItems[i].ID, layerItems[i].Priority);
        }

        OnLayerItemsPrioritiesChanged?.Invoke(priorities);
    }
}
