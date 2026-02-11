using System.Collections.Generic;
using UnityEngine;

public class EnableDisableGroup : MonoBehaviour, IEnableDisable
{
    public GameObject[] enableDisableGameObjects;
    List<IEnableDisable> enableDisables = new List<IEnableDisable>();
    [Header("Start Enabled")]
    public bool StartEnabled;

    void Awake()
    {
        foreach(var item in enableDisableGameObjects)
        {
           if(item.TryGetComponent(out IEnableDisable component))
            {
                enableDisables.Add(component);
            }
        }
        if(!StartEnabled)
            DisableInterface();
    }

    public void DisableInterface()
    {
        foreach(var item in enableDisables)
        {
            item.DisableInterface();
        }
    }

    public void EnableInterface()
    {
        foreach (var item in enableDisables)
        {
            item.EnableInterface();
        }

    }
}
