using UnityEngine;
using UnityEngine.Events;

public class CutsceneTrigger : MonoBehaviour,ISaveable
{

    [SerializeField] bool onlyOnce;
    bool activated;
    public NewCutsceneBuilder newCutsceneBuilder;
    public UnityEvent onActivatedEvent;

    public int Get_Unique_ID { get => unique_id; set {unique_id = value;} }
    public int unique_id;
    public bool Get_Should_Save => activated;

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

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        activated = true;
    }
}
