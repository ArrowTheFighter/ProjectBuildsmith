using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRespawnManager : MonoBehaviour
{
    [SerializeField] float respawn_check_delay = 10;
    public Dictionary<GameObject, float> item_respawns = new Dictionary<GameObject, float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Respawn_Items());
    }

    IEnumerator Respawn_Items()
    {
        while (true)
        {
            List<GameObject> items_to_remove = new List<GameObject>();
            foreach (KeyValuePair<GameObject, float> respawn_item in item_respawns)
            {
                if (respawn_item.Value < Time.time)
                {
                    respawn_item.Key.SetActive(true);
                    items_to_remove.Add(respawn_item.Key);
                }
            }
            foreach (GameObject key in items_to_remove)
            {
                item_respawns.Remove(key);
            }
            yield return new WaitForSeconds(respawn_check_delay);
        }
        
     }
}
