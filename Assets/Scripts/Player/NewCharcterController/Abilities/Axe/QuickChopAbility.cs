using System.Collections;
using UnityEngine;

public class QuickChopAbility : PlayerAbility
{
    //bool BasicAttackPressed;
    bool IsChopping;
    float finishChopDelay;
    public float damageStrength = 1;
    public AttackType[] attackTypes;

    public override void FixedUpdateAbility()
    {

    }

    public override void UpdateAbility()
    {
        if (ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["BasicAttack"].ReadValue<float>() > 0)
        {
            if (!IsChopping)
            {
                if (characterMovement.grounded && !characterMovement.MovementControlledByAbility)
                {
                    Chop();
                }
            }
            
            //BasicAttackPressed = true;
        }
        else
        {
            //BasicAttackPressed = false;
        }

        if (Time.time > finishChopDelay) IsChopping = false;
    }

    void Chop()
    {
        IsChopping = true;
        characterMovement.OnBasicAttack?.Invoke();
        characterMovement.tilt_amount = 0;
        finishChopDelay = Time.time + 0.35f;
        AudioCollection audioCollection = ScriptRefrenceSingleton.instance.playerAudioManager.GetAudioClipByID("AxeSwingWoosh");
        AudioCollection audioCollection2 = ScriptRefrenceSingleton.instance.playerAudioManager.GetAudioClipByID("AxeSwingWoosh2");

        ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(transform, audioCollection, audioCollection2);

        //ScriptRefrenceSingleton.instance.soundFXManager.PlaySoundFXClip(audioCollection.audioClip, transform, audioCollection.audioClipVolume, audioCollection.audioClipPitch);
        //StartCoroutine(finishedChopDelay());
    }

    public void AttackCheck()
    {
        if (this != null && gameObject != null)
        {

            Vector3 size = new Vector3(3, 2, 2);
            Collider[] hits = Physics.OverlapBox(transform.position + transform.forward * 1.5f, size * 0.5f, transform.rotation);
            foreach (Collider colliderHit in hits)
            {
                if (colliderHit.TryGetComponent(out IDamagable damagable))
                {
                    damagable.TakeDamage(damageStrength, attackTypes, characterMovement.gameObject);
                    // AudioCollection audioCollection = ScriptRefrenceSingleton.instance.playerAudioManager.GetAudioClipByID("AxeChop");
                    // ScriptRefrenceSingleton.instance.soundFXManager.PlaySoundFXClip(audioCollection.audioClip, transform, audioCollection.audioClipVolume, audioCollection.audioClipPitch);
                }
                if (colliderHit.TryGetComponent(out ItemPickup itemPickup))
                {
                    itemPickup.Pickup();
                }
            }

        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator finishedChopDelay()
    {
        yield return new WaitForSeconds(0.5f);
        IsChopping = false;
    }

    public override void ResetAbility()
    {
        //No need to do anything
    }
}
