using System;
using System.Linq;

public class SpecialItemPickup : ItemPickup
{
    public int pickup_id;

    void Start()
    {
        SaveLoadManager.instance.OnSaveLoaded += SaveFileLoaded;
    }
    
    void SaveFileLoaded(SaveFileStruct saveFileStruct)
    {
        if(saveFileStruct.special_items_collected.Contains(pickup_id))
        {
            Destroy(gameObject);
        }
    }

    public override void Pickup()
    {
        SaveLoadManager.instance.AddSpecialItemCollected(pickup_id);
        base.Pickup();

    }
    

    
}