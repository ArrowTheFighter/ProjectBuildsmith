using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{

    CharacterMovement characterMovement;
    [SerializeField] int MaxHealth;
    [SerializeField] int Health;
    bool dying;

    public Action<int> OnTakeDamage;

    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
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
