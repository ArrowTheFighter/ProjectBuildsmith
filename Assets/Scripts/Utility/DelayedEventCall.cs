using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEventCall : MonoBehaviour
{
    [SerializeField] float delay;
    [SerializeField] UnityEvent unityEvent;

    [Button]
    public void CallDelayedEvent()
    {
        StartCoroutine(DelayCall());
    }

    IEnumerator DelayCall()
    {
        yield return new WaitForSecondsRealtime(delay);
        unityEvent?.Invoke();
    }
}
