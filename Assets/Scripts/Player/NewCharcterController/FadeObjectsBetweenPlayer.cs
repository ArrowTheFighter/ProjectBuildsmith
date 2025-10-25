using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeObjectsBetweenPlayer : MonoBehaviour
{
    Transform PlayerTransform;
    public Vector3 targetoffset;
    RaycastHit[] TransparentTargets = new RaycastHit[10];
    int ObjectsHit;
    public LayerMask layer;
    List<Transform> trackedTransforms = new List<Transform>();

    void Start()
    {
        PlayerTransform = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform;
    }

    void Update()
    {
        Vector3 dir = (PlayerTransform.position + targetoffset - transform.position - transform.forward * 3).normalized;
        float distance = Vector3.Distance(PlayerTransform.position + targetoffset, transform.position);

        // Raycast into the buffer
        ObjectsHit = Physics.RaycastNonAlloc(transform.position, dir, TransparentTargets, distance, layer);

        // Build the set of hits for this frame
        HashSet<Transform> currentHits = new HashSet<Transform>();

        for (int i = 0; i < ObjectsHit; i++)
        {
            RaycastHit hit = TransparentTargets[i];
            if (hit.transform != null && hit.transform.TryGetComponent(out MeshRenderer meshRenderer))
            {
                currentHits.Add(hit.transform);

                if (!trackedTransforms.Contains(hit.transform))
                {
                    // New detection: add and fade it out
                    trackedTransforms.Add(hit.transform);

                    Material mat = meshRenderer.material; // use .material so it's unique to this renderer
                    if (mat.HasFloat("_FadeAmount"))
                    {
                        DOTween.Kill(mat);
                        mat.DOFloat(0.4f, "_FadeAmount", 0.2f);
                        //mat.SetFloat("_FadeAmount", 0.4f);
                    }
                }
            }
        }

        // Remove any transforms no longer detected
        for (int i = trackedTransforms.Count - 1; i >= 0; i--)
        {
            Transform t = trackedTransforms[i];
            if (!currentHits.Contains(t))
            {
                if (t != null && t.TryGetComponent(out MeshRenderer meshRenderer))
                {
                    Material mat = meshRenderer.material;
                    if (mat.HasFloat("_FadeAmount"))
                    {
                        //mat.SetFloat("_FadeAmount", 0f);
                        DOTween.Kill(mat);
                        mat.DOFloat(0f, "_FadeAmount", 0.2f);
                    }
                }

                trackedTransforms.RemoveAt(i);
            }
        }
    }

}
