using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StorageContainer : MonoBehaviour, IInteractable, IStorable,ISaveable
{

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    bool started_locked;
    public bool IsLocked;
    public bool StayUnlocked;
    public bool ConsumeKey;
    public item_requirement[] required_keys;
    public item_requirement[] required_items => required_keys;

    public bool CanUse = true;
    public bool CanInteract { get => CanUse; set { CanUse = value; } }


    public event Action OnOpened;

    [SerializeField] InventroyTypes inventroyType;

    public UnityEvent storageEmtpied;
    public UnityEvent storageFilled;

    public bool ContainerIsEmpty = true;

    public int unique_id;

    public int Get_Unique_ID { get => unique_id; set { unique_id = value; } }

    public bool Get_Should_Save => IsLocked != started_locked;

    void Awake()
    {
        started_locked = IsLocked;
    }

    public bool Interact(Interactor interactor)
    {
        if (!CanUse) return false;
        if (IsLocked && required_keys.Length > 0)
        {
            foreach (item_requirement item in required_keys)
            {
                int current_item_amount = GameplayUtils.instance.get_item_holding_amount(item.item_id);
                if (current_item_amount < item.item_amount)
                {
                    GameplayUtils.instance.ShowCustomNotifCenter("Incorrect Key");
                    return false;
                }
                if (ConsumeKey)
                {
                    GameplayUtils.instance.remove_items_from_inventory(item.item_id, item.item_amount);
                }
            }
            if (StayUnlocked)
            {
                IsLocked = false;
            }
        }

        GameplayUtils.instance.OpenInventoryUI(inventroyType);
        OnOpened?.Invoke();
        return true;
    }

    public void InventoryUpdated(List<InventorySlot> inventorySlots)
    {
        bool updateIsEmpty = true;
        foreach (InventorySlot inventorySlot in inventorySlots)
        {
            if (!inventorySlot.isEmpty) updateIsEmpty = false;
            break;
        }
        if (updateIsEmpty && !ContainerIsEmpty)
        {
            storageEmtpied?.Invoke();
        }
        else if (!updateIsEmpty && ContainerIsEmpty)
        {
            storageFilled?.Invoke();
        }
        ContainerIsEmpty = updateIsEmpty;
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        IsLocked = false;
    }
}
