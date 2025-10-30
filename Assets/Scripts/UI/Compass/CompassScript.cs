
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CompassScript : MonoBehaviour
{
    Transform PlayerTransform;
    [Header("Compass Icons")]
    public RectTransform NorthIcon;
    public RectTransform EastIcon;
    public RectTransform SouthIcon;
    public RectTransform WestIcon;
    [Header("Compass positions")]
    public RectTransform CompassLeftPosition;
    public RectTransform CompassRightPosition;

    [Header("Marker Objects")]
    public Transform MarkerParent;
    public GameObject MarkerPrefab;
    public GameObject MarkerWorldPrefab;
    List<QuestMarkerInfo> activeQuestMarkers = new List<QuestMarkerInfo>();

    [Header("Debug")]
    public Transform testTransform;
    public Vector3 markerOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerTransform = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform;
        ScriptRefrenceSingleton.instance.gameSettings.OnEnableCompassChanged += ToggleCompass;
    }

    void ToggleCompass(bool turnOn)
    {
        gameObject.SetActive(turnOn);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerTransform == null) return;
        Vector3 playerForward = Camera.main.transform.forward;
        playerForward.y = 0;
        float SignedAngle = 1 - (Vector3.SignedAngle(Vector3.forward, playerForward, Vector3.up) / 360 + 0.5f);
        SetIconPos(NorthIcon, SignedAngle, 0);
        SetIconPos(EastIcon, SignedAngle, 0.25f);
        SetIconPos(SouthIcon, SignedAngle, 0.5f);
        SetIconPos(WestIcon, SignedAngle, 0.75f);

        foreach (var marker in activeQuestMarkers)
        {
            if (marker.MarkerWorldTransform == null) RemoveQuestMarker(marker);
            UpdateMarkerPos(playerForward, marker.MarkerWorldTransform.position, marker.MarkerRectTransform);
        }

    }

    [Button("Add test marker")]
    public void DEBUG_AddTestTransform()
    {
        AddNewQuestMarkerWithWorldMarker(testTransform, markerOffset);
    }

    public void AddNewQuestMarkerWithWorldMarker(Transform worldObjectTransform)
    {
        if (worldObjectTransform == null) return;
        GameObject particle = Instantiate(MarkerWorldPrefab, worldObjectTransform);
        particle.GetComponent<ParticleSystem>().Play();
        particle.transform.position = worldObjectTransform.position;

        AddNewQuestMarker(worldObjectTransform,particle);
    }

    public void AddNewQuestMarkerWithWorldMarker(Transform worldObjectTransform, Vector3 worldMarkerOffset)
    {
        if (worldObjectTransform == null) return;
        GameObject particle = Instantiate(MarkerWorldPrefab, worldObjectTransform);
        particle.GetComponent<ParticleSystem>().Play();
        particle.transform.position = worldObjectTransform.position + worldMarkerOffset;

        AddNewQuestMarker(worldObjectTransform,particle);
    }

    public void AddNewQuestMarker(Transform worldObjectTransform, GameObject particleObject = null)
    {
        GameObject spawnedMarker = Instantiate(MarkerPrefab, MarkerParent);

        activeQuestMarkers.Add(new QuestMarkerInfo(spawnedMarker.GetComponent<RectTransform>(), worldObjectTransform,particleObject));

        StartCoroutine(ScaleIn(spawnedMarker.transform.GetChild(0).GetChild(0)));
        // sequence.Append(spawnedMarker.transform.GetChild(0).DOScale(1.5f, 0.15f).From(Vector3.zero).SetLink(spawnedMarker.transform.GetChild(0).gameObject))
        //     .Append(spawnedMarker.transform.GetChild(0).DOScale(1f, 0.2f).SetEase(Ease.InOutQuint).SetLink(spawnedMarker.transform.GetChild(0).gameObject));
    }

    void RemoveQuestMarker(QuestMarkerInfo questMarkerInfo)
    {
        if (!activeQuestMarkers.Contains(questMarkerInfo)) return;
        if (questMarkerInfo.ParticleObject != null) Destroy(questMarkerInfo.ParticleObject);
        StartCoroutine(ScaleOutAndDestroy(questMarkerInfo));
        // sequence.Append(questMarkerInfo.MarkerRectTransform.transform.GetChild(0).DOScale(1.5f, 0.15f).SetLink(questMarkerInfo.MarkerRectTransform.transform.GetChild(0).gameObject))
        //     .Append(questMarkerInfo.MarkerRectTransform.transform.GetChild(0).DOScale(0f, 0.2f).SetEase(Ease.InOutQuint).SetLink(questMarkerInfo.MarkerRectTransform.transform.GetChild(0).gameObject)).OnComplete(() =>
        //     {
        //         Destroy(questMarkerInfo.MarkerRectTransform.gameObject);
        //         activeQuestMarkers.Remove(questMarkerInfo);
        //     });
    }

    public void RemoveQuestMarker(Transform worldObjectTransform)
    {
        QuestMarkerInfo questMarkerInfo = activeQuestMarkers.FirstOrDefault(marker => marker.MarkerWorldTransform == worldObjectTransform);
        if (questMarkerInfo == null) return;

        RemoveQuestMarker(questMarkerInfo);

    }

    public void RemoveQuestMarkerByParent(Transform worldObjectParentTransform)
    {
        QuestMarkerInfo questMarkerInfo = activeQuestMarkers.FirstOrDefault(marker => marker.MarkerWorldTransform.parent == worldObjectParentTransform);
        if (questMarkerInfo == null) return;

        RemoveQuestMarker(questMarkerInfo);
    }


    void UpdateMarkerPos(Vector3 playerForward,Vector3 markerWorldPos,RectTransform markerRectTransform)
    {
        Vector3 dirToMarker = markerWorldPos - ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.position;
        dirToMarker.y = 0;
        float SignedAngle = 1 - (Vector3.SignedAngle(dirToMarker.normalized, playerForward, Vector3.up) / 360 + 0.5f);

        SetIconPos(markerRectTransform, SignedAngle, 0);

        float distanceToMarker = Vector3.Distance(ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.position, markerWorldPos);
        float scaleT = Mathf.InverseLerp(175, 35f, distanceToMarker);
        markerRectTransform.GetChild(0).localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, scaleT);
        markerRectTransform.GetComponentInChildren<Image>().color = Color.Lerp(new Color(0.7f, 0.7f, 0.7f),Color.white, scaleT);

    }

    void SetIconPos(RectTransform icon, float signedAngle, float offset)
    {
        float adjustedSignedAngle = Mathf.Repeat(signedAngle + offset, 1);
        float IconPos = Mathf.InverseLerp(0.25f, 0.75f, adjustedSignedAngle);
        // if (IconPos > 1) IconPos -= 1;
        // if (IconPos < 0) IconPos += 1;

        if (icon != null)
        {
            float scaleTime = 0;
            if (adjustedSignedAngle > 0.7f)
            {
                scaleTime = Mathf.InverseLerp(0.7f, 0.75f, adjustedSignedAngle);
            }
            else if (adjustedSignedAngle < 0.3f)
            {
                scaleTime = Mathf.InverseLerp(0.3f, 0.25f, adjustedSignedAngle);
            }
            //NorthIcon.GetComponent<Image>().enabled = !(SignedAngle > 0.75f || SignedAngle < 0.25f);
            icon.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, scaleTime);
            icon.anchoredPosition = Vector2.Lerp(CompassLeftPosition.anchoredPosition, CompassRightPosition.anchoredPosition, IconPos);
        }
    }

    private IEnumerator ScaleIn(Transform target)
    {
        if (target == null) yield break;
        // --- Stage 1: scale 0 → 1.5 ---
        float t = 0f;
        float durationUp = 0.15f;
        Vector3 startScale = target.localScale;
        Vector3 midScale = Vector3.one * 1.5f;
        while (t < durationUp)
        {
            if (target == null) yield break;
            t += Time.deltaTime;
            float progress = t / durationUp;
            Vector3 newScale = Vector3.Lerp(startScale, midScale, progress);
            target.localScale = newScale;
            yield return null;
        }

        // --- Stage 2: scale down to 0 ---
        t = 0f;
        float durationDown = 0.2f;
        Vector3 endScale = Vector3.one;

        while (t < durationDown)
        {
            if (target == null) yield break;
            t += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, t / durationDown); // smooth ease
            target.localScale = Vector3.Lerp(midScale, endScale, progress);
            yield return null;
        }

        target.localScale = endScale;
    }


    private IEnumerator ScaleOutAndDestroy(QuestMarkerInfo questMarkerInfo)
    {
        Transform target = questMarkerInfo.MarkerRectTransform.transform.GetChild(0);
        if (target == null) yield break;

        // --- Stage 1: scale up to 1.5x ---
        float t = 0f;
        float durationUp = 0.15f;
        Vector3 startScale = target.localScale;
        Vector3 midScale = Vector3.one * 1.5f;

        while (t < durationUp)
        {
            if (target == null) yield break;
            t += Time.deltaTime;
            float progress = t / durationUp;
            target.localScale = Vector3.Lerp(startScale, midScale, progress);
            yield return null;
        }

        // --- Stage 2: scale down to 0 ---
        t = 0f;
        float durationDown = 0.2f;
        Vector3 endScale = Vector3.zero;

        while (t < durationDown)
        {
            if (target == null) yield break;
            t += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, t / durationDown); // smooth ease
            target.localScale = Vector3.Lerp(midScale, endScale, progress);
            yield return null;
        }

        // --- Cleanup ---
        if (questMarkerInfo.MarkerRectTransform != null)
            Destroy(questMarkerInfo.MarkerRectTransform.gameObject);

        activeQuestMarkers.Remove(questMarkerInfo);
    }


}

class QuestMarkerInfo
{
    public RectTransform MarkerRectTransform;
    public Transform MarkerWorldTransform;
    public GameObject ParticleObject;

    public QuestMarkerInfo(RectTransform rectTransform, Transform transform)
    {
        MarkerRectTransform = rectTransform;
        MarkerWorldTransform = transform;
    }

    public QuestMarkerInfo(RectTransform rectTransform, Transform transform,GameObject particleOBj)
    {
        MarkerRectTransform = rectTransform;
        MarkerWorldTransform = transform;
        ParticleObject = particleOBj;
    }
}
