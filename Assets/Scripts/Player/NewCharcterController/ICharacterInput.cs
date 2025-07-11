using UnityEngine;
using System;

public interface ICharacterInput
{
    public Vector3 GetMovementInput();
    bool GetJumpInput();
    bool GetDashInput();

    public event Action OnJump;
    public event Action OnDive;
}
