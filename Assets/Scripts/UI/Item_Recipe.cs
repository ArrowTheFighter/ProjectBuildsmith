using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Recipe")]
public class Item_Recipe : ScriptableObject
{
    public string ID;
    public string Item_Name;
    public int Output_Amount;
    [SerializeField] public item_requirement[] item_Requirements;
}

[Serializable]
public class item_requirement
{
    public string item_id;
    public string item_name;
    public int item_amount;
}
