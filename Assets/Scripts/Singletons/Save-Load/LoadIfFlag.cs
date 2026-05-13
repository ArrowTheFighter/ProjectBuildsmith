using System;
using UnityEngine;
using UnityEngine.Events;

public class LoadIfFlag : MonoBehaviour, ISaveable
{
    public int unique_id;
    public int Get_Unique_ID { get => unique_id; set {unique_id = value;} }

    public bool Get_Should_Save { get
        {
            foreach(var rule in flagRules)
            {
                if(FlagManager.Get_Flag_Value(rule.Flag))
                    return true;               
            }
            return false;
        }
    }

    [SerializeField] FlagRule[] flagRules;

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        Debug.Log("Load if Flag running",gameObject);
        for (int i = flagRules.Length - 1; i >= 0 ; i--)
        {
            if(FlagManager.Get_Flag_Value(flagRules[i].Flag))
            {
                Debug.Log($"Loading event because flag {flagRules[i].Flag} was true",gameObject);
                flagRules[i].FlagUnitEvent?.Invoke();
                return;
            }
        }
    }

}

[Serializable]
class FlagRule
{
    public string Flag;
    public UnityEvent FlagUnitEvent;
}
