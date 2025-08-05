using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraftingTableBase : MonoBehaviour
{
    public abstract void InventoryUpdated();
    public List<CraftingRecipeData> craftingRecipeData;
    public CraftingStationTypes craftingStationType;
    public Action OnItemCrafted;
    public InventorySlotComponent[] craftingTableSlots;
    public InventorySlotComponent outputSlot;
}
