using UnityEngine;
using System;

public interface IMoveingPlatform
{
    public event Action OnBeforePlatformMove;

    public Transform getInterfaceTransform();
 }
