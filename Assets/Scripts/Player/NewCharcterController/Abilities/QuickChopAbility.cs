using System.Collections;
using UnityEngine;

public class QuickChopAbility : PlayerAbility
{
    bool BasicAttackPressed;
    bool IsChopping;
    float finishChopDelay;
    int damageStrength = 1;
    public override void FixedUpdateAbility()
    {

    }

    public override void UpdateAbility()
    {
        if (GameplayInput.instance.playerInput.actions["BasicAttack"].ReadValue<float>() > 0)
        {
            if (!IsChopping)
            {
                if (characterMovement.grounded && !characterMovement.MovementControlledByAbility)
                {
                    Chop();
                }
            }
            
            BasicAttackPressed = true;
        }
        else
        {
            BasicAttackPressed = false;
        }

        if (Time.time > finishChopDelay) IsChopping = false;
    }

    void Chop()
    {
        IsChopping = true;
        characterMovement.OnBasicAttack?.Invoke();
        characterMovement.tilt_amount = 0;
        finishChopDelay = Time.time + 0.35f;
        //StartCoroutine(finishedChopDelay());
    }

    public void AttackCheck()
    {
        print("TryingToAttack");
        Vector3 size = new Vector3(3, 2, 2);
        Collider[] hits = Physics.OverlapBox(transform.position + transform.forward * 1.5f, size * 0.5f, transform.rotation);
        foreach (Collider colliderHit in hits)
        {
            if (colliderHit.TryGetComponent(out IDamagable damagable))
            {
                print("dealing Damage");
                damagable.TakeDamage(damageStrength,characterMovement.gameObject);
            }
         }
     }

    IEnumerator finishedChopDelay()
    {
        yield return new WaitForSeconds(0.5f);
        IsChopping = false;
    }
}
