using UnityEngine;

public class AddMaterialParticleManager : MonoBehaviour
{
    GameObject ActiveParticleForceField;
    public GameObject particlePrefab;
    public Transform spawnPos;
    public void SpawnParticle(Collider collider, GameObject meshObj, RepairStructure repairStructure)
    {
        print("spawning particle");
        GameObject spawnedParticle = Instantiate(particlePrefab, spawnPos.position, Quaternion.identity);

        if (spawnedParticle.TryGetComponent(out ParticleKillOnEnterTrigger component))
        {
            print(collider);
            component.LaunchParticle(meshObj, collider);
            component.OnParticleEnter += repairStructure.SpawnStartParticle;
        }
    }
    
    public void SetActiveForceField(GameObject forceFieldObject)
    {
        if (ActiveParticleForceField != null)
        {
            ActiveParticleForceField.SetActive(false);
        }
        forceFieldObject.SetActive(true);
        ActiveParticleForceField = forceFieldObject;
    }
}
