using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{

    [SerializeField] bool onlyOnce;
    bool activated;
    public NewCutsceneBuilder newCutsceneBuilder;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (other.tag == "Player")
        {
            print("CutsceneTriggerEntered");
            newCutsceneBuilder.PlayCutscene();
            if (onlyOnce) activated = true;
        }
    }
}
