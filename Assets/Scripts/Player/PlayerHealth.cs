using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamagable
{

    CharacterMovement characterMovement;
    [SerializeField] int MaxHealth;
    [SerializeField] int Health;
    bool dying;

    public Action<int> OnTakeDamage;

    [SerializeField] Image[] heartImages;
    [SerializeField] Transform heartImageHolder;
    [SerializeField] CanvasGroup heartImageHolderCanvasGroup;
    [SerializeField] Vector3 heartImageHolderScale = new Vector3(1, 1, 1);
    [SerializeField] float heartScaleInTime = 1f;
    [SerializeField] float heartFadeInTime = 0.3f;
    private bool heartImagesOnScreen;

    [SerializeField] float heartFadeOutDelay = 5f;
    [SerializeField] float heartFadeOutTime = 2f;
    private bool heartImagesIsFadingOut;

    [SerializeField] Vector3 finalHeartBlinkSize = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] float finalHeartBlinkSpeed = 0.25f;
    private bool finalHeartBlinking;

    [SerializeField] float heartDamageShakeTime = 0.3f;
    [SerializeField] float heartDamageShakeStrength = 0.2f;
    [SerializeField] int heartDamageShakeVibrato = 8;
    [SerializeField] float heartDamageShakeRandomness = 90;

    [SerializeField] float regenDelay = 10f;         // Time after last damage before regen starts
    [SerializeField] float regenInterval = 10f;      // Time between each heart regen
    [SerializeField] int regenAmount = 1;            // How much health to regenerate per tick

    private Coroutine regenCoroutine;
    private float timeSinceLastDamage;
    private bool canRegen;

    [SerializeField] AudioClip heartRefillPopSoundFX;
    [SerializeField] float heartRefillPopSoundFXVolume = 0.4f;
    [SerializeField] float heartRefillPopSoundFXPitch = 1f;


    [HideInInspector] public bool PlayerCanStomp { get; set ; }

    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }


    //This is DD's really unoptimized code, you will probably know how to make it more efficient, but I'm getting to a workable state first
    public void Update()
    {
        if (Health < MaxHealth)
        {
            timeSinceLastDamage += Time.deltaTime;

            if (!canRegen && timeSinceLastDamage >= regenDelay)
            {
                canRegen = true;
                regenCoroutine = StartCoroutine(RegenerateHealthOverTime());
            }
        }

        if (Health <= 2)
        {
            DOTween.Kill("HeartFadeOut");
            DOTween.Kill("HeartScaleOut");
            heartImageHolderCanvasGroup.DOFade(1f, heartFadeInTime).SetEase(Ease.OutQuad);

            if (!heartImagesOnScreen)
            {
                heartImagesOnScreen = true;
                heartImageHolder.DOScale(heartImageHolderScale, heartScaleInTime).SetEase(Ease.InOutElastic);
            }  
        }

        if(Health == 3 && heartImagesOnScreen && !heartImagesIsFadingOut)
        {
            heartImagesIsFadingOut = true;
            Invoke("FadeOutHealthUI", heartFadeOutDelay);
        }
        

        if(Health == 2)
        {
            heartImages[2].fillAmount = 0f;
        }

        if(Health == 1)
        {
            heartImages[1].fillAmount = 0f;
            if (!finalHeartBlinking)
            {
                finalHeartBlinking = true;
                heartImages[0].transform.DOScale(finalHeartBlinkSize, finalHeartBlinkSpeed)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetId("FinalHeartBlink");
            }         
        }

        if (Health != 1)
        {
            if (finalHeartBlinking)
            {
                finalHeartBlinking = false;
                DOTween.Kill("FinalHeartBlink");
                heartImages[0].transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad);
            }
        }

        if (Health == 0)
        {
            heartImages[0].fillAmount = 0f;
        }
    }

    private void FadeOutHealthUI()
    {
        if (Health != 3) return;

        Sequence heartSequence = DOTween.Sequence();
        heartSequence.Append(heartImageHolder.DOScale(new Vector3(0,0,0), heartFadeOutTime)).SetId("HeartScaleOut");
        heartSequence.Join(heartImageHolderCanvasGroup.DOFade(0f, heartFadeOutTime).SetId("HeartFadeOut"));
        heartImagesOnScreen = false;
        heartImagesIsFadingOut = false;
    }

    public void TakeDamage(int amount, GameObject source)
    {
        if (dying) return;
        if (source == GameplayUtils.instance.PlayerTransform.gameObject) return;

        if (heartImagesOnScreen)
        {
            heartImageHolder.DOComplete();
            heartImageHolder.DOShakeScale(heartDamageShakeTime, heartDamageShakeStrength, heartDamageShakeVibrato, heartDamageShakeRandomness)
                            .SetEase(Ease.OutQuad)
                            .SetId("HeartShake");
        }

        // Reset regen tracking
        timeSinceLastDamage = 0f;
        canRegen = false;
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        Health -= amount;

        if (Health <= 0)
        {
            Respawn();
        }

        OnTakeDamage?.Invoke(Health);
        PlayerParticlesManager.instance.PlayPlayerTakeHitParticles();

        AudioCollection audioCollection = PlayerAudioManager.instance.GetAudioClipByID("Hurt");
        SoundFXManager.instance.PlaySoundFXClip(audioCollection.audioClip, transform, audioCollection.audioClipVolume, UnityEngine.Random.Range(audioCollection.audioClipPitch * 0.9f, audioCollection.audioClipPitch * 1.1f));
    }

    public void TakeDamage(int amount, GameObject source, float knockbackStrength = 1)
    {
        if (dying) return;
        TakeKnockback(source, knockbackStrength);
        TakeDamage(amount, source);
     }

    IEnumerator RegenerateHealthOverTime()
    {
        while (Health < MaxHealth)
        {
            yield return new WaitForSeconds(regenInterval);

            // If player took damage during wait, cancel
            if (!canRegen) yield break;

            Health = Mathf.Min(Health + regenAmount, MaxHealth);
            OnTakeDamage?.Invoke(Health); // To refresh UI (optional)

            for (int i = 0; i < heartImages.Length; i++)
            {
                if (i == Health - 1) // Just filled this one
                {
                    heartImages[i].fillAmount = 1f;
                    heartImages[i].transform.DOKill(); // <--- prevent conflicts
                    heartImages[i].transform.localScale = Vector3.one * 0.6f;
                    heartImages[i].transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                    SoundFXManager.instance.PlaySoundFXClip(heartRefillPopSoundFX, transform, heartRefillPopSoundFXVolume, heartRefillPopSoundFXPitch);
                }
                else
                {
                    heartImages[i].fillAmount = (i < Health) ? 1f : 0f;
                }
            }

            // Regen complete
            regenCoroutine = null;
            canRegen = false;
        }
    }

    void TakeKnockback(GameObject source, float knockbackStrength)
    {
        characterMovement.tilt_amount = 0;
        characterMovement.MovementControlledByAbility = true;
        characterMovement.OverrideGravity = true;
        Vector3 dir = transform.position - source.transform.position;
        dir.y = 0;
        //characterMovement.ManualTurn(-dir);

        characterMovement.OverrideAirDragAmount = 3.5f;
        GetComponent<Rigidbody>().AddForce(dir.normalized * 40 * knockbackStrength, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(Vector3.up * 15 * knockbackStrength, ForceMode.Impulse);
        StartCoroutine(knockbackFinishDelay());
     }

    IEnumerator knockbackFinishDelay()
    {
        yield return new WaitForSeconds(0.5f);
        characterMovement.MovementControlledByAbility = false;
        characterMovement.OverrideGravity = false;
        characterMovement.OverrideAirDragAmount = 0;
    }

    public void Respawn()
    {
        StopAllCoroutines();
        print("respawning");
        dying = true;
        CharacterMovement characterMovement = GetComponent<CharacterMovement>();
        characterMovement.MovementControlledByAbility = true;
        characterMovement.orientation.gameObject.SetActive(false);
        characterMovement.OverrideGravity = false;
        characterMovement.OverrideAirDragAmount = 0;
        GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(MovePlayerToRespawnPointDelay());
        StartCoroutine(RespawnDelay());

        DOTween.Kill("HeartShake");
    }

    IEnumerator MovePlayerToRespawnPointDelay()
    {
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<PlayerCheckpointPosition>().MovePlayerToCheckpointSmoothly();
    }

    IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(2f);
        //gameObject.GetComponent<PlayerCheckpointPosition>().SetPlayerToCheckpointPosition();
        CharacterMovement characterMovement = GetComponent<CharacterMovement>();
        characterMovement.orientation.DOScale(Vector3.one, 0.2f).From(Vector3.zero).SetEase(Ease.InOutQuad);
        characterMovement.orientation.gameObject.SetActive(true);
        characterMovement.MovementControlledByAbility = false;
        GetComponent<Rigidbody>().isKinematic = false;

        //Respawns the player back in with 1 health instead of full health
        //We can decide which one we want, waiting for health to regenerate might be too slow/boring, but also might discourage players from being careless
        //Mostly want to try it out and see how it feels
        Health = 1;
        //Health = MaxHealth;

        dying = false;

        // Reset and show heart UI
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].fillAmount = (i < Health) ? 1f : 0f;
        }

        heartImageHolderCanvasGroup.alpha = 1f;
        heartImageHolder.localScale = heartImageHolderScale;
        heartImagesOnScreen = true;

        // Shake effect when hearts refill
        heartImageHolder.DOComplete();
        heartImageHolder.DOShakeScale(heartDamageShakeTime, heartDamageShakeStrength, heartDamageShakeVibrato, heartDamageShakeRandomness)
            .SetEase(Ease.OutQuad)
            .SetId("HeartShake");
    }

   

    public void TakeDamage(int amount, GameObject source, out float ExtraForce)
    {
        ExtraForce = 0;
        return;
    }
}