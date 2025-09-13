using UnityEngine;
using UnityEngine.Events;

public class CutsceneTrigger : MonoBehaviour
{

    [SerializeField] bool onlyOnce;
    bool activated;
    public NewCutsceneBuilder newCutsceneBuilder;
    public UnityEvent onActivatedEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (other.tag == "Player")
        {
            print("CutsceneTriggerEntered");
            newCutsceneBuilder.PlayCutscene();
            if (onlyOnce) activated = true;
            onActivatedEvent?.Invoke();
        }
    }
}
