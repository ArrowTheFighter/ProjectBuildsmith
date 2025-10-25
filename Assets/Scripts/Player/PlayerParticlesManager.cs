using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerParticlesManager : MonoBehaviour
{
    public List<ParticleSystemsCollection> particleSystems = new List<ParticleSystemsCollection>();
    public List<ParticlePrefabCollection> particlePrefabs = new List<ParticlePrefabCollection>();

    public void PlayChopSlamParticles()
    {
        foreach (ParticleSystemsCollection collection in particleSystems)
        {
            if (collection.ParticleID == "ChopSlam")
            {
                collection.particleSystem.Play();
            }
        }
    }

    public void PlayPlayerTakeHitParticles()
    {
        foreach (ParticleSystemsCollection collection in particleSystems)
        {
            if (collection.ParticleID == "TakeDamage")
            {
                collection.particleSystem.Play();
            }
        }
    }

    public void PlayParticleByID(string id)
    {
        foreach (ParticleSystemsCollection collection in particleSystems)
        {
            if (collection.ParticleID == id)
            {
                collection.particleSystem.Play();
            }
        }
    }

    public ParticleSystem GetParticleByID(string id)
    {
        foreach (ParticleSystemsCollection collection in particleSystems)
        {
            if (collection.ParticleID == id)
            {
                return collection.particleSystem;
            }
        }
        return null;
    }

    public GameObject GetTargetParticlePrefab()
    {
        foreach (ParticlePrefabCollection collection in particlePrefabs)
        {
            if (collection.ParticleID == "Target")
            {
                return collection.particlePredab;
            }
        }
        return null;
    }

    public GameObject GetParticlePredabByID(string id)
    {
        foreach (ParticlePrefabCollection collection in particlePrefabs)
        {
            if (collection.ParticleID == id)
            {
                return collection.particlePredab;
            }
        }
        return null;
    }
}

[Serializable]
public class ParticleSystemsCollection
{
    public string ParticleID;
    public ParticleSystem particleSystem;
}

[Serializable]
public class ParticlePrefabCollection
{
    public string ParticleID;
    public GameObject particlePredab;
}
