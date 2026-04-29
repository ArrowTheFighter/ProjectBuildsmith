using UnityEngine;
using UnityEngine.Events;

public class Zone1BossAnimationEvents : MonoBehaviour
{

    public UnityEvent GrabArtifactUnityEvent;

    public void GrabArtifactEvent()
    {
        GrabArtifactUnityEvent?.Invoke();
    }
}
