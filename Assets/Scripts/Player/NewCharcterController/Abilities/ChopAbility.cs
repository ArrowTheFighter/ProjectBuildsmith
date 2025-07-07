using System.Collections;
using UnityEngine;

public class ChopAbility : PlayerAbility
{
    bool BasicAttackPressed;
    bool IsChopping;
    public override void FixedUpdateAbility()
    {

    }

    public override void UpdateAbility()
    {
        if (GameplayInput.instance.playerInput.actions["BasicAttack"].ReadValue<float>() > 0)
        {
            if (IsChopping) return;
            if (!BasicAttackPressed)
            {
                if (characterMovement.grounded)
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
    }

    void Chop()
    {
        IsChopping = true;
        characterMovement.OnBasicAttack?.Invoke();
        characterMovement.MovementControlledByAbility = true;
        characterMovement.tilt_amount = 0;
        StartCoroutine(finishedChopDelay());
    }

    IEnumerator finishedChopDelay()
    {
        yield return new WaitForSeconds(1.25f);
        IsChopping = false;
        characterMovement.MovementControlledByAbility = false;
    }
}
