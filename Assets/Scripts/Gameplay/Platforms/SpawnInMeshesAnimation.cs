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
    public float SpawnInMoveInitalDuration;
    public float SpawnInRotationDuration;
    public Ease SpawnInScaleEase;
    public Ease SpawnInMoveEase;
    public Ease SpawnInMoveInitalEase;
    public Ease SpawnInRotationEase;
    public Vector3 spawnOffset;
    public Vector3 additionalOffset;
    Vector3 currentOffset;
    public Vector3 rotationOffset;

    public float SpawnDelay;
    public float FinalDelay = 0.5f;

    bool runningCoroutine;

    [Header("Audio")]
    public AudioCollection[] SpawnMeshAudioCollection;
    public AudioCollection[] MeshFinishAudioCollection;


    [Button("Spawn Meshed")]
    public void SpawnMeshes()
    {
        if (runningCoroutine) return;
        DuplicateAndReparent("TestPartent");
        MainGO.SetActive(false);
        StartCoroutine(SpawnMeshesCoroutine());
    }

    IEnumerator SpawnMeshesCoroutine()
    {
        
        runningCoroutine = true;
        List<spawn_info> spawn_Info = new List<spawn_info>();
        currentOffset = spawnOffset;
        foreach (var _transform in visualsTransforms)
        {
            spawn_Info.Add(new spawn_info(_transform, _transform.localScale, _transform.parent.position, _transform.parent.rotation));

            // _transform.parent.position += spawnOffset;
            // _transform.parent.rotation = _transform.rotation * Quaternion.Euler(rotationOffset);
            // _transform.localScale = Vector3.zero;
        }
        foreach (var _transform in visualsTransforms)
        {
            //_transform.parent.position += currentOffset;
            //currentOffset += additionalOffset;
            _transform.parent.Rotate(rotationOffset);
            _transform.localScale = Vector3.zero;
        }
        float currentSpawnDelay = SpawnDelay;
        foreach (var info in spawn_Info)
        {
            ScriptRefrenceSingleton.instance.soundFXManager.PlayAllSoundCollection(transform, SpawnMeshAudioCollection);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(info.spawn_transform.DOScale(info.spawn_scale, SpawnInScaleDuration).SetEase(SpawnInScaleEase))
                .Join(info.spawn_transform.parent.DORotateQuaternion(info.spawn_rotation, SpawnInRotationDuration * 2).SetEase(SpawnInRotationEase))
                .Join(info.spawn_transform.parent.DOMove(info.spawn_position + currentOffset, SpawnInMoveInitalDuration * 2).SetEase(SpawnInMoveInitalEase))
                .Append(info.spawn_transform.parent.DOMove(info.spawn_position, SpawnInMoveDuration).SetEase(SpawnInMoveEase))
                .OnComplete(() => ScriptRefrenceSingleton.instance.soundFXManager.PlayAllSoundCollection(transform, MeshFinishAudioCollection));
            yield return new WaitForSeconds(currentSpawnDelay);
            currentSpawnDelay -= currentSpawnDelay * 0.1f;
        }
        yield return new WaitForSeconds(FinalDelay);

        MainGO.SetActive(true);
        yield return null;
        Destroy(visualsParent);
        runningCoroutine = false;
    }

    public void DuplicateAndReparent(string newParentName = "DuplicatedObjects")
    {
        visualsTransforms.Clear(); // Clear any previous duplicates

        if (transformsToSpawn.Count == 0 && transformsToSpawnWithChildren.Count == 0)
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
            GameObject duplicateParent = new GameObject(original.name + "Parent");
            duplicateParent.transform.SetParent(visualsParent.transform);
            duplicateParent.transform.position = original.transform.position;
            duplicateParent.transform.localRotation = Quaternion.identity;
            // Duplicate the object
            //GameObject duplicateGO = Instantiate(original.gameObject, original.transform.position, original.transform.rotation);
            GameObject duplicateGO = new GameObject(original.name);
            duplicateGO.AddComponent<MeshFilter>().mesh = original.GetComponent<MeshFilter>().mesh;
            duplicateGO.AddComponent<MeshRenderer>().materials = original.GetComponent<MeshRenderer>().materials;
            duplicateGO.transform.position = original.transform.position;
            duplicateGO.transform.rotation = original.transform.rotation;
            duplicateGO.transform.localScale = original.transform.lossyScale;
            for (int i = duplicateGO.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(duplicateGO.transform.GetChild(i).gameObject);
            }
            // Parent to new parent while keeping world position
            duplicateGO.transform.SetParent(duplicateParent.transform, worldPositionStays: true);

            // Store the transform in the new list
            visualsTransforms.Add(duplicateGO.transform);
        }

        foreach (Transform original in transformsToSpawnWithChildren)
        {
            if (original == null) continue;
            GameObject duplicateParent = new GameObject(original.name + "Parent");
            duplicateParent.transform.SetParent(visualsParent.transform);
            duplicateParent.transform.position = original.transform.position;
            // Duplicate the object
            GameObject duplicateGO = Instantiate(original.gameObject, original.transform.position, original.transform.rotation);
            
            // Parent to new parent while keeping world position
            duplicateGO.transform.SetParent(duplicateParent.transform, worldPositionStays: true);

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
