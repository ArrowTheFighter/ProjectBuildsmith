using UnityEngine;

public interface ICharacterInput
{
    public Vector3 GetMovementInput();
    bool GetJumpInput();
}
