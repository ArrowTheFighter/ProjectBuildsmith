using UnityEngine;
using System;

public interface IMoveingPlatform
{
    public event Action<Vector3> OnPlatformMove;
    public event Action OnBeforePlatformMove;
    public event Action OnAfterPlatformMove;

    public Transform getInterfaceTransform();
 }
