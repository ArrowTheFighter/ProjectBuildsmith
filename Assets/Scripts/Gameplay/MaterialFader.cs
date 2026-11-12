using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFader : MonoBehaviour
{
    [SerializeField] Renderer[] renderersToEffect;
    [SerializeField] Material materialToEffect;
    [SerializeField] Material finalMaterial;
    [SerializeField] float duration = 1f;

    [ContextMenu("Start Fade")]
    public void StartFade()
    {
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        float progress = 0f;
        List<MaterialTarget> targets = new List<MaterialTarget>();

        // 1. Initialize and cache material instances ONCE before the loop starts.
        // This prevents memory leaks and ensures we modify the same instances over time.
        foreach (var rend in renderersToEffect)
        {
            if (rend == null) continue;

            Material[] sharedMats = rend.sharedMaterials;
            Material[] workingMats = new Material[sharedMats.Length];

            for (int i = 0; i < sharedMats.Length; i++)
            {
                if (sharedMats[i] == materialToEffect)
                {
                    // 1. Instance ONLY this specific slot so we can fade it
                    workingMats[i] = new Material(materialToEffect);

                    targets.Add(new MaterialTarget
                    {
                        targetMaterial = workingMats[i],
                        startMaterial = sharedMats[i]
                    });
                }
                else
                {
                    // 2. Leave all other slots pointing to the original shared material (NOT instanced)
                    workingMats[i] = sharedMats[i];
                }
            }

            // Apply the mixed array back to the renderer
            rend.materials = workingMats;
        }

        // 2. Smoothly lerp over time inside the coroutine loop
        while (progress < duration)
        {
            progress += Time.deltaTime;
            float t = Mathf.Clamp01(progress / duration);

            foreach (var target in targets)
            {
                // Lerps properties from startMaterial to finalMaterial onto targetMaterial
                target.targetMaterial.Lerp(target.startMaterial, finalMaterial, t);
            }

            yield return null;
        }

        // 3. Ensure it reaches the exact final state at t = 1.0
        foreach (var target in targets)
        {
            target.targetMaterial.Lerp(target.startMaterial, finalMaterial, 1f);
        }
    }

    private struct MaterialTarget
    {
        public Material targetMaterial;
        public Material startMaterial;
    }
}