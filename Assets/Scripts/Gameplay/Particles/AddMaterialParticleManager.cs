using System.Collections.Generic;
using UnityEngine;

public class AddMaterialParticleManager : MonoBehaviour
{
    GameObject ActiveParticleForceField;

    public Material material;
    public GameObject particlePrefab;
    public Transform spawnPos;
    public AudioCollection[] ParticleSpawnSoundFX;

    public int spawnedParticles;
    public void SpawnParticle(Collider collider, GameObject meshObj, RepairStructure repairStructure)
    {
        GameObject spawnedParticle = Instantiate(particlePrefab, spawnPos.position, Quaternion.identity);
        
        spawnedParticles++;

        ScriptRefrenceSingleton.instance.soundFXManager.PlayAllSoundCollection(transform, ParticleSpawnSoundFX);

        Material _mat = meshObj.GetComponentInChildren<MeshRenderer>().sharedMaterial;

        if (spawnedParticle.TryGetComponent(out ParticleKillOnEnterTrigger component))
        {
            print(collider);
            component.LaunchParticle(meshObj, collider, _mat);
            component.OnParticleEnter += repairStructure.SpawnStartParticle;
            component.OnParticleEnter += ParticleDied;
        }
    }

    void ParticleDied(Vector3 _pos, ParticleKillOnEnterTrigger _trigger)
    {
        _trigger.OnParticleEnter -= ParticleDied;
        spawnedParticles--;
    }

    public bool CanUseThisField(GameObject forceFieldObject)
    {
        if (ActiveParticleForceField != null)
        {
            if(forceFieldObject != ActiveParticleForceField && spawnedParticles > 0)
            {
                return false;
            }
        }
        return true;
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
