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

        foreach (var rend in renderersToEffect)
        {
            if (rend == null) continue;

            Material[] sharedMats = rend.sharedMaterials;
            Material[] workingMats = new Material[sharedMats.Length];

            for (int i = 0; i < sharedMats.Length; i++)
            {
                if (sharedMats[i] == materialToEffect)
                {
                    workingMats[i] = new Material(materialToEffect);

                    targets.Add(new MaterialTarget
                    {
                        targetMaterial = workingMats[i],
                        startMaterial = sharedMats[i]
                    });
                }
                else
                {
                    workingMats[i] = sharedMats[i];
                }
            }

            rend.materials = workingMats;
        }

        while (progress < duration)
        {
            progress += Time.deltaTime;
            float t = Mathf.Clamp01(progress / duration);

            foreach (var target in targets)
            {
                target.targetMaterial.Lerp(target.startMaterial, finalMaterial, t);
            }

            yield return null;
        }

        foreach (var target in targets)
        {
            target.targetMaterial.Lerp(target.startMaterial, finalMaterial, 1f);
        }
    }

    public void Skip()
    {
        List<MaterialTarget> targets = new List<MaterialTarget>();

        foreach (var rend in renderersToEffect)
        {
            if (rend == null) continue;

            Material[] sharedMats = rend.sharedMaterials;
            Material[] workingMats = new Material[sharedMats.Length];

            for (int i = 0; i < sharedMats.Length; i++)
            {
                if (sharedMats[i] == materialToEffect)
                {
                    workingMats[i] = new Material(materialToEffect);

                    targets.Add(new MaterialTarget
                    {
                        targetMaterial = workingMats[i],
                        startMaterial = sharedMats[i]
                    });
                }
                else
                {
                    workingMats[i] = sharedMats[i];
                }
            }

            rend.materials = workingMats;
        }

        foreach (var target in targets)
        {
            target.targetMaterial.Lerp(target.startMaterial, finalMaterial, 1);
        }
    }

    private struct MaterialTarget
    {
        public Material targetMaterial;
        public Material startMaterial;
    }
}