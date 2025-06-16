using UnityEngine;

public class SceneManagerUtils : MonoBehaviour
{

    public static SceneManagerUtils instance;


    public bool ShowStartCutscene;

    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }
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
