using System;
using System.Collections.Generic;
using UnityEngine;

public class NewCraftingTable : MonoBehaviour, IInteractable, IStorable
{
    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_requirements;
    public item_requirement[] required_items => item_requirements;

    public bool CanUse = true;
    public bool CanInteract { get => CanUse; set { CanUse = value; } }


    public CraftingStationTypes craftingStationType;

    public event Action OnOpened;

    public bool Interact(Interactor interactor)
    {
        if (!CanUse) return false;
        GameplayUtils.instance.OpenCraftingMenu(craftingStationType);
        OnOpened?.Invoke();
        return true;
    }

    public void InventoryUpdated(List<InventorySlot> inventorySlots)
    {
        return;
    }
}
