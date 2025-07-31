using UnityEngine;

public class NewCutsceneBuilder : MonoBehaviour
{
    public CutsceneData cutsceneData;

    public void PlayCutscene()
    {
        CutsceneManager.instance.StartCutscene(cutsceneData);
     }
}
