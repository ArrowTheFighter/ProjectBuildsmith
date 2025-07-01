using UnityEngine;


[CreateAssetMenu(menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string item_id;
    public string item_name;
    public GameObject item_pickup_object;
    public Sprite item_ui_image;
}
