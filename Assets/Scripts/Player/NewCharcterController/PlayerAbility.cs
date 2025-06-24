using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
    protected CharacterMovement characterMovement;
    public virtual void Initialize(CharacterMovement player)
    {
        characterMovement = player;
    }

    public abstract void FixedUpdateAbility();

    public abstract void UpdateAbility();

}
