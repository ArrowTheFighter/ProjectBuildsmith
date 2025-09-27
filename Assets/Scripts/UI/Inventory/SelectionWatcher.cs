using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SelectionWatcher : MonoBehaviour
{
    public static event Action<GameObject> OnSelectionChanged;

    private GameObject lastSelected;

    void Update()
    {
        if (EventSystem.current == null)
            return;

        var current = EventSystem.current.currentSelectedGameObject;
        if (current != lastSelected && current != null)
        {
            lastSelected = current;
            OnSelectionChanged?.Invoke(current);
        }
    }
}
