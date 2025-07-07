using System;
using UnityEngine;

[Serializable]
public class ObjectiveCollectItems : QuestObjective
{

    public string Item_ID;
    public int Item_Amount;

    public override bool ObjectiveComplete()
    {
        if (GameplayUtils.instance.get_item_holding_amount(Item_ID) >= Item_Amount) return true;
        return false;
    }


}
