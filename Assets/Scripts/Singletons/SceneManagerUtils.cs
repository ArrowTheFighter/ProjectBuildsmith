using UnityEngine;

public class SceneManagerUtils : MonoBehaviour
{


    public bool ShowStartCutscene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Set_ShowStartCutscene(bool should_show)
    {
        ShowStartCutscene = should_show;
     }
}
