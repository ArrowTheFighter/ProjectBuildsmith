using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ActivatorCrystal : MonoBehaviour, IDamagable
{
    public bool PlayerCanStomp { get => false; set => value = false; }

    bool isActive;
    float runningTime;
    public float Duration;
    float DisplayTime;
    float LerpedDisplayTime;

    [Header("Audio")]
    public AudioCollection SwitchAudio;
    public AudioCollection SwitchOnHitAudio;

    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;

    Material mat;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(transform, SwitchAudio);
        ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(transform, SwitchOnHitAudio);
        StartCoroutine(CooldownProcess());
    }

    void Update()
    {

        LerpedDisplayTime = Mathf.Lerp(LerpedDisplayTime,DisplayTime, 0.1f);
        mat.SetFloat("_FillAmount", LerpedDisplayTime);
     }

    IEnumerator CooldownProcess()
    {
        runningTime = -0;
        if (isActive) yield break;
        isActive = true;
        OnActivated?.Invoke();
        while (runningTime < Duration)
        {
            runningTime += Time.deltaTime;
            DisplayTime = Mathf.InverseLerp(Duration , 0 + Duration * 0.35f, runningTime);

            yield return new WaitForEndOfFrame();
        }
        runningTime = -0f;
        DisplayTime = -0f;
        isActive = false;
        ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(transform, SwitchAudio);
        OnDeactivated?.Invoke();

    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        ExtraForce = 0;
        TakeDamage(amount, attackTypes, source);
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1)
    {
        TakeDamage(amount, attackTypes, source);
    }

}
