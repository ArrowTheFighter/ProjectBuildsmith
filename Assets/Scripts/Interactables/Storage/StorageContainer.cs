using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StorageContainer : MonoBehaviour, IInteractable, IStorable
{

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_requirements;
    public item_requirement[] required_items => item_requirements;

    public bool CanUse = true;
    public bool CanInteract { get => CanUse; set { CanUse = value; } }

    public event Action OnOpened;

    [SerializeField] InventroyTypes inventroyType;

    public UnityEvent storageEmtpied;
    public UnityEvent storageFilled;

    public bool ContainerIsEmpty = true;


    public bool Interact(Interactor interactor)
    {
        if (!CanUse) return false;
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
}
