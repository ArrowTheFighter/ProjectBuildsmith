using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string item_id;
    public string item_name;
    public int MaxStackSize = 25;
    public GameObject item_pickup_object;
    [Header("Holding Item")]
    public GameObject holdingItem;
    public GameObject item_prefab_obj;
    public Vector3 PositionOffset;
    public Vector3 Rotation;
    [Header("Sprite")]
    public Sprite item_ui_image;
    [Header("Abilities")]
    [SerializeReference]
    public List<AbilityData> abilityConfigs = new List<AbilityData>();
}
