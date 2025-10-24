using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptRefrenceSingleton : MonoBehaviour
{
    public static ScriptRefrenceSingleton instance;

    public GameplayUtils gameplayUtils;
    public Transform playerTransformTest;

    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }

    [Button]
    public void ResetScene()
    {
        SceneManager.LoadScene(gameObject.scene.name);
    }
}
