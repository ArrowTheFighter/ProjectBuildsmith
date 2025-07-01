using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public InputAction toggleInventoryAction;

    [HideInInspector] public bool inventoryIsOpen;

    [Header("InventoryObjects")]
    public GameObject InventoryObject;
    public GameObject InventorySlotsParent;
    public GameObject InventorySlotPrefab;
    [SerializeField] public InventorySlotComponent MouseSlot;

    public Action OnInventoryClosed;

    float inventoryToggleCooldown;

    [Header("InventorySlots")]
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameplayInput.instance.playerInput.actions["Inventory"].performed += ToggleInventory;
        GameplayInput.instance.playerInput.actions["CloseInventory"].performed += ToggleInventory;
        for (int i = 0; i < 30; i++)
        {
            AddInventorySlot(i);
        }
    }

    void AddInventorySlot(int slotID)
    {
        InventorySlot inventorySlot = new InventorySlot();
        //inventorySlot.inventoryItemStack = new InventoryItemStack();

        inventorySlots.Add(inventorySlot);
        GameObject slot = Instantiate(InventorySlotPrefab, InventorySlotsParent.transform);
        InventorySlotComponent inventorySlotComponent = slot.GetComponent<InventorySlotComponent>();
        inventorySlotComponent.SlotID = slotID;
        inventorySlotComponent.inventorySlot = inventorySlot;
        inventorySlot.inventorySlotComponent = inventorySlotComponent;
    }

    public void AddItemToMouseSlot(ItemData itemData, int amount = 1, bool force = false)
    {
        if (force || MouseSlot.inventorySlot.isEmpty)
        {
            MouseSlot.inventorySlot.isEmpty = false;
            InventoryItemStack newItemStack = new InventoryItemStack(itemData.item_id, itemData.item_name, amount);
            MouseSlot.inventorySlot.inventoryItemStack = newItemStack;
            MouseSlot.inventorySlot.inventorySlotComponent.SetSlotFilled(itemData.item_name, amount, itemData.item_ui_image);
        }
    }

    public void AddItemToSlot(InventorySlot inventorySlot, ItemData itemData, int amount = 1, bool force = false)
    {
        if (force || inventorySlot.isEmpty)
        {
            print("adding item to slot");
            inventorySlot.isEmpty = false;
            InventoryItemStack newItemStack = new InventoryItemStack(itemData.item_id, itemData.item_name, amount);
            print("new itemStack size is: " + newItemStack.Amount);
            inventorySlot.inventoryItemStack = newItemStack;
            inventorySlot.inventorySlotComponent.SetSlotFilled(itemData.item_name, amount, itemData.item_ui_image);
        }
    }

    public bool AddItemToInventory(ItemData itemData, int amount = 1)
    {
        InventorySlot inventorySlot = GetSlotWithItemButNotFull(itemData.item_id);
        if (inventorySlot != null)
        {
            int newAmount = amount + inventorySlot.inventoryItemStack.Amount;
            inventorySlot.inventoryItemStack.Amount = newAmount;
            inventorySlot.inventorySlotComponent.SetSlotFilled(newAmount);
            //inventorySlot.inventorySlotComponent.setSlotAmountText(newAmount);
            return true;
        }

        // --- There isn't a slot we can add to ---

        inventorySlot = GetFirstEmptySlot();
        // If there are no empty slots
        if (inventorySlot == null)
        {
            return false;
        }

        //Adding item to an empty slot
        inventorySlot.isEmpty = false;
        InventoryItemStack newItemStack = new InventoryItemStack(itemData.item_id, itemData.item_name, amount);
        inventorySlot.inventoryItemStack = newItemStack;
        inventorySlot.inventorySlotComponent.SetSlotFilled(itemData.item_name, amount, itemData.item_ui_image);
        return true;
    }

    public InventorySlot GetFirstEmptySlot()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].isEmpty)
            {
                return inventorySlots[i];

            }
        }
        return null;
    }

    public InventorySlot GetSlotWithItemButNotFull(string item_id)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (!inventorySlots[i].isEmpty)
            {
                if (inventorySlots[i].inventoryItemStack.ID == item_id)
                {
                    if (inventorySlots[i].inventoryItemStack.Amount < inventorySlots[i].inventoryItemStack.MaxStackSize)
                    {
                        return inventorySlots[i];
                    }
                }

            }
        }
        return null;
    }

    void ToggleInventory(InputAction.CallbackContext context)
    {
        if (inventoryToggleCooldown > Time.time) return;
        inventoryToggleCooldown = Time.time + 0.1f;
        if (InventoryObject != null)
        {
            //InventoryObject.SetActive(inventoryIsOpen);
            if (InventoryObject.TryGetComponent(out CanvasGroup canvasGroup))
            {
                if (!inventoryIsOpen)
                {
                    if (GameplayUtils.instance.GetOpenMenu()) return;
                    //print("opening inventory");
                    UIInputHandler.instance.defaultButton = InventorySlotsParent.transform.GetChild(0).gameObject;
                    GameplayUtils.instance.OpenMenu();

                }
                else
                {
                    //print("closing inventory");
                    GameplayUtils.instance.CloseMenu();
                    OnInventoryClosed?.Invoke();
                    UIInputHandler.instance.defaultButton = null;
                }
                inventoryIsOpen = !inventoryIsOpen;
                canvasGroup.alpha = inventoryIsOpen ? 1 : 0;
                canvasGroup.blocksRaycasts = inventoryIsOpen;
                canvasGroup.interactable = inventoryIsOpen;

            }
        }
    }

    public int getAmountOfItemByID(string item_id)
    {
        int itemAmount = 0;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (!inventorySlots[i].isEmpty && inventorySlots[i].inventoryItemStack.ID == item_id)
            {
                itemAmount += inventorySlots[i].inventoryItemStack.Amount;
            }
        }
        return itemAmount;
    }

    public bool removeItemsByID(string item_id, int amount = 1)
    {
        int remainingAmount = amount;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (!inventorySlots[i].isEmpty && inventorySlots[i].inventoryItemStack.ID == item_id)
            {
                // -- Stack has more then we need to remove --
                if (inventorySlots[i].inventoryItemStack.Amount > remainingAmount)
                {
                    ItemData itemData = GameplayUtils.instance.GetItemDataByID(inventorySlots[i].inventoryItemStack.ID);
                    AddItemToSlot(inventorySlots[i], itemData, inventorySlots[i].inventoryItemStack.Amount - remainingAmount, true);
                    remainingAmount = 0;
                }
                // -- Stack has exactly the right amount to remove --
                else if (inventorySlots[i].inventoryItemStack.Amount == remainingAmount)
                {
                    inventorySlots[i].inventorySlotComponent.RemoveItemFromSlot(false);
                    remainingAmount -= inventorySlots[i].inventoryItemStack.Amount;
                }
                // -- The amount to remove is bigger then the stack --
                else
                {
                    remainingAmount -= inventorySlots[i].inventoryItemStack.Amount;
                    inventorySlots[i].inventorySlotComponent.RemoveItemFromSlot(false);
                }
                if (remainingAmount <= 0) return true;
            }
        }
        return false;
     }
}

[Serializable]
public class InventorySlot
{
    public bool isEmpty = true;
    public InventoryItemStack inventoryItemStack;
    public InventorySlotComponent inventorySlotComponent;

 }

[Serializable]
public class InventoryItemStack
{
    public string ID;
    public string Name;
    public int Amount;
    public int MaxStackSize;

    public InventoryItemStack()
    {
        Amount = 1;
    }

    public InventoryItemStack(string _id, string _name, int _amount, int _maxStack = 99)
    {
        ID = _id;
        Name = _name;
        Amount = _amount;
        MaxStackSize = _maxStack;
    }
}
