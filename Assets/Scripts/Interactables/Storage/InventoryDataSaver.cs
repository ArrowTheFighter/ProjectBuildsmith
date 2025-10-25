using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class InventoryDataSaver : MonoBehaviour,ISaveable
{
    public List<InventorySlotComponent> inventorySlots = new List<InventorySlotComponent>();
    public List<InventorySlot> savedSlots = new List<InventorySlot>();
    public bool ActiveContainer;

    public event Action slotsUpdated;

    public event Action OnFinshedInitalizing;

    public int unique_id;

    public int Get_Unique_ID { get => unique_id; set { unique_id = value; } }

    public bool Get_Should_Save => true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScriptRefrenceSingleton.instance.saveLoadManager.inventoryDataSavers.Add(this);
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
        ScriptRefrenceSingleton.instance.gameplayUtils.OnInventoryClosed += ContainerClosed;
    }

    void ContainerClosed()
    {
        ActiveContainer = false;
        ScriptRefrenceSingleton.instance.gameplayUtils.OnInventoryClosed -= ContainerClosed;
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
                    ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(inventorySlot.inventoryItemStack.ID);
                    if (itemData == null)
                    {
                        slot.RemoveItemFromSlot(false, false);
                    }
                    else
                    {
                        ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(slot.inventorySlot, itemData, inventorySlot.inventoryItemStack.Amount, true, false);
                    }

                }

            }

        }
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        var saved_inventory = saveFileStruct.saved_inventory_savers.FirstOrDefault(item => item.inventory_id == unique_id);

        if (saved_inventory != null)
        {
            savedSlots = new List<InventorySlot>();
            foreach (var saved_slot in saved_inventory.saved_inventory_slots)
            {
                savedSlots.Add(new InventorySlot(saved_slot));
            }
        }
    }
}
