using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
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
    public GameObject InventoryMenuDefaultButton;

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

    [Header("Unlocked Recipes")]
    public List<string> UnlockedRecipes = new List<string>();

    [Space(20)]
    [Header("QuestMenu")]
    public GameObject QuestMenuObject;
    public QuestInfoBox questInfoBox;
    public GameObject QuestItemsParent;
    public GameObject QuestMenuDefaultButton;
    [SerializeField] GameObject QuestButton;
    [SerializeField] GameObject PinQuestButton;
    [SerializeField] TextMeshProUGUI PinQuestText;
    public List<QuestInfo> activeQuests = new List<QuestInfo>();
    public List<QuestInfo> CompletedQuests = new List<QuestInfo>();

    [Header("Pinned Quests")]
    public Transform PinnedQuestsParent;
    public GameObject PinnedQuestPrefab;
    public List<PinnedQuestItem> activedPinnedQuests = new List<PinnedQuestItem>();
    public QuestData selectedQuest;

    void Awake()
    {
        ScriptRefrenceSingleton.OnScriptLoaded += BindInputs;
    }

    void BindInputs()
    {
        ScriptRefrenceSingleton.OnScriptLoaded -= BindInputs;

        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Inventory"].performed += ToggleInventory;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Submit"].performed += CloseInventoryFromInteract;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Quests"].performed += ToggleQuests;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["CloseQuests"].performed += ToggleQuests;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["CloseInventory"].performed += ToggleInventory;

        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu += UnBindInputs;
    }
    
    void UnBindInputs()
    {
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu -= UnBindInputs;

        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Inventory"].performed -= ToggleInventory;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Submit"].performed -= CloseInventoryFromInteract;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Quests"].performed -= ToggleQuests;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["CloseQuests"].performed -= ToggleQuests;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["CloseInventory"].performed -= ToggleInventory;

        FlagManager.OnFlagSet -= QuestsObjectiveCheck;
        ScriptRefrenceSingleton.instance.saveLoadManager.OnSaveLoaded -= SaveLoaded;
        OnInventoryUpdated -= QuestsObjectiveCheck;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

        ScriptRefrenceSingleton.instance.saveLoadManager.OnSaveLoaded += SaveLoaded;
    }
    
    void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        //Load special items
        foreach (KeyValuePair<string, int> item in saveFileStruct.special_items)
        {
            AddSpecialItem(item.Key, item.Value);
        }
        //Load inventory items
        foreach(SaveableInventroySlot slot in saveFileStruct.inventory_slots)
        {
            if (slot.isEmpty) continue;
            ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(slot.inventoryItemStack.ID);
            AddItemToSlotByID(slot.slot_id, itemData, slot.inventoryItemStack.Amount, true, false);
        }
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
                    ScriptRefrenceSingleton.instance.gameplayUtils.OpenCraftingMenu(CraftingStationTypes.Tool);

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
            if (ScriptRefrenceSingleton.instance.gameplayUtils.GetOpenMenu()) return;
            //print("opening inventory");
            ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = InventorySlotsParent.transform.GetChild(0).gameObject;
            ScriptRefrenceSingleton.instance.gameplayUtils.CloseAllCraftingMenus();
            ScriptRefrenceSingleton.instance.gameplayUtils.OpenMenu();


            inventoryIsOpen = true;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            SwitchToInventoryMenu();

            openMenu = InventoryMenus.Inventory;

        }
    }

    void CloseInventoryFromInteract(InputAction.CallbackContext context)
    {
        if (ScriptRefrenceSingleton.instance.uIInputHandler.currentScheme == "Keyboard&Mouse") CloseInventory();
    }

    public void CloseInventory()
    {
        if (openMenu == InventoryMenus.none) return;
        if (InventoryObject.TryGetComponent(out CanvasGroup canvasGroup))
        {
            SwitchToInventoryMenu();
            InventoryObject.SetActive(false);
            //print("closing inventory");
            ScriptRefrenceSingleton.instance.gameplayUtils.CloseMenu();
            OnInventoryClosed?.Invoke();
            ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = null;
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
            ScriptRefrenceSingleton.instance.gameplayUtils.CloseAllCraftingMenus();

            openMenu = InventoryMenus.none;
            ScriptRefrenceSingleton.instance.itemTitlePopupManager.HidePopup();
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

        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = QuestMenuDefaultButton;
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
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

        foreach (CraftingTableRecipeDisplay display in ScriptRefrenceSingleton.instance.gameplayUtils.craftingTableRecipeDisplays)
        {
            display.HideRecipe();
            display.recipesBookManager.HideRecipeBook();
        }

        openMenu = InventoryMenus.Inventory;
        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = InventoryMenuDefaultButton;
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
    }

    /* #region Inventory */
    void AddInventorySlot(int slotID)
    {
        InventorySlot inventorySlot = new InventorySlot();
        inventorySlot.slot_id = slotID;
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
        inventorySlot.slot_id = slotID;
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

    public void AddItemToSlot(InventorySlot inventorySlot, ItemData itemData, int amount = 1, bool force = false, bool broadcastEvent = true)
    {
        if (force || inventorySlot.isEmpty)
        {
            inventorySlot.isEmpty = false;
            InventoryItemStack newItemStack = new InventoryItemStack(itemData.item_id, itemData.item_name, amount, itemData.MaxStackSize);
            inventorySlot.inventoryItemStack = newItemStack;
            if (inventorySlot.inventorySlotComponent != null)
            {
                inventorySlot.inventorySlotComponent.SetSlotFilled(itemData.item_name, amount, itemData.item_ui_image, broadcastEvent);
            }
            OnInventoryUpdated?.Invoke();
        }
    }

    public void AddItemToSlotByID(int slotID, ItemData itemData, int amount, bool force = false, bool broadcastEvent = true)
    {
        if(slotID <= inventorySlots.Count)
        {
            InventorySlot inventorySlot = inventorySlots[slotID];
            AddItemToSlot(inventorySlot, itemData, amount, force, broadcastEvent);
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
    
    public void RemoveSpecialItem(string item_id, int amount)
    {
        if (specialItems.ContainsKey(item_id))
        {
            specialItems[item_id] = Mathf.Max(specialItems[item_id] - amount,0);
        }
        OnInventoryUpdated?.Invoke();
    }

    public bool removeItemsByID(string item_id, int amount = 1)
    {
        int remainingAmount = amount;
        if (GetSpecialItemAmount(item_id) != -1)
        {
            RemoveSpecialItem(item_id, amount);
            return true;
        }
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (!inventorySlots[i].isEmpty && inventorySlots[i].inventoryItemStack.ID == item_id)
            {
                // -- Stack has more then we need to remove --
                if (inventorySlots[i].inventoryItemStack.Amount > remainingAmount)
                {
                    ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(inventorySlots[i].inventoryItemStack.ID);
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
                if (remainingAmount <= 0)
                {
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
        }
        OnInventoryUpdated?.Invoke();
        return false;
    }
    
    public bool AddUnlockedRecipe(string recipe_id)
    {
        if (UnlockedRecipes.Contains(recipe_id)) return false;
        UnlockedRecipes.Add(recipe_id);
        return true;
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

    public void ClearQuestButtons()
    {
        for (int i = QuestItemsParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(QuestItemsParent.transform.GetChild(0).gameObject);
        }
    }

    [Button]
    public void SpawnActiveQuestButtons()
    {
        ClearQuestButtons();
        foreach (var quest in activeQuests)
        {
            AddQuestButton(quest);
        }
    }
    
    [Button]
    public void SpawnCompletedQuestButtons()
    {
        ClearQuestButtons();
        foreach(var quest in CompletedQuests)
        {
            AddQuestButton(quest);
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


                QuestInfo questInfo = new QuestInfo(data);

                AddQuestButton(questInfo,true);

                foreach (QuestObjective questObjective in data.questObjectives)
                {
                    questObjective.isComplete = false;
                }

                activeQuests.Add(questInfo);

                if (data.AutoPinQuest)
                {
                    AddNewPinnedQuest(data);
                }

                ScriptRefrenceSingleton.instance.gameplayUtils.ShowCustomNotif($"Quest Added {data.QuestName}", 6);
            }
        }
        //return null;
    }

    void AddQuestButton(QuestInfo questInfo, bool unread = false)
    {
        GameObject questButton = Instantiate(QuestButton, QuestItemsParent.transform);

        QuestInfoButton questInfoButton = questButton.GetComponent<QuestInfoButton>();
        questInfoButton.QuestID = questInfo.QuestData.ID;
        questInfoButton.questData = questInfo.QuestData;
        if(unread)
        {
            questInfoButton.buttonName.text = $"<font-weight=700>!</font-weight> {questInfo.QuestData.QuestName}";
        }
        else
        {
            questInfoButton.buttonName.text = $"{questInfo.QuestData.QuestName}";
        }

        
    }

    public void LoadSavedQuest(SaveableQuestInfo quest, bool completed = false)
    {
        QuestData[] allQuests = Resources.LoadAll<QuestData>("QuestData");
        foreach (var data in allQuests)
        {
            if (data.ID == quest.quest_id)
            {   
                foreach (QuestInfo info in activeQuests)
                {
                    if (info.QuestData == data) return;
                }
                QuestInfo questInfo = new QuestInfo(data);

                if(!completed)
                {
                    for (int i = 0; i < data.questObjectives.Count; i++)
                    {
                        questInfo.QuestData.questObjectives[i].isComplete = quest.completed_objectives[i];
                    }
                }
                

                questInfo.IsComplete = quest.is_complete;

                if (!completed)
                {
                    AddQuestButton(questInfo);
                    activeQuests.Add(questInfo);
                }
                else
                {
                    CompletedQuests.Add(questInfo);
                }

                if (quest.is_pinned)
                {
                    AddNewPinnedQuest(data);
                }

                //ScriptRefrenceSingleton.instance.gameplayUtils.ShowCustomNotif($"Quest Added {data.QuestName}", 6);
            }
        }
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
                //         if (ScriptRefrenceSingleton.instance.gameplayUtils.get_item_holding_amount(objectiveCollect.Item_ID) >= objectiveCollect.Item_Amount)
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
                ScriptRefrenceSingleton.instance.gameplayUtils.ShowCustomNotif("Quest Complete!", 8);
                RemovePinnedQuest(info.QuestData);
                CompletedQuests.Add(info);
                for (int i = 0; i < QuestItemsParent.transform.childCount; i++)
                {
                    if (QuestItemsParent.transform.GetChild(i).TryGetComponent(out QuestInfoButton questInfoButton))
                    {
                        questInfoButton.SetComplete();
                        Destroy(questInfoButton.gameObject);
                    }
                }
            }
        }
        foreach(var info in CompletedQuests)
        {
            QuestInfo questInfo = activeQuests.FirstOrDefault(_activeQuest => _activeQuest == info);

            if(questInfo != null)
            {
                activeQuests.Remove(questInfo);
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

    public InventorySlot(SaveableInventroySlot saveableInventroySlot)
    {
        isEmpty = saveableInventroySlot.isEmpty;
        slot_id = saveableInventroySlot.slot_id;
        inventoryItemStack = saveableInventroySlot.inventoryItemStack;
    }

}

[Serializable]
public class SaveableInventroySlot
{
    public bool isEmpty = true;
    public int slot_id;
    public InventoryItemStack inventoryItemStack;

    public SaveableInventroySlot()
    {
        inventoryItemStack = new InventoryItemStack();
    }

    public SaveableInventroySlot(bool _isEmpty, int _slot_id)
    {
        isEmpty = _isEmpty;
        slot_id = _slot_id;
        inventoryItemStack = new InventoryItemStack(0);
    }

    public SaveableInventroySlot(bool _isEmpty, int _slot_id, InventoryItemStack _inventoryItemStack)
    {
        isEmpty = _isEmpty;
        slot_id = _slot_id;
        inventoryItemStack = _inventoryItemStack;
    }
    
    public SaveableInventroySlot(InventorySlot slot)
    {
        isEmpty = slot.isEmpty;
        slot_id = slot.slot_id;
        inventoryItemStack = slot.inventoryItemStack;
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


