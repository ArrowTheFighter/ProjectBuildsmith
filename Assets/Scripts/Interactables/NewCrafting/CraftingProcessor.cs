using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingProcessor : CraftingTableBase
{

    public List<InventorySlot> processorSlots = new List<InventorySlot>();
    public InventorySlot processorOutputSlot;

    public float CraftTime;
    float processTime;

    InventoryDataSaver inventoryDataSaver;


    bool HasValidRecipe;
    CraftingRecipeData currentRecipe;

    public bool isCrafting;

    bool CheckingForRecipe = true;

    public AudioSource ProcessorRunningAudioSource;
    public AudioCollection ProcessorRunningAudioCollection;

    public AudioCollection[] ProcessorFinishedCrafting;

    //public event Action<float> SliderUpdated;



    void Awake()
    {
        inventoryDataSaver = GetComponent<InventoryDataSaver>();
        inventoryDataSaver.OnFinshedInitalizing += Initalize;
    }

    void Initalize()
    {
        //inventoryDataSaver = GetComponent<InventoryDataSaver>();
        inventoryDataSaver.slotsUpdated += InventoryUpdated;

        processorOutputSlot = inventoryDataSaver.savedSlots[0];
        for (int i = 0; i < inventoryDataSaver.savedSlots.Count; i++)
        {
            if (i == 0)
            {
                processorOutputSlot = inventoryDataSaver.savedSlots[i];
            }
            else
            {
                processorSlots.Add(inventoryDataSaver.savedSlots[i]);
            }
        }

        foreach (CraftingRecipeData recipeData in ScriptRefrenceSingleton.instance.gameplayUtils.RecipeDatabase.recipes)
        {
            if (recipeData.stationType == craftingStationType)
            {
                craftingRecipeData.Add(recipeData);
            }
        }
        ProcessorRunningAudioSource.volume = ProcessorRunningAudioCollection.audioClipVolume;
        ProcessorRunningAudioSource.pitch = ProcessorRunningAudioCollection.audioClipPitch;
        ProcessorRunningAudioSource.clip = ProcessorRunningAudioCollection.audioClip;
    }

    public override void InventoryUpdated()
    {
        if (!CheckingForRecipe) return;
        if (IsValidRecipe(out CraftingRecipeData validRecipe))
        {
            HasValidRecipe = true;
            currentRecipe = validRecipe;
            if (!ProcessorRunningAudioSource.isPlaying)
            {
                ProcessorRunningAudioSource.Play();
            }
            StartCoroutine(CraftingProcess());
        }
        else
        {
            if (ProcessorRunningAudioSource.isPlaying)
            {
                ProcessorRunningAudioSource.Stop();
            }
            HasValidRecipe = false;
            currentRecipe = null;
        }
    }

    void Update()
    {
        if (inventoryDataSaver.ActiveContainer)
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.SetSawmillProgressBar(processTime / CraftTime);
        }
    }

    IEnumerator CraftingProcess()
    {
        if (!HasValidRecipe || currentRecipe == null) yield break;
        if (isCrafting) yield break;

        isCrafting = true;
        processTime = 0;
        while (HasValidRecipe && CanCraftRecipe() && processTime < CraftTime)
        {
            processTime += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        if (!HasValidRecipe || !CanCraftRecipe())
        {
            processTime = 0;
            isCrafting = false;

            ProcessorRunningAudioSource.Stop();
            yield break;
        }
        isCrafting = false;
        processTime = 0;
        CraftItem();

    }

    void CraftItem()
    {
        if (currentRecipe != null)
        {
            ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(currentRecipe.recipe_output_id);
            if (processorOutputSlot.inventoryItemStack.Amount < processorOutputSlot.inventoryItemStack.MaxStackSize)
            {
                processorOutputSlot.inventoryItemStack.Amount += 1;

            }
            else if (processorOutputSlot.isEmpty)
            {
                ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(processorOutputSlot, itemData);
            }
            foreach (InventorySlot slot in processorSlots)
            {
                if (slot.inventoryItemStack.Amount >= 2)
                {
                    slot.inventoryItemStack.Amount -= 1;
                }
                else
                {
                    slot.inventoryItemStack.Amount = 0;
                    slot.inventoryItemStack.ID = "";
                    slot.inventoryItemStack.Name = "";
                    slot.inventoryItemStack.MaxStackSize = 0;
                    slot.isEmpty = true;
                }
            }
        }
        inventoryDataSaver.SetContainerSlots();
        InventoryUpdated();

        ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(transform, ProcessorFinishedCrafting);
        //inventoryDataSaver.UpdateSavedSlots();
    }

    bool CanCraftRecipe()
    {
        if (currentRecipe == null)
        {
            return false;
        }
        if (processorOutputSlot.isEmpty || string.IsNullOrEmpty(processorOutputSlot.inventoryItemStack.ID))
        {
            return true;
        }
        if (processorOutputSlot.inventoryItemStack.ID == currentRecipe.recipe_output_id)
        {
            if (processorOutputSlot.inventoryItemStack.Amount < processorOutputSlot.inventoryItemStack.MaxStackSize)
            {
                return true;
            }
        }
        return false;
     }

  

    bool IsValidRecipe(out CraftingRecipeData validRecipeData)
    {
        validRecipeData = null;
        foreach (CraftingRecipeData recipeData in craftingRecipeData)
        {
            bool recipeIsValid = true;
            for (int i = 0; i < processorSlots.Count; i++)
            {
                if (string.IsNullOrEmpty(recipeData.recipe_items[i]) && string.IsNullOrEmpty(processorSlots[i].inventoryItemStack.ID))
                {
                    continue;
                }
                if (recipeData.recipe_items[i] != processorSlots[i].inventoryItemStack.ID) recipeIsValid = false;
            }
            if (recipeIsValid)
            {
                validRecipeData = recipeData;
                return true;
            }
        }
        return false;
    }

    
}
