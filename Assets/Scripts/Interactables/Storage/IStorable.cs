using System;
using System.Collections.Generic;
using UnityEngine;

public interface IStorable
{
    public event Action OnOpened;
    public void InventoryUpdated(List<InventorySlot> inventorySlots);
}
