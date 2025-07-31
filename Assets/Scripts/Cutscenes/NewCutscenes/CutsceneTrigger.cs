using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{

    public NewCutsceneBuilder newCutsceneBuilder;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            print("CutsceneTriggerEntered");
            newCutsceneBuilder.PlayCutscene();
        }
    }
}
