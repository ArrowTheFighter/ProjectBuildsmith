using UnityEngine;

public class SpawnObj : MonoBehaviour
{
    public GameObject[] PrefabsToSpawn;
    public Vector3 spawnOffset;

    public void SpawnObjects()
    {
        foreach (GameObject obj in PrefabsToSpawn)
        {
            Instantiate(obj, transform.position + spawnOffset, Quaternion.identity);    
        }
    }
}
