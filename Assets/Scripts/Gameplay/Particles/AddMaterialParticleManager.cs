using UnityEngine;

public class AddMaterialParticleManager : MonoBehaviour
{
    GameObject ActiveParticleForceField;

    public Material material;
    public GameObject particlePrefab;
    public Transform spawnPos;
    public void SpawnParticle(Collider collider, GameObject meshObj, RepairStructure repairStructure)
    {
        GameObject spawnedParticle = Instantiate(particlePrefab, spawnPos.position, Quaternion.identity);

        if (spawnedParticle.TryGetComponent(out ParticleKillOnEnterTrigger component))
        {
            print(collider);
            component.LaunchParticle(meshObj, collider,material);
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
