using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotComponent : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public InventorySlot inventorySlot;
    public int SlotID;
    public bool  IsHotbar;
    public bool Selected;
    public Image slotImage;
    public TextMeshProUGUI slotText;
    public TextMeshProUGUI slotAmountText;

    public Action slotEmptied;
    public Action slotFilled;

    void Start()
    {
        // Button button = GetComponent<Button>();
        // button.onClick.AddListener(slotClicked);
        GameplayInput.instance.playerInput.actions["Submit"].performed += context => { ControllerMainPress(); };
        GameplayInput.instance.playerInput.actions["Cancel"].performed += context => { ControllerSecondaryPress(); };
    }


    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // --- Left clicked on this slot ---
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SlotMainPress();
        }
        // -- Slot was right clicked --
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            SlotSecondaryPress();
        }
    }

    void ControllerMainPress()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            SlotMainPress();
        }
     }

    void ControllerSecondaryPress()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            SlotSecondaryPress();
        }
    }

    void SlotMainPress()
    {
        // --- Mouse has an item stack ---
        if (!GameplayUtils.instance.inventoryManager.MouseSlot.inventorySlot.isEmpty)
        {
            // -- Mouse has an item but the click slot is empty --
            if (inventorySlot.isEmpty)
            {
                InventoryItemStack itemStack = GameplayUtils.instance.inventoryManager.MouseSlot.inventorySlot.inventoryItemStack;
                ItemData itemData = GameplayUtils.instance.GetItemDataByID(itemStack.ID);
                GameplayUtils.instance.inventoryManager.AddItemToSlot(inventorySlot, itemData, itemStack.Amount);
                GameplayUtils.instance.inventoryManager.MouseSlot.inventorySlot.inventorySlotComponent.RemoveItemFromSlot(false);
                return;
            }
            // -- Mouse has an item and the slot has an item --
            else
            {
                InventoryItemStack mouseStack = GameplayUtils.instance.inventoryManager.MouseSlot.inventorySlot.inventoryItemStack;
                ItemData mouseItemData = GameplayUtils.instance.GetItemDataByID(mouseStack.ID);

                InventoryItemStack slotStack = inventorySlot.inventoryItemStack;
                ItemData slotItemData = GameplayUtils.instance.GetItemDataByID(slotStack.ID);

                // -- Both stacks have the same ID
                if (mouseStack.ID == slotStack.ID)
                {
                    // -- If the clicked slot is not a full stack
                    if (slotStack.Amount < slotStack.MaxStackSize)
                    {
                        int amountToMax = slotStack.MaxStackSize - slotStack.Amount;

                        int newAmount = mouseStack.Amount + slotStack.Amount;
                        // -- If the amount in the mouse slot can fit full in the new stack
                        if (mouseStack.Amount <= amountToMax)
                        {
                            /// -- Move mouse stack fully into the clicked slot
                            GameplayUtils.instance.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, newAmount, true);
                            GameplayUtils.instance.inventoryManager.MouseSlot.RemoveItemFromSlot(false);
                        }
                        else
                        {
                            int leftOver = newAmount - slotStack.MaxStackSize;

                            GameplayUtils.instance.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, slotStack.MaxStackSize, true);

                            GameplayUtils.instance.inventoryManager.AddItemToMouseSlot(mouseItemData, leftOver, true);
                        }
                    }
                    else
                    {
                        RemoveItemFromSlot(false);
                        GameplayUtils.instance.inventoryManager.MouseSlot.RemoveItemFromSlot(false);

                        GameplayUtils.instance.inventoryManager.AddItemToSlot(inventorySlot, mouseItemData, mouseStack.Amount);
                        GameplayUtils.instance.inventoryManager.AddItemToMouseSlot(slotItemData, slotStack.Amount);
                    }

                    return;
                }

                // -- Swaping Items --
                RemoveItemFromSlot(false);
                GameplayUtils.instance.inventoryManager.MouseSlot.RemoveItemFromSlot(false);

                GameplayUtils.instance.inventoryManager.AddItemToSlot(inventorySlot, mouseItemData, mouseStack.Amount);
                GameplayUtils.instance.inventoryManager.AddItemToMouseSlot(slotItemData, slotStack.Amount);
            }
        }
        else if (!inventorySlot.isEmpty)
        {
            ItemData itemData = GameplayUtils.instance.GetItemDataByID(inventorySlot.inventoryItemStack.ID);
            GameplayUtils.instance.inventoryManager.AddItemToMouseSlot(itemData, inventorySlot.inventoryItemStack.Amount);
            RemoveItemFromSlot(false);
            return;
        }
    }

    void SlotSecondaryPress()
    {
        // -- The clicked slot was not empty --
        if (!inventorySlot.isEmpty)
        {
            // --- Mouse doesn't have an item stack ---
            if (GameplayUtils.instance.inventoryManager.MouseSlot.inventorySlot.isEmpty)
            {
                // -- If the clicked slot has more then 1 item split it in half
                if (inventorySlot.inventoryItemStack.Amount > 1)
                {
                    int splitAmount = inventorySlot.inventoryItemStack.Amount / 2;
                    int remainingAmount = inventorySlot.inventoryItemStack.Amount - splitAmount;


                    ItemData itemData = GameplayUtils.instance.GetItemDataByID(inventorySlot.inventoryItemStack.ID);

                    GameplayUtils.instance.inventoryManager.AddItemToMouseSlot(itemData, splitAmount);
                    RemoveItemFromSlot(false);
                    GameplayUtils.instance.inventoryManager.AddItemToSlot(inventorySlot, itemData, remainingAmount);
                }
                else
                {
                    ItemData itemData = GameplayUtils.instance.GetItemDataByID(inventorySlot.inventoryItemStack.ID);
                    GameplayUtils.instance.inventoryManager.AddItemToMouseSlot(itemData, inventorySlot.inventoryItemStack.Amount);
                    RemoveItemFromSlot(false);
                }

            }
            // --- The Mouse does have an item ---
            else
            {
                InventoryItemStack mouseStack = GameplayUtils.instance.inventoryManager.MouseSlot.inventorySlot.inventoryItemStack;
                ItemData mouseItemData = GameplayUtils.instance.GetItemDataByID(mouseStack.ID);

                InventoryItemStack slotStack = inventorySlot.inventoryItemStack;
                ItemData slotItemData = GameplayUtils.instance.GetItemDataByID(slotStack.ID);

                // -- Mouse Slot and Clicked Slot both have the same type of item;
                if (mouseStack.ID == slotStack.ID)
                {
                    if (slotStack.Amount < slotStack.MaxStackSize)
                    {
                        GameplayUtils.instance.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, slotStack.Amount + 1, true);
                        // -- If the mouse slot only has one item remove it;
                        if (mouseStack.Amount - 1 <= 0)
                        {
                            GameplayUtils.instance.inventoryManager.MouseSlot.RemoveItemFromSlot(false);
                        }
                        // -- If the mouse slot has more then one item remove one item from it
                        else
                        {
                            GameplayUtils.instance.inventoryManager.AddItemToMouseSlot(mouseItemData, mouseStack.Amount - 1, true);
                        }

                    }
                }
                // -- Swaping items
                else
                {
                    GameplayUtils.instance.inventoryManager.AddItemToSlot(inventorySlot, mouseItemData, mouseStack.Amount, true);
                    GameplayUtils.instance.inventoryManager.AddItemToMouseSlot(slotItemData, slotStack.Amount, true);
                }
            }
        }
        // -- The clicked slot was empty --
        else
        {
            // -- The mouse has an item
            if (!GameplayUtils.instance.inventoryManager.MouseSlot.inventorySlot.isEmpty)
            {
                InventoryItemStack mouseStack = GameplayUtils.instance.inventoryManager.MouseSlot.inventorySlot.inventoryItemStack;
                ItemData mouseItemData = GameplayUtils.instance.GetItemDataByID(mouseStack.ID);

                GameplayUtils.instance.inventoryManager.AddItemToSlot(inventorySlot, mouseItemData, 1);
                if (mouseStack.Amount - 1 <= 0)
                {
                    GameplayUtils.instance.inventoryManager.MouseSlot.RemoveItemFromSlot(false);
                }
                else
                {
                    GameplayUtils.instance.inventoryManager.AddItemToMouseSlot(mouseItemData, mouseStack.Amount - 1, true);
                }
            }
        }
    }



    public void RemoveItemFromSlot(bool dropInWorld)
    {
        if (dropInWorld) DropItem();

        inventorySlot.inventoryItemStack = new InventoryItemStack();
        inventorySlot.isEmpty = true;
        setSlotText("");
        setSlotAmountText(0);
        slotImage.sprite = null;
        slotImage.color = new Color(0, 0, 0, 0);
        slotEmptied?.Invoke();
        if (IsHotbar)
        {
            HotbarManager.instance.ClearSlotVisuals(SlotID);
        }
    }

    public void DropItem(int amount = -1)
    {
        if (amount == -1) amount = inventorySlot.inventoryItemStack.Amount;
        GameplayUtils.instance.PlayerDropItem(inventorySlot.inventoryItemStack.ID, amount);
    }

    public void SetSlotFilled(string _name, int _amount, Sprite sprite)
    {
        //setSlotText(_name);
        setSlotAmountText(_amount);
        slotImage.color = new Color(1, 1, 1, 1);
        slotImage.sprite = sprite;
        slotFilled?.Invoke();
        if (IsHotbar)
        {
            HotbarManager.instance.SetSlotVisuals(SlotID,this);
         }
    }

    public void SetSlotFilled(int _amount)
    {
        setSlotAmountText(_amount);
        slotFilled?.Invoke();
        if (IsHotbar)
        {
            HotbarManager.instance.SetSlotVisuals(SlotID, this);
        }
    }

    public void setSlotText(string _text)
    {
        slotText.text = _text;
    }

    public void setSlotAmountText(int amount_text)
    {
        string newText = amount_text.ToString();
        if (amount_text == 0) newText = "";

        slotAmountText.text = newText;
     }
}
