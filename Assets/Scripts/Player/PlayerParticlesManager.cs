using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticlesManager : MonoBehaviour
{
    public static PlayerParticlesManager instance;

    public List<ParticleSystemsCollection> particleSystems = new List<ParticleSystemsCollection>();

    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }
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
}

[Serializable]
public class ParticleSystemsCollection
{
    public string ParticleID;
    public ParticleSystem particleSystem;
}
