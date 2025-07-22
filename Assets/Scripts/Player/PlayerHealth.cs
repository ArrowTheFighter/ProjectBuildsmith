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
    private bool heartImagesOnScreen;

    [SerializeField] float heartFadeOutDelay = 5f;
    [SerializeField] float heartFadeOutTime = 2f;
    private bool heartImagesIsFadingOut;

    [SerializeField] Vector3 finalHeartBlinkSize = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] float finalHeartBlinkSpeed = 0.25f;
    private bool finalHeartBlinking;

    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }


    //This is DD's really unoptimized code, you will probably know how to make it more efficient, but I'm getting to a workable state first
    public void Update()
    {
        if(Health <= 2)
        {
            DOTween.Kill("HeartFadeOut");
            DOTween.Kill("HeartScaleOut");
            heartImageHolderCanvasGroup.alpha = 1;

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
        
        if (Health == 3)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                heartImages[i].fillAmount = 1f;
            }
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
                heartImages[0].transform.DOScale(finalHeartBlinkSize, finalHeartBlinkSpeed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetId("FinalHeartBlink");
            }         
        }

        if(Health != 1)
        {
            if (finalHeartBlinking)
            {
                finalHeartBlinking = false;
                DOTween.Kill("FinalHeartBlink");
            }
        }

        if(Health == 0)
        {
            heartImages[0].fillAmount = 0f;
        }
    }

    private void FadeOutHealthUI()
    {
        if (Health != 3) return;

        heartImageHolderCanvasGroup.DOFade(0f, heartFadeOutTime).SetId("HeartFadeOut");
        Sequence heartSequence = DOTween.Sequence();
        heartSequence.Append(heartImageHolder.DOScale(new Vector3(0,0,0), heartFadeOutTime)).SetId("HeartScaleOut");
        heartImagesOnScreen = false;
        heartImagesIsFadingOut = false;
    }

    public void TakeDamage(int amount, GameObject source)
    {
        if (dying) return;
        if (source == GameplayUtils.instance.PlayerTransform.gameObject) return;
        Health -= amount;
        if (Health <= 0)
        {
            Respawn();
        }
        OnTakeDamage?.Invoke(Health);
        PlayerParticlesManager.instance.PlayPlayerTakeHitParticles();
    }

    public void TakeDamage(int amount, GameObject source, float knockbackStrength = 1)
    {
        if (dying) return;
        TakeKnockback(source, knockbackStrength);
        TakeDamage(amount, source);
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
        Health = MaxHealth;
        dying = false;
    }

   

    public void TakeDamage(int amount, GameObject source, out float ExtraForce)
    {
        ExtraForce = 0;
        return;
    }
}
