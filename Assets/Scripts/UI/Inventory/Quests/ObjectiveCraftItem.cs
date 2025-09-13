using System;
using UnityEngine;

[Serializable]
public class ObjectiveCraftItem : QuestObjective
{

    public string Item_ID;
    public int Item_Amount;

    protected override bool CheckObjectiveComplete()
    {
        if (GameplayUtils.instance.GetItemCraftedAmount(Item_ID) >= Item_Amount) return true;
        return false;
    }


}
