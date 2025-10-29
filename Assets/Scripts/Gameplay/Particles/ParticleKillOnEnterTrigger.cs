using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ParticleKillOnEnterTrigger : MonoBehaviour
{
    ParticleSystem ps;
    public float slowDownAmount = 0.5f;
    public event Action<Vector3, ParticleKillOnEnterTrigger> OnParticleEnter;
    List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void LaunchParticle(GameObject prefab, Collider collider)
    {

        if (ps == null || prefab == null)
        {
            Debug.LogWarning("ParticleSystem or prefab is missing!");
            return;
        }

        // Get mesh and material from the prefab
        MeshFilter meshFilter = prefab.GetComponentInChildren<MeshFilter>();
        MeshRenderer meshRenderer = prefab.GetComponentInChildren<MeshRenderer>();

        if (meshFilter == null || meshRenderer == null)
        {
            Debug.LogWarning("Prefab is missing MeshFilter or MeshRenderer!");
            return;
        }

        // Get the particle system renderer
        ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();

        // Apply the prefab’s mesh and material to the particle system renderer
        psRenderer.renderMode = ParticleSystemRenderMode.Mesh;
        psRenderer.mesh = meshFilter.sharedMesh;
        psRenderer.material = meshRenderer.sharedMaterial;

        // Optionally set the trigger collider
        if (collider != null)
        {
            ps.trigger.SetCollider(0, collider);
        }

        // Launch the particles
        ps.Play();
    }


    void OnParticleTrigger()
    {
        int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = inside[i];
            OnParticleEnter?.Invoke(p.position,this);
        }

        for (int i = 0; i < numInside; i++)
        {
            ParticleSystem.Particle p = inside[i];
            //p.remainingLifetime = 0.5f;     // Kill soon
            //p.startSize = Mathf.Clamp(p.startSize - 0.1f, 0, 100);             // Shrink
            p.startSize = 0;
            if (p.startSize < 0.1)
            {
                p.remainingLifetime = 0;
            }
            inside[i] = p;
        }

        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);
    }


}
