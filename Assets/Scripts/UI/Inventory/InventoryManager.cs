using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public InputAction toggleInventoryAction;

    [HideInInspector] public bool inventoryIsOpen;

    [Header("InventoryObjects")]
    public GameObject InventoryObject;
    public GameObject InventorySlotsParent;
    public GameObject InventoryHotbarSlotsParent;
    public GameObject InventorySlotPrefab;

    enum InventoryMenus { none, Inventory, Quests }
    InventoryMenus openMenu = InventoryMenus.none;

    [SerializeField] public InventorySlotComponent MouseSlot;
    public int TotalInventorySlots = 30;
    public int TotalHotbarSlots = 5;

    public Action OnInventoryClosed;
    public Action OnInventoryUpdated;

    float inventoryToggleCooldown;

    [Header("InventorySlots")]
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public Dictionary<string, int> specialItems = new Dictionary<string, int>();

    [Space(20)]
    [Header("QuestMenu")]
    public GameObject QuestMenuObject;
    public QuestInfoBox questInfoBox;
    public GameObject QuestItemsParent;
    [SerializeField] GameObject QuestButton;
    [SerializeField] GameObject PinQuestButton;
    [SerializeField] TextMeshProUGUI PinQuestText;
    List<QuestInfo> activeQuests = new List<QuestInfo>();

    [Header("Pinned Quests")]
    public Transform PinnedQuestsParent;
    public GameObject PinnedQuestPrefab;
    public List<PinnedQuestItem> activedPinnedQuests = new List<PinnedQuestItem>();
    public QuestData selectedQuest;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameplayInput.instance.playerInput.actions["Inventory"].performed += ToggleInventory;
        GameplayInput.instance.playerInput.actions["Submit"].performed += (context) => { CloseInventory(); };
        GameplayInput.instance.playerInput.actions["Quests"].performed += ToggleQuests;
        GameplayInput.instance.playerInput.actions["CloseQuests"].performed += ToggleQuests;
        GameplayInput.instance.playerInput.actions["CloseInventory"].performed += ToggleInventory;
        for (int i = 0; i < TotalInventorySlots + TotalHotbarSlots; i++)
        {
            if (i < TotalHotbarSlots)
            {
                AddHotbarSlots(i);
                continue;
             }
            AddInventorySlot(i);
        }
        OnInventoryUpdated += QuestsObjectiveCheck;
        FlagManager.OnFlagSet += QuestsObjectiveCheck;
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
                    OpenInventory();

                }
                else
                {
                    switch (openMenu)
                    {
                        case InventoryMenus.none:
                        case InventoryMenus.Inventory:
                            CloseInventory();
                            break;
                        case InventoryMenus.Quests:
                            SwitchToInventoryMenu();
                            break;
                    }
                }

            }
        }
    }

    void ToggleQuests(InputAction.CallbackContext context)
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
                    OpenInventory();
                    SwitchToQuestsMenu();
                }
                else
                {
                    switch (openMenu)
                    {
                        case InventoryMenus.none:
                        case InventoryMenus.Quests:
                            CloseInventory();
                            break;
                        case InventoryMenus.Inventory:
                            SwitchToQuestsMenu();
                            break;
                    }
                }

            }
        }
    }

    public void OpenInventory()
    {
        if (InventoryObject == null) return;
        if (InventoryObject.TryGetComponent(out CanvasGroup canvasGroup))
        {
            InventoryObject.SetActive(true);
            if (GameplayUtils.instance.GetOpenMenu()) return;
            //print("opening inventory");
            UIInputHandler.instance.defaultButton = InventorySlotsParent.transform.GetChild(0).gameObject;
            GameplayUtils.instance.CloseAllCraftingMenus();
            GameplayUtils.instance.OpenMenu();


            inventoryIsOpen = true;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            SwitchToInventoryMenu();

            openMenu = InventoryMenus.Inventory;

        }
    }

    public void CloseInventory()
    {
        if (openMenu == InventoryMenus.none) return;
        if (InventoryObject.TryGetComponent(out CanvasGroup canvasGroup))
        {
            SwitchToInventoryMenu();
            InventoryObject.SetActive(false);
            //print("closing inventory");
            GameplayUtils.instance.CloseMenu();
            OnInventoryClosed?.Invoke();
            UIInputHandler.instance.defaultButton = null;
            if (QuestMenuObject.TryGetComponent(out CanvasGroup QuestCanvasGroup))
            {
                QuestCanvasGroup.alpha = 0;
                QuestCanvasGroup.interactable = false;
                QuestCanvasGroup.blocksRaycasts = false;
                questInfoBox.ClearQuestInfo();
                selectedQuest = null;
            }
            inventoryIsOpen = false;
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            GameplayUtils.instance.CloseAllCraftingMenus();

            openMenu = InventoryMenus.none;
            ItemTitlePopupManager.instance.HidePopup();
        }

    }


    public void SwitchToQuestsMenu()
    {
        InventoryObject.SetActive(false);
        CanvasGroup inventroyCanvasGroup = InventoryObject.GetComponent<CanvasGroup>();
        inventroyCanvasGroup.alpha = 0;
        inventroyCanvasGroup.blocksRaycasts = false;
        inventroyCanvasGroup.interactable = false;

        QuestMenuObject.SetActive(true);
        CanvasGroup questsCanvasGroup = QuestMenuObject.GetComponent<CanvasGroup>();
        questsCanvasGroup.alpha = 1;
        questsCanvasGroup.blocksRaycasts = true;
        questsCanvasGroup.interactable = true;

        openMenu = InventoryMenus.Quests;
    }

    public void SwitchToInventoryMenu()
    {
        InventoryObject.SetActive(true);
        CanvasGroup inventroyCanvasGroup = InventoryObject.GetComponent<CanvasGroup>();
        inventroyCanvasGroup.alpha = 1;
        inventroyCanvasGroup.blocksRaycasts = true;
        inventroyCanvasGroup.interactable = true;


        QuestMenuObject.SetActive(false);
        CanvasGroup questsCanvasGroup = QuestMenuObject.GetComponent<CanvasGroup>();
        questsCanvasGroup.alpha = 0;
        questsCanvasGroup.blocksRaycasts = false;
        questsCanvasGroup.interactable = false;

        selectedQuest = null;
        PinQuestButton.SetActive(false);
        questInfoBox.ClearQuestInfo();

        foreach (CraftingTableRecipeDisplay display in GameplayUtils.instance.craftingTableRecipeDisplays)
        {
            display.HideRecipe();
            display.recipesBookManager.HideRecipeBook();
        }

        openMenu = InventoryMenus.Inventory;
    }

    /* #region Inventory */
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

    void AddHotbarSlots(int slotID)
    {
        InventorySlot inventorySlot = new InventorySlot();
        //inventorySlot.inventoryItemStack = new InventoryItemStack();

        inventorySlots.Add(inventorySlot);
        GameObject slot = Instantiate(InventorySlotPrefab, InventoryHotbarSlotsParent.transform);
        InventorySlotComponent inventorySlotComponent = slot.GetComponent<InventorySlotComponent>();
        inventorySlotComponent.SlotID = slotID;
        inventorySlotComponent.IsHotbar = true;
        inventorySlotComponent.inventorySlot = inventorySlot;
        inventorySlot.inventorySlotComponent = inventorySlotComponent;
    }

    public void AddItemToMouseSlot(ItemData itemData, int amount = 1, bool force = false)
    {
        if (force || MouseSlot.inventorySlot.isEmpty)
        {
            MouseSlot.inventorySlot.isEmpty = false;
            InventoryItemStack newItemStack = new InventoryItemStack(itemData.item_id, itemData.item_name, amount, itemData.MaxStackSize);
            MouseSlot.inventorySlot.inventoryItemStack = newItemStack;
            MouseSlot.inventorySlot.inventorySlotComponent.SetSlotFilled(itemData.item_name, amount, itemData.item_ui_image);
        }
    }

    public void AddItemToSlot(InventorySlot inventorySlot, ItemData itemData, int amount = 1, bool force = false,bool broadcastEvent = true)
    {
        if (force || inventorySlot.isEmpty)
        {
            inventorySlot.isEmpty = false;
            InventoryItemStack newItemStack = new InventoryItemStack(itemData.item_id, itemData.item_name, amount, itemData.MaxStackSize);
            inventorySlot.inventoryItemStack = newItemStack;
            if (inventorySlot.inventorySlotComponent != null)
            {
                inventorySlot.inventorySlotComponent.SetSlotFilled(itemData.item_name, amount, itemData.item_ui_image,broadcastEvent);
            }
            OnInventoryUpdated?.Invoke();
        }
    }

    public int AddItemToInventory(ItemData itemData, int amount = 1)
    {

        int remainingAmount = amount;
        InventorySlot inventorySlot = GetSlotWithItemButNotFull(itemData.item_id);
        if (inventorySlot != null)
        {
            for (int i = 0; i < InventorySlotsParent.transform.childCount; i++)
            {
                int amountCanAdd = inventorySlot.inventoryItemStack.MaxStackSize - inventorySlot.inventoryItemStack.Amount;
                // -- Remaining items can fit into the found slot --
                if (remainingAmount <= amountCanAdd)
                {
                    int finalAmount = remainingAmount + inventorySlot.inventoryItemStack.Amount;
                    inventorySlot.inventoryItemStack.Amount = finalAmount;
                    inventorySlot.inventorySlotComponent.SetSlotFilled(finalAmount);
                    OnInventoryUpdated?.Invoke();
                    return 0;
                }
                // -- Only some of the items can fit into the slot --
                else
                {
                    int newAmount = inventorySlot.inventoryItemStack.Amount + amountCanAdd;
                    remainingAmount -= amountCanAdd;
                    inventorySlot.inventoryItemStack.Amount = newAmount;
                    inventorySlot.inventorySlotComponent.SetSlotFilled(newAmount);
                }
                inventorySlot = GetSlotWithItemButNotFull(itemData.item_id);
                if (inventorySlot == null) break;
            }
            if (remainingAmount <= 0)
            {
                OnInventoryUpdated?.Invoke();
                return 0;
            }
        }

        // --- There isn't an exisiting stack of items we can add to ---

        for (int i = 0; i < InventorySlotsParent.transform.childCount; i++)
        {
            // -- Get the next empty slot --
            inventorySlot = GetFirstEmptySlot();
            // If there are not empty slots then break from the loop
            if (inventorySlot == null)
            {
                break;
            }

            // -- Add the correct amount to the slot --
            int amountToAdd = Math.Min(remainingAmount, itemData.MaxStackSize);
            remainingAmount -= amountToAdd;
            inventorySlot.isEmpty = false;
            InventoryItemStack newItemStack = new InventoryItemStack(itemData.item_id, itemData.item_name, amountToAdd, itemData.MaxStackSize);
            inventorySlot.inventoryItemStack = newItemStack;
            inventorySlot.inventorySlotComponent.SetSlotFilled(itemData.item_name, amountToAdd, itemData.item_ui_image);

            if (remainingAmount <= 0) break;


        }
        OnInventoryUpdated?.Invoke();
        return remainingAmount;
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

    public int GetSpecialItemAmount(string item_id)
    {
        if (specialItems.ContainsKey(item_id))
        {
            return specialItems[item_id];
        }
        return -1;
    }

    public void AddSpecialItem(string item_id, int amount)
    {
        if (specialItems.ContainsKey(item_id))
        {
            specialItems[item_id] += amount;
        }
        else
        {
            specialItems[item_id] = amount;
        }
        OnInventoryUpdated?.Invoke();

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
                    remainingAmount -= inventorySlots[i].inventoryItemStack.Amount;
                    inventorySlots[i].inventorySlotComponent.RemoveItemFromSlot(false);
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
        OnInventoryUpdated?.Invoke();
        return false;
    }
    /* #endregion */

    /* #region Quests */
    public void ShowQuestInfo(QuestData questData)
    {
        questInfoBox.ShowQuestInfo(questData);
        selectedQuest = questData;
        if (PinQuestButton != null)
        {
            PinQuestButton.SetActive(true);
            PinQuestText.text = "Pin Quest";
            foreach (PinnedQuestItem pinnedQuestItem in activedPinnedQuests)
            {
                if (pinnedQuestItem.storedQuestData == questData)
                {
                    PinQuestText.text = "Unpin Quest";
                }
            }
        }
    }


    public void AssignQuest(string quest_id)
    {
        QuestData[] allQuests = Resources.LoadAll<QuestData>("QuestData");
        foreach (var data in allQuests)
        {
            if (data.ID == quest_id)
            {
                foreach (QuestInfo info in activeQuests)
                {
                    if (info.QuestData == data) return;
                }
                GameObject questButton = Instantiate(QuestButton, QuestItemsParent.transform);

                QuestInfoButton questInfoButton = questButton.GetComponent<QuestInfoButton>();
                questInfoButton.QuestID = quest_id;
                questInfoButton.questData = data;
                questInfoButton.buttonName.text = $"<font-weight=700>!</font-weight> {data.QuestName}";

                QuestInfo questInfo = new QuestInfo(data);

                foreach (QuestObjective questObjective in data.questObjectives)
                {
                    questObjective.isComplete = false;
                }

                activeQuests.Add(questInfo);

                if (data.AutoPinQuest)
                {
                    AddNewPinnedQuest(data);
                }

                GameplayUtils.instance.ShowCustomNotif($"Quest Added {data.QuestName}", 6);
            }
        }
        //return null;
    }

    public void QuestsObjectiveCheck()
    {
        UpdatePinnedQuests();
        foreach (QuestInfo info in activeQuests)
        {
            if (info.IsComplete) continue;
            bool questComplete = true;
            for (int i = 0; i < info.QuestData.questObjectives.Count; i++)
            {
                if (!info.QuestData.questObjectives[i].ObjectiveComplete())
                {
                    questComplete = false;
                    break;
                } 
                // switch (info.QuestData.questObjectives[i])
                // {
                //     case ObjectiveCollectItems objectiveCollect:
                //         if (GameplayUtils.instance.get_item_holding_amount(objectiveCollect.Item_ID) >= objectiveCollect.Item_Amount)
                //         {
                //             continue;
                //         }
                //         return;

                //     case ObjectiveTalkToNPCFlag objectiveTalkToNPCFlag:
                //         if (FlagManager.Get_Flag_Value(objectiveTalkToNPCFlag.flag_id))
                //         {
                //             continue;
                //         }
                //         return;
                //     case ObjectiveUseInput objectiveUseInput:
                //         if (!objectiveUseInput.ObjectiveComplete()) return;
                //         break;
                // }
            }
            if (questComplete)
            {
                print("quest competed!");
                info.IsComplete = true;
                GameplayUtils.instance.ShowCustomNotif("Quest Complete!", 8);
                RemovePinnedQuest(info.QuestData);
                for (int i = 0; i < QuestItemsParent.transform.childCount; i++)
                {
                    if (QuestItemsParent.transform.GetChild(i).TryGetComponent(out QuestInfoButton questInfoButton))
                    {
                        questInfoButton.SetComplete();
                    }
                }
            }
        }
    }

    void UpdatePinnedQuests()
    {
        foreach (PinnedQuestItem pinnedQuestItem in activedPinnedQuests)
        {
            pinnedQuestItem.UpdateText();
         }
     }

    public void AddNewPinnedQuest(QuestData questData)
    {
        foreach (PinnedQuestItem item in activedPinnedQuests)
        {
            if (item.storedQuestData == questData)
            {
                Destroy(item.gameObject);
                activedPinnedQuests.Remove(item);
                PinQuestText.text = "Pin Quest";
                return;
            }
        }
        GameObject pinnedQuest = Instantiate(PinnedQuestPrefab, PinnedQuestsParent);
        PinnedQuestItem pinnedQuestItem = pinnedQuest.GetComponent<PinnedQuestItem>();

        pinnedQuestItem.SetQuestText(questData);
        activedPinnedQuests.Add(pinnedQuestItem);

        PinQuestText.text = "Unpin Quest";
    }

    void RemovePinnedQuest(QuestData questData)
    {
        foreach (PinnedQuestItem item in activedPinnedQuests)
        {
            if (item.storedQuestData == questData)
            {
                Destroy(item.gameObject);
                activedPinnedQuests.Remove(item);
                PinQuestText.text = "Pin Quest";
                return;
            }
        }
    }

    public void PinSelectedQuest()
    {
        if (selectedQuest == null) return;
        AddNewPinnedQuest(selectedQuest);
    }

    /* #endregion */
}

[Serializable]
public class InventorySlot
{
    public bool isEmpty = true;
    public int slot_id;
    public InventoryItemStack inventoryItemStack;
    public InventorySlotComponent inventorySlotComponent;

    public InventorySlot()
    {

    }

    public InventorySlot(int _id)
    {
        slot_id = _id;
        inventoryItemStack = new InventoryItemStack(0);
    }

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

    public InventoryItemStack(int _amount)
    {
        Amount = _amount;
    }

    public InventoryItemStack(string _id, string _name, int _amount, int _maxStack = 5)
    {
        ID = _id;
        Name = _name;
        Amount = _amount;
        MaxStackSize = _maxStack;
    }
}

[Serializable]
public class QuestInfo
{
    public QuestData QuestData;
    public bool IsComplete;

    public QuestInfo(QuestData _questData, bool _complete = false)
    {
        QuestData = _questData;
        IsComplete = _complete;
     }
}


