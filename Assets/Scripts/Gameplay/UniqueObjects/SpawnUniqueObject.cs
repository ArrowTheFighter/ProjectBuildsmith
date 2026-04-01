using UnityEngine;

public class SpawnUniqueObject : MonoBehaviour
{
    [SerializeField] GameObject SpawnPrefab;
    GameObject spawnedObject;
    
    public void SpawnNewUniqueObject()
    {
        if(spawnedObject != null)
        {
            Destroy(spawnedObject);
        }
        spawnedObject = Instantiate(SpawnPrefab,transform.position,Quaternion.identity);
    }
}
