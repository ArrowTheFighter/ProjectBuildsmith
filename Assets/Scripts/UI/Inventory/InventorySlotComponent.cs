using System;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventorySlotComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public InventorySlot inventorySlot;
    public int SlotID;
    public bool  IsHotbar;
    public bool Selected;
    public bool PlayerCanPlace = true;
    public Image slotImage;
    public TextMeshProUGUI slotText;
    public TextMeshProUGUI slotAmountText;

    public Action<InventoryItemStack> slotEmptied;
    public Action slotFilled;

    void Awake()
    {
        ScriptRefrenceSingleton.OnScriptLoaded += BindInputs;
    }

    void BindInputs()
    {
        ScriptRefrenceSingleton.OnScriptLoaded -= BindInputs;

        var gameplayInput = ScriptRefrenceSingleton.instance.gameplayInput;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Submit"].performed += ControllerMainPress;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Cancel"].performed += ControllerSecondaryPress;
        ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.OnInventoryClosed += UnsubscribeFromItemPopups;
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu += UnBindInputs;
    }

    void UnBindInputs()
    {
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu -= UnBindInputs;

        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Submit"].performed -= ControllerMainPress;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Cancel"].performed -= ControllerSecondaryPress;
        ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.OnInventoryClosed -= UnsubscribeFromItemPopups;
    }

    void Start()
    {
        
        if (inventorySlot.inventorySlotComponent == null)
        {
            inventorySlot.inventorySlotComponent = this;
        }
        //ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.OnInventoryClosed += UnsubscribeFromItemPopups;
        SelectionWatcher.OnSelectionChanged += HandleSelectionChange;
    }

    void OnEnable()
    {
        Invoke(nameof(BindInputs), 0f); // Schedules after this frame
    }

    

    

    void HandleSelectionChange(GameObject _gameObject)
    {
        if (ScriptRefrenceSingleton.instance.uIInputHandler.currentScheme == "Gamepad" && gameObject == _gameObject)
        {
            if (!Selected)
            {
                slotEmptied += HideAndUnsubscribeItemPopup;
                slotFilled += ShowAndUnsubscribeItemPopup;
                Selected = true;
                if (inventorySlot.isEmpty)
                {
                    ScriptRefrenceSingleton.instance.itemTitlePopupManager.HidePopup();

                }
                else 
                {
                    ScriptRefrenceSingleton.instance.itemTitlePopupManager.ShowPopup(inventorySlot.inventoryItemStack.Name);
                    
                }
            }
        }
        else if(Selected)
        {
            Selected = false;
            slotEmptied -= HideAndUnsubscribeItemPopup;
            slotFilled -= ShowAndUnsubscribeItemPopup;
        }
    }

    void UnsubscribeFromItemPopups()
    {
        if (slotEmptied != null)
        {
            var targetMethod = GetType()
            .GetMethod(nameof(HideAndUnsubscribeItemPopup),
                       System.Reflection.BindingFlags.NonPublic |
                       System.Reflection.BindingFlags.Instance);

            foreach (var sub in slotEmptied.GetInvocationList())
            {
                if (sub.Method == targetMethod)
                {
                    slotEmptied -= (Action<InventoryItemStack>)sub;
                }
            }   
         }
        if (slotFilled != null)
        {

            var targetMethod = GetType()
           .GetMethod(nameof(ShowAndUnsubscribeItemPopup),
                      System.Reflection.BindingFlags.NonPublic |
                      System.Reflection.BindingFlags.Instance);

            foreach (var sub in slotFilled.GetInvocationList())
            {
                if (sub.Method == targetMethod)
                {
                    slotEmptied -= (Action<InventoryItemStack>)sub;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        slotFilled += ShowAndUnsubscribeItemPopup;
        if (inventorySlot.isEmpty) return;
        ScriptRefrenceSingleton.instance.itemTitlePopupManager.ShowPopup(inventorySlot.inventoryItemStack.Name);
        slotEmptied += HideAndUnsubscribeItemPopup;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ScriptRefrenceSingleton.instance.itemTitlePopupManager.HidePopup();
        slotEmptied -= HideAndUnsubscribeItemPopup;
        slotFilled -= ShowAndUnsubscribeItemPopup;
    }

    public void HideAndUnsubscribeItemPopup(InventoryItemStack inventoryItemStack)
    {
        ScriptRefrenceSingleton.instance.itemTitlePopupManager.HidePopup();
        //slotEmptied -= HideAndUnsubscribeItemPopup;
    }

    public void ShowAndUnsubscribeItemPopup()
    {
        ScriptRefrenceSingleton.instance.itemTitlePopupManager.ShowPopup(inventorySlot.inventoryItemStack.Name);
        //slotFilled -= ShowAndUnsubscribeItemPopup;
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

    void ControllerMainPress(InputAction.CallbackContext context)
    {
        if (gameObject == null || this == null) return;
        if (!gameObject.activeInHierarchy) return;
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            SlotMainPress();
        }
     }

    void ControllerSecondaryPress(InputAction.CallbackContext context)
    {
        if (gameObject == null || this == null) return;
        if (!gameObject.activeInHierarchy) return;
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            SlotSecondaryPress();
        }
    }

    void SlotMainPress()
    {
        // --- Mouse has an item stack ---
        if (!ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.inventorySlot.isEmpty)
        {
            // -- Mouse has an item but the click slot is empty --
            if (inventorySlot.isEmpty)
            {
                if (!PlayerCanPlace) return;
                InventoryItemStack itemStack = ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.inventorySlot.inventoryItemStack;
                ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(itemStack.ID);
                ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, itemData, itemStack.Amount);
                ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.inventorySlot.inventorySlotComponent.RemoveItemFromSlot(false);
                return;
            }
            // -- Mouse has an item and the slot has an item --
            else
            {
                InventoryItemStack mouseStack = ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.inventorySlot.inventoryItemStack;
                ItemData mouseItemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(mouseStack.ID);

                InventoryItemStack slotStack = inventorySlot.inventoryItemStack;
                ItemData slotItemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(slotStack.ID);

                // -- Both stacks have the same ID
                if (mouseStack.ID == slotStack.ID)
                {

                    if (!PlayerCanPlace)
                    {
                        int amountToMax = mouseStack.MaxStackSize - mouseStack.Amount;
                        int newAmount = slotStack.Amount + mouseStack.Amount;
                        if (slotStack.Amount <= amountToMax)
                        {
                            /// -- Move mouse stack fully into the clicked slot
                            //ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, newAmount, true);
                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(slotItemData, newAmount, true);
                            RemoveItemFromSlot(false);
                            //ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.RemoveItemFromSlot(false);
                        }
                        else
                        {
                            int leftOver = newAmount - mouseStack.MaxStackSize;

                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, leftOver, true);

                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(mouseItemData, mouseStack.MaxStackSize, true);
                        }
                        return;
                    }
                    // -- If the clicked slot is not a full stack
                    if (slotStack.Amount < slotStack.MaxStackSize)
                    {
                        int amountToMax = slotStack.MaxStackSize - slotStack.Amount;

                        int newAmount = mouseStack.Amount + slotStack.Amount;
                        // -- If the amount in the mouse slot can fit full in the new stack
                        if (mouseStack.Amount <= amountToMax)
                        {
                            /// -- Move mouse stack fully into the clicked slot
                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, newAmount, true);
                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.RemoveItemFromSlot(false);
                        }
                        else
                        {
                            int leftOver = newAmount - slotStack.MaxStackSize;

                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, slotStack.MaxStackSize, true);

                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(mouseItemData, leftOver, true);
                        }
                    }
                    else
                    {
                        if (!PlayerCanPlace) return;
                        RemoveItemFromSlot(false);
                        ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.RemoveItemFromSlot(false);

                        ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, mouseItemData, mouseStack.Amount);
                        ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(slotItemData, slotStack.Amount);
                    }

                    return;
                }

                if (!PlayerCanPlace) return;
                // -- Swaping Items --
                RemoveItemFromSlot(false);
                ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.RemoveItemFromSlot(false);

                ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, mouseItemData, mouseStack.Amount);
                ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(slotItemData, slotStack.Amount);
            }
        }
        else if (!inventorySlot.isEmpty)
        {
            ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(inventorySlot.inventoryItemStack.ID);
            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(itemData, inventorySlot.inventoryItemStack.Amount);
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
            if (ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.inventorySlot.isEmpty)
            {
                // -- If the clicked slot has more then 1 item split it in half
                if (inventorySlot.inventoryItemStack.Amount > 1 && PlayerCanPlace)
                {
                    int splitAmount = inventorySlot.inventoryItemStack.Amount / 2;
                    int remainingAmount = inventorySlot.inventoryItemStack.Amount - splitAmount;


                    ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(inventorySlot.inventoryItemStack.ID);

                    ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(itemData, splitAmount);
                    RemoveItemFromSlot(false);
                    ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, itemData, remainingAmount);
                }
                else
                {
                    ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(inventorySlot.inventoryItemStack.ID);
                    ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(itemData, inventorySlot.inventoryItemStack.Amount);
                    RemoveItemFromSlot(false);
                }

            }
            // --- The Mouse does have an item ---
            else
            {
                InventoryItemStack mouseStack = ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.inventorySlot.inventoryItemStack;
                ItemData mouseItemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(mouseStack.ID);

                InventoryItemStack slotStack = inventorySlot.inventoryItemStack;
                ItemData slotItemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(slotStack.ID);

                // -- Mouse Slot and Clicked Slot both have the same type of item;
                if (mouseStack.ID == slotStack.ID)
                {
                    if (!PlayerCanPlace)
                    {
                        if (mouseStack.Amount < mouseStack.MaxStackSize)
                        {
                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(mouseItemData, mouseStack.Amount + 1, true);

                            if (slotStack.Amount == 1)
                            {
                                RemoveItemFromSlot(false);
                            }
                            else
                            {
                                ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, slotStack.Amount - 1, true);
                            }
                        }
                        return;
                        
                    }
                    if (slotStack.Amount < slotStack.MaxStackSize && PlayerCanPlace)
                    {
                        ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, slotStack.Amount + 1, true);
                        // -- If the mouse slot only has one item remove it;
                        if (mouseStack.Amount - 1 <= 0)
                        {
                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.RemoveItemFromSlot(false);
                        }
                        // -- If the mouse slot has more then one item remove one item from it
                        else
                        {
                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(mouseItemData, mouseStack.Amount - 1, true);
                        }

                    }
                    else
                    {

                        if (mouseStack.Amount + slotStack.Amount <= mouseStack.MaxStackSize)
                        {
                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(mouseItemData, mouseStack.Amount + slotStack.Amount, true);

                            if (slotStack.Amount == 1)
                            {
                                RemoveItemFromSlot(false);
                            }
                            else
                            {
                                ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, slotStack.Amount - 1, true);
                            }
                        }
                        else
                        {
                            int remainingSpace = mouseStack.MaxStackSize - mouseStack.Amount;
                            int remainingItems = slotStack.Amount - remainingSpace;
                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(mouseItemData, mouseStack.MaxStackSize, true);
                            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, slotItemData, remainingItems, true);
                        }
                    }
                }
                // -- Swaping items
                else
                {
                    ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, mouseItemData, mouseStack.Amount, true);
                    ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(slotItemData, slotStack.Amount, true);
                }
            }
        }
        // -- The clicked slot was empty --
        else
        {
            if (!PlayerCanPlace) return;
            // -- The mouse has an item
            if (!ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.inventorySlot.isEmpty)
            {
                InventoryItemStack mouseStack = ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.inventorySlot.inventoryItemStack;
                ItemData mouseItemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(mouseStack.ID);

                ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(inventorySlot, mouseItemData, 1);
                if (mouseStack.Amount - 1 <= 0)
                {
                    ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.MouseSlot.RemoveItemFromSlot(false);
                }
                else
                {
                    ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToMouseSlot(mouseItemData, mouseStack.Amount - 1, true);
                }
            }
        }
    }



    public void RemoveItemFromSlot(bool dropInWorld, bool broadcastEvent = true)
    {
        if (dropInWorld) DropItem();
        bool shouldBroadcast = (!string.IsNullOrEmpty(inventorySlot.inventoryItemStack.ID) && broadcastEvent);
        InventoryItemStack oldStack = new InventoryItemStack(
            inventorySlot.inventoryItemStack.ID,
            inventorySlot.inventoryItemStack.Name,
            inventorySlot.inventoryItemStack.Amount,
            inventorySlot.inventoryItemStack.MaxStackSize
        );
        inventorySlot.inventoryItemStack = new InventoryItemStack(0);
        inventorySlot.isEmpty = true;
        setSlotText("");
        setSlotAmountText(0);
        slotImage.sprite = null;
        slotImage.color = new Color(0, 0, 0, 0);

        if (IsHotbar)
        {
            ScriptRefrenceSingleton.instance.hotbarManager.ClearSlotVisuals(SlotID);
        }
        if (shouldBroadcast) slotEmptied?.Invoke(oldStack);
    }

    public void DropItem(int amount = -1)
    {
        if (amount == -1) amount = inventorySlot.inventoryItemStack.Amount;
        ScriptRefrenceSingleton.instance.gameplayUtils.PlayerDropItem(inventorySlot.inventoryItemStack.ID, amount);
    }

    public void SetSlotFilled(string _name, int _amount, Sprite sprite,bool broadcastEvent = true)
    {
        //setSlotText(_name);
        setSlotAmountText(_amount);
        slotImage.color = new Color(1, 1, 1, 1);
        slotImage.sprite = sprite;
        if(broadcastEvent) slotFilled?.Invoke();
        if (IsHotbar)
        {
            ScriptRefrenceSingleton.instance.hotbarManager.SetSlotVisuals(SlotID,this);
         }
    }

    public void SetSlotFilled(int _amount)
    {
        setSlotAmountText(_amount);
        slotFilled?.Invoke();
        if (IsHotbar)
        {
            ScriptRefrenceSingleton.instance.hotbarManager.SetSlotVisuals(SlotID, this);
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
