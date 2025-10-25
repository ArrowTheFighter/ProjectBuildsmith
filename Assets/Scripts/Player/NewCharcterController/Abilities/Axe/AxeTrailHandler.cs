using UnityEngine;

public class AxeTrailHandler : MonoBehaviour
{
    AnimationEvents animationEvents;
    TrailRenderer trailRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animationEvents = ScriptRefrenceSingleton.instance.gameplayUtils.animationEvents;
        trailRenderer = GetComponent<TrailRenderer>();
        animationEvents.OnAxeSwingStart += ShowTrail;
        animationEvents.OnAxeSwingEnd += HideTrail;
    }

    void OnDestroy()
    {
        if (animationEvents == null) return;
        animationEvents.OnAxeSwingStart -= ShowTrail;
        animationEvents.OnAxeSwingEnd -= HideTrail;
    }

    void ShowTrail()
    {
        trailRenderer.emitting = true;
    }

    void HideTrail()
    {
        trailRenderer.emitting = false;
    }
}
