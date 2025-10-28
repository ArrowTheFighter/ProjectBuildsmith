using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpawnInMeshesAnimation : MonoBehaviour
{
    public List<Transform> transformsToSpawn = new List<Transform>();
    public List<Transform> transformsToSpawnWithChildren = new List<Transform>();
    public List<Transform> visualsTransforms = new List<Transform>();
    public GameObject MainGO;
    GameObject visualsParent;
    public float SpawnInScaleDuration;
    public float SpawnInMoveDuration;
    public Ease SpawnInScaleEase;
    public Ease SpawnInMoveEase;
    public Vector3 spawnOffset;
    public Vector3 rotationOffset;

    public float SpawnDelay;
    public float FinalDelay = 0.5f;

    [Button("Spawn Meshed")]
    public void SpawnMeshes()
    {
        DuplicateAndReparent("TestPartent");
        MainGO.SetActive(false);
        StartCoroutine(SpawnMeshesCoroutine());
    }

    IEnumerator SpawnMeshesCoroutine()
    {
        List<spawn_info> spawn_Info = new List<spawn_info>();
        foreach (var _transform in visualsTransforms)
        {
            spawn_Info.Add(new spawn_info(_transform, _transform.localScale, _transform.position,_transform.rotation));
            _transform.localScale = Vector3.zero;
            _transform.position += spawnOffset;
            _transform.rotation = _transform.rotation * Quaternion.Euler(rotationOffset);
        }
        foreach (var info in spawn_Info)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(info.spawn_transform.DOScale(info.spawn_scale, SpawnInScaleDuration).SetEase(SpawnInScaleEase))
                .Join(info.spawn_transform.DORotateQuaternion(info.spawn_rotation, SpawnInScaleDuration * 2).SetEase(SpawnInScaleEase))
                .Append(info.spawn_transform.DOMove(info.spawn_position, SpawnInMoveDuration).SetEase(SpawnInMoveEase));
            yield return new WaitForSeconds(SpawnDelay);
        }
        yield return new WaitForSeconds(FinalDelay);
        visualsParent.SetActive(false);
        MainGO.SetActive(true);
    }

    public void DuplicateAndReparent(string newParentName = "DuplicatedObjects")
    {
        visualsTransforms.Clear(); // Clear any previous duplicates

        if (transformsToSpawn.Count == 0)
        {
            Debug.LogWarning("No transforms to duplicate!");
            return;
        }

        // Create a new empty parent
        visualsParent = new GameObject(newParentName);
        visualsParent.transform.position = MainGO.transform.position;
        visualsParent.transform.rotation = MainGO.transform.rotation;

        foreach (Transform original in transformsToSpawn)
        {
            if (original == null) continue;

            // Duplicate the object
            GameObject duplicateGO = Instantiate(original.gameObject, original.transform.position, original.transform.rotation);
            for (int i = duplicateGO.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(duplicateGO.transform.GetChild(i).gameObject);
            }
            // Parent to new parent while keeping world position
            duplicateGO.transform.SetParent(visualsParent.transform, worldPositionStays: true);

            // Store the transform in the new list
            visualsTransforms.Add(duplicateGO.transform);
        }

        foreach (Transform original in transformsToSpawnWithChildren)
        {
            if (original == null) continue;

            // Duplicate the object
            GameObject duplicateGO = Instantiate(original.gameObject, original.transform.position, original.transform.rotation);
            
            // Parent to new parent while keeping world position
            duplicateGO.transform.SetParent(visualsParent.transform, worldPositionStays: true);

            // Store the transform in the new list
            visualsTransforms.Add(duplicateGO.transform);
        }

        Debug.Log($"Duplicated {visualsTransforms.Count} objects under '{newParentName}'");
    }

}

class spawn_info
{
    public Transform spawn_transform;
    public Vector3 spawn_scale;
    public Vector3 spawn_position;
    public Quaternion spawn_rotation;

    public spawn_info(Transform _transform, Vector3 scale,Vector3 pos,Quaternion rotation)
    {
        spawn_transform = _transform;
        spawn_scale = scale;
        spawn_position = pos;
        spawn_rotation = rotation;
    }
}
