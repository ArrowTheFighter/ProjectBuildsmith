using UnityEngine;
using UnityEngine.Events;
using System;

public class ActivivateWithFlagRules : MonoBehaviour
{
    
    [SerializeField] FlagRule[] flagRules;

   

    public void Activate()
    {
        for (int i = flagRules.Length - 1; i >= 0; i--)
        {
            if (FlagManager.Get_Flag_Value(flagRules[i].Flag) == flagRules[i].CheckIfTrue)
            {
                flagRules[i].FlagUnitEvent?.Invoke();
                return;
            }
        }
    }

}


