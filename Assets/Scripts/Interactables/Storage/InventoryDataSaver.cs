using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryDataSaver : MonoBehaviour
{
    public List<InventorySlotComponent> inventorySlots = new List<InventorySlotComponent>();
    public List<InventorySlot> savedSlots = new List<InventorySlot>();
    public bool ActiveContainer;

    public event Action slotsUpdated;

    public event Action OnFinshedInitalizing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (InventorySlotComponent slot in inventorySlots)
        {
            slot.slotFilled += UpdateSavedSlots;
            slot.slotEmptied += (context) => { UpdateSavedSlots(); };
            bool slotExists = false;
            foreach (InventorySlot inventorySlot in savedSlots)
            {
                if (inventorySlot.slot_id == slot.SlotID)
                {
                    slotExists = true;
                    break;
                }
            }
            if (!slotExists)
                savedSlots.Add(new InventorySlot(slot.SlotID));
        }
        GetComponent<IStorable>().OnOpened += ContainerOpened;
        OnFinshedInitalizing?.Invoke();
        SendInventoryUpdate();
    }

    void SendInventoryUpdate()
    {
        if(TryGetComponent(out IStorable storable))
        {
            storable.InventoryUpdated(savedSlots);
        };
    }

    public void UpdateSavedSlots()
    {
        if (!ActiveContainer) return;
        foreach (InventorySlotComponent slot in inventorySlots)
        {
            foreach (InventorySlot inventorySlot in savedSlots)
            {
                if (inventorySlot.slot_id == slot.SlotID)
                {
                    inventorySlot.inventoryItemStack = new InventoryItemStack(
                        slot.inventorySlot.inventoryItemStack.ID,
                        slot.inventorySlot.inventoryItemStack.Name,
                        slot.inventorySlot.inventoryItemStack.Amount,
                        slot.inventorySlot.inventoryItemStack.MaxStackSize
                        );
                    inventorySlot.isEmpty = slot.inventorySlot.isEmpty;
                    break;
                }
            }
        }
        slotsUpdated?.Invoke();
        SendInventoryUpdate();
    }

    void ContainerOpened()
    {
        ActiveContainer = true;
        SetContainerSlots();
        GameplayUtils.instance.OnInventoryClosed += ContainerClosed;
    }

    void ContainerClosed()
    {
        ActiveContainer = false;
        GameplayUtils.instance.OnInventoryClosed -= ContainerClosed;
        ClearSlots();
    }

    void ClearSlots()
    {
        foreach (InventorySlotComponent slot in inventorySlots)
        {
            slot.RemoveItemFromSlot(false, false);
        }
    }

    public void SetContainerSlots()
    {
        if (!ActiveContainer) return;
        foreach (InventorySlotComponent slot in inventorySlots)
        {
            foreach (InventorySlot inventorySlot in savedSlots)
            {
                if (inventorySlot.slot_id == slot.SlotID)
                {
                    if (inventorySlot.inventoryItemStack.Amount == 0 || inventorySlot.inventoryItemStack.ID == "")
                    {
                        slot.RemoveItemFromSlot(false, false);
                        break;
                    }
                    ItemData itemData = GameplayUtils.instance.GetItemDataByID(inventorySlot.inventoryItemStack.ID);
                    if (itemData == null)
                    {
                        slot.RemoveItemFromSlot(false, false);
                    }
                    else
                    {
                        GameplayUtils.instance.inventoryManager.AddItemToSlot(slot.inventorySlot, itemData, inventorySlot.inventoryItemStack.Amount, true, false);
                    }

                }

            }

        }
    }
}
